// Decompiled with JetBrains decompiler
// Type: IGServices.Common.Types.Response.ActionLink
// Assembly: IGServices, Version=0.1.7340.16006, Culture=neutral, PublicKeyToken=null
// MVID: DC8EC0DC-64DA-4A3E-B3DA-A4691D2D5367
// Assembly location: \\cgd1intsvcpd\c$\inetpub\wwwroot\IGServices\bin\IGServices.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace IGServices.Common.Types.Response
{
    [CollectionDataContract(Namespace = "")]
    public class ActionLinks : List<ActionLink>
    {
    }

    [DataContract(Namespace = "")]
    public class ActionLink
    {
        public ActionLink()
        {
        }

        public ActionLink(string name, string desc, string href)
        {
            Name = name;
            Description = desc;
            Href = href;
        }

        [DataMember(Order = 10)] public string Name { get; set; }

        [DataMember(Order = 20)] public string Description { get; set; }

        [DataMember(Order = 30)] public string Href { get; set; }
    }
}