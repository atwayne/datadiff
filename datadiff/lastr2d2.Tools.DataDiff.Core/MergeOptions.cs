using System.Collections.Generic;

namespace LastR2D2.Tools.DataDiff.Core
{
    public class MergeOptions
    {
        public ICollection<string> compareColumnNames { get; private set; }
        public IDictionary<string, double> gapSettingForNumericColumn { get; private set; }
        public string leftTableAlias { get; private set; }
        public string rightTableAlias { get; private set; }

        public MergeOptions(string leftTableAlias, string rightTableAlias, ICollection<string> compareColumnNames, IDictionary<string, double> gapSettingForNumericColumn)
        {
            this.leftTableAlias = leftTableAlias;
            this.rightTableAlias = rightTableAlias;
            this.compareColumnNames = compareColumnNames;
            this.gapSettingForNumericColumn = gapSettingForNumericColumn;
        }
    }
}
