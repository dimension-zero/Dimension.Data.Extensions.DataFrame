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

    public static void SaveToCsv(this Microsoft.Data.Analysis.DataFrame dataFrame, string fullPath, string sep = ",", bool includeHeader = true)
    {
        var csvContent = new StringBuilder();

        var numColumns = dataFrame.Columns.Count;
        if (includeHeader)
        {
            for (var i = 0; i < numColumns; i++)
            {
                csvContent.Append(dataFrame.Columns[i].Name);
                if (i < numColumns - 1)
                {
                    csvContent.Append(sep);
                }
            }

            csvContent.AppendLine();
        }

        for (long i = 0; i < dataFrame.Rows.Count; i++)
        {
            var row = dataFrame.Rows[i];
            for (var j = 0; j < numColumns; j++)
            {
                var value = row[j]?.ToString() ?? "";
                // Handle potential separator in value (simple escape mechanism, consider enhancing for full CSV compliance)
                if (value.Contains(sep))
                {
                    value = $"\"{value}\"";
                }

                csvContent.Append(value);
                if (j < numColumns - 1)
                {
                    csvContent.Append(sep);
                }
            }

            csvContent.AppendLine();
        }

        File.WriteAllText(fullPath, csvContent.ToString());
    }
}