using System;
using System.Numerics;
using Microsoft.Data.Analysis;

namespace Dimension.DataFrame.Extensions;

/// <summary>
/// Shift extension methods to make Microsoft's DataFrame a little more user-friendly.
/// </summary>
public static class DataFrameExtensionsShifts
{
    /// <summary>
    /// Shifts a column by a specified number of rows
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="column"Source columns to shift></param>
    /// <param name="rows">Number of rows to shift the column</param>
    /// <param name="fillValue">Value to use in cells vacated by shift</param>
    /// <param name="name">Optional name of shifted column</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static PrimitiveDataFrameColumn<T> Shift<T>(this PrimitiveDataFrameColumn<T>? column,
                                                       int                               rows,
                                                       T?                                fillValue = null,
                                                       string                            name      = "")
        where T : unmanaged, INumber<T>
    {
        if (column == null)
        {
            throw new ArgumentNullException(nameof(column), "Column cannot be null.");
        }

        var columnName = string.IsNullOrEmpty(name) ? $"{column.Name}_Shifted{rows}" : name;
        var newColumn = new PrimitiveDataFrameColumn<T>(columnName, column.Length);

        // Determine the start and end indices for the shift
        long start = rows > 0 ? rows : 0;
        var end = rows > 0 ? column.Length : column.Length + rows;

        // Fill the beginning of the column with the fill value if shifting forward
        for (long i = 0; i < start && i < newColumn.Length; i++)
        {
            newColumn[i] = fillValue;
        }

        // Shift the values
        for (var i = start; i < end; i++)
        {
            var value = column[i - rows];
            if (value is T tValue)
            {
                newColumn[i] = tValue;
            }
            else
            {
                newColumn[i] = fillValue;
            }
        }

        // Fill the end of the column with the fill value if shifting backward
        for (var i = end; i < newColumn.Length; i++)
        {
            newColumn[i] = fillValue;
        }

        return newColumn;
    }
}