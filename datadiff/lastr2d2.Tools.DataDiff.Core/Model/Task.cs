using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace lastr2d2.Tools.DataDiff.Core.Model
{
    [XmlTypeAttribute(AnonymousType = true)]
    [XmlRootAttribute(Namespace = "", IsNullable = false)]
    public class Task
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlArrayItem("Source", IsNullable = false)]
        public TaskSource[] Sources { get; set; }

        public TaskColumns Columns { get; set; }

        [XmlArrayItem("Gap", IsNullable = false)]
        public TaskGap[] Gaps { get; set; }

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

        public TaskReport Report { get; set; }

        public static Task LoadFromXml(string path)
        {
            var serializer = new XmlSerializer(typeof(Task));
            using (var fileStream = new FileStream(path, FileMode.Open))
            {
                var innerTask = (Task)serializer.Deserialize(fileStream);
                SetReportPath(innerTask);
                return innerTask;
            }
        }

        private static void SetReportPath(Task innerTask)
        {
            if (Directory.Exists(innerTask.Report.Path))
            {
                innerTask.Report.Path = Path.Combine(innerTask.Report.Path, string.Format("{0}_{1}.xlsx", innerTask.Name, DateTime.Now.ToString("yyyyMMddHHmmssfffffff")));
            }
        }
    }
}