using System.Xml.Serialization;

namespace lastr2d2.Tools.DataDiff.Core.Model
{
    [XmlTypeAttribute(AnonymousType = true)]
    public class TaskReport
    {
        [XmlAttribute("path")]
        public string Path { get; set; }
    }
}