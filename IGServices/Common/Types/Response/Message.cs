using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Serialization;
using System.ServiceModel;
using NLog;

namespace IGServices.Common.Types.Response
{
    [CollectionDataContract(Namespace = "")]
    public class Messages : List<Message>
    {
        public List<Message> Errors()
        {
            return this.Where(x => x.Severity == Message.SeverityType.Error).ToList();
        }

        public List<Message> Warnings()
        {
            return this.Where(x => x.Severity == Message.SeverityType.Warning).ToList();
        }

        public List<Message> Infos()
        {
            return this.Where(x => x.Severity == Message.SeverityType.Info).ToList();
        }

        public void Add(string name, string message, Message.SeverityType severity)
        {
            this.Add(new Message(name, message, severity));
        }

        public void Add(List<Message> messages)
        {
            this.AddRange(messages);
        }
    }

    [DataContract(Namespace = "")]
    public class Message
    {
        Logger logger = LogManager.GetCurrentClassLogger();

        public enum SeverityType
        {
            Error,
            Warning,
            Info
        }

        public Message()
        {
        }

        public Message(string name)
        {
            Name = name;
        }

        public Message(string name, string value)
        {
            Name = name;
            Value = value;
            Severity = SeverityType.Info;
        }

        public Message(string name, string value, SeverityType severity)
        {
            Name = name;
            Value = value;
            Severity = severity;
            if (severity == SeverityType.Warning)
            {
                LogEvent(name, value, severity);
            }
        }

        [DataMember(Order = 10)]
        public string Name { get; set; }
        [DataMember(Order = 20)]
        public string Value { get; set; }
        [DataMember(Order = 30)]
        public SeverityType Severity { get; set; }

        private void LogEvent(string name, string value, Message.SeverityType severity)
        {
            if (OperationContext.Current == null)
            {
                OperationContext.Current = (OperationContext)CallContext.LogicalGetData("WcfOperationContext");
            }
            OperationContext operationContext = OperationContext.Current;

            LogEventInfo logEvent = new LogEventInfo();
            logEvent.Level = LogLevel.Error;
            logEvent.LoggerName = this.GetType().FullName;
            logEvent.Properties.Add("EVENT_DATE", DateTime.Now);
            logEvent.Properties.Add("EVENT_TYPE", severity.ToString());
            logEvent.Properties.Add("LOG_INTERFACE", "");
            logEvent.Properties.Add("ARGUMENTS", "");
            logEvent.Properties.Add("OPERATION_NAME", "");
            try
            {
                logEvent.Properties.Add("USER_NAME", operationContext.ServiceSecurityContext.WindowsIdentity.Name);
            }
            catch
            {
                logEvent.Properties.Add("USER_NAME", "");
            }
            try
            {
                logEvent.Properties.Add("CONTRACT_NAME", operationContext.EndpointDispatcher.ContractName);
            }
            catch
            {
                logEvent.Properties.Add("CONTRACT_NAME", "");
            }
            try
            {
                logEvent.Properties.Add("HOST_NAME", operationContext.InstanceContext.Host.Description?.Name);
            }
            catch
            {
                logEvent.Properties.Add("HOST_NAME", "");
            }
            try
            {
                logEvent.Properties.Add("SCHEME", operationContext.EndpointDispatcher.EndpointAddress.Uri.Scheme);
            }
            catch
            {
                logEvent.Properties.Add("SCHEME", "");
            }
            try
            {
                logEvent.Properties.Add("PORT", operationContext.EndpointDispatcher.EndpointAddress.Uri.Port);
            }
            catch
            {
                logEvent.Properties.Add("PORT", "");
            }
            try
            {
                logEvent.Properties.Add("SERVER_ADDRESS", operationContext.Channel.LocalAddress.Uri.ToString());
            }
            catch
            {
                logEvent.Properties.Add("SERVER_ADDRESS", "");
            }
            try
            {
                logEvent.Properties.Add("SESSION_ID", operationContext.SessionId);
            }
            catch
            {
                logEvent.Properties.Add("SESSION_ID", "");
            }
            logEvent.Properties.Add("ELAPSED_MS", 0);
            logEvent.Properties.Add("COMMENTS", $"{name} {value} {severity.ToString()}");
            logger.Log(logEvent);
        }
    }
}
