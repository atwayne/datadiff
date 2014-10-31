using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using LastR2D2.Tools.DataDiff.Core.Interfaces;
using LastR2D2.Tools.DataDiff.Core.Model;

namespace LastR2D2.Tools.DataDiff.Core
{
    public class DataMerger : IDataMerger
    {
        private DataTable LeftTable { get; set; }
        private DataTable RightTable { get; set; }
        private MergeOptions MergeOptions { get; set; }

        private List<Field> AllColumns { get; set; }
        private List<Field> NonPrimaryColumns { get; set; }
        private List<Field> PrimaryColumns { get; set; }
        private ICollection<string> CompareColumnNames { get; set; }
        private string LeftTableAlias { get; set; }
        private string RightTableAlias { get; set; }

        private IColumnNameBuilder ColumnNameBuilder { get; set; }
        public DataMerger(DataTable leftTable, DataTable rightTable, MergeOptions mergeOptions, IColumnNameBuilder columnNameBuilder)
        {

            if (leftTable == null)
                throw new ArgumentNullException("leftTable");
            if (rightTable == null)
                throw new ArgumentNullException("rightTable");

            LeftTable = leftTable;
            RightTable = rightTable;
            MergeOptions = mergeOptions;

            AllColumns = GetAllColumns();
            NonPrimaryColumns = AllColumns.Where(field => !field.IsKey).ToList();
            PrimaryColumns = AllColumns.Where(field => field.IsKey).ToList();

            LeftTableAlias = mergeOptions.LeftTableAlias ?? leftTable.TableName;
            RightTableAlias = mergeOptions.RightTableAlias ?? rightTable.TableName;

            ColumnNameBuilder = columnNameBuilder;
        }

        public DataTable Merge()
        {
            CompareColumnNames =
                (MergeOptions.CompareColumnNames == null || !MergeOptions.CompareColumnNames.Any())
                ? NonPrimaryColumns.Select(column => column.Name).ToList()
                : MergeOptions.CompareColumnNames;

            var result = BuildDataTableTemplate();

            MergeInto(result, LeftTable, RightTable, LeftTableAlias, RightTableAlias);
            MergeInto(result, RightTable, LeftTable, RightTableAlias, LeftTableAlias);

            return result;
        }

        private DataTable BuildDataTableTemplate()
        {
            var result = RightTable.Clone();

            foreach (var column in NonPrimaryColumns)
            {
                var columnType = result.Columns[column.Name].DataType;
                result.Columns.Remove(column.Name);
                var leftColumnName = ColumnNameBuilder.BuildColumName(LeftTableAlias, column.Name);
                var rightColumnName = ColumnNameBuilder.BuildColumName(RightTableAlias, column.Name);
                result.Columns.Add(leftColumnName, columnType);
                result.Columns.Add(rightColumnName, columnType);
            }

            foreach (var columnName in CompareColumnNames)
            {
                var column = AllColumns.FirstOrDefault(c => c.Name.Equals(columnName));
                if (column != null)
                {
                    var gapColumnName = ColumnNameBuilder.BuildGapColumnName(column.Name);
                    var compareColumnName = ColumnNameBuilder.BuildCompareResultColumnName(column.Name);

                    result.Columns.Add(gapColumnName, typeof(double));
                    result.Columns.Add(compareColumnName, typeof(double));
                }
            }
            return result;
        }

        private void MergeInto(DataTable result, DataTable sourceTable, DataTable referenceTable, string alias, string aliasOfReferenceTable)
        {
            foreach (var row in sourceTable.AsEnumerable())
            {
                var keys = new object[sourceTable.PrimaryKey.Count()];

                for (var i = 0; i < keys.Length; i++)
                {
                    keys[i] = row[sourceTable.PrimaryKey[i].ColumnName];
                }

                var matchingRow = referenceTable.Rows.Find(keys);
                var newRow = result.Rows.Find(keys);
                if (newRow != null)
                    continue;
                newRow = result.NewRow();

                foreach (var primaryKey in PrimaryColumns)
                {
                    newRow[primaryKey.Name] = row[primaryKey.Name];
                }

                foreach (var nonPrimaryKey in NonPrimaryColumns)
                {
                    var leftColumnName = ColumnNameBuilder.BuildColumName(alias, nonPrimaryKey.Name);
                    var rightColumnName = ColumnNameBuilder.BuildColumName(aliasOfReferenceTable, nonPrimaryKey.Name);

                    var gapColumnName = ColumnNameBuilder.BuildGapColumnName(nonPrimaryKey.Name);

                    newRow[leftColumnName] = row[nonPrimaryKey.Name];
                    newRow[rightColumnName] = matchingRow == null ? DBNull.Value : matchingRow[nonPrimaryKey.Name];

                    if (CompareColumnNames.Contains(nonPrimaryKey.Name, StringComparer.OrdinalIgnoreCase))
                    {
                        newRow[gapColumnName] = nonPrimaryKey.Gap;
                    }
                }
                result.Rows.Add(newRow);
                result.AcceptChanges();
            }
        }

        private List<Field> GetAllColumns()
        {
            var sourceTable = LeftTable;
            var gapList = MergeOptions.GapSettingForNumericColumn;

            var fields = new List<Field>();
            for (int i = 0; i < sourceTable.Columns.Count; i++)
            {
                var column = sourceTable.Columns[i];

                fields.Add(new Field
                {
                    Name = column.ColumnName,
                    FieldType = column.DataType,
                    IsKey = sourceTable.PrimaryKey.Contains(column)
                });
            }

            if (gapList != null && gapList.Any())
            {
                fields.ForEach(field =>
                {
                    if (field.IsNumericType)
                    {
                        field.Gap = gapList.ContainsKey(field.Name) ? gapList[field.Name] :
                            (gapList.ContainsKey(string.Empty) ? gapList[string.Empty] : 0);
                    }
                });
            }

            return fields;
        }
    }
}
