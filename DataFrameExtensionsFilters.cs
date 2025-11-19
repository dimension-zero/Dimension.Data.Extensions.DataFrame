using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.Analysis;

namespace Dimension.DataFrame.Extensions;

/// <summary>
/// Filter extension methods to make Microsoft's DataFrame a little more user-friendly.
/// </summary>
public static class DataFrameExtensionsFilters
{
    public static Microsoft.Data.Analysis.DataFrame Filter<T>(this Microsoft.Data.Analysis.DataFrame df, string filteringColumnName, Func<T?, bool> predicate)
        where T : unmanaged
    {
        // Attempt to get the specified column and verify its type.
        if (df.Columns.All(c => c.Name != filteringColumnName))
        {
            throw new ArgumentException($"DataFrame doesn't contain column called {filteringColumnName}");
        }

        var column = df[filteringColumnName];
        if (column.DataType != typeof(T))
        {
            throw new ArgumentException($"Column {filteringColumnName} is not of type {typeof(T).Name}.");
        }

        // Cast the column to the appropriate type.
        var typedColumn = column as PrimitiveDataFrameColumn<T>;
        if (typedColumn == null)
        {
            throw new InvalidOperationException($"Column {filteringColumnName} could not be cast to type {typeof(T).Name}.");
        }

        // Create a boolean mask for rows to keep based on the predicate.
        var mask = new PrimitiveDataFrameColumn<bool>("Filter", column.Length);
        for (var i = 0; i < column.Length; i++)
        {
            mask[i] = predicate(typedColumn[i]);
        }

        // Filter the DataFrame based on the mask and return the result.
        return df.Filter(mask);
    }


    public static Microsoft.Data.Analysis.DataFrame Filter(this Microsoft.Data.Analysis.DataFrame df, IEnumerable<int> rowsToKeep)
    {
        var newColumns = new List<DataFrameColumn>();
        foreach (var column in df.Columns)
        {
            var newColumn = CreateColumnByType(column.DataType, column.Name);
            newColumns.Add(newColumn);
        }

        var newDf = new Microsoft.Data.Analysis.DataFrame(newColumns);

        foreach (var rowIndex in rowsToKeep)
        {
            if (rowIndex < 0 || rowIndex >= df.Rows.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(rowsToKeep),
                    $"Row index {rowIndex} is out of bounds. DataFrame has {df.Rows.Count} rows (valid indices: 0 to {df.Rows.Count - 1}).");
            }

            var row = df.Rows[rowIndex];
            newDf.AddRow(row);
        }

        return newDf;
    }

    /// <summary>
    /// Creates a new DataFrame column based on the specified type
    /// </summary>
    /// <param name="dataType">The type of data the column will hold</param>
    /// <param name="columnName">The name for the new column</param>
    /// <returns>A new DataFrameColumn of the appropriate type</returns>
    /// <exception cref="NotSupportedException">Thrown when the data type is not supported</exception>
    private static DataFrameColumn CreateColumnByType(Type dataType, string columnName)
    {
        // Use pattern matching for cleaner type checking
        if (dataType == typeof(int)) return new PrimitiveDataFrameColumn<int>(columnName);
        if (dataType == typeof(long)) return new PrimitiveDataFrameColumn<long>(columnName);
        if (dataType == typeof(float)) return new PrimitiveDataFrameColumn<float>(columnName);
        if (dataType == typeof(double)) return new PrimitiveDataFrameColumn<double>(columnName);
        if (dataType == typeof(decimal)) return new PrimitiveDataFrameColumn<decimal>(columnName);
        if (dataType == typeof(bool)) return new PrimitiveDataFrameColumn<bool>(columnName);
        if (dataType == typeof(byte)) return new PrimitiveDataFrameColumn<byte>(columnName);
        if (dataType == typeof(sbyte)) return new PrimitiveDataFrameColumn<sbyte>(columnName);
        if (dataType == typeof(short)) return new PrimitiveDataFrameColumn<short>(columnName);
        if (dataType == typeof(ushort)) return new PrimitiveDataFrameColumn<ushort>(columnName);
        if (dataType == typeof(uint)) return new PrimitiveDataFrameColumn<uint>(columnName);
        if (dataType == typeof(ulong)) return new PrimitiveDataFrameColumn<ulong>(columnName);
        if (dataType == typeof(char)) return new PrimitiveDataFrameColumn<char>(columnName);
        if (dataType == typeof(DateTime)) return new PrimitiveDataFrameColumn<DateTime>(columnName);
        if (dataType == typeof(string)) return new StringDataFrameColumn(columnName);

        throw new NotSupportedException(
            $"Column type {dataType.Name} is not supported. " +
            "Supported types: int, long, float, double, decimal, bool, byte, sbyte, short, ushort, uint, ulong, char, DateTime, string");
    }
}