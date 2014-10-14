using System.Collections.Generic;
using System.Xml.Serialization;

namespace LastR2D2.Tools.DataDiff.Core.Model
{
    [XmlRoot("Tasks")]
    public class Tasks : List<Task>
    {
    }
}