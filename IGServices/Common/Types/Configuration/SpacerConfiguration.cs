using System.Configuration;

namespace IGServices.Common.Types.Configuration
{
    public class SpacerConfiguration : ConfigurationSection
    {
        [ConfigurationProperty("SpacerTypes", IsDefaultCollection = false)]
        public SpacerTypes SpacerTypes
        {
            get
            {
                SpacerTypes spacerTypesCollection = (SpacerTypes)base["SpacerTypes"];
                return spacerTypesCollection;
            }
        }

        [ConfigurationProperty("SpacerTokens", IsDefaultCollection = false)]
        public SpacerTokens SpacerTokens
        {
            get
            {
                SpacerTokens SpacerTokensCollection = (SpacerTokens)base["SpacerTokens"];
                return SpacerTokensCollection;
            }
        }
    }

    public class SpacerTypes : ConfigurationElementCollection
    {
        public new SpacerType this[string name]
        {
            get
            {
                if (IndexOf(name) < 0) return null;
                return (SpacerType)BaseGet(name);
            }
        }

        public SpacerType this[int index]
        {
            get { return (SpacerType)BaseGet(index); }
        }

        public int IndexOf(string name)
        {
            name = name.ToLower();

            for (int idx = 0; idx < base.Count; idx++)
            {
                if (this[idx].name.ToLower() == name)
                    return idx;
            }
            return -1;
        }

        public override ConfigurationElementCollectionType CollectionType
        {
            get { return ConfigurationElementCollectionType.BasicMap; }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new SpacerType();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((SpacerType)element).name;
        }

        protected override string ElementName
        {
            get { return "SpacerType"; }
        }
    }



    public class SpacerType : ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired = true, IsKey = true, DefaultValue = "")]
        public string name
        {
            get { return base["name"] as string; }
            set { base["name"] = value; }
        }
        [ConfigurationProperty("material", IsRequired = true, IsKey = true, DefaultValue = "")]
        public string material
        {
            get { return base["material"] as string; }
            set { base["material"] = value; }
        }
        [ConfigurationProperty("color", IsRequired = true, IsKey = true, DefaultValue = "")]
        public string color
        {
            get { return base["color"] as string; }
            set { base["color"] = value; }
        }
        [ConfigurationProperty("painted", IsRequired = true, IsKey = true, DefaultValue = "")]
        public string painted
        {
            get { return base["painted"] as string; }
            set { base["painted"] = value; }
        }
        [ConfigurationProperty("profile", IsRequired = true, IsKey = true, DefaultValue = "")]
        public string profile
        {
            get { return base["profile"] as string; }
            set { base["profile"] = value; }
        }
        [ConfigurationProperty("surface", IsRequired = true, IsKey = true, DefaultValue = "")]
        public string surface
        {
            get { return base["surface"] as string; }
            set { base["surface"] = value; }
        }
    }

    public class SpacerTokens : ConfigurationElementCollection
    {
        public new SpacerToken this[string name]
        {
            get
            {
                if (IndexOf(name) < 0) return null;
                return (SpacerToken)BaseGet(name);
            }
        }

        public SpacerToken this[int index]
        {
            get { return (SpacerToken)BaseGet(index); }
        }

        public int IndexOf(string name)
        {
            name = name.ToLower();

            for (int idx = 0; idx < base.Count; idx++)
            {
                if (this[idx].name.ToLower() == name)
                    return idx;
            }
            return -1;
        }

        public override ConfigurationElementCollectionType CollectionType
        {
            get { return ConfigurationElementCollectionType.BasicMap; }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new SpacerToken();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((SpacerToken)element).name;
        }

        protected override string ElementName
        {
            get { return "SpacerToken"; }
        }
    }

    public class SpacerToken : ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired = true, IsKey = true, DefaultValue = "")]
        public string name
        {
            get { return base["name"] as string; }
            set { base["name"] = value; }
        }
        [ConfigurationProperty("type", IsRequired = false, IsKey = false, DefaultValue = "")]
        public string type
        {
            get { return base["type"] as string; }
            set { base["type"] = value; }
        }
        [ConfigurationProperty("profile", IsRequired = false, IsKey = false, DefaultValue = "")]
        public string profile
        {
            get { return base["profile"] as string; }
            set { base["profile"] = value; }
        }
        [ConfigurationProperty("prekeyed", IsRequired = false, IsKey = false, DefaultValue = "")]
        public string prekeyed
        {
            get { return base["prekeyed"] as string; }
            set { base["prekeyed"] = value; }
        }
    }

}
