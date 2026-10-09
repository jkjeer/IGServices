using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IGServices.Common.Managers.E1;
using IGServices.Common.Managers.Enterprise;
using IGServices.Common.Types.E1;
using IGServices.Common.Types.Enterprise;
using IGServices.Common.Types.Response;
using IGServices.Common.Utilities;
using NLog;

namespace IGServices.Common.Managers.Process
{
    public class BusinessSystemTransferProcessor
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public delegate ProcessResponse ProcessDelegate(Plant plant, BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Dictionary<string, object> parms);

        public class InvocationObject
        {
            public ProcessDelegate Processor { get; set; }
            public Dictionary<string, object> Parms { get; set; }
            public List<string> Resources { get; set; }

            public string Serialize(Plant plant)
            {
                var sb = new StringBuilder();
                sb.Append("===================================");
                sb.Append(Environment.NewLine);
                sb.Append($"Plant: {plant.PlantCode}");
                sb.Append(Environment.NewLine);
                sb.Append($"Processor: {Processor.Method.Name}");
                sb.Append(Environment.NewLine);
                foreach (var p in Parms)
                {
                    sb.Append($"Parm: {p.Key} {p.Value}");
                    sb.Append(Environment.NewLine);
                }
                foreach (var r in Resources)
                {
                    sb.Append($"Resource: {r}");
                    sb.Append(Environment.NewLine);
                }
                return sb.ToString();
            }
        }

        public static string SerializeInvocations(Plant plant, List<InvocationObject> invocations)
        {
            var sb = new StringBuilder();
            sb.Append(Environment.NewLine);
            sb.Append(Environment.NewLine);
            sb.Append("Invocations:");
            sb.Append(Environment.NewLine);
            foreach (var i in invocations)
            {
                sb.Append(i.Serialize(plant));
            }
            sb.Append(Environment.NewLine);
            return sb.ToString();
        }

        /********************************************************************************
        * INVOKE
        ********************************************************************************/
        // Subscriptions
        public static ProcessResponse Invoke(string plants, SubscriptionTypes subscriptionTypes, TimeSpan timeSpan, bool useHighWaterMark, bool parallel)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"BEGIN: {DateTime.Now.ToLocalTime()}");
            var dict = new Dictionary<Plant, List<InvocationObject>>();
            // Common Parameters
            var parms = new Dictionary<string, object>
            {
                {"TimeSpan", timeSpan},
                {"HighWaterMark", useHighWaterMark},
                {"Parallel", parallel},
                {"ServiceContext", new ServiceContext() }
            };
            var plantItems = PlantManager.GetPlants(plants);
            foreach (var plant in plantItems)
            {
                var activeList = SubscriptionManager.GetSubscriptionList(plant);
                var invocations = new List<InvocationObject>();
                foreach (var subscriptionType in subscriptionTypes)
                {
                    if (activeList.Contains(subscriptionType.SubscriptionTypeCode))
                    {
                        Logger.Info($"Adding Subscription {plant.PlantCode} {subscriptionType.SubscriptionTypeCode}");
                        try
                        {
                            var invocation = new InvocationObject
                            {
                                Processor = subscriptionType.DelegateMethod,
                                Parms = parms,
                                Resources = subscriptionType.ResourceList
                            };
                            invocations.Add(invocation);
                            if (subscriptionType.DelegateMethod == null)
                            {
                                throw new Exception($"Invocation Delegate Error {plant.PlantCode} {subscriptionType.SubscriptionTypeCode} {subscriptionType.ClassName}");
                            }
                        }
                        catch (Exception e)
                        {
                            Logger.Error($"{e.Message}: {e.StackTrace}");
                        }
                    }
                    else
                    {
                        Logger.Info($"WARNING: Plant {plant.PlantCode} Subscription {subscriptionType.SubscriptionTypeCode} is INACTIVE");
                    }
                }
                if (invocations.Any())
                {
                    Logger.Info($"Adding {invocations.Count} Invocations for {plant.PlantCode}");
                    Logger.Info(SerializeInvocations(plant, invocations));
                    dict.Add(plant, invocations);
                }
            }
            return parallel ? DownloadParallel(dict) : DownloadSynchronous(dict);
        }

        // Direct
        public static ProcessResponse Invoke(string plants, ProcessDelegate method, bool parallel, Dictionary<string, object> parms, List<string> resources)
        {
            var dict = new Dictionary<Plant, List<InvocationObject>>();
            var plantItems = PlantManager.GetPlants(plants);
            foreach (var plant in plantItems)
            {
                var invocations = new List<InvocationObject>();
                var invocation = new InvocationObject();
                invocation.Processor = method;
                invocation.Parms = parms;
                invocation.Resources = resources;
                invocations.Add(invocation);
                if (invocations.Any())
                {
                    Logger.Info($"Adding {invocations.Count} Invocations for {plant.PlantCode}");
                    Logger.Info(SerializeInvocations(plant, invocations));
                    dict.Add(plant, invocations);
                }
            }
            return parallel ? DownloadParallel(dict) : DownloadSynchronous(dict);
        }

        /********************************************************************************
        * PARALLEL
        ********************************************************************************/
        public static ProcessResponse DownloadParallel(Dictionary<Plant, List<InvocationObject>> plantJobs)
        {
            var parallelResponse = new ProcessResponse("Parallel Processor", true);
            parallelResponse.IsParallel = true;
            var parallelProcessResponses = new ConcurrentQueue<ProcessResponse>();
            var exceptions = new ConcurrentQueue<Exception>();
            try
            {
                Parallel.ForEach(plantJobs, kvp =>
                {
                    Logger.Info($"{kvp.Key.PlantCode} Parallel Processor");
                    var response = new ProcessResponse($"{kvp.Key.PlantCode} Parallel Processor");
                    var session = new BusinessSystemSession();
                    var sqlCns = new Dictionary<string, SqlConnection>();
                    try
                    {
                        var step1 = new ProcessStep($"Resolve Plant {kvp.Key.PlantCode}");
                        try
                        {
                            response.Plant = kvp.Key;
                        }
                        catch (Exception e)
                        {
                            //Logger.Error($"ERROR: {e.Message}: {e.StackTrace}");
                            step1.Errors.Add(new Error(e));
                        }
                        step1.Done();
                        response.AddStep(step1);
                        var step2 = new ProcessStep($"Get {kvp.Key.PlantCode} Database Connections");
                        try
                        {
                            // Biz Session
                            if (kvp.Value.Any(x => x.Resources.Contains("E1.PD")))
                            {
                                session = BusinessSystemSessionManager.GetBusinessSession(ConfigurationManager.AppSettings["PrimaryEnvironment"]);
                            }
                            else
                            {
                                session = BusinessSystemSessionManager.GetBusinessSession(ConfigurationManager.AppSettings["ReplicationEnvironment"]);
                            }

                            // Plant Integration Connection
                            if (kvp.Value.Any(x => x.Resources.Contains("PLANT.INTEGRATION")))
                            {
                                var cnPlantInt = ConnectionManager.GetSqlConnectionFromKey(kvp.Key.PlantCode + ".INTEGRATION");
                                cnPlantInt.Open();
                                sqlCns.Add("PLANT.INTEGRATION", cnPlantInt);
                            }
                            // Plant Shop Floor Connection
                            if (kvp.Value.Any(x => x.Resources.Contains("PLANT.SHOPFLOOR")))
                            {
                                var cnPlantSf = ConnectionManager.GetSqlConnectionFromKey(kvp.Key.PlantCode + ".SHOPFLOOR", false);
                                cnPlantSf.Open();
                                sqlCns.Add("PLANT.SHOPFLOOR", cnPlantSf);
                            }
                            // Plant Shop Floor Scheduling Connection
                            if (kvp.Value.Any(x => x.Resources.Contains("PLANT.SHOPFLOORSCHEDULING")))
                            {
                                var cnPlantSfSched = ConnectionManager.GetSqlConnectionFromKey(kvp.Key.PlantCode + ".SHOPFLOORSCHEDULING", false);
                                cnPlantSfSched.Open();
                                sqlCns.Add("PLANT.SHOPFLOORSCHEDULING", cnPlantSfSched);
                            }
                            // Plant Clear Path Connection
                            if (kvp.Value.Any(x => x.Resources.Contains("PLANT.CLEARPATH")))
                            {
                                var cnPlantCp = ConnectionManager.GetSqlConnectionFromKey(kvp.Key.PlantCode + ".CLEARPATH", false);
                                cnPlantCp.Open();
                                sqlCns.Add("PLANT.CLEARPATH", cnPlantCp);
                            }
                            // Corporate Integration Connection
                            if (kvp.Value.Any(x => x.Resources.Contains("CORP.INTEGRATION")))
                            {
                                var cnCorpInt = ConnectionManager.GetSqlConnectionFromKey("CORP.INTEGRATION");
                                cnCorpInt.Open();
                                sqlCns.Add("CORP.INTEGRATION", cnCorpInt);
                            }
                        }
                        catch (Exception e)
                        {
                            step2.Errors.Add(new Error($"Plant: {kvp.Key.PlantCode} Error: {e.Message} {SerializeInvocations(kvp.Key, kvp.Value)}"));
                            Logger.Error($"ERROR: {e.Message}: {e.StackTrace}");
                        }

                        Logger.Info($"{kvp.Key.PlantCode} SQL Connections {String.Join(", ", sqlCns.Keys)}");

                        step2.Done();
                        response.AddStep(step2);
                        if (!response.HasErrors())
                        {
                            foreach (var invocation in kvp.Value)
                            {
                                var connectionState = true;
                                foreach (var sqlCn in sqlCns)
                                {
                                    if (sqlCn.Value.State != ConnectionState.Open)
                                    {
                                        connectionState = false;
                                        Logger.Info($"Parallel Call {kvp.Key.PlantCode} {invocation.Processor.Method.Name} Processor connection {sqlCn.Value.DataSource}.{sqlCn.Value.Database} is NOT Open");
                                        parallelResponse.Errors.Add(new Error($"Parallel Call {kvp.Key.PlantCode} {invocation.Processor.Method.Name} Processor connection {sqlCn.Value.DataSource}.{sqlCn.Value.Database} is NOT Open"));
                                    }
                                }
                                if (connectionState)
                                {
                                    var step3 = new ProcessStep();
                                    try
                                    {
                                        Logger.Info($"Parallel Call {kvp.Key.PlantCode} {invocation.Processor.Method.Name} Processor");
                                        step3.Name = $"Call {kvp.Key.PlantCode} {invocation.Processor.Method.Name} Processor";
                                        step3.ProcessResponses.Add(invocation.Processor(kvp.Key, session, sqlCns, invocation.Parms));
                                    }
                                    catch (Exception e)
                                    {
                                        Logger.Error($"ERROR: {e.Message}: {e.StackTrace}");
                                        step3.Errors.Add(new Error(e));
                                    }
                                    finally
                                    {
                                        step3.Done();
                                        response.AddStep(step3);
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.Error($"ERROR: {e.Message}: {e.StackTrace}");
                        response.Errors.Add(new Error(e));
                    }
                    finally
                    {
                        session.Close();
                        foreach (var sqlCn in sqlCns)
                        {
                            sqlCn.Value.Close();
                        }
                        response.Close();
                    }
                    parallelProcessResponses.Enqueue(response);
                    // parallelResponse.ProcessResponses.Add(response);
                });
            }
            catch (AggregateException e)
            {
                foreach (var ex in e.InnerExceptions)
                {
                    parallelResponse.Errors.Add(new Error(ex));
                }
            }

            foreach (var pr in parallelProcessResponses)
            {
                parallelResponse.ProcessResponses.Add(pr);
            }

            parallelResponse.Close();
            return parallelResponse;
        }

        /********************************************************************************
        * SYNCHRONOUS
        ********************************************************************************/
        public static ProcessResponse DownloadSynchronous(Dictionary<Plant, List<InvocationObject>> invocations)
        {
            var synchResponse = new ProcessResponse("Synchronous Processor", true);
            synchResponse.IsParallel = false;
            try
            {
                foreach (var kvp in invocations)
                {
                    Logger.Info($"{kvp.Key.PlantCode} Synchronous Processor");
                    var sqlCns = new Dictionary<string, SqlConnection>();
                    var response = new ProcessResponse($"{kvp.Key} Synchronous Processor");
                    var session = new BusinessSystemSession();
                    try
                    {
                        var step1 = new ProcessStep($"Resolve Plant {kvp.Key.PlantCode}");
                        try
                        {
                            response.Plant = kvp.Key;
                        }
                        catch (Exception e)
                        {
                            Logger.Error($"ERROR: {e.Message}: {e.StackTrace}");
                            step1.Errors.Add(new Error(e));
                        }
                        step1.Done();
                        response.AddStep(step1);
                        var step2 = new ProcessStep($"Get {kvp.Key.PlantCode} Database Connections");
                        try
                        {
                            // Biz Session
                            if (kvp.Value.Any(x => x.Resources.Contains("E1.PD")))
                            {
                                session = BusinessSystemSessionManager.GetBusinessSession(ConfigurationManager.AppSettings["PrimaryEnvironment"]);
                            }
                            else
                            {
                                session = BusinessSystemSessionManager.GetBusinessSession(ConfigurationManager.AppSettings["ReplicationEnvironment"]);
                            }

                            // Plant Integration Connection
                            if (kvp.Value.Any(x => x.Resources.Contains("PLANT.INTEGRATION")))
                            {
                                var cnPlantInt = ConnectionManager.GetSqlConnectionFromKey(kvp.Key.PlantCode + ".INTEGRATION");
                                cnPlantInt.Open();
                                sqlCns.Add("PLANT.INTEGRATION", cnPlantInt);
                            }
                            // Plant Shop Floor Connection
                            if (kvp.Value.Any(x => x.Resources.Contains("PLANT.SHOPFLOOR")))
                            {
                                var cnPlantSf = ConnectionManager.GetSqlConnectionFromKey(kvp.Key.PlantCode + ".SHOPFLOOR", false);
                                cnPlantSf.Open();
                                sqlCns.Add("PLANT.SHOPFLOOR", cnPlantSf);
                            }
                            // Plant Shop Floor Scheduling Connection
                            if (kvp.Value.Any(x => x.Resources.Contains("PLANT.SHOPFLOORSCHEDULING")))
                            {
                                var cnPlantSfSched = ConnectionManager.GetSqlConnectionFromKey(kvp.Key.PlantCode + ".SHOPFLOORSCHEDULING", false);
                                cnPlantSfSched.Open();
                                sqlCns.Add("PLANT.SHOPFLOORSCHEDULING", cnPlantSfSched);
                            }
                            // Plant Clear Path Connection
                            if (kvp.Value.Any(x => x.Resources.Contains("PLANT.CLEARPATH")))
                            {
                                var cnPlantCp = ConnectionManager.GetSqlConnectionFromKey(kvp.Key.PlantCode + ".CLEARPATH", false);
                                cnPlantCp.Open();
                                sqlCns.Add("PLANT.CLEARPATH", cnPlantCp);
                            }
                            // Corporate Integration Connection
                            if (kvp.Value.Any(x => x.Resources.Contains("CORP.INTEGRATION")))
                            {
                                var cnCorpInt = ConnectionManager.GetSqlConnectionFromKey("CORP.INTEGRATION");
                                cnCorpInt.Open();
                                sqlCns.Add("CORP.INTEGRATION", cnCorpInt);
                            }
                        }
                        catch (Exception e)
                        {
                            step2.Errors.Add(new Error($"Plant: {kvp.Key.PlantCode} Error: {e.Message} {SerializeInvocations(kvp.Key, kvp.Value)}"));
                            Logger.Error($"ERROR: {e.Message}: {e.StackTrace}");
                        }
                        Logger.Info($"{kvp.Key.PlantCode} SQL Connections {String.Join(", ", sqlCns.Keys)}");
                        step2.Done();
                        response.AddStep(step2);
                        if (!response.HasErrors())
                        {
                            foreach (var invocation in kvp.Value)
                            {
                                var connectionState = true;
                                foreach (var sqlCn in sqlCns)
                                {
                                    if (sqlCn.Value.State != ConnectionState.Open)
                                    {
                                        connectionState = false;
                                        Logger.Info($"Synchronous Call {kvp.Key.PlantCode} {invocation.Processor.Method.Name} Processor connection {sqlCn.Value.DataSource}.{sqlCn.Value.Database} is NOT Open");
                                        synchResponse.Errors.Add(new Error($"Synchronous Call {kvp.Key.PlantCode} {invocation.Processor.Method.Name} Processor connection {sqlCn.Value.DataSource}.{sqlCn.Value.Database} is NOT Open"));
                                    }
                                }
                                if (connectionState)
                                {
                                    var step3 = new ProcessStep();
                                    try
                                    {
                                        Logger.Info($"Synchronous Call {kvp.Key.PlantCode} {invocation.Processor.Method.Name} Processor");
                                        step3.Name = $"Call {kvp.Key.PlantCode} {invocation.Processor.Method.Name} Processor";
                                        step3.ProcessResponses.Add(invocation.Processor(kvp.Key, session, sqlCns, invocation.Parms));
                                    }
                                    catch (Exception e)
                                    {
                                        Logger.Error($"ERROR: {e.Message}: {e.StackTrace}");
                                        step3.Errors.Add(new Error(e));
                                    }
                                    finally
                                    {
                                        step3.Done();
                                        response.AddStep(step3);
                                    }
                                    //Logger.Info($"Synchronous Call {kvp.Key.PlantCode} {invocation.Processor.Method.Name} Processor");
                                    //var step3 = new ProcessStep($"Call {kvp.Key.PlantCode} {invocation.Processor.Method.Name} Processor");
                                    //try
                                    //{
                                    //    step3.ProcessResponses.Add(invocation.Processor(kvp.Key, session, sqlCns, invocation.Parms));
                                    //}
                                    //catch (Exception e)
                                    //{
                                    //    Logger.Error($"ERROR: {e.Message}: {e.StackTrace}");
                                    //    step3.Errors.Add(new Error(e));
                                    //}
                                    //finally
                                    //{
                                    //    step3.Done();
                                    //    response.AddStep(step3);
                                    //}
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.Error($"ERROR: {e.Message}: {e.StackTrace}");
                        response.Errors.Add(new Error(e));
                    }
                    finally
                    {
                        session.Close();
                        foreach (var sqlCn in sqlCns)
                        {
                            sqlCn.Value.Close();
                        }
                        response.Close();
                    }
                    synchResponse.ProcessResponses.Add(response);
                }
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {e.Message}: {e.StackTrace}");
                synchResponse.Errors.Add(new Error(e));
            }
            synchResponse.Close();
            return synchResponse;
        }
    }
}