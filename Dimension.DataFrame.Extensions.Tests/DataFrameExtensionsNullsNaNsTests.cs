using FluentAssertions;
using Microsoft.Data.Analysis;
using Xunit;

namespace Dimension.DataFrame.Extensions.Tests;

public class DataFrameExtensionsNullsNaNsTests
{
    [Fact]
    public void DropNulls_Column_RemovesNullValues()
    {
        // Arrange
        var column = new PrimitiveDataFrameColumn<int>("A", new int?[] { 1, null, 3, null, 5 });

        // Act
        var result = column.DropNulls();

        // Assert
        result.Length.Should().Be(3);
        result[0].Should().Be(1);
        result[1].Should().Be(3);
        result[2].Should().Be(5);
    }

    [Fact]
    public void DropNulls_ColumnWithNoNulls_ReturnsAllValues()
    {
        // Arrange
        var column = new PrimitiveDataFrameColumn<int>("A", new[] { 1, 2, 3, 4, 5 });

        // Act
        var result = column.DropNulls();

        // Assert
        result.Length.Should().Be(5);
    }

    [Fact]
    public void DropNulls_DataFrame_RemovesRowsWithNulls()
    {
        // Arrange
        var df = new Microsoft.Data.Analysis.DataFrame(
            new PrimitiveDataFrameColumn<int>("A", new int?[] { 1, null, 3, 4 }),
            new PrimitiveDataFrameColumn<int>("B", new int?[] { 10, 20, null, 40 })
        );

        // Act
        var result = df.DropNulls();

        // Assert
        result.Rows.Count.Should().Be(2); // Only rows 0 and 3 have no nulls
        ((int?)result["A"][0]).Should().Be(1);
        ((int?)result["B"][0]).Should().Be(10);
        ((int?)result["A"][1]).Should().Be(4);
        ((int?)result["B"][1]).Should().Be(40);
    }

    [Fact]
    public void DropNAs_DataFrame_RemovesRowsWithNaNs()
    {
        // Arrange
        var df = new Microsoft.Data.Analysis.DataFrame(
            new PrimitiveDataFrameColumn<double>("A", new[] { 1.0, double.NaN, 3.0 }),
            new PrimitiveDataFrameColumn<double>("B", new[] { 10.0, 20.0, double.NaN })
        );

        // Act
        var result = df.DropNAs();

        // Assert
        result.Rows.Count.Should().Be(1); // Only row 0 has no NaNs
        ((double?)result["A"][0]).Should().Be(1.0);
        ((double?)result["B"][0]).Should().Be(10.0);
    }

    [Fact]
    public void DropNullsOrNAs_DataFrame_RemovesRowsWithNullsOrNaNs()
    {
        // Arrange
        var df = new Microsoft.Data.Analysis.DataFrame(
            new PrimitiveDataFrameColumn<double>("A", new double?[] { 1.0, null, 3.0, 4.0 }),
            new PrimitiveDataFrameColumn<double>("B", new[] { 10.0, 20.0, double.NaN, 40.0 })
        );

        // Act
        var result = df.DropNullsOrNAs();

        // Assert
        result.Rows.Count.Should().Be(2); // Rows 0 and 3 have neither nulls nor NaNs
    }

    [Fact]
    public void HasNulls_DataFrameRow_ReturnsTrueWhenRowHasNulls()
    {
        // Arrange
        var df = new Microsoft.Data.Analysis.DataFrame(
            new PrimitiveDataFrameColumn<int>("A", new int?[] { 1, null }),
            new PrimitiveDataFrameColumn<int>("B", new[] { 10, 20 })
        );

        // Act
        var row1HasNulls = df.Rows[1].HasNulls();

        // Assert
        row1HasNulls.Should().BeTrue();
    }

    [Fact]
    public void HasNulls_DataFrameRow_ReturnsFalseWhenRowHasNoNulls()
    {
        // Arrange
        var df = new Microsoft.Data.Analysis.DataFrame(
            new PrimitiveDataFrameColumn<int>("A", new[] { 1, 2 }),
            new PrimitiveDataFrameColumn<int>("B", new[] { 10, 20 })
        );

        // Act
        var row0HasNulls = df.Rows[0].HasNulls();

        // Assert
        row0HasNulls.Should().BeFalse();
    }

    [Fact]
    public void HasNulls_DataFrameColumn_ReturnsTrueWhenColumnHasNulls()
    {
        // Arrange
        var column = new PrimitiveDataFrameColumn<int>("A", new int?[] { 1, null, 3 });

        // Act
        var result = column.HasNulls();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void HasNulls_DataFrameColumn_ReturnsFalseWhenColumnHasNoNulls()
    {
        // Arrange
        var column = new PrimitiveDataFrameColumn<int>("A", new[] { 1, 2, 3 });

        // Act
        var result = column.HasNulls();

        // Assert
        result.Should().BeFalse();
    }
}
