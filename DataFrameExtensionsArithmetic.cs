using System;
using System.Linq;
using System.Numerics;
using Microsoft.Data.Analysis;

namespace Dimension.DataFrame.Extensions;

/// <summary>
/// Arithmetic extension methods to make Microsoft's DataFrame a little more user-friendly.
/// </summary>
public static class DataFrameExtensionsArithmetic
{
    public static PrimitiveDataFrameColumn<T> Plus<T>(this PrimitiveDataFrameColumn<T> column, PrimitiveDataFrameColumn<T> otherColumn, string name = "")
        where T : unmanaged, INumber<T>
    {
        return column.Plus<T>(name, otherColumn);
    }

    public static PrimitiveDataFrameColumn<T> Plus<T>(this PrimitiveDataFrameColumn<T> column, string name = "", params PrimitiveDataFrameColumn<T>[] otherColumns)
        where T : unmanaged, INumber<T>
    {
        if (otherColumns.Any(c => c.Length != column.Length))
        {
            throw new ArgumentException("All columns must have the same length.");
        }

        var result = new T[column.Length];
        for (var i = 0; i < column.Length; i++)
        {
            var sum = column[i] ?? default;
            foreach (var otherColumn in otherColumns)
            {
                sum += otherColumn[i] ?? default;
            }

            result[i] = sum;
        }

        if (string.IsNullOrEmpty(name))
        {
            var namesToConcat = new[] {column.Name}.Concat(otherColumns.Select(c => c.Name));
            name = string.Join("+", namesToConcat);
        }

        return new PrimitiveDataFrameColumn<T>(name, result);
    }

    public static PrimitiveDataFrameColumn<T> Minus<T>(this PrimitiveDataFrameColumn<T> column, PrimitiveDataFrameColumn<T> columnToSubtract, string name = "")
        where T : unmanaged, INumber<T>
    {
        if (column.Length != columnToSubtract.Length)
        {
            throw new ArgumentException("Both columns must have the same length.");
        }

        var result = new T[column.Length];
        for (var i = 0; i < column.Length; i++)
        {
            var value1 = column[i] ?? default;           // Handle nulls as default(T)
            var value2 = columnToSubtract[i] ?? default; // Handle nulls as default(T)
            result[i] = value1 - value2;                 // Correct use of subtraction with INumber<T>
        }

        if (string.IsNullOrEmpty(name))
        {
            name = $"{column.Name}_Minus_{columnToSubtract.Name}";
        }

        return new PrimitiveDataFrameColumn<T>(name, result);
    }

    public static PrimitiveDataFrameColumn<T> Times<T>(this PrimitiveDataFrameColumn<T> column, PrimitiveDataFrameColumn<T> columnToMultiplyBy, string name = "")
        where T : unmanaged, INumber<T>
    {
        // Wrapper around the Times method with params
        return Times(column, name, columnToMultiplyBy);
    }

    // Times method for a params array of columns
    public static PrimitiveDataFrameColumn<T> Times<T>(this PrimitiveDataFrameColumn<T> column, string name = "", params PrimitiveDataFrameColumn<T>[] otherColumns)
        where T : unmanaged, INumber<T>
    {
        if (otherColumns.Any(c => c.Length != column.Length))
        {
            throw new ArgumentException("All columns must have the same length.");
        }

        var result = new T[column.Length];
        for (var i = 0; i < column.Length; i++)
        {
            var product = column[i] ?? default;
            foreach (var otherColumn in otherColumns)
            {
                product *= otherColumn[i] ?? default;
            }

            result[i] = product;
        }

        if (string.IsNullOrEmpty(name))
        {
            var namesToConcat = new[] {column.Name}.Concat(otherColumns.Select(c => c.Name));
            name = $"{column.Name}_Times_{string.Join("_", namesToConcat)}";
        }

        return new PrimitiveDataFrameColumn<T>(name, result);
    }

    public static PrimitiveDataFrameColumn<T> Divide<T>(this PrimitiveDataFrameColumn<T> numeratorColumn, PrimitiveDataFrameColumn<T> divisorColumn, string name)
        where T : unmanaged, INumber<T>
    {
        if (numeratorColumn.Length != divisorColumn.Length)
        {
            throw new ArgumentException("Both columns must have the same length.");
        }

        var result = new T[numeratorColumn.Length];
        for (var i = 0; i < numeratorColumn.Length; i++)
        {
            var numerator = numeratorColumn[i] ?? default; // Handle nulls as default(T)
            var divisor = divisorColumn[i] ?? default;     // Handle nulls as default(T)

            if (divisor == T.Zero || divisor == default) // Check for division by zero
            {
                result[i] = typeof(T) == typeof(float) ? (T) (object) float.NaN :
                            typeof(T) == typeof(double) ? (T) (object) double.NaN :
                            default;
            }
            else
            {
                result[i] = numerator / divisor; // Perform division
            }
        }

        if (string.IsNullOrEmpty(name))
        {
            name = $"{numeratorColumn.Name}/{divisorColumn.Name}";
        }

        return new PrimitiveDataFrameColumn<T>(name, result);
    }
}