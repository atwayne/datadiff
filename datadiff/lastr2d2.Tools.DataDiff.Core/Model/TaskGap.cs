using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace LastR2D2.Tools.DataDiff.Core.Model
{
    [XmlTypeAttribute(AnonymousType = true)]
    public class TaskGap
    {
        public double Value { get; set; }

        [XmlElement("Columns")]
        public string ColumnsString { get; set; }

        [XmlIgnore]
        public string[] Columns
        {
            get
            {
                if (string.IsNullOrWhiteSpace(ColumnsString))
                {
                    return new[] { string.Empty };
                }
                else
                {
                    return ColumnsString.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                }
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