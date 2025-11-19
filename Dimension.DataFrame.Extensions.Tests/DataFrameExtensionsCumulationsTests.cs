using FluentAssertions;
using Microsoft.Data.Analysis;
using Xunit;

namespace Dimension.DataFrame.Extensions.Tests;

public class DataFrameExtensionsCumulationsTests
{
    [Fact]
    public void Cumulate_ValidColumn_ReturnsRunningSum()
    {
        // Arrange
        var column = new PrimitiveDataFrameColumn<int>("A", new[] { 1, 2, 3, 4, 5 });

        // Act
        var result = column.Cumulate();

        // Assert
        result.Length.Should().Be(5);
        result.Name.Should().Be("A_Cumulative");
        result[0].Should().Be(1);
        result[1].Should().Be(3);   // 1 + 2
        result[2].Should().Be(6);   // 1 + 2 + 3
        result[3].Should().Be(10);  // 1 + 2 + 3 + 4
        result[4].Should().Be(15);  // 1 + 2 + 3 + 4 + 5
    }

    [Fact]
    public void Cumulate_WithNulls_HandlesNullsCorrectly()
    {
        // Arrange
        var column = new PrimitiveDataFrameColumn<int>("A", new int?[] { 1, null, 3, 4 });

        // Act
        var result = column.Cumulate();

        // Assert
        result[0].Should().Be(1);
        result[1].Should().Be(default(int)); // null handling
        result[2].Should().Be(default(int)); // sum becomes invalid after null
        result[3].Should().Be(default(int));
    }

    [Fact]
    public void Cumulate_WithCustomName_UsesCustomName()
    {
        // Arrange
        var column = new PrimitiveDataFrameColumn<int>("A", new[] { 1, 2, 3 });

        // Act
        var result = column.Cumulate("CustomSum");

        // Assert
        result.Name.Should().Be("CustomSum");
    }

    [Fact]
    public void Cumulate_NullColumn_ThrowsArgumentNullException()
    {
        // Arrange
        PrimitiveDataFrameColumn<int>? column = null;

        // Act & Assert
        var act = () => column.Cumulate();
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("*Column cannot be null*");
    }

    [Fact]
    public void Cumulate_WithUseNaN_ReturnsNaNForNulls()
    {
        // Arrange
        var column = new PrimitiveDataFrameColumn<double>("A", new double?[] { 1.0, null, 3.0 });

        // Act
        var result = column.Cumulate("", true);

        // Assert
        result[0].Should().Be(1.0);
        double.IsNaN(result[1].GetValueOrDefault()).Should().BeTrue();
    }

    [Fact]
    public void CumulateAbs_ValidColumn_ReturnsAbsoluteRunningSum()
    {
        // Arrange
        var column = new PrimitiveDataFrameColumn<int>("A", new[] { -1, 2, -3, 4, -5 });

        // Act
        var result = column.CumulateAbs();

        // Assert
        result.Length.Should().Be(5);
        result.Name.Should().Be("A_CumulativeAbs");
        result[0].Should().Be(1);   // |-1|
        result[1].Should().Be(3);   // |-1| + |2|
        result[2].Should().Be(6);   // |-1| + |2| + |-3|
        result[3].Should().Be(10);  // |-1| + |2| + |-3| + |4|
        result[4].Should().Be(15);  // |-1| + |2| + |-3| + |4| + |-5|
    }

    [Fact]
    public void CumulateAbs_WithNulls_HandlesNullsCorrectly()
    {
        // Arrange
        var column = new PrimitiveDataFrameColumn<int>("A", new int?[] { -1, null, 3 });

        // Act
        var result = column.CumulateAbs();

        // Assert
        result[0].Should().Be(1);
        result[1].Should().Be(default(int));
        result[2].Should().Be(default(int));
    }

    [Fact]
    public void CumulateAbs_WithCustomName_UsesCustomName()
    {
        // Arrange
        var column = new PrimitiveDataFrameColumn<int>("A", new[] { -1, 2, -3 });

        // Act
        var result = column.CumulateAbs("AbsSum");

        // Assert
        result.Name.Should().Be("AbsSum");
    }

    [Fact]
    public void CumulateAbs_WithDoubles_WorksCorrectly()
    {
        // Arrange
        var column = new PrimitiveDataFrameColumn<double>("A", new[] { -1.5, 2.5, -3.5 });

        // Act
        var result = column.CumulateAbs();

        // Assert
        result[0].Should().Be(1.5);
        result[1].Should().Be(4.0);   // 1.5 + 2.5
        result[2].Should().Be(7.5);   // 1.5 + 2.5 + 3.5
    }
}
