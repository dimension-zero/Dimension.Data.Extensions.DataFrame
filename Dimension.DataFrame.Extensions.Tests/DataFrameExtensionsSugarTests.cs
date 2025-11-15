using FluentAssertions;
using Microsoft.Data.Analysis;
using Xunit;

namespace Dimension.DataFrame.Extensions.Tests;

public class DataFrameExtensionsSugarTests
{
    [Fact]
    public void WithName_ValidColumn_RenamesColumn()
    {
        // Arrange
        var column = new PrimitiveDataFrameColumn<int>("OldName", new[] { 1, 2, 3 });

        // Act
        var result = column.WithName<int>("NewName");

        // Assert
        result.Name.Should().Be("NewName");
        result.Should().BeSameAs(column); // Should be same instance
    }

    [Fact]
    public void WithName_NullColumn_ThrowsArgumentNullException()
    {
        // Arrange
        DataFrameColumn? column = null;

        // Act & Assert
        var act = () => column.WithName<int>("NewName");
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("*Column cannot be null*");
    }

    [Fact]
    public void WithName_WrongType_ThrowsInvalidOperationException()
    {
        // Arrange
        DataFrameColumn column = new PrimitiveDataFrameColumn<int>("A", new[] { 1, 2, 3 });

        // Act & Assert - trying to cast int column as double
        var act = () => column.WithName<double>("NewName");
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*not of type Double*");
    }

    [Fact]
    public void AddTo_NewColumn_AddsColumnToDataFrame()
    {
        // Arrange
        var df = new Microsoft.Data.Analysis.DataFrame();
        var column = new PrimitiveDataFrameColumn<int>("A", new[] { 1, 2, 3 });

        // Act
        var result = column.AddTo(df);

        // Assert
        df.Columns.Count.Should().Be(1);
        df.Columns[0].Name.Should().Be("A");
        result.Should().BeSameAs(column);
    }

    [Fact]
    public void AddTo_WithCustomName_RenamesAndAddsColumn()
    {
        // Arrange
        var df = new Microsoft.Data.Analysis.DataFrame();
        var column = new PrimitiveDataFrameColumn<int>("OldName", new[] { 1, 2, 3 });

        // Act
        var result = column.AddTo(df, "NewName");

        // Assert
        df.Columns[0].Name.Should().Be("NewName");
        column.Name.Should().Be("NewName"); // Original column is renamed
    }

    [Fact]
    public void AddTo_ExistingColumn_ThrowsByDefault()
    {
        // Arrange
        var df = new Microsoft.Data.Analysis.DataFrame(
            new PrimitiveDataFrameColumn<int>("A", new[] { 1, 2, 3 })
        );
        var newColumn = new PrimitiveDataFrameColumn<int>("A", new[] { 10, 20, 30 });

        // Act & Assert
        var act = () => newColumn.AddTo(df);
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*column with the name 'A' already exists*");
    }

    [Fact]
    public void AddTo_ExistingColumn_KeepOriginal_DoesNotReplace()
    {
        // Arrange
        var df = new Microsoft.Data.Analysis.DataFrame(
            new PrimitiveDataFrameColumn<int>("A", new[] { 1, 2, 3 })
        );
        var newColumn = new PrimitiveDataFrameColumn<int>("A", new[] { 10, 20, 30 });

        // Act
        newColumn.AddTo(df, clashBehaviour: ClashBehaviour.KeepOriginal);

        // Assert
        df.Columns.Count.Should().Be(1);
        ((int?)df["A"][0]).Should().Be(1); // Original value
    }

    [Fact]
    public void AddTo_ExistingColumn_ReplaceOriginal_ReplacesColumn()
    {
        // Arrange
        var df = new Microsoft.Data.Analysis.DataFrame(
            new PrimitiveDataFrameColumn<int>("A", new[] { 1, 2, 3 })
        );
        var newColumn = new PrimitiveDataFrameColumn<int>("A", new[] { 10, 20, 30 });

        // Act
        newColumn.AddTo(df, clashBehaviour: ClashBehaviour.ReplaceOriginal);

        // Assert
        df.Columns.Count.Should().Be(1);
        ((int?)df["A"][0]).Should().Be(10); // New value
    }

    [Fact]
    public void AddTo_MethodChaining_WorksCorrectly()
    {
        // Arrange
        var df = new Microsoft.Data.Analysis.DataFrame();
        var column1 = new PrimitiveDataFrameColumn<int>("A", new[] { 1, 2, 3 });
        var column2 = new PrimitiveDataFrameColumn<int>("B", new[] { 10, 20, 30 });

        // Act
        column1.Plus(column2)
            .WithName<int>("Sum")
            .AddTo(df);

        // Assert
        df.Columns.Count.Should().Be(1);
        df.Columns[0].Name.Should().Be("Sum");
        ((int?)df["Sum"][0]).Should().Be(11);
        ((int?)df["Sum"][1]).Should().Be(22);
        ((int?)df["Sum"][2]).Should().Be(33);
    }
}
