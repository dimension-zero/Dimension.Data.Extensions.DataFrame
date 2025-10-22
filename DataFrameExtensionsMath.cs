using System;
using System.Numerics;
using Microsoft.Data.Analysis;

namespace Dimension.DataFrame.Extensions;

/// <summary>
/// Mathematical extension methods to make Microsoft's DataFrame a little more user-friendly.
/// </summary>
public static class DataFrameExtensionsMath
{
    /// <summary>
    /// Calculates the absolute value of each element in a column
    /// </summary>
    /// <typeparam name="T">Numeric type</typeparam>
    /// <param name="column">Column to apply absolute value to</param>
    /// <param name="name">Optional name for the new column</param>
    /// <returns>New column with absolute values</returns>
    public static PrimitiveDataFrameColumn<T> Abs<T>(this PrimitiveDataFrameColumn<T> column, string name = "")
        where T : unmanaged, INumber<T>
    {
        if (string.IsNullOrEmpty(name))
        {
            name = column.Name + "_Abs";
        }

        var result = new PrimitiveDataFrameColumn<T>(name, column.Length);

        for (var i = 0; i < column.Length; i++)
        {
            var value = column[i];
            if (value.HasValue)
            {
                result[i] = T.Abs(value.Value);
            }
            else
            {
                result[i] = null;
            }
        }

        return result;
    }

    /// <summary>
    /// Calculates the natural logarithm (base e) of each element in a column
    /// </summary>
    /// <typeparam name="T">Numeric type</typeparam>
    /// <param name="column">Column to apply logarithm to</param>
    /// <param name="name">Optional name for the new column</param>
    /// <returns>New column with natural logarithm values</returns>
    public static PrimitiveDataFrameColumn<double> Log<T>(this PrimitiveDataFrameColumn<T> column, string name = "")
        where T : unmanaged, INumber<T>
    {
        if (string.IsNullOrEmpty(name))
        {
            name = column.Name + "_Log";
        }

        var result = new PrimitiveDataFrameColumn<double>(name, column.Length);

        for (var i = 0; i < column.Length; i++)
        {
            var value = column[i];
            if (value.HasValue)
            {
                var doubleValue = Convert.ToDouble(value.Value);
                if (doubleValue > 0)
                {
                    result[i] = Math.Log(doubleValue);
                }
                else
                {
                    result[i] = double.NaN; // Log of non-positive number
                }
            }
            else
            {
                result[i] = null;
            }
        }

        return result;
    }

    /// <summary>
    /// Calculates the logarithm with a specified base of each element in a column
    /// </summary>
    /// <typeparam name="T">Numeric type</typeparam>
    /// <param name="column">Column to apply logarithm to</param>
    /// <param name="logBase">Base of the logarithm</param>
    /// <param name="name">Optional name for the new column</param>
    /// <returns>New column with logarithm values</returns>
    public static PrimitiveDataFrameColumn<double> Log<T>(this PrimitiveDataFrameColumn<T> column, double logBase, string name = "")
        where T : unmanaged, INumber<T>
    {
        if (string.IsNullOrEmpty(name))
        {
            name = $"{column.Name}_Log{logBase}";
        }

        var result = new PrimitiveDataFrameColumn<double>(name, column.Length);

        for (var i = 0; i < column.Length; i++)
        {
            var value = column[i];
            if (value.HasValue)
            {
                var doubleValue = Convert.ToDouble(value.Value);
                if (doubleValue > 0 && logBase > 0 && logBase != 1)
                {
                    result[i] = Math.Log(doubleValue, logBase);
                }
                else
                {
                    result[i] = double.NaN;
                }
            }
            else
            {
                result[i] = null;
            }
        }

        return result;
    }

    /// <summary>
    /// Calculates the base-10 logarithm of each element in a column
    /// </summary>
    /// <typeparam name="T">Numeric type</typeparam>
    /// <param name="column">Column to apply logarithm to</param>
    /// <param name="name">Optional name for the new column</param>
    /// <returns>New column with base-10 logarithm values</returns>
    public static PrimitiveDataFrameColumn<double> Log10<T>(this PrimitiveDataFrameColumn<T> column, string name = "")
        where T : unmanaged, INumber<T>
    {
        if (string.IsNullOrEmpty(name))
        {
            name = column.Name + "_Log10";
        }

        var result = new PrimitiveDataFrameColumn<double>(name, column.Length);

        for (var i = 0; i < column.Length; i++)
        {
            var value = column[i];
            if (value.HasValue)
            {
                var doubleValue = Convert.ToDouble(value.Value);
                if (doubleValue > 0)
                {
                    result[i] = Math.Log10(doubleValue);
                }
                else
                {
                    result[i] = double.NaN;
                }
            }
            else
            {
                result[i] = null;
            }
        }

        return result;
    }

    /// <summary>
    /// Calculates e raised to the power of each element in a column
    /// </summary>
    /// <typeparam name="T">Numeric type</typeparam>
    /// <param name="column">Column to apply exponential to</param>
    /// <param name="name">Optional name for the new column</param>
    /// <returns>New column with exponential values</returns>
    public static PrimitiveDataFrameColumn<double> Exp<T>(this PrimitiveDataFrameColumn<T> column, string name = "")
        where T : unmanaged, INumber<T>
    {
        if (string.IsNullOrEmpty(name))
        {
            name = column.Name + "_Exp";
        }

        var result = new PrimitiveDataFrameColumn<double>(name, column.Length);

        for (var i = 0; i < column.Length; i++)
        {
            var value = column[i];
            if (value.HasValue)
            {
                var doubleValue = Convert.ToDouble(value.Value);
                result[i] = Math.Exp(doubleValue);
            }
            else
            {
                result[i] = null;
            }
        }

        return result;
    }

    /// <summary>
    /// Calculates the square root of each element in a column
    /// </summary>
    /// <typeparam name="T">Numeric type</typeparam>
    /// <param name="column">Column to apply square root to</param>
    /// <param name="name">Optional name for the new column</param>
    /// <returns>New column with square root values</returns>
    public static PrimitiveDataFrameColumn<double> Sqrt<T>(this PrimitiveDataFrameColumn<T> column, string name = "")
        where T : unmanaged, INumber<T>
    {
        if (string.IsNullOrEmpty(name))
        {
            name = column.Name + "_Sqrt";
        }

        var result = new PrimitiveDataFrameColumn<double>(name, column.Length);

        for (var i = 0; i < column.Length; i++)
        {
            var value = column[i];
            if (value.HasValue)
            {
                var doubleValue = Convert.ToDouble(value.Value);
                if (doubleValue >= 0)
                {
                    result[i] = Math.Sqrt(doubleValue);
                }
                else
                {
                    result[i] = double.NaN; // Square root of negative number
                }
            }
            else
            {
                result[i] = null;
            }
        }

        return result;
    }

    /// <summary>
    /// Calculates the sine of each element in a column (values in radians)
    /// </summary>
    /// <typeparam name="T">Numeric type</typeparam>
    /// <param name="column">Column to apply sine to</param>
    /// <param name="name">Optional name for the new column</param>
    /// <returns>New column with sine values</returns>
    public static PrimitiveDataFrameColumn<double> Sin<T>(this PrimitiveDataFrameColumn<T> column, string name = "")
        where T : unmanaged, INumber<T>
    {
        if (string.IsNullOrEmpty(name))
        {
            name = column.Name + "_Sin";
        }

        var result = new PrimitiveDataFrameColumn<double>(name, column.Length);

        for (var i = 0; i < column.Length; i++)
        {
            var value = column[i];
            if (value.HasValue)
            {
                result[i] = Math.Sin(Convert.ToDouble(value.Value));
            }
            else
            {
                result[i] = null;
            }
        }

        return result;
    }

    /// <summary>
    /// Calculates the cosine of each element in a column (values in radians)
    /// </summary>
    /// <typeparam name="T">Numeric type</typeparam>
    /// <param name="column">Column to apply cosine to</param>
    /// <param name="name">Optional name for the new column</param>
    /// <returns>New column with cosine values</returns>
    public static PrimitiveDataFrameColumn<double> Cos<T>(this PrimitiveDataFrameColumn<T> column, string name = "")
        where T : unmanaged, INumber<T>
    {
        if (string.IsNullOrEmpty(name))
        {
            name = column.Name + "_Cos";
        }

        var result = new PrimitiveDataFrameColumn<double>(name, column.Length);

        for (var i = 0; i < column.Length; i++)
        {
            var value = column[i];
            if (value.HasValue)
            {
                result[i] = Math.Cos(Convert.ToDouble(value.Value));
            }
            else
            {
                result[i] = null;
            }
        }

        return result;
    }

    /// <summary>
    /// Rounds each element in a column to the nearest integer
    /// </summary>
    /// <typeparam name="T">Numeric type</typeparam>
    /// <param name="column">Column to round</param>
    /// <param name="decimals">Number of decimal places (default 0)</param>
    /// <param name="name">Optional name for the new column</param>
    /// <returns>New column with rounded values</returns>
    public static PrimitiveDataFrameColumn<double> Round<T>(this PrimitiveDataFrameColumn<T> column, int decimals = 0, string name = "")
        where T : unmanaged, INumber<T>
    {
        if (string.IsNullOrEmpty(name))
        {
            name = column.Name + "_Round";
        }

        var result = new PrimitiveDataFrameColumn<double>(name, column.Length);

        for (var i = 0; i < column.Length; i++)
        {
            var value = column[i];
            if (value.HasValue)
            {
                result[i] = Math.Round(Convert.ToDouble(value.Value), decimals);
            }
            else
            {
                result[i] = null;
            }
        }

        return result;
    }
}
