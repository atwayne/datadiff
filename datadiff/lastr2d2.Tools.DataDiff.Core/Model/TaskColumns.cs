using System;
using System.Xml.Serialization;

namespace lastr2d2.Tools.DataDiff.Core.Model
{
    [XmlTypeAttribute(AnonymousType = true)]
    public partial class TaskColumns
    {
        private string primaryColumnsField;

        private string compareColumnsField;

        [XmlIgnore]
        public string[] PrimaryColumns
        {
            get
            {
                return PrimaryColumnsString.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            }
        }

        [XmlIgnore]
        public string[] CompareColumns
        {
            get
            {
                return CompareColumnsString.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            }
        }

        [XmlElement("PrimaryColumns")]
        public string PrimaryColumnsString
        {
            get
            {
                return this.primaryColumnsField;
            }
            set
            {
                this.primaryColumnsField = value;
            }
        }

        [XmlElement("CompareColumns")]
        public string CompareColumnsString
        {
            get
            {
                return this.compareColumnsField;
            }
            set
            {
                this.compareColumnsField = value;
            }
        }
    }
}