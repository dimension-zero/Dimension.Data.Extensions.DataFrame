using FluentAssertions;
using Microsoft.Data.Analysis;
using Xunit;

namespace Dimension.DataFrame.Extensions.Tests;

public class DataFrameExtensionsColumnsTests
{
    [Fact]
    public void SelectColumns_ValidNames_ReturnsSelectedColumns()
    {
        // Arrange
        var df = new Microsoft.Data.Analysis.DataFrame(
            new PrimitiveDataFrameColumn<int>("A", new[] { 1, 2, 3 }),
            new PrimitiveDataFrameColumn<int>("B", new[] { 10, 20, 30 }),
            new PrimitiveDataFrameColumn<int>("C", new[] { 100, 200, 300 })
        );

        // Act
        var result = df.SelectColumns("A", "C");

        // Assert
        result.Columns.Count.Should().Be(2);
        result.Columns[0].Name.Should().Be("A");
        result.Columns[1].Name.Should().Be("C");
        result.Rows.Count.Should().Be(3);
    }

    [Fact]
    public void SelectColumns_SingleColumn_ReturnsDataFrameWithOneColumn()
    {
        // Arrange
        var df = new Microsoft.Data.Analysis.DataFrame(
            new PrimitiveDataFrameColumn<int>("A", new[] { 1, 2, 3 }),
            new PrimitiveDataFrameColumn<int>("B", new[] { 10, 20, 30 })
        );

        // Act
        var result = df.SelectColumns("B");

        // Assert
        result.Columns.Count.Should().Be(1);
        result.Columns[0].Name.Should().Be("B");
    }

    [Fact]
    public void SelectColumns_NonExistentColumn_ThrowsArgumentException()
    {
        // Arrange
        var df = new Microsoft.Data.Analysis.DataFrame(
            new PrimitiveDataFrameColumn<int>("A", new[] { 1, 2, 3 })
        );

        // Act & Assert
        var act = () => df.SelectColumns("A", "NonExistent");
        act.Should().Throw<ArgumentException>()
            .WithMessage("One or more column names do not exist in the DataFrame.");
    }

    [Fact]
    public void ColumnExists_ExistingColumn_ReturnsTrue()
    {
        // Arrange
        var df = new Microsoft.Data.Analysis.DataFrame(
            new PrimitiveDataFrameColumn<int>("A", new[] { 1, 2, 3 }),
            new PrimitiveDataFrameColumn<int>("B", new[] { 10, 20, 30 })
        );

        // Act
        var result = df.ColumnExists("A");

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void ColumnExists_NonExistingColumn_ReturnsFalse()
    {
        // Arrange
        var df = new Microsoft.Data.Analysis.DataFrame(
            new PrimitiveDataFrameColumn<int>("A", new[] { 1, 2, 3 })
        );

        // Act
        var result = df.ColumnExists("NonExistent");

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void TryGetColumn_ExistingColumn_ReturnsTrueAndColumn()
    {
        // Arrange
        var df = new Microsoft.Data.Analysis.DataFrame(
            new PrimitiveDataFrameColumn<int>("A", new[] { 1, 2, 3 }),
            new PrimitiveDataFrameColumn<double>("B", new[] { 1.5, 2.5, 3.5 })
        );

        // Act
        var success = df.TryGetColumn<int>("A", out var column);

        // Assert
        success.Should().BeTrue();
        column.Should().NotBeNull();
        column!.Name.Should().Be("A");
        column.Length.Should().Be(3);
    }

    [Fact]
    public void TryGetColumn_NonExistingColumn_ReturnsFalseAndNull()
    {
        // Arrange
        var df = new Microsoft.Data.Analysis.DataFrame(
            new PrimitiveDataFrameColumn<int>("A", new[] { 1, 2, 3 })
        );

        // Act
        var success = df.TryGetColumn<int>("NonExistent", out var column);

        // Assert
        success.Should().BeFalse();
        column.Should().BeNull();
    }

    [Fact]
    public void TryGetColumn_WrongType_ReturnsFalseAndNull()
    {
        // Arrange
        var df = new Microsoft.Data.Analysis.DataFrame(
            new PrimitiveDataFrameColumn<int>("A", new[] { 1, 2, 3 })
        );

        // Act - trying to get as double when it's int
        var success = df.TryGetColumn<double>("A", out var column);

        // Assert
        success.Should().BeFalse();
        column.Should().BeNull();
    }
}
