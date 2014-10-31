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
        private DataTable leftTable { get; set; }
        private DataTable rightTable { get; set; }
        private MergeOptions mergeOptions { get; set; }

        private List<Field> allColumns { get; set; }
        private List<Field> nonPrimaryColumns { get; set; }
        private List<Field> primaryColumns { get; set; }
        private ICollection<string> compareColumnNames { get; set; }
        private string leftTableAlias { get; set; }
        private string rightTableAlias { get; set; }

        private IColumnNameBuilder columnNameBuilder { get; set; }
        public DataMerger(DataTable leftTable, DataTable rightTable, MergeOptions mergeOptions, IColumnNameBuilder columnNameBuilder)
        {

            if (leftTable == null)
                throw new ArgumentNullException("leftTable");
            if (rightTable == null)
                throw new ArgumentNullException("rightTable");

            this.leftTable = leftTable;
            this.rightTable = rightTable;
            this.mergeOptions = mergeOptions;

            allColumns = GetAllColumns();
            nonPrimaryColumns = allColumns.Where(field => !field.IsKey).ToList();
            primaryColumns = allColumns.Where(field => field.IsKey).ToList();

            leftTableAlias = mergeOptions.leftTableAlias ?? leftTable.TableName;
            rightTableAlias = mergeOptions.rightTableAlias ?? rightTable.TableName;

            this.columnNameBuilder = columnNameBuilder;
        }

        public DataTable Merge()
        {
            compareColumnNames =
                (mergeOptions.compareColumnNames == null || !mergeOptions.compareColumnNames.Any())
                ? nonPrimaryColumns.Select(column => column.Name).ToList()
                : mergeOptions.compareColumnNames;

            var keyFields = allColumns.Where(field => field.IsKey).ToList();

            var result = BuildDataTableTemplate();

            MergeInto(result, leftTable, rightTable, leftTableAlias, rightTableAlias);
            MergeInto(result, rightTable, leftTable, rightTableAlias, leftTableAlias);

            return result;
        }

        private DataTable BuildDataTableTemplate()
        {
            var result = rightTable.Clone();

            foreach (var column in nonPrimaryColumns)
            {
                var columnType = result.Columns[column.Name].DataType;
                result.Columns.Remove(column.Name);
                var leftColumnName = columnNameBuilder.BuildColumName(leftTableAlias, column.Name);
                var rightColumnName = columnNameBuilder.BuildColumName(rightTableAlias, column.Name);
                result.Columns.Add(leftColumnName, columnType);
                result.Columns.Add(rightColumnName, columnType);
            }

            foreach (var columnName in compareColumnNames)
            {
                var column = allColumns.FirstOrDefault(c => c.Name.Equals(columnName));
                if (column != null)
                {
                    var gapColumnName = columnNameBuilder.BuildGapColumnName(column.Name);
                    var compareColumnName = columnNameBuilder.BuildCompareResultColumnName(column.Name);

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

                foreach (var primaryKey in primaryColumns)
                {
                    newRow[primaryKey.Name] = row[primaryKey.Name];
                }

                foreach (var nonPrimaryKey in nonPrimaryColumns)
                {
                    var leftColumnName = columnNameBuilder.BuildColumName(alias, nonPrimaryKey.Name);
                    var rightColumnName = columnNameBuilder.BuildColumName(aliasOfReferenceTable, nonPrimaryKey.Name);

                    var gapColumnName = columnNameBuilder.BuildGapColumnName(nonPrimaryKey.Name);

                    newRow[leftColumnName] = row[nonPrimaryKey.Name];
                    newRow[rightColumnName] = matchingRow == null ? DBNull.Value : matchingRow[nonPrimaryKey.Name];

                    if (compareColumnNames.Contains(nonPrimaryKey.Name, StringComparer.OrdinalIgnoreCase))
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
            var sourceTable = leftTable;
            var gapList = mergeOptions.gapSettingForNumericColumn;

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
