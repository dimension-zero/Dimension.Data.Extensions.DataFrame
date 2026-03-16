using FluentAssertions;
using Microsoft.Data.Analysis;
using Xunit;

namespace Dimension.DataFrame.Extensions.Tests;

public class DataFrameExtensionsCalculationsTests
{
    [Fact]
    public void Diff_ValidColumn_ReturnsCorrectDifferences()
    {
        // Arrange
        var column = new PrimitiveDataFrameColumn<int>("A", new[] { 10, 15, 12, 20, 18 });

        // Act
        var result = column.Diff<int>();

        // Assert
        result.Should().NotBeNull();
        result!.Length.Should().Be(5);
        result.Name.Should().Be("A_Diff");
        result[0].Should().BeNull(); // First element is seed (default null)
        result[1].Should().Be(5);    // 15 - 10
        result[2].Should().Be(-3);   // 12 - 15
        result[3].Should().Be(8);    // 20 - 12
        result[4].Should().Be(-2);   // 18 - 20
    }

    [Fact]
    public void Diff_WithCustomName_UsesCustomName()
    {
        // Arrange
        var column = new PrimitiveDataFrameColumn<int>("A", new[] { 1, 2, 3 });

        // Act
        var result = column.Diff<int>("CustomDiff");

        // Assert
        result!.Name.Should().Be("CustomDiff");
    }

    [Fact]
    public void Diff_WithSeed_UsesProvidedSeed()
    {
        // Arrange
        var column = new PrimitiveDataFrameColumn<int>("A", new[] { 10, 15, 12 });

        // Act
        var result = column.Diff<int>("", 100);

        // Assert
        result!.Length.Should().Be(3);
        result[0].Should().Be(100); // Seed value
    }

    [Fact]
    public void Diff_NullColumn_ReturnsNull()
    {
        // Arrange
        DataFrameColumn? column = null;

        // Act
        var result = column.Diff<int>();

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void Apply_WithOperation_TransformsAllValues()
    {
        // Arrange
        var column = new PrimitiveDataFrameColumn<int>("A", new[] { 1, 2, 3, 4, 5 });
        Func<int, int> doubleIt = x => x * 2;

        // Act
        var result = column.Apply(doubleIt);

        // Assert
        result.Length.Should().Be(5);
        result.Name.Should().Be("A_Applied");
        result[0].Should().Be(2);
        result[1].Should().Be(4);
        result[2].Should().Be(6);
        result[3].Should().Be(8);
        result[4].Should().Be(10);
    }

    [Fact]
    public void Apply_WithNulls_PreservesNulls()
    {
        // Arrange
        var column = new PrimitiveDataFrameColumn<int>("A", new int?[] { 1, null, 3, null, 5 });
        Func<int, int> doubleIt = x => x * 2;

        // Act
        var result = column.Apply(doubleIt);

        // Assert
        result[0].Should().Be(2);
        result[1].Should().BeNull();
        result[2].Should().Be(6);
        result[3].Should().BeNull();
        result[4].Should().Be(10);
    }

    [Fact]
    public void Apply_WithCustomName_UsesCustomName()
    {
        // Arrange
        var column = new PrimitiveDataFrameColumn<int>("A", new[] { 1, 2, 3 });
        Func<int, int> square = x => x * x;

        // Act
        var result = column.Apply(square, "Squared");

        // Assert
        result.Name.Should().Be("Squared");
    }

    [Fact]
    public void Pow_PositivePower_ReturnsCorrectValues()
    {
        // Arrange
        var column = new PrimitiveDataFrameColumn<double>("A", new[] { 2.0, 3.0, 4.0 });

        // Act
        var result = column.Pow(2);

        // Assert
        result[0].Should().Be(4.0);
        result[1].Should().Be(9.0);
        result[2].Should().Be(16.0);
        result.Name.Should().Be("A_Pow2");
    }

    [Fact]
    public void Pow_FractionalPower_ReturnsCorrectValues()
    {
        // Arrange
        var column = new PrimitiveDataFrameColumn<double>("A", new[] { 4.0, 9.0, 16.0 });

        // Act
        var result = column.Pow(0.5); // Square root

        // Assert
        result[0].Should().BeApproximately(2.0, 0.0001);
        result[1].Should().BeApproximately(3.0, 0.0001);
        result[2].Should().BeApproximately(4.0, 0.0001);
    }

    [Fact]
    public void Pow_WithNulls_HandlesNullsCorrectly()
    {
        // Arrange
        var column = new PrimitiveDataFrameColumn<double>("A", new double?[] { 2.0, null, 4.0 });

        // Act
        var result = column.Pow(2);

        // Assert
        result[0].Should().Be(4.0);
        result[1].Should().Be(default(double)); // null becomes default
        result[2].Should().Be(16.0);
    }

    [Fact]
    public void Pow_WithCustomName_UsesCustomName()
    {
        // Arrange
        var column = new PrimitiveDataFrameColumn<double>("A", new[] { 2.0, 3.0 });

        // Act
        var result = column.Pow(3, "Cubed");

        // Assert
        result.Name.Should().Be("Cubed");
    }

    [Fact]
    public void Pow_NegativePower_ReturnsCorrectValues()
    {
        // Arrange
        var column = new PrimitiveDataFrameColumn<double>("A", new[] { 2.0, 4.0, 5.0 });

        // Act
        var result = column.Pow(-1); // Reciprocal

        // Assert
        result[0].Should().BeApproximately(0.5, 0.0001);
        result[1].Should().BeApproximately(0.25, 0.0001);
        result[2].Should().BeApproximately(0.2, 0.0001);
    }
}
