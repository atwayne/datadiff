using System.Xml.Serialization;

namespace lastr2d2.Tools.DataDiff.Core.Model
{
    [XmlTypeAttribute(AnonymousType = true)]
    public class TaskReport
    {
        private string pathField;

        [XmlAttributeAttribute("path")]
        public string Path
        {
            get
            {
                return pathField;
            }
            set
            {
                pathField = value;
            }
        }
    }
}