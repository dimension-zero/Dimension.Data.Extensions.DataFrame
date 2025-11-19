using FluentAssertions;
using Microsoft.Data.Analysis;
using Xunit;

namespace Dimension.DataFrame.Extensions.Tests;

public class DataFrameExtensionsMathTests
{
    [Fact]
    public void Abs_PositiveAndNegativeValues_ReturnsAbsoluteValues()
    {
        // Arrange
        var column = new PrimitiveDataFrameColumn<int>("Data", new[] { -5, -2, 0, 3, -8 });

        // Act
        var result = column.Abs();

        // Assert
        result[0].Should().Be(5);
        result[1].Should().Be(2);
        result[2].Should().Be(0);
        result[3].Should().Be(3);
        result[4].Should().Be(8);
    }

    [Fact]
    public void Log_PositiveValues_ReturnsNaturalLog()
    {
        // Arrange
        var column = new PrimitiveDataFrameColumn<double>("Data", new[] { 1.0, Math.E, Math.E * Math.E });

        // Act
        var result = column.Log();

        // Assert
        result[0].Should().BeApproximately(0.0, 0.0001);
        result[1].Should().BeApproximately(1.0, 0.0001);
        result[2].Should().BeApproximately(2.0, 0.0001);
    }

    [Fact]
    public void Log_NegativeValue_ReturnsNaN()
    {
        // Arrange
        var column = new PrimitiveDataFrameColumn<double>("Data", new[] { -1.0 });

        // Act
        var result = column.Log();

        // Assert
        double.IsNaN(result[0]!.Value).Should().BeTrue();
    }

    [Fact]
    public void Log_WithBase_ReturnsCorrectLogarithm()
    {
        // Arrange
        var column = new PrimitiveDataFrameColumn<double>("Data", new[] { 100.0, 1000.0 });

        // Act
        var result = column.Log(10);

        // Assert
        result[0].Should().BeApproximately(2.0, 0.0001);
        result[1].Should().BeApproximately(3.0, 0.0001);
    }

    [Fact]
    public void Log10_ValidValues_ReturnsBase10Log()
    {
        // Arrange
        var column = new PrimitiveDataFrameColumn<double>("Data", new[] { 10.0, 100.0, 1000.0 });

        // Act
        var result = column.Log10();

        // Assert
        result[0].Should().BeApproximately(1.0, 0.0001);
        result[1].Should().BeApproximately(2.0, 0.0001);
        result[2].Should().BeApproximately(3.0, 0.0001);
    }

    [Fact]
    public void Exp_ValidValues_ReturnsExponential()
    {
        // Arrange
        var column = new PrimitiveDataFrameColumn<double>("Data", new[] { 0.0, 1.0, 2.0 });

        // Act
        var result = column.Exp();

        // Assert
        result[0].Should().BeApproximately(1.0, 0.0001);
        result[1].Should().BeApproximately(Math.E, 0.0001);
        result[2].Should().BeApproximately(Math.E * Math.E, 0.0001);
    }

    [Fact]
    public void Sqrt_PositiveValues_ReturnsSquareRoot()
    {
        // Arrange
        var column = new PrimitiveDataFrameColumn<double>("Data", new[] { 0.0, 1.0, 4.0, 9.0, 16.0 });

        // Act
        var result = column.Sqrt();

        // Assert
        result[0].Should().Be(0.0);
        result[1].Should().Be(1.0);
        result[2].Should().Be(2.0);
        result[3].Should().Be(3.0);
        result[4].Should().Be(4.0);
    }

    [Fact]
    public void Sqrt_NegativeValue_ReturnsNaN()
    {
        // Arrange
        var column = new PrimitiveDataFrameColumn<double>("Data", new[] { -1.0 });

        // Act
        var result = column.Sqrt();

        // Assert
        double.IsNaN(result[0]!.Value).Should().BeTrue();
    }

    [Fact]
    public void Sin_ValidValues_ReturnsSine()
    {
        // Arrange
        var column = new PrimitiveDataFrameColumn<double>("Data", new[] { 0.0, Math.PI / 2, Math.PI });

        // Act
        var result = column.Sin();

        // Assert
        result[0].Should().BeApproximately(0.0, 0.0001);
        result[1].Should().BeApproximately(1.0, 0.0001);
        result[2].Should().BeApproximately(0.0, 0.0001);
    }

    [Fact]
    public void Cos_ValidValues_ReturnsCosine()
    {
        // Arrange
        var column = new PrimitiveDataFrameColumn<double>("Data", new[] { 0.0, Math.PI / 2, Math.PI });

        // Act
        var result = column.Cos();

        // Assert
        result[0].Should().BeApproximately(1.0, 0.0001);
        result[1].Should().BeApproximately(0.0, 0.0001);
        result[2].Should().BeApproximately(-1.0, 0.0001);
    }

    [Fact]
    public void Round_DefaultDecimals_RoundsToInteger()
    {
        // Arrange
        var column = new PrimitiveDataFrameColumn<double>("Data", new[] { 1.4, 1.5, 2.5, 3.6 });

        // Act
        var result = column.Round();

        // Assert
        result[0].Should().Be(1.0);
        result[1].Should().Be(2.0);
        result[2].Should().Be(2.0); // Banker's rounding
        result[3].Should().Be(4.0);
    }

    [Fact]
    public void Round_TwoDecimals_RoundsCorrectly()
    {
        // Arrange
        var column = new PrimitiveDataFrameColumn<double>("Data", new[] { 1.234, 1.235, 1.236 });

        // Act
        var result = column.Round(2);

        // Assert
        result[0].Should().BeApproximately(1.23, 0.001);
        result[1].Should().BeApproximately(1.24, 0.001);
        result[2].Should().BeApproximately(1.24, 0.001);
    }

    [Fact]
    public void Abs_WithNulls_PreservesNulls()
    {
        // Arrange
        var column = new PrimitiveDataFrameColumn<int>("Data", new int?[] { -5, null, 3 });

        // Act
        var result = column.Abs();

        // Assert
        result[0].Should().Be(5);
        result[1].Should().BeNull();
        result[2].Should().Be(3);
    }

    [Fact]
    public void Abs_CustomName_UsesCustomName()
    {
        // Arrange
        var column = new PrimitiveDataFrameColumn<int>("Data", new[] { -5, 3 });

        // Act
        var result = column.Abs("AbsoluteValues");

        // Assert
        result.Name.Should().Be("AbsoluteValues");
    }
}
