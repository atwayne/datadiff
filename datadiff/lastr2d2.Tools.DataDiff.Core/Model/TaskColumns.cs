using System;
using System.Xml.Serialization;

namespace lastr2d2.Tools.DataDiff.Core.Model
{
    [XmlTypeAttribute(AnonymousType = true)]
    public class TaskColumns
    {
        private string primaryColumnsField;

        private string compareColumnsField;

        [XmlIgnore]
        public string[] PrimaryColumns
        {
            get
            {
                return PrimaryColumnsString.Split(new [] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            }
        }

        [XmlIgnore]
        public string[] CompareColumns
        {
            get
            {
                return CompareColumnsString.Split(new [] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            }
        }

        [XmlElement("PrimaryColumns")]
        public string PrimaryColumnsString
        {
            get
            {
                return primaryColumnsField;
            }
            set
            {
                primaryColumnsField = value;
            }
        }

        [XmlElement("CompareColumns")]
        public string CompareColumnsString
        {
            get
            {
                return compareColumnsField;
            }
            set
            {
                compareColumnsField = value;
            }
        }
    }
}