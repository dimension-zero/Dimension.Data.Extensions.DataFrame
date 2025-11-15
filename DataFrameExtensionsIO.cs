using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Dimension.DataFrame.Extensions;

/// <summary>
/// I/O extension methods to make Microsoft's DataFrame a little more user-friendly.
/// </summary>
public static class DataFrameExtensionsIO
{
    public static void Print(this Microsoft.Data.Analysis.DataFrame df,
                             string                                 numberFormat = "F2",
                             int                                    numRows      = -1,
                             int                                    colSpacing   = 2)
    {
        var sb = new StringBuilder();
        var maxRows = numRows > 0 ? Math.Min(numRows, (int) df.Rows.Count) : (int) df.Rows.Count;
        var columnWidths = new List<int>();
        var numericColumns = new bool[df.Columns.Count];

        // Determine column widths and if column is predominantly numeric
        for (var colIndex = 0; colIndex < df.Columns.Count; colIndex++)
        {
            var column = df.Columns[colIndex];
            var maxColumnWidth = column.Name.Length;
            var numericCount = 0;

            for (var rowIndex = 0; rowIndex < maxRows; rowIndex++)
            {
                var value = column[rowIndex];
                var formattedValue = FormatValue(value, numberFormat);
                var valueLength = formattedValue?.Length ?? 0;
                maxColumnWidth = Math.Max(maxColumnWidth, valueLength);

                if (value.IsNumeric())
                {
                    numericCount++;
                }
            }

            numericColumns[colIndex] = numericCount > maxRows / 2; // Consider numeric if more than half of the values are numeric
            columnWidths.Add(maxColumnWidth + colSpacing);         // Add padding
        }

        // Header
        for (var i = 0; i < df.Columns.Count; i++)
        {
            var columnName = df.Columns[i].Name;
            if (numericColumns[i])
            {
                // Right-align numeric column headers
                sb.Append(columnName.PadLeft(columnWidths[i]));
            }
            else
            {
                // Left-align non-numeric column headers
                sb.Append(columnName.PadRight(columnWidths[i]));
            }
        }

        sb.AppendLine();

        // Rows
        for (var rowIndex = 0; rowIndex < maxRows; rowIndex++)
        {
            for (var colIndex = 0; colIndex < df.Columns.Count; colIndex++)
            {
                var value = df.Columns[colIndex][rowIndex];
                var formattedValue = FormatValue(value, numberFormat);
                var colWidth = columnWidths[colIndex];

                if (numericColumns[colIndex])
                {
                    sb.Append(formattedValue?.PadLeft(colWidth) ?? "");
                }
                else
                {
                    sb.Append(formattedValue?.PadRight(colWidth) ?? "");
                }
            }

            sb.AppendLine();
        }

        // Handle case where the DataFrame is longer than numRows
        if (df.Rows.Count > numRows && numRows > 0)
        {
            sb.AppendLine("...");
        }

        Debug.WriteLine(sb.ToString());
    }

    private static bool IsNumeric(this object? value)
    {
        if (value == null)
        {
            return false;
        }

        return value switch
               {
                   double _  => true,
                   float _   => true,
                   decimal _ => true,
                   int _     => true,
                   long _    => true,
                   short _   => true,
                   byte _    => true,
                   string    => double.TryParse(value.ToString(), out _),
                   _         => false
               };
    }

    private static string? FormatValue(object? value, string numberFormat)
    {
        if (value == null)
        {
            return "";
        }

        return value switch
               {
                   double dblValue        => dblValue.ToString(numberFormat),
                   DateTime dateTimeValue => dateTimeValue.TimeOfDay == TimeSpan.Zero ? dateTimeValue.ToString("yyyy-MM-dd") : dateTimeValue.ToString("yyyy-MM-dd HH:mm:ss"),
                   _                      => value.ToString()
               };
    }

    /// <summary>
    /// Saves DataFrame to CSV file with RFC 4180 compliance
    /// </summary>
    /// <param name="dataFrame">The DataFrame to save</param>
    /// <param name="fullPath">Full path to output CSV file</param>
    /// <param name="sep">Column separator (default comma)</param>
    /// <param name="includeHeader">Include column names as header row</param>
    public static void SaveToCsv(this Microsoft.Data.Analysis.DataFrame dataFrame, string fullPath, string sep = ",", bool includeHeader = true)
    {
        try
        {
            var csvContent = new StringBuilder();
            var numColumns = dataFrame.Columns.Count;

            // Write header if requested
            if (includeHeader)
            {
                for (var i = 0; i < numColumns; i++)
                {
                    csvContent.Append(EscapeCsvValue(dataFrame.Columns[i].Name, sep));
                    if (i < numColumns - 1)
                    {
                        csvContent.Append(sep);
                    }
                }
                csvContent.AppendLine();
            }

            // Write data rows
            for (long i = 0; i < dataFrame.Rows.Count; i++)
            {
                var row = dataFrame.Rows[i];
                for (var j = 0; j < numColumns; j++)
                {
                    var value = row[j]?.ToString() ?? "";
                    csvContent.Append(EscapeCsvValue(value, sep));

                    if (j < numColumns - 1)
                    {
                        csvContent.Append(sep);
                    }
                }
                csvContent.AppendLine();
            }

            File.WriteAllText(fullPath, csvContent.ToString());
        }
        catch (Exception ex)
        {
            throw new IOException($"Failed to save CSV to '{fullPath}': {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Escapes a CSV value according to RFC 4180 and prevents CSV injection
    /// </summary>
    /// <param name="value">The value to escape</param>
    /// <param name="separator">The column separator</param>
    /// <returns>Escaped CSV value</returns>
    private static string EscapeCsvValue(string value, string separator)
    {
        if (string.IsNullOrEmpty(value))
        {
            return string.Empty;
        }

        // CSV Injection prevention - sanitize values starting with formula characters
        // These can be exploited in Excel/LibreOffice to execute formulas
        if (value.Length > 0)
        {
            var firstChar = value[0];
            if (firstChar == '=' || firstChar == '+' || firstChar == '-' || firstChar == '@' || firstChar == '\t' || firstChar == '\r')
            {
                // Prefix with single quote to prevent formula interpretation
                value = "'" + value;
            }
        }

        // RFC 4180: Fields containing separators, double quotes, or newlines must be quoted
        var needsQuoting = value.Contains(separator) ||
                          value.Contains('"') ||
                          value.Contains('\n') ||
                          value.Contains('\r');

        if (!needsQuoting)
        {
            return value;
        }

        // RFC 4180: Escape double quotes by doubling them
        var escaped = value.Replace("\"", "\"\"");

        // RFC 4180: Wrap the field in double quotes
        return $"\"{escaped}\"";
    }
}