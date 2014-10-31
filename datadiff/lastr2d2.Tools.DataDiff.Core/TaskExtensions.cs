using LastR2D2.Tools.DataDiff.Core.Model;
using System.Collections.Generic;
using System.Linq;

namespace LastR2D2.Tools.DataDiff.Core
{
    internal static class TaskExtensions
    {
        public static void LoadConfig(this Task task, string defaultOutputFile, IDictionary<string, string> queryParameters)
        {
            LoadQueryParameters(task, queryParameters);
            LoadReportPath(task, defaultOutputFile);
        }

        private static void LoadReportPath(Task task, string defaultOutputFile)
        {
            if (!string.IsNullOrEmpty(defaultOutputFile)
                && string.IsNullOrEmpty(task.Report.Path))
            {
                task.Report.Path = defaultOutputFile;
            }
        }

        private static void LoadQueryParameters(Task task, IDictionary<string, string> queryParameters)
        {
            if (queryParameters == null || !queryParameters.Any()) return;

            foreach (var dataSource in task.Sources)
            {
                if (dataSource.QueryParameters == null || !dataSource.QueryParameters.Any())
                {
                    dataSource.QueryParameters = queryParameters;
                }
            }
        }
    }
}
