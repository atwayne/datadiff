using System.Xml.Serialization;

namespace LastR2D2.Tools.DataDiff.Core.Model
{
    [XmlTypeAttribute(AnonymousType = true)]
    public class TaskReport
    {
        [XmlAttribute("path")]
        public string Path { get; set; }
    }
}