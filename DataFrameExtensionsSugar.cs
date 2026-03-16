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
}