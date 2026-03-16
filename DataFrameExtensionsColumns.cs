using System;
using System.Linq;
using Microsoft.Data.Analysis;

namespace Dimension.DataFrame.Extensions;

/// <summary>
/// Column extension methods to make Microsoft's DataFrame a little more user-friendly.
/// </summary>
public static class DataFrameExtensionsColumns
{
    public static Microsoft.Data.Analysis.DataFrame SelectColumns(this Microsoft.Data.Analysis.DataFrame dataframe, params string[] columnNames)
    {
        var selectedColumns = dataframe.Columns
            .Where(c => columnNames.Contains(c.Name))
            .ToList();

        if (selectedColumns.Count != columnNames.Length)
        {
            throw new ArgumentException("One or more column names do not exist in the DataFrame.");
        }

        return new Microsoft.Data.Analysis.DataFrame(selectedColumns);
    }

    public static bool ColumnExists(this Microsoft.Data.Analysis.DataFrame dataFrame, string columnName)
    {
        return dataFrame.Columns.Any(column => column.Name == columnName);
    }

    public static bool TryGetColumn<T>(this Microsoft.Data.Analysis.DataFrame dataFrame, string columnName, out PrimitiveDataFrameColumn<T>? column)
        where T : unmanaged
    {
        column = null;
        var untypedColumn = dataFrame.Columns.FirstOrDefault(col => col.Name == columnName);
        if (untypedColumn is PrimitiveDataFrameColumn<T> typedColumn)
        {
            column = typedColumn;
            return true;
        }

        return false;
    }
}