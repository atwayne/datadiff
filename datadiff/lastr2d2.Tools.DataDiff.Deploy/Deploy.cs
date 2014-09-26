using System;
using System.Data;
using System.Linq;
using System.Xml.Linq;
using lastr2d2.Tools.DataDiff.Core;
using lastr2d2.Tools.DataDiff.Core.Model;

namespace lastr2d2.Tools.DataDiff.Deploy
{
    public class Deploy
    {
        private static void Main()
        {
            var task = Task.LoadFromXml("config.xml");

            var leftDataTable = PrepareDataTable(task, 0);
            var rightDataTable = PrepareDataTable(task, 1);

            var mergeResult = DataTableMerger.Merge(leftDataTable, rightDataTable);

            var execelGenerator = new ExcelGenerator();
            execelGenerator.Export(leftDataTable, task.Report.Path, leftDataTable.TableName);
            execelGenerator.Export(rightDataTable, task.Report.Path, rightDataTable.TableName);
            execelGenerator.Export(mergeResult, task.Report.Path, "Result");
        }

        private static DataTable PrepareDataTable(Task task, int sourceIndex)
        {
            var source = task.Sources[sourceIndex];
            var sqlServer = new SQLServerHelper(source.ConnectionString);
            var dataTable = sqlServer.GetDataTable(source.QueryString);
            dataTable.TableName = source.Name;

            dataTable.PrimaryKey = task.Columns.PrimaryColumns.Select(column => dataTable.Columns[column]).ToArray();
            return dataTable;
        }
    }
}
