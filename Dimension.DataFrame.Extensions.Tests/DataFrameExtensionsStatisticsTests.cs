using FluentAssertions;
using Microsoft.Data.Analysis;
using Xunit;

namespace Dimension.DataFrame.Extensions.Tests;

public class DataFrameExtensionsStatisticsTests
{
    [Fact]
    public void Mean_ValidColumn_ReturnsCorrectMean()
    {
        // Arrange
        var column = new PrimitiveDataFrameColumn<int>("Data", new[] { 1, 2, 3, 4, 5 });

        // Act
        var result = column.Mean();

        // Assert
        result.Should().Be(3); // (1+2+3+4+5)/5 = 3
    }

    [Fact]
    public void Mean_WithNulls_IgnoresNulls()
    {
        // Arrange
        var column = new PrimitiveDataFrameColumn<int>("Data", new int?[] { 1, null, 3, null, 5 });

        // Act
        var result = column.Mean();

        // Assert
        result.Should().Be(3); // (1+3+5)/3 = 3
    }

    [Fact]
    public void Median_OddCount_ReturnsMiddleValue()
    {
        // Arrange
        var column = new PrimitiveDataFrameColumn<int>("Data", new[] { 1, 3, 2, 5, 4 });

        // Act
        var result = column.Median();

        // Assert
        result.Should().Be(3); // Sorted: [1,2,3,4,5], median is 3
    }

    [Fact]
    public void Median_EvenCount_ReturnsAverageOfMiddleTwo()
    {
        // Arrange
        var column = new PrimitiveDataFrameColumn<int>("Data", new[] { 1, 2, 3, 4 });

        // Act
        var result = column.Median();

        // Assert
        result.Should().Be(2.5); // Average of 2 and 3 = 2.5 (now returns double for precision)
    }

    [Fact]
    public void StdDev_ValidColumn_ReturnsCorrectStdDev()
    {
        // Arrange
        var column = new PrimitiveDataFrameColumn<double>("Data", new[] { 2.0, 4.0, 4.0, 4.0, 5.0, 5.0, 7.0, 9.0 });

        // Act
        var result = column.StdDev(sample: true);

        // Assert
        result.Should().NotBeNull();
        result!.Value.Should().BeApproximately(2.138, 0.001); // Sample std dev
    }

    [Fact]
    public void Variance_ValidColumn_ReturnsCorrectVariance()
    {
        // Arrange
        var column = new PrimitiveDataFrameColumn<double>("Data", new[] { 1.0, 2.0, 3.0, 4.0, 5.0 });

        // Act
        var result = column.Variance(sample: true);

        // Assert
        result.Should().NotBeNull();
        result!.Value.Should().BeApproximately(2.5, 0.001); // Sample variance
    }

    [Fact]
    public void Min_ValidColumn_ReturnsMinimum()
    {
        // Arrange
        var column = new PrimitiveDataFrameColumn<int>("Data", new[] { 5, 2, 8, 1, 9 });

        // Act
        var result = column.Min();

        // Assert
        result.Should().Be(1);
    }

    [Fact]
    public void Max_ValidColumn_ReturnsMaximum()
    {
        // Arrange
        var column = new PrimitiveDataFrameColumn<int>("Data", new[] { 5, 2, 8, 1, 9 });

        // Act
        var result = column.Max();

        // Assert
        result.Should().Be(9);
    }

    [Fact]
    public void Sum_ValidColumn_ReturnsSum()
    {
        // Arrange
        var column = new PrimitiveDataFrameColumn<int>("Data", new[] { 1, 2, 3, 4, 5 });

        // Act
        var result = column.Sum();

        // Assert
        result.Should().Be(15);
    }

    [Fact]
    public void Count_ValidColumn_ReturnsNonNullCount()
    {
        // Arrange
        var column = new PrimitiveDataFrameColumn<int>("Data", new int?[] { 1, null, 3, null, 5 });

        // Act
        var result = column.Count();

        // Assert
        result.Should().Be(3);
    }

    [Fact]
    public void Quantile_25thPercentile_ReturnsCorrectValue()
    {
        // Arrange
        var column = new PrimitiveDataFrameColumn<double>("Data", new[] { 1.0, 2.0, 3.0, 4.0, 5.0 });

        // Act
        var result = column.Quantile(0.25);

        // Assert
        result.Should().NotBeNull();
        result!.Value.Should().BeApproximately(2.0, 0.1);
    }

    [Fact]
    public void Quantile_75thPercentile_ReturnsCorrectValue()
    {
        // Arrange
        var column = new PrimitiveDataFrameColumn<double>("Data", new[] { 1.0, 2.0, 3.0, 4.0, 5.0 });

        // Act
        var result = column.Quantile(0.75);

        // Assert
        result.Should().NotBeNull();
        result!.Value.Should().BeApproximately(4.0, 0.1);
    }

    [Fact]
    public void Describe_ValidColumn_ReturnsAllStatistics()
    {
        // Arrange
        var column = new PrimitiveDataFrameColumn<double>("Data", new[] { 1.0, 2.0, 3.0, 4.0, 5.0 });

        // Act
        var result = column.Describe();

        // Assert
        result.Count.Should().Be(5);
        result.Mean.Should().Be(3.0);
        result.Min.Should().Be(1.0);
        result.Max.Should().Be(5.0);
        result.Median.Should().Be(3.0);
    }

    [Fact]
    public void Mean_EmptyColumn_ReturnsNull()
    {
        // Arrange
        var column = new PrimitiveDataFrameColumn<int>("Data", Array.Empty<int>());

        // Act
        var result = column.Mean();

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void StdDev_LessThanTwoValues_ReturnsNull()
    {
        // Arrange
        var column = new PrimitiveDataFrameColumn<double>("Data", new[] { 1.0 });

        // Act
        var result = column.StdDev(sample: true);

        // Assert
        result.Should().BeNull();
    }
}
