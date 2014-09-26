using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace lastr2d2.Tools.DataDiff.Core.Model
{
    [XmlTypeAttribute(AnonymousType = true)]
    public partial class TaskGap
    {
        private double valueField;

        private string columnsField;

        public double Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }

        [XmlElement("Columns")]
        public string ColumnsString
        {
            get
            {
                return this.columnsField;
            }
            set
            {
                this.columnsField = value;
            }
        }

        [XmlIgnore]
        public string[] Columns
        {
            get
            {
                return ColumnsString.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            }
        }

        [XmlIgnore]
        public Dictionary<string, double> GapMapping
        {
            get
            {
                var result = new Dictionary<string, double>();
                foreach (var column in Columns)
                {
                    result[column] = Value;
                }
                return result;
            }
        }
    }
}