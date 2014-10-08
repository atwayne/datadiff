using System.Collections.Generic;
using System.Xml.Serialization;

namespace lastr2d2.Tools.DataDiff.Core.Model
{
    [XmlRoot("Tasks")]
    public class Tasks : List<Task>
    {
    }
}
