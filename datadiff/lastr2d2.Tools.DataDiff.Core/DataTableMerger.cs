using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using LastR2D2.Tools.DataDiff.Core.Model;

namespace LastR2D2.Tools.DataDiff.Core
{
    public static class DataTableMerger
    {
        public static DataTable Merge(DataTable leftTable, DataTable rightTable,
            ICollection<string> compareColumnNames = null,
            string leftTableAlias = null, string rightTableAlias = null, IDictionary<string, double> gapSettingForNumericColumn = null)
        {
            if (leftTable == null)
                throw new ArgumentNullException("leftTable");
            if (rightTable == null)
                throw new ArgumentNullException("rightTable");

            var columns = GetAllColumns(leftTable, gapSettingForNumericColumn);
            var nonPrimaryColumns = columns.Where(field => !field.IsKey).ToList();

            if (compareColumnNames == null || !compareColumnNames.Any())
                compareColumnNames = nonPrimaryColumns.Select(column => column.Name).ToList();

            leftTableAlias = leftTableAlias ?? leftTable.TableName;
            rightTableAlias = rightTableAlias ?? rightTable.TableName;

            var keyFields = columns.Where(field => field.IsKey).ToList();

            var result = BuildDataTable(rightTable, compareColumnNames, leftTableAlias, rightTableAlias, columns, nonPrimaryColumns);

            Merge(result, leftTable, rightTable, leftTableAlias, rightTableAlias, keyFields, nonPrimaryColumns, compareColumnNames);
            Merge(result, rightTable, leftTable, rightTableAlias, leftTableAlias, keyFields, nonPrimaryColumns, compareColumnNames);

            return result;
        }

        private static DataTable BuildDataTable(DataTable rightTable,
            ICollection<string> compareColumnNames, string leftTableAlias, string rightTableAlias,
            ICollection<Field> columns, ICollection<Field> nonPrimaryColumns)
        {
            var result = rightTable.Clone();

            foreach (var column in nonPrimaryColumns)
            {
                var columnType = result.Columns[column.Name].DataType;
                result.Columns.Remove(column.Name);
                result.Columns.Add(string.Format(CultureInfo.InvariantCulture, "{0}_{1}", column.Name, leftTableAlias), columnType);
                result.Columns.Add(string.Format(CultureInfo.InvariantCulture, "{0}_{1}", column.Name, rightTableAlias), columnType);

            }

            foreach (var columnName in compareColumnNames)
            {
                var column = columns.FirstOrDefault(c => c.Name.Equals(columnName));
                if (column != null)
                {
                    result.Columns.Add(string.Format(CultureInfo.InvariantCulture, "{0}_{1}", column.Name, "Gap"), typeof(double));
                    result.Columns.Add(string.Format(CultureInfo.InvariantCulture, "{0}_{1}", column.Name, "Compare"), typeof(double));
                }
            }
            return result;
        }

        private static void Merge(DataTable result,
            DataTable sourceTable, DataTable referTable,
            string alias, string referAlias,
            ICollection<Field> keyFields, ICollection<Field> nonPrimaryColumns, ICollection<string> compareColumnNames)
        {
            foreach (var row in sourceTable.AsEnumerable())
            {
                var keys = new object[sourceTable.PrimaryKey.Count()];

                for (var i = 0; i < keys.Length; i++)
                {
                    keys[i] = row[sourceTable.PrimaryKey[i].ColumnName];
                }

                var matchingRow = referTable.Rows.Find(keys);
                var newRow = result.Rows.Find(keys);
                if (newRow != null)
                    continue;
                newRow = result.NewRow();

                foreach (var key in keyFields)
                {
                    newRow[key.Name] = row[key.Name];
                }

                foreach (var key in nonPrimaryColumns)
                {
                    newRow[string.Format(CultureInfo.InvariantCulture, "{0}_{1}", key.Name, alias)] = row[key.Name];
                    newRow[string.Format(CultureInfo.InvariantCulture, "{0}_{1}", key.Name, referAlias)] =
                        matchingRow == null ? DBNull.Value : matchingRow[key.Name];

                    if (compareColumnNames.Contains(key.Name, StringComparer.OrdinalIgnoreCase))
                    {
                        newRow[string.Format(CultureInfo.InvariantCulture, "{0}_{1}", key.Name, "Gap")] = key.Gap;
                    }
                }
                result.Rows.Add(newRow);
                result.AcceptChanges();
            }
        }

        private static List<Field> GetAllColumns(DataTable sourceTable, IDictionary<string, double> gapList = null)
        {
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