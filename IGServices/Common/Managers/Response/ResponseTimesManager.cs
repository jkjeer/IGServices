using System;
using System.Globalization;
using IGServices.Common.Types.Response;

namespace IGServices.Common.Managers.Response
{
    public class ResponseTimesManager
    {
        public ResponseTimes OpenResponseTime(ResponseTimes responseTimes, string name, string description, string connectionString)
        {
            var responseTime = new ResponseTime
            {
                StartTime = DateTime.Now.ToLocalTime(),
                Name = name,
                Description = description,
                ConnectionString = connectionString
            };
            responseTime.StartTimeString = responseTime.StartTime.ToString("M/dd/yyyy hh:mm:ss tt", CultureInfo.CreateSpecificCulture("en-US"));
            responseTimes.Add(responseTime);
            return responseTimes;
        }

        public ResponseTimes CloseResponseTime(ResponseTimes responseTimes, string name, string description, string connectionString)
        {
            var responseTime = responseTimes.Find(item => item.Name == name && item.Description == description && item.ConnectionString == connectionString);
            if (responseTime == null)
            {
                OpenResponseTime(responseTimes, name, description, connectionString);
                responseTime = responseTimes.Find(item => item.Name == name && item.Description == description && item.ConnectionString == connectionString);
            }
            responseTime.EndTime = DateTime.Now.ToLocalTime();
            responseTime.ElapsedSeconds = Convert.ToDecimal((responseTime.EndTime - responseTime.StartTime).TotalSeconds);
            return responseTimes;
        }

        public DateTime GetStartTime(ResponseTimes responseTimes, string name, string description, string connectionString)
        {
            var responseTime = responseTimes.Find(item => item.Name == name && item.Description == description && item.ConnectionString == connectionString);
            var startTime = responseTime.StartTime;
            return startTime;
        }

        public DateTime GetEndTime(ResponseTimes responseTimes, string name, string description, string connectionString)
        {
            var responseTime = responseTimes.Find(item => item.Name == name && item.Description == description && item.ConnectionString == connectionString);
            var endTime = responseTime.EndTime;
            return endTime;
        }

        public decimal GetElapsedSeconds(ResponseTimes responseTimes, string name, string description, string connectionString)
        {
            decimal elapsedSeconds;
            try
            {
                var responseTime = responseTimes.Find(item => item.Name == name && item.Description == description && item.ConnectionString == connectionString);
                elapsedSeconds = responseTime.ElapsedSeconds;
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.ToString());
            }
            return elapsedSeconds;
        }
    }
}
