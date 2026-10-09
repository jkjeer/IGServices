using System;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace IGServices.Common.Types.Enterprise
{
    [DataContract(Namespace = "")]
    public class ItemAttribute
    {
        [DataMember(Order = 10)]
        public string Name { get; set; }
        [DataMember(Order = 20)]
        public string Value { get; set; }
        [DataMember(Order = 30)]
        public string Desc1 { get; set; }
        [DataMember(Order = 40)]
        public string Desc2 { get; set; }
        [DataMember(Order = 50)]
        public string Desc3 { get; set; }
    }

    [CollectionDataContract(Namespace = "")]
    public class ItemAttributes : KeyedCollection<string, ItemAttribute>
    {
        protected override string GetKeyForItem(ItemAttribute attribute)
        {
            return attribute.Name;
        }

        public decimal GetIntAttributeValue(string attributeName)
        {
            if (Contains(attributeName))
            {
                return Convert.ToInt32(this[attributeName].Value.Trim());
            }
            return 0;
        }

        public decimal GetDecimalAttributeValue(string attributeName)
        {
            if (Contains(attributeName))
            {
                return Convert.ToDecimal(this[attributeName].Value.Trim());
            }
            return 0;
        }

        public string GetStringAttributeValue(string attributeName, string defaultValue = null)
        {
            if (Contains(attributeName))
            {
                return this[attributeName].Value.Trim();
            }
            return defaultValue;
        }

        public string GetStringAttributeDesc1(string attributeName)
        {
            if (Contains(attributeName))
            {
                return this[attributeName].Desc1.Trim();
            }
            return null;
        }

        public string GetStringAttributeDesc2(string attributeName)
        {
            if (Contains(attributeName))
            {
                return this[attributeName].Desc2.Trim();
            }
            return null;
        }

        public string GetStringAttributeDesc3(string attributeName, string defaultValue = null)
        {
            if (Contains(attributeName))
            {
                return this[attributeName].Desc3.Trim();
            }
            return defaultValue;
        }
    }
}