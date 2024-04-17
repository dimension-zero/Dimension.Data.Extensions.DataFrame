using System;
using System.Linq;
using System.Numerics;
using Microsoft.Data.Analysis;

namespace Dimension.DataFrame.Extensions;

/// <summary>
/// Syntactic sugar to make Microsoft's DataFrame a little more user-friendly.
/// </summary>
public static class DataFrameExtensionsSugar
{
    /// <summary>
    /// Renames a column in a DataFrame, enabling method-chaining.
    /// Usage: var someVariable = sourceColumn.WithName<double>("NewName");
    /// Allows df.Add(sourceColumn.WithName<double>("NewName"));
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="column"></param>
    /// <param name="newName"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    public static PrimitiveDataFrameColumn<T> WithName<T>(this DataFrameColumn? column, string newName)
        where T : unmanaged, INumber<T>
    {
        if (column is null)
        {
            throw new ArgumentNullException(nameof(column), "Column cannot be null.");
        }

        if (column is PrimitiveDataFrameColumn<T> typedColumn)
        {
            typedColumn.SetName(newName);
            return typedColumn;
        }
        else
        {
            throw new InvalidOperationException($"Column is not of type {typeof(T).Name}.");
        }
    }

    /// <summary>
    /// Allows chain method to be added to any kind of column transformation.
    /// So instead of
    ///     var newColumn = someColumn.Minus(someOtherColumn);
    ///     var squares = newColumn.Pow(2);
    ///     df.Columns.Add(squares);
    /// We can write:
    ///     var newColumn = someColumn.Minus(someOtherColumn).Pow(2).AddTo(df);
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="column"></param>
    /// <param name="df"></param>
    /// <returns></returns>
    public static PrimitiveDataFrameColumn<T> AddTo<T>(this PrimitiveDataFrameColumn<T> column, Microsoft.Data.Analysis.DataFrame df, string name = "", ClashBehaviour clashBehaviour = ClashBehaviour.Exception)
        where T : unmanaged, INumber<T>
    {
        if (!string.IsNullOrEmpty(name))
        {
            column.SetName(name);
        }

        if (df.Columns.Any(c => c.Name == column.Name))
        {
            switch (clashBehaviour)
            {
                case ClashBehaviour.KeepOriginal:
                    return column;
                case ClashBehaviour.ReplaceOriginal:
                    df.Columns.Remove(column.Name);
                    break;
                case ClashBehaviour.Exception:
                    throw new InvalidOperationException($"A column with the name '{column.Name}' already exists in the DataFrame.");
            }
        }

        df.Columns.Add(column);
        return column;
    }

    private static bool ValuesAreEqual<T>(T? a, T? b, T relativeTolerance)
        where T : struct, INumber<T>
    {
        if (!a.HasValue && !b.HasValue)
        {
            return true; // Both are null/missing
        }

        if (!a.HasValue || !b.HasValue)
        {
            return false; // One is null/missing, the other isn't
        }

        // Special handling for NaN values for floating-point types
        if (typeof(T) == typeof(float))
        {
            // Explicitly handle float NaN comparisons
            if (float.IsNaN((float) (object) a) && float.IsNaN((float) (object) b))
            {
                return true;
            }
        }
        else if (typeof(T) == typeof(double))
        {
            // Explicitly handle double NaN comparisons
            if (double.IsNaN((double) (object) a) && double.IsNaN((double) (object) b))
            {
                return true;
            }
        }

        // Calculate the absolute difference
        var absoluteDifference = a.Value - b.Value;
        if (absoluteDifference == T.Zero)
        {
            return true;
        }

        if (absoluteDifference < T.Zero)
        {
            absoluteDifference *= -T.One;
        }

        // Calculate the absolute maximum of the two numbers
        var maxAbsolute = a.Value > b.Value ? a.Value : b.Value;
        if (maxAbsolute == T.Zero)
        {
            // avoid DBZ error
            return a.Value == b.Value;
            return true;
        }

        // Calculate the relative difference based on the maximum absolute value
        var relativeDifference = absoluteDifference / maxAbsolute;

        // Check if the relative difference is within the relative tolerance
        return relativeDifference <= relativeTolerance;
    }

    private static T GetTolerance<T>() where T : struct, INumber<T>
    {
        // Define tolerance based on type
        if (typeof(T) == typeof(float))
        {
            return (T) (object) (float) 1e-6f; // Example tolerance for float
        }
        else if (typeof(T) == typeof(double))
        {
            return (T) (object) (double) 1e-15; // Example tolerance for double
        }
        else if (typeof(T) == typeof(decimal))
        {
            return (T) (object) (decimal) 1e-28M; // Lower tolerance for decimal
        }
        else if (typeof(T) == typeof(int) || typeof(T) == typeof(long))
        {
            // For integral types, exact match is expected, so tolerance is zero
            return T.Zero;
        }
        else
        {
            // Default tolerance for other types, adjust as necessary
            return T.One / T.CreateChecked(1000000);
        }
    }
}