using FluentAssertions;
using Microsoft.Data.Analysis;
using Xunit;

namespace Dimension.DataFrame.Extensions.Tests;

public class DataFrameExtensionsShiftsTests
{
    [Fact]
    public void Shift_ForwardPositive_ShiftsValuesDown()
    {
        // Arrange
        var column = new PrimitiveDataFrameColumn<int>("A", new[] { 1, 2, 3, 4, 5 });

        // Act
        var result = column.Shift(2);

        // Assert
        result.Length.Should().Be(5);
        result[0].Should().BeNull();  // Fill value
        result[1].Should().BeNull();  // Fill value
        result[2].Should().Be(1);     // Shifted from index 0
        result[3].Should().Be(2);     // Shifted from index 1
        result[4].Should().Be(3);     // Shifted from index 2
    }

    [Fact]
    public void Shift_BackwardNegative_ShiftsValuesUp()
    {
        // Arrange
        var column = new PrimitiveDataFrameColumn<int>("A", new[] { 1, 2, 3, 4, 5 });

        // Act
        var result = column.Shift(-2);

        // Assert
        result.Length.Should().Be(5);
        result[0].Should().Be(3);     // Shifted from index 2
        result[1].Should().Be(4);     // Shifted from index 3
        result[2].Should().Be(5);     // Shifted from index 4
        result[3].Should().BeNull();  // Fill value
        result[4].Should().BeNull();  // Fill value
    }

    [Fact]
    public void Shift_WithCustomFillValue_UsesFillValue()
    {
        // Arrange
        var column = new PrimitiveDataFrameColumn<int>("A", new[] { 1, 2, 3, 4, 5 });

        // Act
        var result = column.Shift(2, 999);

        // Assert
        result[0].Should().Be(999);
        result[1].Should().Be(999);
        result[2].Should().Be(1);
    }

    [Fact]
    public void Shift_WithCustomName_UsesCustomName()
    {
        // Arrange
        var column = new PrimitiveDataFrameColumn<int>("A", new[] { 1, 2, 3 });

        // Act
        var result = column.Shift(1, name: "Lagged");

        // Assert
        result.Name.Should().Be("Lagged");
    }

    [Fact]
    public void Shift_DefaultName_GeneratesCorrectName()
    {
        // Arrange
        var column = new PrimitiveDataFrameColumn<int>("Price", new[] { 1, 2, 3 });

        // Act
        var result = column.Shift(1);

        // Assert
        result.Name.Should().Be("Price_Shifted1");
    }

    [Fact]
    public void Shift_NullColumn_ThrowsArgumentNullException()
    {
        // Arrange
        PrimitiveDataFrameColumn<int>? column = null;

        // Act & Assert
        var act = () => column.Shift(1);
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("*Column cannot be null*");
    }

    [Fact]
    public void Shift_ZeroShift_ReturnsColumnWithSameValues()
    {
        // Arrange
        var column = new PrimitiveDataFrameColumn<int>("A", new[] { 1, 2, 3, 4, 5 });

        // Act
        var result = column.Shift(0);

        // Assert
        result[0].Should().Be(1);
        result[1].Should().Be(2);
        result[2].Should().Be(3);
        result[3].Should().Be(4);
        result[4].Should().Be(5);
    }

    [Fact]
    public void Shift_LargeShift_FillsAllWithFillValue()
    {
        // Arrange
        var column = new PrimitiveDataFrameColumn<int>("A", new[] { 1, 2, 3 });

        // Act
        var result = column.Shift(10, 0);

        // Assert
        result[0].Should().Be(0);
        result[1].Should().Be(0);
        result[2].Should().Be(0);
    }
}
