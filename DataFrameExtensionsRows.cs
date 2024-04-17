using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CSharp.RuntimeBinder;

namespace Dimension.DataFrame.Extensions;

/// <summary>
/// Row extension methods to make Microsoft's DataFrame a little more user-friendly.
/// </summary>
public static class DataFrameExtensionsRows
{
    public static void AddRow(this Microsoft.Data.Analysis.DataFrame df, params object[] rowValues)
    {
        df.AddRow((IEnumerable<object>) rowValues);
    }

    public static void AddRow(this Microsoft.Data.Analysis.DataFrame df, IEnumerable<object> rowValues)
    {
        if (rowValues.Count() != df.Columns.Count)
        {
            throw new ArgumentException("The number of provided values must match the number of columns in the DataFrame.");
        }

        var rowValuesList = rowValues.ToList();

        for (var i = 0; i < df.Columns.Count; i++)
        {
            dynamic column = df.Columns[i];
            dynamic value = rowValuesList[i];
            try
            {
                column.Append(value);
            }
            catch (RuntimeBinderException ex)
            {
                throw new InvalidOperationException($"Error appending value to column '{column.Name}'. The value '{value}' may not be compatible with the column's data type.", ex);
            }
        }
    }
}