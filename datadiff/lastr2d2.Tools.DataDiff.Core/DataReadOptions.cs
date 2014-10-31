using System.Collections.Generic;
namespace LastR2D2.Tools.DataDiff.Core
{
    public abstract class DataReadOptions
    {
        public string TableName { get; protected set; }
        public ICollection<string> PrimaryColumnNames { get; protected set; }
    }
}
