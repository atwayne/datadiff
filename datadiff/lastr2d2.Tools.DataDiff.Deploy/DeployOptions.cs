using System.Text;
using CommandLine;

namespace LastR2D2.Tools.DataDiff.Deploy
{
    internal class DeployOptions
    {
        [Option('o', "output", HelpText = "output file")]
        public string Output { get; set; }

        [Option('i', "input", HelpText = "input task file to read")]
        public string Input { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            var usage = new StringBuilder();
            usage.AppendLine("DataDiff Application by lastr2d2");
            return usage.ToString();
        }
    }
}
