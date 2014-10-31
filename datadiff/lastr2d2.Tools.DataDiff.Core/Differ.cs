using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LastR2D2.Tools.DataDiff.Core.Interfaces;
using LastR2D2.Tools.DataDiff.Core.Model;
using Task = LastR2D2.Tools.DataDiff.Core.Model.Task;

namespace LastR2D2.Tools.DataDiff.Core
{
    public class Differ : IDiffer
    {
        private Task task { get; set; }

        public Differ(Task task)
        {
            this.task = task;
        }

        public void Diff() { }
    }
}
