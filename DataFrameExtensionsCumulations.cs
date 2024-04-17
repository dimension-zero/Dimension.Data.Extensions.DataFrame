using System.Numerics;
using Microsoft.Data.Analysis;

namespace Dimension.DataFrame.Extensions;

/// <summary>
/// Cumulative extension methods to make Microsoft's DataFrame a little more user-friendly.
/// </summary>
public static class DataFrameExtensionsCumulations
{
    public static PrimitiveDataFrameColumn<T> Cumulate<T>(this PrimitiveDataFrameColumn<T>? column, string newName = "", bool useNaN = false)
        where T : unmanaged, INumber<T>
    {
        var newColumnName = string.IsNullOrEmpty(newName) ? column.Name + "_Cumulative" : newName;
        var newColumn = new PrimitiveDataFrameColumn<T>(newColumnName, new T[column.Length]);
        T? sum = T.Zero;
        for (var i = 0; i < column.Length; i++)
        {
            var value = column[i];
            if (value.HasValue && sum.HasValue)
            {
                sum          += value.Value;
                newColumn[i] =  sum;
            }
            else
            {
                newColumn[i] = useNaN ? CreateNaN<T>() : default;
            }
        }

        return newColumn;
    }

    public static PrimitiveDataFrameColumn<T> CumulateAbs<T>(this PrimitiveDataFrameColumn<T> column, string newName = "", bool useNaN = false)
        where T : unmanaged, INumber<T>
    {
        if (string.IsNullOrEmpty(newName))
        {
            newName = string.IsNullOrEmpty(newName) ? column.Name + "_Abs" : newName;
        }

        var newColumn = new PrimitiveDataFrameColumn<T>(newName, new T[column.Length]);
        T? sum = T.Zero;
        for (var i = 0; i < column.Length; i++)
        {
            var value = column[i];
            if (value.HasValue && sum.HasValue)
            {
                var absValue = T.Abs(value.Value);
                sum          += absValue;
                newColumn[i] =  sum;
            }
            else
            {
                newColumn[i] = useNaN ? CreateNaN<T>() : default;
            }
        }

        return newColumn;
    }

    // Utility method to generate NaN for supported types, default otherwise
    private static T CreateNaN<T>()
        where T : unmanaged, INumber<T>
    {
        if (typeof(T) == typeof(float))
        {
            return (T) (object) float.NaN;
        }
        else if (typeof(T) == typeof(double))
        {
            return (T) (object) double.NaN;
        }
        // Extend to other types if they support NaN
        else
        {
            return default; // Fallback for types that do not support NaN
        }
    }
}