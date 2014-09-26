using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace lastr2d2.Tools.DataDiff.Core.Model
{
    [XmlTypeAttribute(AnonymousType = true)]
    [XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class Task
    {
        private TaskSource[] sourcesField;

        private TaskColumns columnsField;

        private TaskGap[] gapsField;

        private TaskReport reportField;

        [XmlArrayItemAttribute("Source", IsNullable = false)]
        public TaskSource[] Sources
        {
            get
            {
                return this.sourcesField;
            }
            set
            {
                this.sourcesField = value;
            }
        }

        public TaskColumns Columns
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

        [XmlArrayItemAttribute("Gap", IsNullable = false)]
        public TaskGap[] Gaps
        {
            get
            {
                return this.gapsField;
            }
            set
            {
                this.gapsField = value;
            }
        }

        [XmlIgnore]
        public Dictionary<string, double> GapMapping
        {
            get
            {
                var result = new Dictionary<string, double>();
                foreach (var gapSetting in Gaps)
                {
                    foreach (var pair in gapSetting.GapMapping)
                    {
                        result[pair.Key] = pair.Value;
                    }
                }
                return result;
            }
        }

        public TaskReport Report
        {
            get
            {
                return this.reportField;
            }
            set
            {
                this.reportField = value;
            }
        }

        public static Task LoadFromXml(string path)
        {
            var serializer = new XmlSerializer(typeof(Task));
            using (var fileStream = new FileStream(path, FileMode.Open))
            {
                var innerTask = (Task)serializer.Deserialize(fileStream);
                return innerTask;
            }
        }
    }
}