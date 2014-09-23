using System.Collections.Generic;

namespace lastr2d2.Tools.DataDiff.Core.Model
{
    public class Task
    {
        public IEnumerable<Source> Sources { get; set; }
        public IEnumerable<Field> Fields { get; set;}
        public string ReportSaveTo { get; set; }
    }
}
