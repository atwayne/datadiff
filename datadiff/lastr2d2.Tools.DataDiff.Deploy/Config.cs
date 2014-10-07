using System.Configuration;

namespace lastr2d2.Tools.DataDiff.Deploy
{
    internal static class Config
    {
        public static string DefaultInputDirectory { get; set; }
        public static string DefaultOutputFile { get; set; }
        public static string DefaultInputFileNamePattern { get; set; }

        public static void Load()
        {
            DefaultInputDirectory = ConfigurationManager.AppSettings["DefaultInputDirectory"] ?? "./tasks/";
            DefaultOutputFile = ConfigurationManager.AppSettings["DefaultOutputFile"] ?? "./CompareResult.xlst";
            DefaultInputFileNamePattern = ConfigurationManager.AppSettings["DefaultInputFileNamePattern"] ?? "*.xml";
        }
    }
}
