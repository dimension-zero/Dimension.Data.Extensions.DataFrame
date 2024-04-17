using System;
using System.Linq;
using System.Numerics;
using Microsoft.Data.Analysis;

namespace Dimension.DataFrame.Extensions;

/// <summary>
/// Methods for adding calculations extension methods to make Microsoft's DataFrame a little more user-friendly.
/// </summary>
public static class DataFrameExtensionsCalculations
{
    public static PrimitiveDataFrameColumn<T>? Diff<T>(this DataFrameColumn? column, string name = "", T? seed = default)
        where T : unmanaged, INumber<T>
    {
        if (column is null)
        {
            return null;
        }

        var newName = string.IsNullOrEmpty(name) ? column.Name + "_Diff" : name;
        var newColumn = new PrimitiveDataFrameColumn<T>(newName, Enumerable.Repeat(seed, (int) column.Length));
        for (var i = 1; i < column.Length; i++)
        {
            newColumn[i] = (dynamic) column[i] - (dynamic) column[i - 1];
        }

        return newColumn;
    }

    public static PrimitiveDataFrameColumn<T> Apply<T>(this PrimitiveDataFrameColumn<T> column, Func<T, T> operation, string name = "")
        where T : unmanaged, INumber<T>
    {
        if (string.IsNullOrEmpty(name))
        {
            name = string.IsNullOrEmpty(name) ? column.Name + "_Applied" : name;
        }

        var newColumn = new PrimitiveDataFrameColumn<T>(name, column.Length);
        for (var i = 0; i < column.Length; i++)
        {
            var rawValue = column[i];
            if (rawValue != null)
            {
                var castedValue = (T) rawValue;
                newColumn[i] = operation(castedValue);
            }
            else
            {
                newColumn[i] = null;
            }
        }

        return newColumn;
    }

    public static PrimitiveDataFrameColumn<T> Pow<T>(this PrimitiveDataFrameColumn<T> column, double power, string name = "")
        where T : unmanaged, INumber<T>
    {
        var result = new T[column.Length];
        for (var i = 0; i < column.Length; i++)
        {
            var value = column[i];
            if (value.HasValue)
            {
                // Convert to double, apply power, and then try to convert back to T.
                // This approach has limitations and might not work for all INumber<T> types, especially those that cannot be accurately represented as double.
                var poweredValue = Math.Pow(Convert.ToDouble(value.GetValueOrDefault()), power);
                result[i] = T.CreateChecked(poweredValue);
            }
            else
            {
                // Use default(T) to represent missing values, as NaN is not universally applicable.
                result[i] = default;
            }
        }

        if (string.IsNullOrEmpty(name))
        {
            name = $"{column.Name}_Pow{power}";
        }

        return new PrimitiveDataFrameColumn<T>(name, result);
    }
}