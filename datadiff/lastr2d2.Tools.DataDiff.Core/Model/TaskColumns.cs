using System;
using System.Xml.Serialization;

namespace LastR2D2.Tools.DataDiff.Core.Model
{
    [XmlTypeAttribute(AnonymousType = true)]
    public class TaskColumns
    {
        [XmlIgnore]
        public string[] PrimaryColumns
        {
            get
            {
                return PrimaryColumnsString.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            }
        }

        [XmlIgnore]
        public string[] CompareColumns
        {
            get
            {
                return CompareColumnsString.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            }
        }

        [XmlElement("PrimaryColumns")]
        public string PrimaryColumnsString { get; set; }

        [XmlElement("CompareColumns")]
        public string CompareColumnsString { get; set; }
    }
}