using System;
using System.Linq;
using System.Numerics;
using Microsoft.Data.Analysis;

namespace Dimension.DataFrame.Extensions;

/// <summary>
/// Statistical extension methods to make Microsoft's DataFrame a little more user-friendly.
/// </summary>
public static class DataFrameExtensionsStatistics
{
    /// <summary>
    /// Calculates the mean (average) of a column
    /// </summary>
    /// <typeparam name="T">Numeric type</typeparam>
    /// <param name="column">Column to calculate mean for</param>
    /// <returns>Mean value, or null if column is empty or all values are null</returns>
    public static T? Mean<T>(this PrimitiveDataFrameColumn<T> column)
        where T : unmanaged, INumber<T>
    {
        if (column == null || column.Length == 0)
        {
            return null;
        }

        var sum = T.Zero;
        var count = 0;

        for (var i = 0; i < column.Length; i++)
        {
            var value = column[i];
            if (value.HasValue)
            {
                sum += value.Value;
                count++;
            }
        }

        if (count == 0)
        {
            return null;
        }

        return sum / T.CreateChecked(count);
    }

    /// <summary>
    /// Calculates the median of a column
    /// </summary>
    /// <typeparam name="T">Numeric type</typeparam>
    /// <param name="column">Column to calculate median for</param>
    /// <returns>Median value, or null if column is empty or all values are null</returns>
    public static T? Median<T>(this PrimitiveDataFrameColumn<T> column)
        where T : unmanaged, INumber<T>
    {
        if (column == null || column.Length == 0)
        {
            return null;
        }

        var values = column.Where(v => v.HasValue).Select(v => v!.Value).OrderBy(v => v).ToList();

        if (values.Count == 0)
        {
            return null;
        }

        var middleIndex = values.Count / 2;

        if (values.Count % 2 == 0)
        {
            // Even number of elements - average the two middle values
            return (values[middleIndex - 1] + values[middleIndex]) / T.CreateChecked(2);
        }
        else
        {
            // Odd number of elements - return the middle value
            return values[middleIndex];
        }
    }

    /// <summary>
    /// Calculates the standard deviation of a column (population standard deviation)
    /// </summary>
    /// <typeparam name="T">Numeric type</typeparam>
    /// <param name="column">Column to calculate standard deviation for</param>
    /// <param name="sample">If true, calculates sample standard deviation (n-1); if false, population standard deviation (n)</param>
    /// <returns>Standard deviation, or null if column has fewer than 2 values</returns>
    public static double? StdDev<T>(this PrimitiveDataFrameColumn<T> column, bool sample = true)
        where T : unmanaged, INumber<T>
    {
        var variance = column.Variance(sample);
        return variance.HasValue ? Math.Sqrt(variance.Value) : null;
    }

    /// <summary>
    /// Calculates the variance of a column
    /// </summary>
    /// <typeparam name="T">Numeric type</typeparam>
    /// <param name="column">Column to calculate variance for</param>
    /// <param name="sample">If true, calculates sample variance (n-1); if false, population variance (n)</param>
    /// <returns>Variance, or null if column has fewer than 2 values</returns>
    public static double? Variance<T>(this PrimitiveDataFrameColumn<T> column, bool sample = true)
        where T : unmanaged, INumber<T>
    {
        if (column == null || column.Length == 0)
        {
            return null;
        }

        var values = column.Where(v => v.HasValue).Select(v => Convert.ToDouble(v!.Value)).ToList();

        if (values.Count < (sample ? 2 : 1))
        {
            return null;
        }

        var mean = values.Average();
        var sumOfSquaredDifferences = values.Sum(v => Math.Pow(v - mean, 2));
        var divisor = sample ? values.Count - 1 : values.Count;

        return sumOfSquaredDifferences / divisor;
    }

    /// <summary>
    /// Calculates the minimum value in a column
    /// </summary>
    /// <typeparam name="T">Numeric type</typeparam>
    /// <param name="column">Column to find minimum for</param>
    /// <returns>Minimum value, or null if column is empty or all values are null</returns>
    public static T? Min<T>(this PrimitiveDataFrameColumn<T> column)
        where T : unmanaged, INumber<T>
    {
        if (column == null || column.Length == 0)
        {
            return null;
        }

        var values = column.Where(v => v.HasValue).Select(v => v!.Value);

        return values.Any() ? values.Min() : null;
    }

    /// <summary>
    /// Calculates the maximum value in a column
    /// </summary>
    /// <typeparam name="T">Numeric type</typeparam>
    /// <param name="column">Column to find maximum for</param>
    /// <returns>Maximum value, or null if column is empty or all values are null</returns>
    public static T? Max<T>(this PrimitiveDataFrameColumn<T> column)
        where T : unmanaged, INumber<T>
    {
        if (column == null || column.Length == 0)
        {
            return null;
        }

        var values = column.Where(v => v.HasValue).Select(v => v!.Value);

        return values.Any() ? values.Max() : null;
    }

    /// <summary>
    /// Calculates the sum of all values in a column
    /// </summary>
    /// <typeparam name="T">Numeric type</typeparam>
    /// <param name="column">Column to calculate sum for</param>
    /// <returns>Sum of all non-null values</returns>
    public static T Sum<T>(this PrimitiveDataFrameColumn<T> column)
        where T : unmanaged, INumber<T>
    {
        if (column == null || column.Length == 0)
        {
            return T.Zero;
        }

        var sum = T.Zero;

        for (var i = 0; i < column.Length; i++)
        {
            var value = column[i];
            if (value.HasValue)
            {
                sum += value.Value;
            }
        }

        return sum;
    }

    /// <summary>
    /// Calculates the count of non-null values in a column
    /// </summary>
    /// <typeparam name="T">Numeric type</typeparam>
    /// <param name="column">Column to count values for</param>
    /// <returns>Count of non-null values</returns>
    public static long Count<T>(this PrimitiveDataFrameColumn<T> column)
        where T : unmanaged, INumber<T>
    {
        if (column == null)
        {
            return 0;
        }

        return column.Count(v => v.HasValue);
    }

    /// <summary>
    /// Calculates descriptive statistics for a column
    /// </summary>
    /// <typeparam name="T">Numeric type</typeparam>
    /// <param name="column">Column to calculate statistics for</param>
    /// <returns>Tuple containing (count, mean, stddev, min, 25th percentile, median, 75th percentile, max)</returns>
    public static (long Count, T? Mean, double? StdDev, T? Min, T? Q25, T? Median, T? Q75, T? Max) Describe<T>(this PrimitiveDataFrameColumn<T> column)
        where T : unmanaged, INumber<T>
    {
        var count = column.Count();
        var mean = column.Mean();
        var stdDev = column.StdDev();
        var min = column.Min();
        var q25 = column.Quantile(0.25);
        var median = column.Median();
        var q75 = column.Quantile(0.75);
        var max = column.Max();

        return (count, mean, stdDev, min, q25, median, q75, max);
    }

    /// <summary>
    /// Calculates a quantile (percentile) of a column
    /// </summary>
    /// <typeparam name="T">Numeric type</typeparam>
    /// <param name="column">Column to calculate quantile for</param>
    /// <param name="quantile">Quantile to calculate (0.0 to 1.0, e.g., 0.25 for 25th percentile)</param>
    /// <returns>Quantile value, or null if column is empty</returns>
    public static T? Quantile<T>(this PrimitiveDataFrameColumn<T> column, double quantile)
        where T : unmanaged, INumber<T>
    {
        if (column == null || column.Length == 0 || quantile < 0 || quantile > 1)
        {
            return null;
        }

        var values = column.Where(v => v.HasValue).Select(v => v!.Value).OrderBy(v => v).ToList();

        if (values.Count == 0)
        {
            return null;
        }

        var index = quantile * (values.Count - 1);
        var lowerIndex = (int)Math.Floor(index);
        var upperIndex = (int)Math.Ceiling(index);

        if (lowerIndex == upperIndex)
        {
            return values[lowerIndex];
        }

        var weight = T.CreateChecked(index - lowerIndex);
        return values[lowerIndex] + weight * (values[upperIndex] - values[lowerIndex]);
    }
}
