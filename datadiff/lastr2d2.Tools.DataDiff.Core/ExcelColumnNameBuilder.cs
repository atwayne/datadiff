﻿using System.Collections.Generic;
using System.Linq;
using LastR2D2.Tools.DataDiff.Core.Interfaces;

namespace LastR2D2.Tools.DataDiff.Core
{
    public class ExcelColumnNameBuilder : IColumnNameBuilder
    {
        public ICollection<string> Suffixes
        {
            get { return suffixes; }
        }

        private readonly string[] suffixes;

        private readonly string suffixOfGapColumn;
        private readonly string suffixOfCompareResultColumn;

        public ExcelColumnNameBuilder(string suffixOfGapColumn, string suffixOfCompareResultColumn)
        {
            this.suffixOfCompareResultColumn = suffixOfCompareResultColumn;
            this.suffixOfGapColumn = suffixOfGapColumn;
            suffixes = new[] { suffixOfGapColumn, suffixOfCompareResultColumn };
        }

        public string BuildColumName(string nameOfDataSource, string nameOfColumn)
        {
            return string.Format("{0}_{1}", nameOfColumn, nameOfDataSource);
        }
        public string BuildGapColumnName(string nameOfColumn)
        {
            return string.Format("{0}_{1}", nameOfColumn, suffixOfGapColumn);
        }
        public string BuildCompareResultColumnName(string nameOfColumn)
        {
            return string.Format("{0}_{1}", nameOfColumn, suffixOfCompareResultColumn);
        }

        public bool IsGapColumn(string nameOfColumn)
        {
            return nameOfColumn.EndsWith("_" + suffixOfGapColumn);
        }

        public bool IsCompareResultColumn(string nameOfColumn)
        {
            return nameOfColumn.EndsWith("_" + suffixOfCompareResultColumn);
        }

        public bool IsCompareColumn(string nameOfColumn, string nameOfDataSource)
        {
            return nameOfColumn.EndsWith("_" + nameOfDataSource);
        }

        public bool IsGeneratedColumn(string nameOfColumn, ICollection<string> nameOfDataSources)
        {
            if (IsGapColumn(nameOfColumn))
                return true;
            if (IsCompareResultColumn(nameOfColumn))
                return true;

            return nameOfDataSources.Any(nameOfDataSource => IsCompareColumn(nameOfColumn, nameOfDataSource));
        }

        public ICollection<string> GetUnderlyingColumnNames(IEnumerable<string> nameOfColumns, ICollection<string> nameOfDataSources)
        {

            var columns = nameOfColumns.ToList();
            return columns
                .Where(l => IsGeneratedColumn(l, nameOfDataSources))
                .Select(l => l.Substring(0, l.LastIndexOf('_')))
                .Distinct()
                .Where(l =>
                {
                    var gapColumnName = BuildGapColumnName(l);
                    if (!columns.Contains(gapColumnName))
                        return false;

                    var compareColumnName = BuildCompareResultColumnName(l);
                    if (!columns.Contains(compareColumnName))
                        return false;

                    return nameOfDataSources.Select(dataSource => BuildColumName(dataSource, l)).All(columns.Contains);
                })
                .Distinct()
                .ToList();


        }
    }
}
