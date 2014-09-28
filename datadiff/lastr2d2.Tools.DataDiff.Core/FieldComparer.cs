using System;
using System.Data;
using lastr2d2.Tools.DataDiff.Core.Model;

namespace lastr2d2.Tools.DataDiff.Core
{
    public static class FieldCompare
    {
        public static FieldCompareResult Compare(DataRow left, DataRow right, Field field)
        {
            var missingInLeft = left.IsNull(field.Name);
            var missingInRight = right.IsNull(field.Name);
            if (missingInLeft && missingInRight)
                return FieldCompareResult.Identical;

            if (missingInLeft)
                return FieldCompareResult.MissingInLeft;
            if (missingInRight)
                return FieldCompareResult.MissingInRight;
            var leftValue = left[field.Name];
            var rightValue = right[field.Name];

            if (field.Type == null && (left.Table.Columns[field.Name].DataType == right.Table.Columns[field.Name].DataType))
            {
                field.Type = left.Table.Columns[field.Name].DataType;
            }

            if (field.IsNumericType)
            {
                var leftNumber = Convert.ToDouble(leftValue);
                var rightNumber = Convert.ToDouble(rightValue);
                var diff = leftNumber - rightNumber;

                if (diff < double.Epsilon)
                    return FieldCompareResult.Identical;

                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (leftNumber == 0)
                    return FieldCompareResult.QuiteDifferent;

                var gap = Math.Abs(diff / leftNumber);
                if (gap < field.Gap)
                    return FieldCompareResult.Similar;

                return FieldCompareResult.QuiteDifferent;
            }

            if (field.Type == typeof(DateTime))
            {
                var leftDate = Convert.ToDateTime(leftValue);
                var rightDate = Convert.ToDateTime(rightValue);

                if (leftDate == rightDate)
                    return FieldCompareResult.Identical;

                return FieldCompareResult.QuiteDifferent;
            }

            var leftText = leftValue.ToString().Trim();
            var rightText = rightValue.ToString().Trim();
            if (leftText.Equals(rightText, StringComparison.CurrentCultureIgnoreCase))
                return FieldCompareResult.Identical;

            return FieldCompareResult.QuiteDifferent;
        }
    }
}