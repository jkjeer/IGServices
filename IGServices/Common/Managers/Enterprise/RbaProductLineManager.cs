using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using IGServices.Common.Types.Enterprise;
using IGServices.Common.Utilities;
using NLog;

namespace IGServices.Common.Managers.Enterprise

{
    public class RbaProductLineManager
    {

        private static Logger _logger = LogManager.GetCurrentClassLogger();

        /**********************************************************************
         *  Product Line Summary
         *********************************************************************/
        public SqlCommand GetRbaProductLines()
        {
            var cmd = new SqlCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = @"
            SELECT
                p.ProductLineCode,
                p.ShortCode,
                p.ProductLineDesc,
                p.HourlyRunRate,
                p.ShiftHours,
                p.MinFloorQty,
                p.MaxFloorQty,
                p.ReorderQty,
                p.RetireDays,
                p.UnitRatio,
                p.Display,
                p.SortIndex,
                b.LastConsumedDate,
                b.LastShipDate
            FROM
                ShopFloor.EBS.RbaProductLine p
                LEFT OUTER JOIN
                (
                    SELECT
                        ri.CardinalProductType,
                        MAX(ri.RackConsumedDate) AS LastConsumedDate,
			            MAX(ri.ShipDate) AS LastShipDate
                    FROM
                        ShopFloor.EBS.RbaShippedRackItem ri
                    GROUP BY
                        ri.CardinalProductType
                ) b
                ON  b.CardinalProductType = p.ProductLineCode
            WHERE
                Display = 1
            ORDER BY
                p.SortIndex;";
            return cmd;
        }

        public RbaProductLines GetRbaProductLines(string plantCode, SqlConnection cn)
        {
            var lines = new RbaProductLines();
            try
            {
                var cmd = GetRbaProductLines();
                cmd.Connection = cn;
                _logger.Trace(Logging.DbCmdString(cmd));
                IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult);
                while (reader.Read())
                {
                    var line = new RbaProductLine();
                    line.ProductLineCode = CastDbNull.To<String>(reader["ProductLineCode"], String.Empty);
                    line.ShortCode = CastDbNull.To<String>(reader["ShortCode"], String.Empty);
                    line.ProductLineName = CastDbNull.To<String>(reader["ProductLineDesc"], String.Empty);
                    line.Display = CastDbNull.To<bool>(reader["Display"], false) ? "Y" : "N";
                    line.SortIndex = CastDbNull.To<int>(reader["SortIndex"], 0);
                    line.HourlyRunRate = CastDbNull.To<decimal>(reader["HourlyRunRate"], 0);
                    line.ShiftHours = CastDbNull.To<int>(reader["ShiftHours"], 0);
                    line.ShiftRunRate = line.HourlyRunRate * line.ShiftHours;
                    line.MinFloorQty = CastDbNull.To<int>(reader["MinFloorQty"], 0);
                    line.MaxFloorQty = CastDbNull.To<int>(reader["MaxFloorQty"], 0);
                    line.ReorderQty = CastDbNull.To<int>(reader["ReorderQty"], 0);
                    line.RetireDays = CastDbNull.To<int>(reader["RetireDays"], 0);
                    line.UnitRatio = CastDbNull.To<decimal>(reader["UnitRatio"], 0);
                    line.LastConsumedDate = CastDbNull.To<DateTime>(reader["LastConsumedDate"], new DateTime(1900, 1, 1));
                    line.LastConsumedDateString = line.LastConsumedDate.ToString("MM/dd/yyyy hh:mm tt");
                    line.LastShipDate = CastDbNull.To<DateTime>(reader["LastShipDate"], new DateTime(1900, 1, 1));
                    line.LastShipDateString = line.LastShipDate.ToString("MM/dd/yyyy hh:mm tt");
                    lines.Add(line);
                }
                reader.Close();
                var nextRacks = GetNextRacks(plantCode, 10, cn);
                var floorRacks = GetFloorRacks(plantCode, cn);
                var trucks = GetTruckList(plantCode, cn);

                var currentReleaseDepartureDate = trucks.AsEnumerable().FirstOrDefault(x => x.Field<string>("CurrentRelease") == "Y")?
                    .Field<DateTime>("DepartureDate");
                // RBA Shifts
                var targetShifts = GetTargetShifts(plantCode, cn, currentReleaseDepartureDate ?? DateTime.Now);
                // Target Unit Adjustments
                var releaseAdjustments = GetShipmentAdjustment(plantCode, cn, currentReleaseDepartureDate ?? DateTime.Now);

                foreach (var productLine in lines)
                {
                    productLine.NextRacks = nextRacks.Where(x => x.ProductLineCode == productLine.ProductLineCode).ToList();
                    productLine.FloorRacks = floorRacks.Where(x => x.ProductLineCode == productLine.ProductLineCode).ToList();
                    productLine.RbaFloorRacks = productLine.FloorRacks.Count;
                    productLine.RbaFloorUnits = productLine.FloorRacks.Sum(x => x.Units);

                    var targetShift = targetShifts
                        .AsEnumerable()
                        .FirstOrDefault(
                            x => x.Field<string>("ProductLineCode") == productLine.ProductLineCode);
                    productLine.TargetShift = targetShift?.Field<int>("ShiftNum") ?? 0;
                    productLine.TargetShiftActive = targetShift?.Field<bool>("Active") ?? false ? "Y" : "N";

                    var adjustment = releaseAdjustments
                        .AsEnumerable()
                        .FirstOrDefault(
                            x => x.Field<string>("ProductLineCode") == productLine.ProductLineCode);
                    productLine.AdjustmentQty = adjustment?.Field<int>("TargetUnits") ?? 0;
                    productLine.ReorderQty = productLine.ReorderQty + productLine.AdjustmentQty;

                    if (productLine.TargetShiftActive == "XXXX")
                    {
                        productLine.RequiredUnits = 0;
                    }
                    else
                    {
                        productLine.RequiredUnits = productLine.ReorderQty - productLine.RbaFloorUnits;
                        var nextReleaseAmt = productLine.HourlyRunRate * 4;
                        int rackAmt = 0;
                        foreach (var rack in productLine.NextRacks)
                        {
                            if (rackAmt <= productLine.RequiredUnits)
                            {
                                rack.IsRequired = "Y";
                                rack.IsNextRelease = "N";
                            }
                            else
                            {
                                if (rackAmt <= productLine.RequiredUnits + nextReleaseAmt)
                                {
                                    rack.IsRequired = "N";
                                    rack.IsNextRelease = "Y";
                                }
                            }
                            rackAmt = rackAmt + rack.Units;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
            return lines;
        }

        /**********************************************************************
         *  Ship Date Cutoff
         *********************************************************************/
        public SqlCommand GetMinShipDate()
        {
            var cmd = new SqlCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = @"
            SELECT
                A.CalendarDate
            FROM
            (
                SELECT
                    CalendarDate,
                    ROW_NUMBER() OVER (ORDER BY
                                           CalendarDate DESC
                                      ) AS RN
                FROM
                    ShopFloor.EBS.Calendar
                WHERE
                    CalendarName = 'SCHEDULE' AND
                    CalendarDate < GETDATE() AND
                    CalendarCode = 'W'
            ) A
            WHERE
                A.RN = 3";
            return cmd;
        }

        public DateTime GetMinShipDate(SqlConnection cn)
        {
            DateTime minShipDate = new DateTime(1900, 1, 1);
            try
            {
                var cmd = GetMinShipDate();
                cmd.Connection = cn;
                _logger.Trace(Logging.DbCmdString(cmd));
                IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow);
                while (reader.Read())
                {
                    minShipDate = Data.CastDbNull.To<DateTime>(reader["CalendarDate"], new DateTime(1900, 1, 1));
                }
                reader.Close();
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
            return minShipDate;
        }

        /**********************************************************************
         *  Next Racks
         *********************************************************************/
        public SqlCommand GetNextRacks()
        {
            var cmd = new SqlCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = @"            
            SELECT
                *
            FROM
            (
                SELECT
                    B.*,
                    pl.NextRacks,
                    ROW_NUMBER() OVER (PARTITION BY
                                           CardinalProductType
                                       ORDER BY
                                           B.BatchBin
                                      ) AS RN
                FROM
                    ShopFloor.EBS.RbaProductLine pl WITH (NOLOCK)
                    INNER JOIN
                    (
                        SELECT
                            A.CardinalProductType,
                            A.RackNum,
                            A.ScheduleNum,
                            A.BoxNum,
                            A.RackType,
                            MAX(   CASE
                                       WHEN A.SequenceNum = 1 THEN A.BatchBin
                                       ELSE 0
                                   END
                               ) AS BatchBin,
                            MIN(A.HeaderStatus) AS HeaderStatus,
                            MIN(A.DetailStatus) AS DetailStatus,
                            MAX(A.PriorityCode) AS PriorityCode,
                            MAX(A.TransactionDate) AS TransactionDate,
                            MIN(A.DueDate) AS DueDate,
                            COUNT(*) AS Units
                        FROM
                        (
                            SELECT
                                ri.CardinalProductType,
                                ri.RackNum,
                                ri.ScheduleNum,
                                ri.BoxNum,
                                ri.RackType,
                                ri.Batch * 100 + ri.Bin AS BatchBin,
                                ri.HeaderStatus AS HeaderStatus,
                                ri.DetailStatus AS DetailStatus,
                                ri.PriorityCode AS PriorityCode,
                                ri.TransactionDate AS TransactionDate,
                                ri.DueDate AS DueDate,
                                CASE
                                    WHEN ri.RackType LIKE 'B%' THEN ROW_NUMBER() OVER (PARTITION BY
                                                                                           ri.RackNum
                                                                                       ORDER BY
                                                                                           ri.SlotNum ASC
                                                                                      )
                                    ELSE ROW_NUMBER() OVER (PARTITION BY
                                                                ri.RackNum
                                                            ORDER BY
                                                                ri.Batch * 100 + ri.Bin ASC
                                                           )
                                END AS SequenceNum
                            FROM
                                ShopFloor.EBS.RbaCurrentRackItem ri WITH (NOLOCK)
                                CROSS APPLY ShopFloor.EBS.RbaShipCalendar sc WITH (NOLOCK)
                            WHERE
                                sc.LoadStartDate <= GETDATE() AND
                                sc.LoadEndDate > GETDATE() AND
                                ri.DueDate <= sc.ProductionDate AND
                                ri.BoxNum < 30 AND
                                ri.BoxNum > 0 AND
                                EXISTS
                            (
                                SELECT
                                    1
                                FROM
                                    ShopFloor.EBS.RbaCurrentRackItem ri2 WITH (NOLOCK)
                                WHERE
                                    ri2.RackNum = ri.RackNum AND
                                    ri2.PriorityCode IN ('', '1D')
                            )
                        ) A
                        GROUP BY
                            A.CardinalProductType,
                            A.RackNum,
                            A.ScheduleNum,
                            A.BoxNum,
                            A.RackType
                    ) B
                    ON  B.CardinalProductType = pl.ProductLineCode
            ) C
            WHERE
                C.RN <= C.NextRacks
            ORDER BY
                CardinalProductType,
                RN;";
            cmd.Parameters.Add(new SqlParameter("@Rows", SqlDbType.Int));
            return cmd;
        }

        public RbaRacks GetNextRacks(string plantCode, int rows, SqlConnection cn)
        {
            var racks = new RbaRacks();
            try
            {
                var cmd = GetNextRacks();
                cmd.Parameters[0].Value = rows;
                cmd.Connection = cn;
                _logger.Trace(Logging.DbCmdString(cmd));
                IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult);
                while (reader.Read())
                {
                    var rack = new RbaRack();
                    rack.RackNum = CastDbNull.To<decimal>(reader["RackNum"], 0);
                    rack.ScheduleNum = CastDbNull.To<decimal>(reader["ScheduleNum"], 0);
                    rack.BatchBin = CastDbNull.To<decimal>(reader["BatchBin"], 0);
                    rack.BoxNum = CastDbNull.To<decimal>(reader["BoxNum"], 0);
                    rack.DueDate = CastDbNull.To<DateTime>(reader["DueDate"], new DateTime(1900, 1, 1));
                    rack.DueDateString = CastDbNull.To<DateTime>(reader["DueDate"], new DateTime(1900, 1, 1)).ToString("MM/dd");
                    rack.RackType = CastDbNull.To<String>(reader["RackType"], String.Empty);
                    rack.HeaderStatus = CastDbNull.To<String>(reader["HeaderStatus"], String.Empty);
                    rack.DetailStatus = CastDbNull.To<String>(reader["DetailStatus"], String.Empty);
                    rack.ProductLineCode = CastDbNull.To<String>(reader["CardinalProductType"], String.Empty);
                    rack.PriorityCode = CastDbNull.To<String>(reader["PriorityCode"], String.Empty);
                    rack.TransactionDate = CastDbNull.To<DateTime>(reader["TransactionDate"], new DateTime(1900, 1, 1));
                    rack.TransactionDateString = CastDbNull.To<DateTime>(reader["TransactionDate"], new DateTime(1900, 1, 1)).ToString("MM/dd/yyyy hh:mm tt");
                    rack.Units = CastDbNull.To<int>(reader["Units"], 0);
                    rack.IsRequired = "N";
                    racks.Add(rack);
                }
                reader.Close();
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
            return racks;
        }

        public void SetIsRequired(RbaProductLines lines, RbaRacks racks)
        {
            try
            {
                var rackLines = racks.GroupBy(x => x.ProductLineCode).ToList();
                foreach (var rackLine in rackLines)
                {
                    var productLine = lines.FirstOrDefault(x => x.ProductLineCode == rackLine.Key);
                    if (productLine != null)
                    {
                        var requiredAmt = productLine.RequiredUnits;
                        var nextReleaseAmt = productLine.HourlyRunRate * 4;
                        int rackAmt = 0;
                        foreach (var rack in rackLine)
                        {
                            if (rackAmt <= requiredAmt)
                            {
                                rack.IsRequired = "Y";
                                rack.IsNextRelease = "N";
                            }
                            else
                            {
                                if (rackAmt <= requiredAmt + nextReleaseAmt)
                                {
                                    rack.IsRequired = "N";
                                    rack.IsNextRelease = "Y";
                                }
                            }
                            rackAmt = rackAmt + rack.Units;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        /**********************************************************************
         *  Floor Racks
         *********************************************************************/
        public SqlCommand GetFloorRacks()
        {
            var cmd = new SqlCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = @"
            SELECT
                ri.CardinalProductType,
                ri.RackNum,
	            ri.RackType,
                ri.ScheduleNum,
                ri.BoxNum,
                ri.HeaderStatus,
                ri.DetailStatus,
                ri.PriorityCode,
                ri.ShipDate,
                MIN(ri.Batch * 100 + ri.Bin) AS BatchBin,
                MIN(ri.DueDate) as DueDate,
                COUNT(*) AS Units
            FROM
                ShopFloor.EBS.RbaShippedRackItem ri
            WHERE
                ri.ShipDate >= @BizDate AND
                ri.RackConsumedDate IS NULL AND
                EXISTS
            (
                SELECT
                    1
                FROM
                    ShopFloor.EBS.RbaShippedRackItem ri2
                WHERE
                    ri2.RackNum = ri.RackNum AND
                    ri2.PriorityCode IN ('', '1D')
            )
            GROUP BY
                ri.RackNum,
	            ri.RackType,
                ri.ShipDate,
                ri.CardinalProductType,
                ri.ScheduleNum,
                ri.BoxNum,
                ri.HeaderStatus,
                ri.DetailStatus,
                ri.PriorityCode
            ORDER BY 1,2,3,4,5";
            cmd.Parameters.Add(new SqlParameter("@BizDate", SqlDbType.DateTime));
            return cmd;
        }

        public RbaRacks GetFloorRacks(string plantCode, SqlConnection cn)
        {
            var racks = new RbaRacks();
            try
            {
                var cmd = GetFloorRacks();
                cmd.Parameters[0].Value = GetMinShipDate(cn);
                cmd.Connection = cn;
                _logger.Trace(Logging.DbCmdString(cmd));
                IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult);
                while (reader.Read())
                {
                    var rack = new RbaRack();
                    rack.RackNum = CastDbNull.To<decimal>(reader["RackNum"], 0);
                    rack.ScheduleNum = CastDbNull.To<decimal>(reader["ScheduleNum"], 0);
                    rack.BatchBin = CastDbNull.To<decimal>(reader["BatchBin"], 0);
                    rack.BoxNum = CastDbNull.To<decimal>(reader["BoxNum"], 0);
                    rack.DueDate = CastDbNull.To<DateTime>(reader["DueDate"], new DateTime(1900, 1, 1));
                    rack.DueDateString = CastDbNull.To<DateTime>(reader["DueDate"], new DateTime(1900, 1, 1)).ToString("MM/dd");
                    rack.RackType = CastDbNull.To<String>(reader["RackType"], String.Empty);
                    rack.HeaderStatus = CastDbNull.To<String>(reader["HeaderStatus"], String.Empty);
                    rack.DetailStatus = CastDbNull.To<String>(reader["DetailStatus"], String.Empty);
                    rack.ProductLineCode = CastDbNull.To<String>(reader["CardinalProductType"], String.Empty);
                    rack.PriorityCode = CastDbNull.To<String>(reader["PriorityCode"], String.Empty);
                    rack.TransactionDate = CastDbNull.To<DateTime>(reader["ShipDate"], new DateTime(1900, 1, 1));
                    rack.TransactionDateString = CastDbNull.To<DateTime>(reader["ShipDate"], new DateTime(1900, 1, 1)).ToString("MM/dd/yyyy hh:mm tt");
                    rack.Units = CastDbNull.To<int>(reader["Units"], 0);
                    racks.Add(rack);
                }
                reader.Close();
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
            return racks;
        }

        /**********************************************************************
         *  RBA Shifts
         *********************************************************************/
        public SqlCommand GetTargetShift()
        {
            var cmd = new SqlCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = @"
            SELECT
                pl.ProductLineCode,
                A.ShiftNum,
                A.Active,
                A.StartDateTime,
                A.EndDateTime,
                A.DateType,
                A.ProductionDate
            FROM
				ShopFloor.EBS.RbaShiftCalendar A
                INNER JOIN ShopFloor.EBS.RbaProductLine pl
                ON  pl.ProductLineId = A.ProductLineId
            WHERE
                A.StartDateTime <= @EffectiveDate AND
                A.EndDateTime > @EffectiveDate
            ORDER BY
                A.StartDateTime ASC;";
            cmd.Parameters.Add(new SqlParameter("@EffectiveDate", SqlDbType.DateTime));
            return cmd;
        }

        public DataTable GetTargetShifts(string plantCode, SqlConnection cn, DateTime effectiveDate)
        {
            var dt = new DataTable("shifts");
            try
            {
                var cmd = GetTargetShift();
                cmd.Parameters[0].Value = effectiveDate;
                cmd.Connection = cn;
                _logger.Trace(Logging.DbCmdString(cmd));
                dt.Load(cmd.ExecuteReader());
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
            return dt;
        }

        /**********************************************************************
         *  TOIG Trucks
         *********************************************************************/
        public SqlCommand GetTruckList()
        {
            var cmd = new SqlCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = @"
            SELECT
                *
            FROM
            (
                SELECT
                    SequenceNum,
                    LoadStartDate,
                    LoadEndDate,
                    ReleaseDate,
                    DepartureDate,
                    ArrivalDate,
                    CASE
                        WHEN GETDATE() > ReleaseDate AND
                             GETDATE() < LEAD(ReleaseDate, 1, 0) OVER (ORDER BY
                                                                           ProductionDate,
                                                                           SequenceNum
                                                                      ) THEN 'Y'
                        ELSE 'N'
                    END AS CurrentRelease,
                    CASE
                        WHEN GETDATE() > LAG(DepartureDate, 1, 0) OVER (ORDER BY
                                                                            ProductionDate,
                                                                            SequenceNum
                                                                       ) AND
                             GETDATE() < DepartureDate THEN 'Y'
                        ELSE 'N'
                    END AS NextTruck,
                    ProductionDate
                FROM
                    ShopFloor.EBS.RbaShipCalendar
            ) A
            WHERE
                A.ProductionDate =
            (
                SELECT
                    ProductionDate
                FROM
                    ShopFloor.EBS.RbaShipCalendar
                WHERE
                    LoadStartDate <= GETDATE() AND
                    LoadEndDate > GETDATE()
            )
            ORDER BY
                SequenceNum";
            return cmd;
        }

        public DataTable GetTruckList(string plantCode, SqlConnection cn)
        {
            var dt = new DataTable("TruckList");
            try
            {
                var cmd = GetTruckList();
                cmd.Connection = cn;
                _logger.Trace(Logging.DbCmdString(cmd));
                dt.Load(cmd.ExecuteReader());
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
            return dt;
        }

        /**********************************************************************
         *  TOIG Loads Today
         *********************************************************************/
        public SqlCommand GetLoads()
        {
            var cmd = new SqlCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = @"
            SELECT
                LoadNum,
                LoadStatus,
                ShipDate,
                ShipEstDate,
                COUNT(DISTINCT RackNum) AS Racks,
                COUNT(*) AS Units
            FROM
                ShopFloor.EBS.RbaShippedRackItem ri
                INNER JOIN ShopFloor.EBS.RbaShipCalendar ss
                ON  ss.LoadStartDate < ri.ShipDate AND
                    ss.LoadEndDate > ri.ShipDate AND
                    ss.ProductionDate = CAST(GETDATE() AS DATE)
            GROUP BY
                LoadNum,
                LoadStatus,
                ShipDate,
                ShipEstDate
            ORDER BY
                1;";
            return cmd;
        }

        public DataTable GetLoads(string plantCode, SqlConnection cn)
        {
            var dt = new DataTable("Loads");
            try
            {
                var cmd = GetLoads();
                cmd.Connection = cn;
                _logger.Trace(Logging.DbCmdString(cmd));
                dt.Load(cmd.ExecuteReader());
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
            return dt;
        }

        /**********************************************************************
         *  TOIG Shipped Today
         *********************************************************************/
        public SqlCommand GetShippedToday()
        {
            var cmd = new SqlCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = @"
            SELECT
                PVT.ShortCode,
                [1],
                [2],
                [3],
                [4],
                [5],
                [6],
                [7],
                [8],
                [9],
				[0]
            FROM
            (
                SELECT
                    ISNULL(ss.SequenceNum, 0) AS SequenceNum,
		            pl.SortIndex,
                    pl.ShortCode,
                    CAST(COUNT(DISTINCT RackNum) AS VARCHAR) + ' - ' + CAST(COUNT(ri.SerialNum) AS VARCHAR) AS VAL
                FROM
                    ShopFloor.EBS.RbaProductLine pl
                    LEFT OUTER JOIN ShopFloor.EBS.RbaShippedRackItem ri
                        INNER JOIN ShopFloor.EBS.RbaShipCalendar ss
                        ON  ss.ArrivalDate = ri.ShipEstDate AND
                            ss.ProductionDate = CAST(GETDATE() AS DATE)
                    ON  ri.CardinalProductType = pl.ProductLineCode
                GROUP BY
                    ss.SequenceNum,
                    pl.ShortCode,
		            Pl.SortIndex
            ) A
            PIVOT
            (
                MIN(VAL)
                FOR SequenceNum IN ([0],[1], [2], [3], [4], [5], [6], [7], [8], [9])
            ) AS PVT
            ORDER BY SortIndex";
            return cmd;
        }

        public DataTable GetShippedToday(string plantCode, SqlConnection cn)
        {
            var dt = new DataTable("ShippedToday");
            try
            {
                var cmd = GetShippedToday();
                cmd.Connection = cn;
                _logger.Trace(Logging.DbCmdString(cmd));
                dt.Load(cmd.ExecuteReader());
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
            return dt;
        }

        /**********************************************************************
         * Rba Shift Calendar
         *********************************************************************/
        public SqlCommand GetRbaShiftCalendar()
        {
            var cmd = new SqlCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = @"
            SELECT
                PVT.ShortCode,
                [1],
                [2],
                [3]
            FROM
            (
                SELECT
                    pl.ShortCode,
		            pl.SortIndex,
		            ShiftNum,
		            CONVERT(varchar(15),CAST(sc.StartDateTime AS TIME),100) + 
                    CASE
                        WHEN Active = 0 THEN ' * '
                        ELSE ' - '
                    END +  
		            CONVERT(varchar(15),CAST(sc.EndDateTime AS TIME),100) AS Range 
                FROM
                    ShopFloor.EBS.RbaShiftCalendar sc
		            INNER JOIN ShopFloor.Ebs.RbaProductLine pl
		            ON pl.ProductLineId = sc.ProductLineId
                WHERE
                    ProductionDate = CAST(GETDATE() AS DATE)
            ) SRC
            PIVOT
            (
                MIN(Range)
                FOR ShiftNum IN ([1], [2], [3])
            ) PVT
            ORDER BY PVT.SortIndex
            ";
            return cmd;
        }

        public DataTable GetRbaShiftCalendar(string plantCode, SqlConnection cn)
        {
            var dt = new DataTable("Shifts");
            try
            {
                var cmd = GetRbaShiftCalendar();
                cmd.Connection = cn;
                _logger.Trace(Logging.DbCmdString(cmd));
                dt.Load(cmd.ExecuteReader());
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
            return dt;
        }

        /**********************************************************************
         *  RBA Consumed Today - Interval
         *********************************************************************/
        public SqlCommand GetConsumedTodayByInterval()
        {
            var cmd = new SqlCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = @"
            SELECT
                PVT.ShortCode,
                [2AM],
                [4AM],
                [6AM],
                [8AM],
                [10AM],
                [12PM],
                [2PM],
                [4PM],
                [6PM],
                [8PM],
                [10PM],
                [12AM]
            FROM
            (
                SELECT
                    hr.IntervalName AS ConsumedHour,
                    pl.SortIndex,
                    pl.ShortCode,
                    CAST(COUNT(DISTINCT RackNum) AS VARCHAR) + CASE
                                                                   WHEN MAX(   CASE
                                                                                   WHEN sc.Active = 0 THEN 0
                                                                                   ELSE 1
                                                                               END
                                                                           ) = 0 THEN ' * '
                                                                   ELSE ' - '
                                                               END + CAST(COUNT(RackNum) AS VARCHAR) AS VAL
                FROM
                    ShopFloor.EBS.RbaProductLine pl
                    LEFT OUTER JOIN ShopFloor.EBS.RbaShippedRackItem ri
                        INNER JOIN ShopFloor.EBS.RbaShipCalendar ss
                        ON  ss.LoadStartDate < ri.RackConsumedDate AND
                            ss.LoadEndDate >= ri.RackConsumedDate AND
                            ss.ProductionDate = CAST(GETDATE() AS DATE)
                        INNER JOIN ShopFloor.EBS.RbaConsumedHour hr
                        ON  hr.HourId = DATEPART(HOUR, ri.RackConsumedDate)
                    ON  ri.CardinalProductType = pl.ProductLineCode
                    LEFT OUTER JOIN ShopFloor.EBS.RbaShiftCalendar sc
                    ON  sc.ProductLineId = pl.ProductLineId AND
                        sc.StartDateTime < ri.RackConsumedDate AND
                        sc.EndDateTime >= ri.RackConsumedDate
                GROUP BY
                    hr.IntervalName,
                    pl.ShortCode,
                    pl.SortIndex
            ) A
            PIVOT
            (
                MIN(VAL)
                FOR ConsumedHour IN ([2AM], [4AM], [6AM], [8AM], [10AM], [12PM], [2PM], [4PM], [6PM], [8PM], [10PM], [12AM])
            ) AS PVT
            ORDER BY
                SortIndex";
            return cmd;
        }

        public DataTable GetConsumedTodayByInterval(string plantCode, SqlConnection cn)
        {
            var dt = new DataTable("ConsumedToday");
            try
            {
                var cmd = GetConsumedTodayByInterval();
                cmd.Connection = cn;
                _logger.Trace(Logging.DbCmdString(cmd));
                dt.Load(cmd.ExecuteReader());
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
            return dt;
        }

        /**********************************************************************
         *  RBA Consumed Today - Truck
         *********************************************************************/
        public SqlCommand GetConsumedTodayByTruck()
        {
            var cmd = new SqlCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = @"
            SELECT
                PVT.ShortCode,
                [1],
                [2],
                [3],
                [4],
                [5],
                [6],
                [7],
                [8],
                [9]
            FROM
            (
                SELECT
                    ss.SequenceNum,
                    pl.SortIndex,
                    pl.ShortCode,
                    CAST(COUNT(DISTINCT RackNum) AS VARCHAR) + CASE
                                                                    WHEN MAX(   CASE
                                                                                    WHEN sc.Active = 0 THEN 0
                                                                                    ELSE 1
                                                                                END
                                                                            ) = 0 THEN ' * '
                                                                    ELSE ' - '
                                                                END + CAST(COUNT(RackNum) AS VARCHAR) AS VAL
                FROM
                    ShopFloor.EBS.RbaProductLine pl
                    LEFT OUTER JOIN ShopFloor.EBS.RbaShippedRackItem ri
                        INNER JOIN ShopFloor.EBS.RbaShipCalendar ss
                        ON  ss.LoadStartDate < ri.RackConsumedDate AND
                            ss.LoadEndDate >= ri.RackConsumedDate AND
                            ss.ProductionDate = CAST(GETDATE() AS DATE)
                    ON  ri.CardinalProductType = pl.ProductLineCode
                    LEFT OUTER JOIN ShopFloor.EBS.RbaShiftCalendar sc
                    ON  sc.ProductLineId = pl.ProductLineId AND
                        sc.StartDateTime < ri.RackConsumedDate AND
                        sc.EndDateTime >= ri.RackConsumedDate
                GROUP BY
                    ss.SequenceNum,
                    pl.ShortCode,
                    pl.SortIndex
            ) A
            PIVOT
            (
                MIN(VAL)
                FOR SequenceNum IN ([1], [2], [3], [4], [5], [6], [7], [8], [9])
            ) AS PVT
            ORDER BY
                SortIndex";
            return cmd;
        }

        public DataTable GetConsumedTodayByTruck(string plantCode, SqlConnection cn)
        {
            var dt = new DataTable("ConsumedToday");
            try
            {
                var cmd = GetConsumedTodayByTruck();
                cmd.Connection = cn;
                _logger.Trace(Logging.DbCmdString(cmd));
                dt.Load(cmd.ExecuteReader());
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
            return dt;
        }

        /**********************************************************************
         *  TOIG Hot Racks
         *********************************************************************/
        public SqlCommand GetHotRacks()
        {
            var cmd = new SqlCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = @"
            SELECT
                RackNum,
                ScheduleNum,
                BoxNum,
                RackType,
                HeaderStatus,
                PriorityCode,
                DueDate
            FROM
                ShopFloor.EBS.RbaCurrentRackItem
            WHERE
                PriorityCode NOT IN ('', '1D') AND
                RackNum > 100";
            return cmd;
        }

        public DataTable GetHotRacks(string plantCode, SqlConnection cn)
        {
            var dt = new DataTable("HotRacks");
            try
            {
                var cmd = GetHotRacks();
                cmd.Connection = cn;
                _logger.Trace(Logging.DbCmdString(cmd));
                dt.Load(cmd.ExecuteReader());
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
            return dt;
        }

        /**********************************************************************
         *  TOIG Shipment Adjustments
         *********************************************************************/
        public SqlCommand GetShipmentAdjustment()
        {
            var cmd = new SqlCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = @"
            SELECT
                ProductLineCode,
                DepartureTime,
                TargetUnits
            FROM
                ShopFloor.EBS.SpecShipmentAdjustment
            WHERE
                DepartureTime = CAST(@DepartureDate AS TIME)";
            cmd.Parameters.Add(new SqlParameter("@DepartureDate", SqlDbType.DateTime));
            return cmd;
        }

        public DataTable GetShipmentAdjustment(string plantCode, SqlConnection cn, DateTime departureDate)
        {
            var dt = new DataTable("Adjustments");
            try
            {
                var cmd = GetShipmentAdjustment();
                cmd.Parameters[0].Value = departureDate;
                cmd.Connection = cn;
                _logger.Trace(Logging.DbCmdString(cmd));
                dt.Load(cmd.ExecuteReader());
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
            return dt;
        }

    }
}
