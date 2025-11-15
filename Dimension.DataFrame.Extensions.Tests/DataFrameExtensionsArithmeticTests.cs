using FluentAssertions;
using Microsoft.Data.Analysis;
using Xunit;

namespace Dimension.DataFrame.Extensions.Tests;

public class DataFrameExtensionsArithmeticTests
{
    [Fact]
    public void Plus_TwoColumns_ReturnsCorrectSum()
    {
        // Arrange
        var column1 = new PrimitiveDataFrameColumn<int>("A", new[] { 1, 2, 3, 4, 5 });
        var column2 = new PrimitiveDataFrameColumn<int>("B", new[] { 10, 20, 30, 40, 50 });

        // Act
        var result = column1.Plus(column2);

        // Assert
        result.Length.Should().Be(5);
        result[0].Should().Be(11);
        result[1].Should().Be(22);
        result[2].Should().Be(33);
        result[3].Should().Be(44);
        result[4].Should().Be(55);
    }

    [Fact]
    public void Plus_MultipleColumns_ReturnsCorrectSum()
    {
        // Arrange
        var column1 = new PrimitiveDataFrameColumn<int>("A", new[] { 1, 2, 3 });
        var column2 = new PrimitiveDataFrameColumn<int>("B", new[] { 10, 20, 30 });
        var column3 = new PrimitiveDataFrameColumn<int>("C", new[] { 100, 200, 300 });

        // Act
        var result = column1.Plus("", column2, column3);

        // Assert
        result.Length.Should().Be(3);
        result[0].Should().Be(111);
        result[1].Should().Be(222);
        result[2].Should().Be(333);
    }

    [Fact]
    public void Plus_WithNulls_TreatsNullsAsDefault()
    {
        // Arrange
        var column1 = new PrimitiveDataFrameColumn<int>("A", new int?[] { 1, null, 3 });
        var column2 = new PrimitiveDataFrameColumn<int>("B", new int?[] { 10, 20, null });

        // Act
        var result = column1.Plus(column2);

        // Assert
        result[0].Should().Be(11);
        result[1].Should().Be(20);  // null + 20 = 0 + 20 = 20
        result[2].Should().Be(3);   // 3 + null = 3 + 0 = 3
    }

    [Fact]
    public void Plus_DifferentLengths_ThrowsArgumentException()
    {
        // Arrange
        var column1 = new PrimitiveDataFrameColumn<int>("A", new[] { 1, 2, 3 });
        var column2 = new PrimitiveDataFrameColumn<int>("B", new[] { 10, 20 });

        // Act & Assert
        var act = () => column1.Plus(column2);
        act.Should().Throw<ArgumentException>()
            .WithMessage("All columns must have the same length.");
    }

    [Fact]
    public void Minus_TwoColumns_ReturnsCorrectDifference()
    {
        // Arrange
        var column1 = new PrimitiveDataFrameColumn<int>("A", new[] { 50, 40, 30 });
        var column2 = new PrimitiveDataFrameColumn<int>("B", new[] { 10, 20, 15 });

        // Act
        var result = column1.Minus(column2);

        // Assert
        result[0].Should().Be(40);
        result[1].Should().Be(20);
        result[2].Should().Be(15);
    }

    [Fact]
    public void Times_TwoColumns_ReturnsCorrectProduct()
    {
        // Arrange
        var column1 = new PrimitiveDataFrameColumn<int>("A", new[] { 2, 3, 4 });
        var column2 = new PrimitiveDataFrameColumn<int>("B", new[] { 10, 20, 30 });

        // Act
        var result = column1.Times(column2);

        // Assert
        result[0].Should().Be(20);
        result[1].Should().Be(60);
        result[2].Should().Be(120);
    }

    [Fact]
    public void Times_MultipleColumns_ReturnsCorrectProduct()
    {
        // Arrange
        var column1 = new PrimitiveDataFrameColumn<int>("A", new[] { 2, 3, 4 });
        var column2 = new PrimitiveDataFrameColumn<int>("B", new[] { 10, 10, 10 });
        var column3 = new PrimitiveDataFrameColumn<int>("C", new[] { 5, 5, 5 });

        // Act
        var result = column1.Times("", column2, column3);

        // Assert
        result[0].Should().Be(100);  // 2 * 10 * 5
        result[1].Should().Be(150);  // 3 * 10 * 5
        result[2].Should().Be(200);  // 4 * 10 * 5
    }

    [Fact]
    public void Divide_ValidDivision_ReturnsCorrectQuotient()
    {
        // Arrange
        var numerator = new PrimitiveDataFrameColumn<double>("A", new[] { 100.0, 50.0, 25.0 });
        var divisor = new PrimitiveDataFrameColumn<double>("B", new[] { 10.0, 5.0, 5.0 });

        // Act
        var result = numerator.Divide(divisor, "Result");

        // Assert
        result[0].Should().Be(10.0);
        result[1].Should().Be(10.0);
        result[2].Should().Be(5.0);
    }

    [Fact]
    public void Divide_ByZero_ReturnsNaN()
    {
        // Arrange
        var numerator = new PrimitiveDataFrameColumn<double>("A", new[] { 100.0, 50.0 });
        var divisor = new PrimitiveDataFrameColumn<double>("B", new[] { 0.0, 5.0 });

        // Act
        var result = numerator.Divide(divisor, "Result");

        // Assert
        double.IsNaN(result[0].GetValueOrDefault()).Should().BeTrue();
        result[1].Should().Be(10.0);
    }

    [Fact]
    public void Divide_DifferentLengths_ThrowsArgumentException()
    {
        // Arrange
        var numerator = new PrimitiveDataFrameColumn<double>("A", new[] { 100.0, 50.0, 25.0 });
        var divisor = new PrimitiveDataFrameColumn<double>("B", new[] { 10.0, 5.0 });

        // Act & Assert
        var act = () => numerator.Divide(divisor, "Result");
        act.Should().Throw<ArgumentException>()
            .WithMessage("Both columns must have the same length.");
    }
}
