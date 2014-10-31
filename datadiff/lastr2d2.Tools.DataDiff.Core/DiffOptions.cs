using System.Collections.Generic;

namespace LastR2D2.Tools.DataDiff.Core
{
    public class DiffOptions
    {
        public IDictionary<string, string> QueryParameters { get; set; }
        public int DefaultTimeout { get; set; }
        public string SuffixOfGapColumn { get; set; }
        public string SuffixOfCompareResultColumn { get; set; }
        public string DefaultOutputFilePath { get; set; }
    }
}
