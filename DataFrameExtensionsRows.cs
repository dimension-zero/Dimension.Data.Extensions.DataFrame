using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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
            var column = df.Columns[i];
            var value = rowValuesList[i];

            try
            {
                // Use reflection to call the Append method on the column
                var appendMethod = column.GetType().GetMethod("Append", new[] { typeof(object) });
                if (appendMethod != null)
                {
                    appendMethod.Invoke(column, new[] { value });
                }
                else
                {
                    throw new InvalidOperationException($"Column '{column.Name}' does not have an Append method.");
                }
            }
            catch (TargetInvocationException ex) when (ex.InnerException != null)
            {
                throw new InvalidOperationException(
                    $"Error appending value to column '{column.Name}'. The value '{value}' (type: {value?.GetType().Name ?? "null"}) may not be compatible with the column's data type ({column.DataType.Name}).",
                    ex.InnerException);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    $"Error appending value to column '{column.Name}'. The value '{value}' may not be compatible with the column's data type.",
                    ex);
            }
        }
    }
}