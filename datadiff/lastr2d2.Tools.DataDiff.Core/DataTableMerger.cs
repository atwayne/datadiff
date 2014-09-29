using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using lastr2d2.Tools.DataDiff.Core.Model;

namespace lastr2d2.Tools.DataDiff.Core
{
    public class DataTableMerger
    {
        public static DataTable Merge(DataTable leftTable, DataTable rightTable,
            List<string> compareColumns = null,
            string leftTableAlias = null, string rightTableAlias = null, Dictionary<string, double> gapSettingForNumbericColumn = null)
        {
            var columns = GetAllColumns(leftTable, gapSettingForNumbericColumn);

            leftTableAlias = leftTableAlias ?? leftTable.TableName;
            rightTableAlias = rightTableAlias ?? rightTable.TableName;

            var keyFields = columns.Where(field => field.IsKey).ToList();

            var result = rightTable.Clone();
            var nonPrimaryColumns = columns.Where(field => !field.IsKey).ToList();
            nonPrimaryColumns.ForEach(column =>
            {
                var columnType = result.Columns[column.Name].DataType;
                result.Columns.Remove(column.Name);
                result.Columns.Add(string.Format("{0}_{1}", column.Name, leftTableAlias), columnType);
                result.Columns.Add(string.Format("{0}_{1}", column.Name, rightTableAlias), columnType);
            });

            if (compareColumns == null || !compareColumns.Any())
                compareColumns = nonPrimaryColumns.Select(column => column.Name).ToList();

            compareColumns.ForEach(columnName =>
            {
                var column = columns.FirstOrDefault(c => c.Name.Equals(columnName));
                if (column != null)
                {
                    result.Columns.Add(string.Format("{0}_{1}", column.Name, "Gap"), typeof(double));
                    result.Columns.Add(string.Format("{0}_{1}", column.Name, "Compare"), typeof(double));
                }
            });

            Merge(result, leftTable, rightTable, leftTableAlias, rightTableAlias, keyFields, nonPrimaryColumns);
            Merge(result, rightTable, leftTable, rightTableAlias, leftTableAlias, keyFields, nonPrimaryColumns);

            return result;
        }

        private static void Merge(DataTable result,
            DataTable sourceTable, DataTable referTable,
            string alias, string referAlias,
            IList<Field> keyFields, List<Field> compareKeys)
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

                foreach (var key in compareKeys)
                {
                    newRow[string.Format("{0}_{1}", key.Name, alias)] = row[key.Name];
                    newRow[string.Format("{0}_{1}", key.Name, referAlias)] =
                        matchingRow == null ? DBNull.Value : matchingRow[key.Name];
                    newRow[string.Format("{0}_{1}", key.Name, "Gap")] = key.Gap;
                }
                result.Rows.Add(newRow);
                result.AcceptChanges();
            }
        }

        private static List<Field> GetAllColumns(DataTable sourceTable, Dictionary<string, double> gapList = null)
        {
            var fields = new List<Field>();
            for (int i = 0; i < sourceTable.Columns.Count; i++)
            {
                var column = sourceTable.Columns[i];

                fields.Add(new Field
                {
                    Name = column.ColumnName,
                    Type = column.DataType,
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