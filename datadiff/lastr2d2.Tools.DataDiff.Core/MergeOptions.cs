using System.Collections.Generic;

namespace LastR2D2.Tools.DataDiff.Core
{
    public class MergeOptions
    {
        public ICollection<string> CompareColumnNames { get; private set; }
        public IDictionary<string, double> GapSettingForNumericColumn { get; private set; }
        public string LeftTableAlias { get; private set; }
        public string RightTableAlias { get; private set; }

        public MergeOptions(string leftTableAlias, string rightTableAlias, ICollection<string> compareColumnNames, IDictionary<string, double> gapSettingForNumericColumn)
        {
            LeftTableAlias = leftTableAlias;
            RightTableAlias = rightTableAlias;
            CompareColumnNames = compareColumnNames;
            GapSettingForNumericColumn = gapSettingForNumericColumn;
        }
    }
}
