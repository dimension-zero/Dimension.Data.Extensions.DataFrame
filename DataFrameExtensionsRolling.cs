using System;
using System.Collections.Generic;
using System.Numerics;
using Microsoft.Data.Analysis;

namespace Dimension.DataFrame.Extensions;

/// <summary>
/// Methods for adding rolling calculations extension methods to make Microsoft's DataFrame a little more user-friendly.
/// </summary>
public static class DataFrameExtensionsRolling
{
    /// <summary>
    /// Applies a function (operation) to a rolling window of a column
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="column"></param>
    /// <param name="windowSize"></param>
    /// <param name="operation"></param>
    /// <returns></returns>
    public static PrimitiveDataFrameColumn<T> Rolling<T>(this PrimitiveDataFrameColumn<T> column,
                                                         int                              windowSize,
                                                         Func<IEnumerable<T?>, T>         operation)
        where T : unmanaged, INumber<T>
    {
        var result = new PrimitiveDataFrameColumn<T>(column.Name + "_Rolling", column.Length);
        for (var i = 0; i < column.Length; i++)
        {
            if (i < windowSize - 1)
            {
                result[i] = null; // Not enough values to fill the window
                continue;
            }

            var window = new List<T?>();
            for (var j = i - windowSize + 1; j <= i; j++)
            {
                if (!column[j].HasValue)
                {
                    continue;
                }

                window.Add(column[j]);
            }

            if (window.Count > 0)
            {
                var opResult = operation(window);
                result[i] = opResult;
            }
            else
            {
                result[i] = null;
            }
        }

        return result;
    }

    public static RollingWindow<T> Rolling<T>(this PrimitiveDataFrameColumn<T> column, int windowSize)
        where T : unmanaged, INumber<T>
    {
        return new RollingWindow<T>(column, windowSize);
    }

    public static PrimitiveDataFrameColumn<T> GetRange<T>(this PrimitiveDataFrameColumn<T> column, int startIndex, int count)
        where T : unmanaged
    {
        if (startIndex < 0 || startIndex >= column.Length)
            throw new ArgumentOutOfRangeException(nameof(startIndex), "Start index is out of range.");

        if (count < 0 || startIndex + count > column.Length)
            throw new ArgumentOutOfRangeException(nameof(count), "Count is out of range.");

        var result = new PrimitiveDataFrameColumn<T>(column.Name + "_Range", count);

        for (int i = 0; i < count; i++)
        {
            result[i] = column[startIndex + i];
        }

        return result;
    }

}