using System;

namespace IGServices.Common.Utilities
{
    public static class CastDbNull
    {
        public static T To<T>(object value, T defaultValue)
        {
            try
            {
                return (value != DBNull.Value) ? (T)value : defaultValue;
            }
            catch (Exception e)
            {
                throw new Exception($"{e.Message} [{typeof(T).Name}] [{value.GetType().Name}] [{value.ToString()}]");
            }
        }
    }

    public static class CastNull
    {
        public static T To<T>(object value, T defaultValue)
        {
            return (value != null) ? (T)Convert.ChangeType(value, typeof(T)) : defaultValue;
        }

    }

}
