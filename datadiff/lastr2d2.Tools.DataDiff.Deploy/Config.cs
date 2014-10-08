using System.Collections.Generic;
using System.Configuration;
using System.Text.RegularExpressions;

namespace lastr2d2.Tools.DataDiff.Deploy
{
    internal static class Config
    {
        public static string DefaultInputDirectory { get; set; }
        public static string DefaultOutputFile { get; set; }
        public static string DefaultInputFileNamePattern { get; set; }
        public static object DefaultOutputFileLock { get; private set; }

        public static IDictionary<string, string> QueryParameters { get; set; }

        public static void Load()
        {
            DefaultInputDirectory = ConfigurationManager.AppSettings["DefaultInputDirectory"] ?? "./tasks/";
            DefaultOutputFile = ConfigurationManager.AppSettings["DefaultOutputFile"] ?? "./CompareResult.xlst";
            DefaultInputFileNamePattern = ConfigurationManager.AppSettings["DefaultInputFileNamePattern"] ?? "*.xml";

            QueryParameters = new Dictionary<string, string>();
            ReadQueryParameters();

            DefaultOutputFileLock = new object();
        }

        private static void ReadQueryParameters()
        {
            var parameterPattern = ConfigurationManager.AppSettings["ParameterPattern"] ?? @"Parameter_(?<name>\w+)";
            var regex = new Regex(parameterPattern, RegexOptions.IgnoreCase);

            var keys = ConfigurationManager.AppSettings.Keys;

            for (int i = 0; i < keys.Count; i++)
            {
                var key = keys[i];
                var match = regex.Match(key);
                if (match.Success)
                {
                    var parameterName = match.Groups["name"].Value;
                    var parameterValue = ConfigurationManager.AppSettings[key];
                    QueryParameters[parameterName] = parameterValue;
                }
            }
        }
    }
}
