using FluentAssertions;
using Microsoft.Data.Analysis;
using Xunit;

namespace Dimension.DataFrame.Extensions.Tests;

public class DataFrameExtensionsRollingTests
{
    [Fact]
    public void Rolling_WithSumOperation_ReturnsRollingSum()
    {
        // Arrange
        var column = new PrimitiveDataFrameColumn<int>("A", new[] { 1, 2, 3, 4, 5 });
        Func<IEnumerable<int?>, int> sum = values => values.Where(v => v.HasValue).Sum(v => v!.Value);

        // Act
        var result = column.Rolling(3, sum);

        // Assert
        result.Length.Should().Be(5);
        result[0].Should().BeNull();  // Not enough values
        result[1].Should().BeNull();  // Not enough values
        result[2].Should().Be(6);     // 1 + 2 + 3
        result[3].Should().Be(9);     // 2 + 3 + 4
        result[4].Should().Be(12);    // 3 + 4 + 5
    }

    [Fact]
    public void Rolling_WithAverageOperation_ReturnsRollingAverage()
    {
        // Arrange
        var column = new PrimitiveDataFrameColumn<double>("A", new[] { 1.0, 2.0, 3.0, 4.0, 5.0 });
        Func<IEnumerable<double?>, double> avg = values =>
            values.Where(v => v.HasValue).Average(v => v!.Value);

        // Act
        var result = column.Rolling(3, avg);

        // Assert
        result[2].Should().BeApproximately(2.0, 0.001);  // (1 + 2 + 3) / 3
        result[3].Should().BeApproximately(3.0, 0.001);  // (2 + 3 + 4) / 3
        result[4].Should().BeApproximately(4.0, 0.001);  // (3 + 4 + 5) / 3
    }

    [Fact]
    public void Rolling_WithNulls_SkipsNullsInCalculation()
    {
        // Arrange
        var column = new PrimitiveDataFrameColumn<int>("A", new int?[] { 1, null, 3, 4, 5 });
        Func<IEnumerable<int?>, int> sum = values => values.Where(v => v.HasValue).Sum(v => v!.Value);

        // Act
        var result = column.Rolling(3, sum);

        // Assert
        result[2].Should().Be(4);     // 1 + 3 (null skipped)
        result[3].Should().Be(7);     // 3 + 4 (null skipped)
        result[4].Should().Be(12);    // 3 + 4 + 5
    }

    [Fact]
    public void Rolling_WindowSizeOne_ReturnsOriginalValues()
    {
        // Arrange
        var column = new PrimitiveDataFrameColumn<int>("A", new[] { 1, 2, 3, 4, 5 });
        Func<IEnumerable<int?>, int> identity = values => values.First()!.Value;

        // Act
        var result = column.Rolling(1, identity);

        // Assert
        result[0].Should().Be(1);
        result[1].Should().Be(2);
        result[2].Should().Be(3);
        result[3].Should().Be(4);
        result[4].Should().Be(5);
    }

    [Fact]
    public void Rolling_ReturnsRollingWindow_CreatesRollingWindow()
    {
        // Arrange
        var column = new PrimitiveDataFrameColumn<int>("A", new[] { 1, 2, 3, 4, 5 });

        // Act
        var result = column.Rolling(3);

        // Assert
        result.Should().NotBeNull();
        result.SourceColumn.Should().BeSameAs(column);
        result.WindowSize.Should().Be(3);
    }

    [Fact]
    public void GetRange_ValidRange_ReturnsSubsetOfColumn()
    {
        // Arrange
        var column = new PrimitiveDataFrameColumn<int>("A", new[] { 10, 20, 30, 40, 50 });

        // Act
        var result = column.GetRange(1, 3);

        // Assert
        result.Length.Should().Be(3);
        result[0].Should().Be(20);
        result[1].Should().Be(30);
        result[2].Should().Be(40);
        result.Name.Should().Be("A_Range");
    }

    [Fact]
    public void GetRange_StartIndexOutOfRange_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var column = new PrimitiveDataFrameColumn<int>("A", new[] { 1, 2, 3 });

        // Act & Assert
        var act = () => column.GetRange(10, 1);
        act.Should().Throw<ArgumentOutOfRangeException>()
            .WithMessage("*Start index is out of range*");
    }

    [Fact]
    public void GetRange_CountOutOfRange_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var column = new PrimitiveDataFrameColumn<int>("A", new[] { 1, 2, 3 });

        // Act & Assert
        var act = () => column.GetRange(1, 10);
        act.Should().Throw<ArgumentOutOfRangeException>()
            .WithMessage("*Count is out of range*");
    }

    [Fact]
    public void GetRange_NegativeStartIndex_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var column = new PrimitiveDataFrameColumn<int>("A", new[] { 1, 2, 3 });

        // Act & Assert
        var act = () => column.GetRange(-1, 2);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }
}
