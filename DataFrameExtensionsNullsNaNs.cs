using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Microsoft.Data.Analysis;

namespace Dimension.DataFrame.Extensions;

/// <summary>
/// Extension methods to clean up nulls and NaN's to make Microsoft's DataFrame a little more user-friendly.
/// </summary>
public static class DataFrameExtensionsNullsNaNs
{
    public static PrimitiveDataFrameColumn<T> DropNulls<T>(this PrimitiveDataFrameColumn<T> column)
        where T : unmanaged, INumber<T>
    {
        var newColumn = new PrimitiveDataFrameColumn<T>(column.Name, column.Length);
        foreach (var value in column)
        {
            var shouldAddValue = value != null && !(value is float f && float.IsNaN(f)) && !(value is double d && double.IsNaN(d));
            if (shouldAddValue)
            {
                newColumn.Append(value);
            }
        }

        return newColumn;
    }

    public static Microsoft.Data.Analysis.DataFrame DropNulls(this Microsoft.Data.Analysis.DataFrame df)
    {
        var rowsToKeep = Enumerable.Range(0, (int) df.Rows.Count)
            .Where(i => !df.Rows[i].HasNulls())
            .ToList();
        return df.Filter(rowsToKeep);
    }

    public static Microsoft.Data.Analysis.DataFrame DropNAs(this Microsoft.Data.Analysis.DataFrame df)
    {
        var rowsToKeep = new List<int>();
        for (var i = 0; i < df.Rows.Count; i++)
        {
            var row = df.Rows[i];
            var hasNull = false;
            foreach (var cell in row)
            {
                if (cell == null || (cell is float && float.IsNaN((float) cell)) || (cell is double && double.IsNaN((double) cell)))
                {
                    hasNull = true;
                    break;
                }
            }

            if (!hasNull)
            {
                rowsToKeep.Add(i);
            }
        }

        return df.Filter(rowsToKeep);
    }

    public static Microsoft.Data.Analysis.DataFrame DropNullsOrNAs(this Microsoft.Data.Analysis.DataFrame df)
    {
        var rowsToKeep = Enumerable.Range(0, (int) df.Rows.Count)
            .Where(i => !df.Rows[i].HasNullsOrNAs())
            .ToList();

        return df.Filter(rowsToKeep);
    }

    private static bool HasNullsOrNAs(this DataFrameRow row)
    {
        foreach (var cell in row)
        {
            if (cell == null || IsNaN(cell))
            {
                return true;
            }
        }

        return false;
    }


    public static bool HasNulls(this DataFrameRow row)
    {
        foreach (var cell in row)
        {
            if (cell == null)
            {
                return true;
            }
        }

        return false;
    }

    public static bool HasNulls(this DataFrameColumn column)
    {
        foreach (var cell in column)
        {
            if (cell == null)
            {
                return true;
            }
        }

        return false;
    }

    private static bool IsNaN(object cell)
    {
        return (cell is float f && float.IsNaN(f)) || (cell is double d && double.IsNaN(d));
    }
}