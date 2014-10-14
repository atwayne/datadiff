using System;
using System.Collections.Generic;

namespace LastR2D2.Tools.DataDiff.Core.Model
{
    public class Field
    {
        public Type FieldType { get; set; }

        public bool IsKey { get; set; }

        public string Name { get; set; }

        public double Gap { get; set; }

        private static readonly HashSet<Type> NumericTypes = new HashSet<Type>
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
                return NumericTypes.Contains(FieldType) ||
                       NumericTypes.Contains(Nullable.GetUnderlyingType(FieldType));
            }
        }
    }
}