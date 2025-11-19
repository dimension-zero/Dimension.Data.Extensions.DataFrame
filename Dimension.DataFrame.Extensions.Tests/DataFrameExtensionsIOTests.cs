using FluentAssertions;
using Microsoft.Data.Analysis;
using System;
using System.IO;
using Xunit;

namespace Dimension.DataFrame.Extensions.Tests;

public class DataFrameExtensionsIOTests
{
    [Fact]
    public void SaveToCsv_BasicDataFrame_CreatesValidCsv()
    {
        // Arrange
        var df = new Microsoft.Data.Analysis.DataFrame(
            new PrimitiveDataFrameColumn<int>("ID", new[] { 1, 2, 3 }),
            new StringDataFrameColumn("Name", new[] { "Alice", "Bob", "Charlie" }),
            new PrimitiveDataFrameColumn<double>("Score", new[] { 95.5, 87.3, 92.1 })
        );
        var tempFile = Path.GetTempFileName();

        try
        {
            // Act
            df.SaveToCsv(tempFile);

            // Assert
            var content = File.ReadAllText(tempFile);
            content.Should().Contain("ID,Name,Score");
            content.Should().Contain("1,Alice,95.5");
            content.Should().Contain("2,Bob,87.3");
            content.Should().Contain("3,Charlie,92.1");
        }
        finally
        {
            if (File.Exists(tempFile))
                File.Delete(tempFile);
        }
    }

    [Fact]
    public void SaveToCsv_WithCustomSeparator_UsesCorrectSeparator()
    {
        // Arrange
        var df = new Microsoft.Data.Analysis.DataFrame(
            new PrimitiveDataFrameColumn<int>("A", new[] { 1, 2 }),
            new PrimitiveDataFrameColumn<int>("B", new[] { 3, 4 })
        );
        var tempFile = Path.GetTempFileName();

        try
        {
            // Act
            df.SaveToCsv(tempFile, sep: ";");

            // Assert
            var content = File.ReadAllText(tempFile);
            content.Should().Contain("A;B");
            content.Should().Contain("1;3");
            content.Should().Contain("2;4");
        }
        finally
        {
            if (File.Exists(tempFile))
                File.Delete(tempFile);
        }
    }

    [Fact]
    public void SaveToCsv_WithoutHeader_DoesNotIncludeColumnNames()
    {
        // Arrange
        var df = new Microsoft.Data.Analysis.DataFrame(
            new PrimitiveDataFrameColumn<int>("ID", new[] { 1, 2 })
        );
        var tempFile = Path.GetTempFileName();

        try
        {
            // Act
            df.SaveToCsv(tempFile, includeHeader: false);

            // Assert
            var content = File.ReadAllText(tempFile);
            content.Should().NotContain("ID");
            content.Should().Contain("1");
            content.Should().Contain("2");
        }
        finally
        {
            if (File.Exists(tempFile))
                File.Delete(tempFile);
        }
    }

    [Fact]
    public void SaveToCsv_WithQuotesInData_EscapesCorrectly()
    {
        // Arrange
        var df = new Microsoft.Data.Analysis.DataFrame(
            new StringDataFrameColumn("Text", new[] { "Hello \"World\"", "Simple text" })
        );
        var tempFile = Path.GetTempFileName();

        try
        {
            // Act
            df.SaveToCsv(tempFile);

            // Assert
            var content = File.ReadAllText(tempFile);
            content.Should().Contain("\"Hello \"\"World\"\"\""); // RFC 4180: quotes doubled
        }
        finally
        {
            if (File.Exists(tempFile))
                File.Delete(tempFile);
        }
    }

    [Fact]
    public void SaveToCsv_WithCommaInData_QuotesField()
    {
        // Arrange
        var df = new Microsoft.Data.Analysis.DataFrame(
            new StringDataFrameColumn("Text", new[] { "Hello, World", "Simple" })
        );
        var tempFile = Path.GetTempFileName();

        try
        {
            // Act
            df.SaveToCsv(tempFile);

            // Assert
            var content = File.ReadAllText(tempFile);
            content.Should().Contain("\"Hello, World\""); // RFC 4180: field with comma must be quoted
            content.Should().Contain("Simple"); // Simple text not quoted
        }
        finally
        {
            if (File.Exists(tempFile))
                File.Delete(tempFile);
        }
    }

    [Fact]
    public void SaveToCsv_WithNewlineInData_QuotesField()
    {
        // Arrange
        var df = new Microsoft.Data.Analysis.DataFrame(
            new StringDataFrameColumn("Text", new[] { "Line1\nLine2", "Simple" })
        );
        var tempFile = Path.GetTempFileName();

        try
        {
            // Act
            df.SaveToCsv(tempFile);

            // Assert
            var content = File.ReadAllText(tempFile);
            content.Should().Contain("\"Line1\nLine2\""); // RFC 4180: field with newline must be quoted
        }
        finally
        {
            if (File.Exists(tempFile))
                File.Delete(tempFile);
        }
    }

    [Fact]
    public void SaveToCsv_WithFormulaInjectionAttempt_SanitizesData()
    {
        // Arrange
        var df = new Microsoft.Data.Analysis.DataFrame(
            new StringDataFrameColumn("Text", new[] { "=SUM(A1:A10)", "+cmd", "-cmd", "@cmd", "\tcmd", "\rcmd" })
        );
        var tempFile = Path.GetTempFileName();

        try
        {
            // Act
            df.SaveToCsv(tempFile);

            // Assert
            var content = File.ReadAllText(tempFile);
            // CSV injection prevention: formula characters should be prefixed with single quote
            content.Should().Contain("'=SUM");
            content.Should().Contain("'+cmd");
            content.Should().Contain("'-cmd");
            content.Should().Contain("'@cmd");
            content.Should().Contain("'\tcmd");
            content.Should().Contain("'\rcmd");
        }
        finally
        {
            if (File.Exists(tempFile))
                File.Delete(tempFile);
        }
    }

    [Fact]
    public void SaveToCsv_WithNullValues_HandlesCorrectly()
    {
        // Arrange
        var df = new Microsoft.Data.Analysis.DataFrame(
            new PrimitiveDataFrameColumn<int>("Num", new int?[] { 1, null, 3 }),
            new StringDataFrameColumn("Text", new[] { "A", null, "C" })
        );
        var tempFile = Path.GetTempFileName();

        try
        {
            // Act
            df.SaveToCsv(tempFile);

            // Assert
            File.Exists(tempFile).Should().BeTrue();
            var content = File.ReadAllText(tempFile);
            content.Should().Contain("Num,Text");
            // Nulls should be represented as empty strings
            var lines = content.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
            lines.Should().HaveCountGreaterOrEqualTo(3); // Header + 3 data rows
        }
        finally
        {
            if (File.Exists(tempFile))
                File.Delete(tempFile);
        }
    }

    [Fact]
    public void SaveToCsv_ToInvalidPath_ThrowsIOException()
    {
        // Arrange
        var df = new Microsoft.Data.Analysis.DataFrame(
            new PrimitiveDataFrameColumn<int>("A", new[] { 1 })
        );
        var invalidPath = "/invalid/path/that/does/not/exist/file.csv";

        // Act & Assert
        var act = () => df.SaveToCsv(invalidPath);
        act.Should().Throw<IOException>();
    }

    [Fact]
    public void SaveToCsv_EmptyDataFrame_CreatesFileWithHeaderOnly()
    {
        // Arrange
        var df = new Microsoft.Data.Analysis.DataFrame(
            new PrimitiveDataFrameColumn<int>("ID"),
            new StringDataFrameColumn("Name")
        );
        var tempFile = Path.GetTempFileName();

        try
        {
            // Act
            df.SaveToCsv(tempFile);

            // Assert
            var content = File.ReadAllText(tempFile);
            content.Should().Contain("ID,Name");
            var lines = content.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
            lines.Should().HaveCount(1); // Only header line
        }
        finally
        {
            if (File.Exists(tempFile))
                File.Delete(tempFile);
        }
    }
}
