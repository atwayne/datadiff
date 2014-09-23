using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Contracts;
using System.Linq;
using lastr2d2.Tools.DataDiff.Core.Model;


namespace lastr2d2.Tools.DataDiff.Core
{
    public class DataTableComparator
    {
        public DataTable Merge(DataTable leftTable, DataTable rightTable, IEnumerable<Field> fields,
            string leftTableAlias = null, string rightTableAlias = null)
        {
            Contract.Requires(fields != null && fields.Any());
            Contract.Requires(fields.Any(field => field.IsKey), "Should be at least one key field");
            Contract.Requires(fields.All(field => leftTable.Columns.Contains(field.Name)
                && rightTable.Columns.Contains(field.Name)));

            if (fields == null)
            {
                var liveFields = new List<Field>();
                for (int i = 0; i < leftTable.Columns.Count; i++)
                {
                    var column = leftTable.Columns[i];
                    liveFields.Add(new Field()
                    {
                        Name = column.ColumnName,
                        Type = column.DataType,
                        IsKey = leftTable.PrimaryKey.Contains(column),
                        Gap = 0
                    });
                }
                fields = liveFields;
            }
            else
            {

            }

            leftTableAlias = leftTableAlias ?? "1";
            rightTableAlias = rightTableAlias ?? "2";

            var keyFields = fields.Where(field => field.IsKey);
            leftTable.PrimaryKey = keyFields.Select(field => leftTable.Columns[field.Name]).ToArray();
            rightTable.PrimaryKey = keyFields.Select(field => rightTable.Columns[field.Name]).ToArray();

            var result = rightTable.Clone();
            var compareKeys = fields.Where(field => !field.IsKey).ToList();
            compareKeys.ForEach(key =>
            {
                var columnType = result.Columns[key.Name].DataType;
                result.Columns.Remove(key.Name);

                result.Columns.Add(string.Format("{0}_{1}", key.Name, leftTableAlias), columnType);
                result.Columns.Add(string.Format("{0}_{1}", key.Name, rightTableAlias), columnType);
                result.Columns.Add(string.Format("{0}_{1}", key.Name, "diff"), typeof(string));
            });

            foreach (var row in leftTable.AsEnumerable())
            {
                var keys = keyFields.Select(key => row[key.Name]).ToArray();
                var rightMatchingRow = rightTable.Rows.Find(keys);
                var newRow = result.NewRow();

                foreach (var key in keyFields)
                {
                    newRow[key.Name] = row[key.Name];
                }

                foreach (var key in compareKeys)
                {
                    newRow[string.Format("{0}_{1}", key.Name, leftTableAlias)] = row[key.Name];
                    if (rightMatchingRow != null)
                    {
                        newRow[string.Format("{0}_{1}", key.Name, rightTableAlias)] = rightMatchingRow[key.Name];
                        newRow[string.Format("{0}_{1}", key.Name, "diff")] = FieldCompare.Compare(row, rightMatchingRow, key).ToString();
                    }
                    else
                    {
                        newRow[string.Format("{0}_{1}", key.Name, "diff")] = FieldCompareResult.MissingInRight.ToString();
                    }
                }
                result.Rows.Add(newRow);
                result.AcceptChanges();
            }

            foreach (var row in rightTable.AsEnumerable())
            {
                var keys = keyFields.Select(key => row[key.Name]).ToArray();
                var leftMatchingRow = leftTable.Rows.Find(keys);
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
                    newRow[string.Format("{0}_{1}", key.Name, leftTableAlias)] = row[key.Name];
                    if (leftMatchingRow != null)
                    {
                        newRow[string.Format("{0}_{1}", key.Name, rightTableAlias)] = leftMatchingRow[key.Name];
                        newRow[string.Format("{0}_{1}", key.Name, "diff")] = FieldCompare.Compare(leftMatchingRow, row, key).ToString();
                    }
                    else
                    {
                        newRow[string.Format("{0}_{1}", key.Name, "diff")] = FieldCompareResult.MissingInLeft.ToString();
                    }
                }
                result.Rows.Add(newRow);
                result.AcceptChanges();
            }

            return result;
        }

    }
}
