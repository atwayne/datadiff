using System;
using System.Collections.Generic;

namespace lastr2d2.Tools.DataDiff.Core.Model
{
    public class Field
    {
        public Type Type { get; set; }

        public bool IsKey { get; set; }

        public string Name { get; set; }

        public double Gap { get; set; }

        private static HashSet<Type> NumericTypes = new HashSet<Type>
        {
            typeof(int),
            typeof(uint),
            typeof(double),
            typeof(decimal),
            typeof(short),
            typeof(ushort),
            typeof(byte),
            typeof(sbyte)
        };

        public bool IsNumericType
        {
            get
            {
                return NumericTypes.Contains(this.Type) ||
                       NumericTypes.Contains(Nullable.GetUnderlyingType(this.Type));
            }
        }
    }
}