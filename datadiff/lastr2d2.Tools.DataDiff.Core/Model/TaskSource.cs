using System.Xml.Serialization;

namespace lastr2d2.Tools.DataDiff.Core.Model
{
    [XmlTypeAttribute(AnonymousType = true)]
    public class TaskSource
    {
        public string ConnectionString { get; set; }

        public string QueryString { get; set; }

        [XmlAttribute("name")]
        public string Name { get; set; }
    }
}