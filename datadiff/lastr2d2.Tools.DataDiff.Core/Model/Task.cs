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

        public static Tasks LoadFromXml(string path)
        {
            var tasks = new Tasks();
            try
            {
                tasks = LoadObjectFromXml<Tasks>(path);
            }
            catch (InvalidOperationException)
            {
                var task = LoadObjectFromXml<Task>(path);
                tasks.Add(task);
            }
            tasks.ForEach(SetReportPath);

            return tasks;
        }

        private static void SetReportPath(Task innerTask)
        {
            if (Directory.Exists(innerTask.Report.Path))
            {
                innerTask.Report.Path = Path.Combine(innerTask.Report.Path, string.Format("{0}_{1}.xlsx", innerTask.Name, DateTime.Now.ToString("yyyyMMddHHmmssfffffff")));
            }
        }

        private static T LoadObjectFromXml<T>(string path)
        {
            var serializer = new XmlSerializer(typeof(T));
            using (var fileStream = new FileStream(path, FileMode.Open))
            {
                var result = (T)serializer.Deserialize(fileStream);
                return result;
            }
        }
    }
}