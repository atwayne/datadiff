using System;
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
            var xmlDocument = XDocument.Load("config.xml");
            var root = xmlDocument.Root;
            var sources = root.Element("Sources").Elements("Source").ToArray();

            var leftSource = sources[0];
            var rightSource = sources[1];

            var leftAlias = leftSource.Attribute("name").Value;
            var rightAlias = rightSource.Attribute("name").Value;

            var leftConnectionString = leftSource.Element("ConnectionString").Value;
            var rightConnectionString = rightSource.Element("ConnectionString").Value;

            var leftQuery = leftSource.Element("QueryString").Value;
            var rightQuery = rightSource.Element("QueryString").Value;

            var reportPath = root.Element("Report").Attribute("path").Value;

            var fields = root.Element("Fields").Elements("Field")
                .Select(l => new Field()
                {
                    Name = l.Value,
                    IsKey = l.Attributes("isKey").Any() && l.Attribute("isKey").Value.Equals("True", StringComparison.CurrentCultureIgnoreCase),
                    Type = null
                }).ToList();

            var leftSqlServerHelper = new SQLServerHelper(leftConnectionString);
            var rightSqlServerHelper = new SQLServerHelper(rightConnectionString);

            var leftResult = leftSqlServerHelper.GetDataTable(leftQuery);
            var rightResult = rightSqlServerHelper.GetDataTable(rightQuery);

            var target = new DataTableComparator();
            var diff = target.Merge(leftResult, rightResult, fields, leftAlias, rightAlias);

            var excelExport = new ExcelGenerator();
            excelExport.Export(leftResult, reportPath, leftAlias);
            excelExport.Export(rightResult, reportPath, rightAlias);
            excelExport.Export(diff, reportPath, "Diff");

        }
    }
}
