using IGServices.Common.Types.Enterprise;

namespace IGServices.Common.Managers.Enterprise
{
    public class GridPlacementsManager
    {
        public GridPlacements GetVerticalPlacements(ItemAttributes segments)
        {
            var gridPlacements = new GridPlacements();
            var positions = new[]
                                {
                                    "Z58IGVP1", "Z58IGVP2", "Z58IGVP3", "Z58IGVP4", "Z58IGVP5",
                                    "Z58IGVP6", "Z58IGVP7", "Z58IGVP8", "Z58IGVP9", "Z58IGV10",
                                    "Z58IGV11", "Z58IGV12", "Z58IGV13", "Z58IGV14", "Z58IGV15",
                                    "Z58IGV16", "Z58IGV17", "Z58IGV18", "Z58IGV19", "Z58IGV20"
                                };
            var angles = new[]
                             {
                                 "Z586220", "Z586250", "Z586280", "Z586310", "Z586340",
                                 "Z586370", "Z586400", "Z586430", "Z586460", "Z586490",
                                 "Z586520", "Z586550", "Z586580", "Z586610", "Z586640",
                                 "Z586670", "Z586700", "Z586730", "Z586760", "Z586790"
                             };
            for (var i = 0; i <= 19; i++)
            {
                var placement = new GridPlacement
                {
                    OrdinalNum = i + 1,
                    Position = segments.GetDecimalAttributeValue(positions[i]),
                    Angle = segments.GetDecimalAttributeValue(angles[i])
                };
                if (placement.Position + placement.Angle > 0)
                {
                    gridPlacements.Add(placement);
                }
            }
            return gridPlacements;
        }

        public GridPlacements GetHorizontalPlacements(ItemAttributes segments)
        {
            var gridPlacements = new GridPlacements();
            var positions = new[]
                                {
                                    "Z58IGHP1", "Z58IGHP2", "Z58IGHP3", "Z58IGHP4", "Z58IGHP5",
                                    "Z58IGHP6", "Z58IGHP7", "Z58IGHP8", "Z58IGHP9", "Z58IGH10",
                                    "Z58IGH11", "Z58IGH12", "Z58IGH13", "Z58IGH14", "Z58IGH15",
                                    "Z58IGH16", "Z58IGH17", "Z58IGH18", "Z58IGH19", "Z58IGH20"
                                };
            var angles = new[]
                             {
                                 "Z586820", "Z586850", "Z586880", "Z586910", "Z586940",
                                 "Z586970", "Z587000", "Z587030", "Z587060", "Z587090",
                                 "Z587120", "Z587150", "Z587180", "Z587210", "Z587240",
                                 "Z587270", "Z587300", "Z587330", "Z587360", "Z587390"
                             };
            for (var i = 0; i <= 19; i++)
            {
                var placement = new GridPlacement
                {
                    OrdinalNum = i + 1,
                    Position = segments.GetDecimalAttributeValue(positions[i]),
                    Angle = segments.GetDecimalAttributeValue(angles[i])
                };
                if (placement.Position + placement.Angle > 0)
                {
                    gridPlacements.Add(placement);
                }
            }
            return gridPlacements;
        }
    }
}