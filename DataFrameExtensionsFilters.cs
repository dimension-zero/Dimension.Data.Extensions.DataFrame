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
            DataFrameColumn newColumn;
            if (column.DataType == typeof(double))
            {
                newColumn = new PrimitiveDataFrameColumn<double>(column.Name);
            }
            else if (column.DataType == typeof(string))
            {
                newColumn = new StringDataFrameColumn(column.Name);
            }
            // Add more types as needed
            else
            {
                throw new NotSupportedException($"Column type {column.DataType} is not supported");
            }

            newColumns.Add(newColumn);
        }

        var newDf = new Microsoft.Data.Analysis.DataFrame(newColumns);

        foreach (var rowIndex in rowsToKeep)
        {
            var row = df.Rows[rowIndex];
            newDf.AddRow(row);
        }

        return newDf;
    }
}