using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace lastr2d2.Tools.DataDiff.Core.Model
{
    [XmlTypeAttribute(AnonymousType = true)]
    public class TaskGap
    {
        private double valueField;

        private string columnsField;

        public double Value
        {
            get
            {
                return valueField;
            }
            set
            {
                valueField = value;
            }
        }

        [XmlElement("Columns")]
        public string ColumnsString
        {
            get
            {
                return columnsField;
            }
            set
            {
                columnsField = value;
            }
        }

        [XmlIgnore]
        public string[] Columns
        {
            get
            {
                return ColumnsString.Split(new [] { ',' }, StringSplitOptions.RemoveEmptyEntries);
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