using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using IBM.Data.DB2.iSeries;
using IGServices.Common.Managers.Process;
using IGServices.Common.Types.E1;
using IGServices.Common.Types.Enterprise;
using NLog;

namespace IGServices.Common.Utilities
{
    public class SubscriptionManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public SubscriptionManager(BusinessSystemSession e1Session)
        {
            var hash = new Dictionary<string, iDB2Command>
            {
                //{"SubscriptionManager.GetSubscriptions", GetSubscriptions()}
            };
            foreach (var keyName in hash.Keys.Where(keyName => !e1Session.DbCommands.ContainsKey(keyName)))
            {
                e1Session.DbCommands.Add(keyName, hash[keyName]);
            }
        }

        /******************************************************************
        * Subscriptions
        ******************************************************************/
        public static SqlCommand GetSubscriptionList()
        {
            var cmd = new SqlCommand
            {
                CommandType = CommandType.Text,
                CommandText = @"
                    SELECT
                        sub.SUBSCRIPTION_TYPE_CD
                    FROM
                        SVCIG.SUBSCRIPTION sub
                        INNER JOIN SVCIG.SUBSCRIPTION_TYPE st
                        ON  st.SUBSCRIPTION_TYPE_CD = sub.SUBSCRIPTION_TYPE_CD
                        INNER JOIN SVCIG.PLANT pt
                        ON  pt.PLANT_ALPHA_CD = sub.PLANT_ALPHA_CD
                    WHERE
                        sub.IS_ACTIVE = 1 AND
                        st.IS_ACTIVE = 1 AND
                        pt.IS_ACTIVE = 1 AND
                        sub.PLANT_ALPHA_CD = @PLANT_CODE"
            };
            cmd.Parameters.Add(new SqlParameter("@PLANT_CODE", SqlDbType.VarChar, 12));
            return cmd;
        }

        public static List<string> GetSubscriptionList(Plant plant)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"BEGIN: {DateTime.Now.ToLocalTime()}  Plant: {plant.PlantCode}");
            var subscriptions = new List<string>();
            SqlConnection cn = new SqlConnection();
            try
            {
                cn = ConnectionManager.GetSqlConnectionFromKey("CORP.INTEGRATION");
                cn.Open();
                var cmd = GetSubscriptionList();
                cmd.Parameters["@PLANT_CODE"].Value = plant.PlantCode;
                cmd.Connection = cn;
                Logger.Trace(Logging.DbCmdString(cmd));
                using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult))
                {
                    while (reader.Read())
                    {
                        subscriptions.Add(CastDbNull.To(reader["SUBSCRIPTION_TYPE_CD"], string.Empty).Trim());
                    }

                    reader.Close();
                }
            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + cn.ConnectionString + " JobName: " + e.Message + ": " + e.StackTrace);
                throw new Exception(cn.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
            }
            finally
            {
                cn.Close();
                Logger.Info("END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS   Plant: " + plant.PlantCode);
            }

            return subscriptions;
        }

        /******************************************************************
         * Subscription Types
         ******************************************************************/
        public static SubscriptionTypes GetSubscriptionTypes(string code)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"BEGIN: {DateTime.Now.ToLocalTime()}");
            var subscriptionTypes = new SubscriptionTypes();
            try
            {
                var subscriptionTypeItemCache = GetSubscriptionTypeCache();
                var subscriptionTypeList = code.Split(',').ToList();
                foreach (var subscriptionTypeItem in subscriptionTypeList)
                {
                    var subscriptionType = subscriptionTypeItemCache.Where(t => t.GroupCode == string.Empty).Where(s => string.Equals(s.SubscriptionType.SubscriptionTypeCode, subscriptionTypeItem, StringComparison.CurrentCultureIgnoreCase)).Select(z => z.SubscriptionType).FirstOrDefault();
                    if (subscriptionType != null)
                    {
                        subscriptionTypes.Add(subscriptionType);
                    }
                    else
                    {
                        var subscriptionTypeItems = subscriptionTypeItemCache.Where(t => string.Equals(t.GroupCode, subscriptionTypeItem, StringComparison.CurrentCultureIgnoreCase)).Select(x => x.SubscriptionType).ToList();
                        subscriptionTypes.AddRange(subscriptionTypeItems);
                    }
                    if (!subscriptionTypes.Any())
                    {
                        throw new Exception($"Error: Subscription Type {code} not found.");
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception($"{e.Message}: {e.StackTrace}");
            }
            finally
            {
                Logger.Info("END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");
            }
            return subscriptionTypes;
        }

        /******************************************************************
         * Subscription Type Cache
         ******************************************************************/
        public static SubscriptionTypeItems GetSubscriptionTypeCache()
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"BEGIN: {DateTime.Now.ToLocalTime()}");
            ObjectCache cache = MemoryCache.Default;

            if (cache.Contains("SubscriptionTypeItems")) return (SubscriptionTypeItems)cache.Get("SubscriptionTypeItems");
            else
            {
                var cn = new SqlConnection();
                SubscriptionTypeItems subscriptionTypeItems;
                try
                {
                    cn = ConnectionManager.GetSqlConnectionFromKey("CORP.INTEGRATION");
                    cn.Open();
                    subscriptionTypeItems = GetSubscriptionTypeItems(cn);

                    // Store data in the cache
                    CacheItemPolicy cacheItemPolicy = new CacheItemPolicy();
                    //cacheItemPolicy.AbsoluteExpiration = DateTime.Now.AddHours(1.0);
                    cache.Add("SubscriptionTypeItems", subscriptionTypeItems, cacheItemPolicy);
                    Logger.Info(XmlSerializationUtility.SerializeObject(Encoding.UTF8, subscriptionTypeItems, true));
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message);
                }
                finally
                {
                    cn.Close();
                    Logger.Info("END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");
                }
                return subscriptionTypeItems;
            }
        }

        public static SubscriptionTypeItems RefreshSubscriptionTypeCache()
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"BEGIN: {DateTime.Now.ToLocalTime()}");
            ObjectCache cache = MemoryCache.Default;
            var cn = new SqlConnection();
            SubscriptionTypeItems subscriptionTypeItems;
            try
            {
                cn = ConnectionManager.GetSqlConnectionFromKey("CORP.INTEGRATION");
                cn.Open();
                subscriptionTypeItems = GetSubscriptionTypeItems(cn);

                // Remove existing cache
                cache.Remove("SubscriptionTypeItems");

                // Store data in the cache
                CacheItemPolicy cacheItemPolicy = new CacheItemPolicy();
                cache.Add("SubscriptionTypeItems", subscriptionTypeItems, cacheItemPolicy);
                Logger.Info(XmlSerializationUtility.SerializeObject(Encoding.UTF8, (SubscriptionTypeItems)cache.Get("SubscriptionTypeItems"), true));
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                cn.Close();
                Logger.Info("END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");
            }

            return subscriptionTypeItems;
        }

        /******************************************************************
         * Subscription Type Items
         ******************************************************************/
        public static SqlCommand GetSubscriptionTypeItems()
        {
            var cmd = new SqlCommand
            {
                CommandType = CommandType.Text,
                CommandText = @"
                    SELECT
                        SG.GROUP_CD,
	                    ST.SUBSCRIPTION_TYPE_CD,
	                    ST.METHOD_NAME,
                        ST.CLASS_NAME,
                        ST.ARGUMENTS,
                        ST.RESOURCES
                    FROM
                        SVCIG.SUBSCRIPTION_TYPE_GROUP SG
                        INNER JOIN SVCIG.SUBSCRIPTION_TYPE_GROUP_XREF SGX
                        ON  SGX.GROUP_CD = SG.GROUP_CD
	                    INNER JOIN SVCIG.SUBSCRIPTION_TYPE ST
	                    ON ST.SUBSCRIPTION_TYPE_CD = SGX.SUBSCRIPTION_TYPE_CD
					WHERE [ST].[METHOD_NAME] IS NOT NULL
						AND [ST].[CLASS_NAME] IS NOT NULL
                    UNION
                    SELECT
                        '' AS GROUP_CD,
	                    ST.SUBSCRIPTION_TYPE_CD,
	                    ST.METHOD_NAME,
                        ST.CLASS_NAME,
                        ST.ARGUMENTS,
                        ST.RESOURCES
                    FROM
                        SVCIG.SUBSCRIPTION_TYPE ST
					WHERE [ST].[METHOD_NAME] IS NOT NULL
						AND [ST].[CLASS_NAME] IS NOT NULL
                "
            };
            return cmd;
        }

        public static SubscriptionTypeItems GetSubscriptionTypeItems(SqlConnection cn)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"BEGIN: {DateTime.Now.ToLocalTime()}");
            var subscriptionTypeItems = new SubscriptionTypeItems();
            try
            {
                var cmd = GetSubscriptionTypeItems();
                cmd.Connection = cn;
                Logger.Trace(Logging.DbCmdString(cmd));
                using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult))
                {
                    while (reader.Read())
                    {
                        var subscriptionTypeItem = new SubscriptionTypeItem();
                        subscriptionTypeItem.GroupCode = CastDbNull.To(reader["GROUP_CD"], string.Empty).Trim();
                        var subscriptionType = new SubscriptionType();
                        subscriptionType.SubscriptionTypeCode = CastDbNull.To(reader["SUBSCRIPTION_TYPE_CD"], string.Empty).Trim();
                        subscriptionType.MethodName = CastDbNull.To(reader["METHOD_NAME"], string.Empty).Trim();
                        subscriptionType.ClassName = CastDbNull.To(reader["CLASS_NAME"], string.Empty).Trim();
                        subscriptionType.Arguments = CastDbNull.To(reader["ARGUMENTS"], string.Empty).Trim();
                        subscriptionType.ArgumentsHash = GetArgumentsHash(subscriptionType.Arguments);
                        subscriptionType.DelegateMethod = GetDownloadDelegateDynamic(subscriptionType.ClassName, subscriptionType.MethodName);
                        if (subscriptionType.DelegateMethod != null)
                        {
                            subscriptionType.DelegateMethodName = subscriptionType.DelegateMethod.Method.Name;
                        }

                        subscriptionType.Resources = CastDbNull.To(reader["RESOURCES"], string.Empty).Trim();
                        subscriptionType.ResourceList = subscriptionType.Resources.Split(',').Select(p => p.Trim()).ToList();
                        subscriptionTypeItem.SubscriptionType = subscriptionType;
                        subscriptionTypeItems.Add(subscriptionTypeItem);
                    }

                    reader.Close();
                }
            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + cn.ConnectionString + " JobName: " + e.Message + ": " + e.StackTrace);
                throw new Exception(cn.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
            }
            finally
            {
                Logger.Info("END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");
            }
            return subscriptionTypeItems;
        }

        /******************************************************************
         * Delegates
         ******************************************************************/
        public static BusinessSystemTransferProcessor.ProcessDelegate GetDownloadDelegateDynamic(string className, string methodName)
        {
            try
            {
                // Instance
                //var classFullName = $"IGServices.{className}";
                //var t = Activator.CreateInstance(null, classFullName).Unwrap();
                //var d = (BusinessSystemTransferProcessor.ProcessDelegate)Delegate.CreateDelegate(typeof(BusinessSystemTransferProcessor.ProcessDelegate), t, methodName);
                //return d;

                // Static
                Type t = Type.GetType($"IGServices.{className}");
                var d = (BusinessSystemTransferProcessor.ProcessDelegate)Delegate.CreateDelegate(typeof(BusinessSystemTransferProcessor.ProcessDelegate), t, methodName);
                return d;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /******************************************************************
         * Arguments
         ******************************************************************/
        public static Dictionary<string, object> GetArgumentsHash(string argumentString)
        {
            var dict = new Dictionary<string, object>();
            try
            {
                var arguments = argumentString.Split('&').ToList();
                if (arguments.Count > 0 && arguments[0] != string.Empty)
                {
                    foreach (var argument in arguments)
                    {
                        // tokenize name/value
                        var token = argument.Split('=');
                        dict.Add(token[0], token[1]);
                    }
                }
                return dict;
            }
            catch (Exception e) { throw new Exception(e.Message); }
        }

    }
}