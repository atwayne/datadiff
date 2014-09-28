using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace lastr2d2.Tools.DataDiff.Core.Model
{
    [XmlTypeAttribute(AnonymousType = true)]
    [XmlRootAttribute(Namespace = "", IsNullable = false)]
    public class Task
    {
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
                return innerTask;
            }
        }
    }
}