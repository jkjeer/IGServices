using System;
using System.Runtime.Serialization;

namespace IGServices.Common.Types.Enterprise
{
    [DataContract(Namespace = "")]
    [KnownType("GetKnownTypes")]
    public class BaseItem
    {
        private static Type[] GetKnownTypes()
        {
            return new[]
                       {
                           typeof (SpacerItem),
                           typeof (GlassItem),
                           typeof (BarItem)
                       };
        }
    }
}