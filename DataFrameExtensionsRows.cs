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
                // Use reflection to find the Append method on the column
                // DataFrame columns have Append methods with specific signatures (e.g., Append(int?), Append(double?))
                // We need to find the right overload by looking at all methods named "Append"
                var columnType = column.GetType();
                var appendMethods = columnType.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                    .Where(m => m.Name == "Append" && m.GetParameters().Length == 1)
                    .ToList();

                if (appendMethods.Count == 0)
                {
                    throw new InvalidOperationException(
                        $"Column '{column.Name}' (type: {columnType.Name}) does not have an Append method. " +
                        $"This may indicate an unsupported column type.");
                }

                // Try to find the best matching Append method
                MethodInfo? appendMethod = null;

                // First try: look for exact parameter type match
                if (value != null)
                {
                    var valueType = value.GetType();
                    appendMethod = appendMethods.FirstOrDefault(m => m.GetParameters()[0].ParameterType == valueType);
                }

                // Second try: look for nullable version of value type
                if (appendMethod == null && value != null)
                {
                    var valueType = value.GetType();
                    var nullableType = typeof(Nullable<>).MakeGenericType(valueType);
                    appendMethod = appendMethods.FirstOrDefault(m => m.GetParameters()[0].ParameterType == nullableType);
                }

                // Third try: use the first Append method found (will let the runtime handle type conversion)
                if (appendMethod == null)
                {
                    appendMethod = appendMethods.First();
                }

                appendMethod.Invoke(column, new[] { value });
            }
            catch (TargetInvocationException ex) when (ex.InnerException != null)
            {
                throw new InvalidOperationException(
                    $"Error appending value to column '{column.Name}' at index {i}. " +
                    $"The value '{value}' (type: {value?.GetType().Name ?? "null"}) is not compatible with the column's data type ({column.DataType.Name}). " +
                    $"Inner error: {ex.InnerException.Message}",
                    ex.InnerException);
            }
            catch (InvalidOperationException)
            {
                // Re-throw our own InvalidOperationExceptions without wrapping
                throw;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    $"Unexpected error appending value to column '{column.Name}' at index {i}. " +
                    $"Value: '{value}' (type: {value?.GetType().Name ?? "null"}), Column type: {column.DataType.Name}. " +
                    $"Error: {ex.Message}",
                    ex);
            }
        }
    }
}