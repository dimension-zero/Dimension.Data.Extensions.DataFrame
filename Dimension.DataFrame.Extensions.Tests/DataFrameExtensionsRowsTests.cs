using FluentAssertions;
using Microsoft.Data.Analysis;
using System;
using Xunit;

namespace Dimension.DataFrame.Extensions.Tests;

public class DataFrameExtensionsRowsTests
{
    [Fact]
    public void AddRow_WithMatchingTypes_AddsRowSuccessfully()
    {
        // Arrange
        var df = new Microsoft.Data.Analysis.DataFrame(
            new PrimitiveDataFrameColumn<int>("ID", new[] { 1, 2 }),
            new StringDataFrameColumn("Name", new[] { "Alice", "Bob" })
        );

        // Act
        df.AddRow(3, "Charlie");

        // Assert
        df.Rows.Count.Should().Be(3);
        df["ID"][2].Should().Be(3);
        df["Name"][2].Should().Be("Charlie");
    }

    [Fact]
    public void AddRow_WithNullableInt_HandlesNullCorrectly()
    {
        // Arrange
        var df = new Microsoft.Data.Analysis.DataFrame(
            new PrimitiveDataFrameColumn<int>("ID", new int?[] { 1, 2 }),
            new StringDataFrameColumn("Name", new[] { "Alice", "Bob" })
        );

        // Act
        df.AddRow(null, "Charlie");

        // Assert
        df.Rows.Count.Should().Be(3);
        df["ID"][2].Should().BeNull();
        df["Name"][2].Should().Be("Charlie");
    }

    [Fact]
    public void AddRow_WithMultipleNumericTypes_AddsCorrectly()
    {
        // Arrange
        var df = new Microsoft.Data.Analysis.DataFrame(
            new PrimitiveDataFrameColumn<int>("IntCol", new[] { 1 }),
            new PrimitiveDataFrameColumn<long>("LongCol", new[] { 100L }),
            new PrimitiveDataFrameColumn<float>("FloatCol", new[] { 1.5f }),
            new PrimitiveDataFrameColumn<double>("DoubleCol", new[] { 2.5 }),
            new PrimitiveDataFrameColumn<decimal>("DecimalCol", new[] { 3.5m })
        );

        // Act
        df.AddRow(2, 200L, 2.5f, 3.5, 4.5m);

        // Assert
        df.Rows.Count.Should().Be(2);
        df["IntCol"][1].Should().Be(2);
        df["LongCol"][1].Should().Be(200L);
        df["FloatCol"][1].Should().Be(2.5f);
        df["DoubleCol"][1].Should().Be(3.5);
        df["DecimalCol"][1].Should().Be(4.5m);
    }

    [Fact]
    public void AddRow_WithBooleanColumn_AddsCorrectly()
    {
        // Arrange
        var df = new Microsoft.Data.Analysis.DataFrame(
            new PrimitiveDataFrameColumn<int>("ID", new[] { 1 }),
            new PrimitiveDataFrameColumn<bool>("Active", new[] { true })
        );

        // Act
        df.AddRow(2, false);

        // Assert
        df.Rows.Count.Should().Be(2);
        df["Active"][1].Should().Be(false);
    }

    [Fact]
    public void AddRow_WithDateTimeColumn_AddsCorrectly()
    {
        // Arrange
        var date1 = new DateTime(2024, 1, 1);
        var date2 = new DateTime(2024, 1, 2);
        var df = new Microsoft.Data.Analysis.DataFrame(
            new PrimitiveDataFrameColumn<int>("ID", new[] { 1 }),
            new PrimitiveDataFrameColumn<DateTime>("Date", new[] { date1 })
        );

        // Act
        df.AddRow(2, date2);

        // Assert
        df.Rows.Count.Should().Be(2);
        df["Date"][1].Should().Be(date2);
    }

    [Fact]
    public void AddRow_WithWrongNumberOfValues_ThrowsArgumentException()
    {
        // Arrange
        var df = new Microsoft.Data.Analysis.DataFrame(
            new PrimitiveDataFrameColumn<int>("ID", new[] { 1 }),
            new StringDataFrameColumn("Name", new[] { "Alice" })
        );

        // Act & Assert
        var act = () => df.AddRow(2); // Missing Name value
        act.Should().Throw<ArgumentException>()
            .WithMessage("*number of provided values must match*");
    }

    [Fact]
    public void AddRow_WithIncompatibleType_ThrowsInvalidOperationException()
    {
        // Arrange
        var df = new Microsoft.Data.Analysis.DataFrame(
            new PrimitiveDataFrameColumn<int>("ID", new[] { 1 }),
            new StringDataFrameColumn("Name", new[] { "Alice" })
        );

        // Act & Assert
        var act = () => df.AddRow("NotAnInt", "Bob"); // String instead of int
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*not compatible*");
    }

    [Fact]
    public void AddRow_WithIEnumerable_AddsRowSuccessfully()
    {
        // Arrange
        var df = new Microsoft.Data.Analysis.DataFrame(
            new PrimitiveDataFrameColumn<int>("ID", new[] { 1, 2 }),
            new StringDataFrameColumn("Name", new[] { "Alice", "Bob" })
        );
        var values = new object[] { 3, "Charlie" };

        // Act
        df.AddRow((System.Collections.Generic.IEnumerable<object>)values);

        // Assert
        df.Rows.Count.Should().Be(3);
        df["ID"][2].Should().Be(3);
        df["Name"][2].Should().Be("Charlie");
    }

    [Fact]
    public void AddRow_ToEmptyDataFrame_CreatesFirstRow()
    {
        // Arrange
        var df = new Microsoft.Data.Analysis.DataFrame(
            new PrimitiveDataFrameColumn<int>("ID"),
            new StringDataFrameColumn("Name")
        );

        // Act
        df.AddRow(1, "Alice");

        // Assert
        df.Rows.Count.Should().Be(1);
        df["ID"][0].Should().Be(1);
        df["Name"][0].Should().Be("Alice");
    }

    [Fact]
    public void AddRow_MultipleRows_MaintainsOrder()
    {
        // Arrange
        var df = new Microsoft.Data.Analysis.DataFrame(
            new PrimitiveDataFrameColumn<int>("ID"),
            new StringDataFrameColumn("Name")
        );

        // Act
        df.AddRow(1, "Alice");
        df.AddRow(2, "Bob");
        df.AddRow(3, "Charlie");

        // Assert
        df.Rows.Count.Should().Be(3);
        df["ID"][0].Should().Be(1);
        df["ID"][1].Should().Be(2);
        df["ID"][2].Should().Be(3);
        df["Name"][0].Should().Be("Alice");
        df["Name"][1].Should().Be("Bob");
        df["Name"][2].Should().Be("Charlie");
    }

    [Fact]
    public void AddRow_WithAllNullValues_AddsRowWithNulls()
    {
        // Arrange
        var df = new Microsoft.Data.Analysis.DataFrame(
            new PrimitiveDataFrameColumn<int>("ID", new int?[] { 1 }),
            new StringDataFrameColumn("Name", new[] { "Alice" })
        );

        // Act
        df.AddRow(null, null);

        // Assert
        df.Rows.Count.Should().Be(2);
        df["ID"][1].Should().BeNull();
        df["Name"][1].Should().BeNull();
    }

    [Fact]
    public void AddRow_WithUnsignedIntegerTypes_AddsCorrectly()
    {
        // Arrange
        var df = new Microsoft.Data.Analysis.DataFrame(
            new PrimitiveDataFrameColumn<byte>("ByteCol", new byte[] { 1 }),
            new PrimitiveDataFrameColumn<ushort>("UShortCol", new ushort[] { 100 }),
            new PrimitiveDataFrameColumn<uint>("UIntCol", new uint[] { 1000 }),
            new PrimitiveDataFrameColumn<ulong>("ULongCol", new ulong[] { 10000 })
        );

        // Act
        df.AddRow((byte)2, (ushort)200, (uint)2000, (ulong)20000);

        // Assert
        df.Rows.Count.Should().Be(2);
        df["ByteCol"][1].Should().Be((byte)2);
        df["UShortCol"][1].Should().Be((ushort)200);
        df["UIntCol"][1].Should().Be((uint)2000);
        df["ULongCol"][1].Should().Be((ulong)20000);
    }
}
