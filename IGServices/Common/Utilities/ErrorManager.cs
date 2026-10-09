using System;
using System.Runtime.Serialization;
using IGServices.Common.Types.Response;

namespace IGServices.Common.Utilities
{
    [DataContract(Namespace = "")]
    public class ErrorManager
    {
        public static Errors AddError(Errors errors, Exception exception)
        {
            try
            {
                if (errors == null)
                {
                    errors = new Errors();
                }
                var err = new Error(exception);
                if (err.Message.Contains("Timeout Expired") & err.Message.Length > 550) err.Message = err.Message.Substring(0, 550);
                if (err.Message.Contains("transport-level error") & err.Message.Length > 232) err.Message = err.Message.Substring(0, 232);
                if (err.Message.Contains("an error occurred during the pre-login handshake") & err.Message.Length > 586) err.Message = err.Message.Substring(0, 586);
                errors.Add(err);
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.ToString());
            }
            return errors;
        }

        public static Errors AddError(Errors errors, string message)
        {
            try
            {
                if (errors == null)
                {
                    errors = new Errors();
                }
                var err = new Error(message);
                if (err.Message.Contains("Timeout Expired") & err.Message.Length > 550) err.Message = err.Message.Substring(0, 550);
                if (err.Message.Contains("an error occurred during the pre-login handshake") & err.Message.Length > 586) err.Message = err.Message.Substring(0, 586);
                if (err.Message.Contains("transport-level error") & err.Message.Length > 232) err.Message = err.Message.Substring(0, 232);
                errors.Add(err);
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.ToString());
            }
            return errors;
        }
    }
}
 