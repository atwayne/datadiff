using System.Collections.Generic;
using System.Xml.Serialization;

namespace LastR2D2.Tools.DataDiff.Core.Model
{
    [XmlTypeAttribute(AnonymousType = true)]
    public class TaskSource
    {
        public string ConnectionString { get; set; }

        public string QueryString { get; set; }

        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlIgnore]
        public IDictionary<string, string> QueryParameters { get; set; }
    }
}