# DataFrame.Extensions

[![CI/CD](https://github.com/dimension-zero/Dimension.Data.Extensions.DataFrame/actions/workflows/ci.yml/badge.svg)](https://github.com/dimension-zero/Dimension.Data.Extensions.DataFrame/actions/workflows/ci.yml)
[![NuGet](https://img.shields.io/nuget/v/Dimension.DataFrame.Extensions.svg)](https://www.nuget.org/packages/Dimension.DataFrame.Extensions/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

A comprehensive set of extension methods for `Microsoft.Data.Analysis.DataFrame` that provides **pandas-like functionality** for .NET data science and numerical computing.

## Features

- **Arithmetic Operations** - Element-wise Plus, Minus, Times, Divide
- **Calculations** - Diff, Apply, Pow operations
- **Cumulative Operations** - Running sums and absolute sums
- **Rolling Windows** - Moving averages and custom rolling calculations
- **Filtering** - Predicate-based and index-based filtering
- **Column Management** - Selection, existence checking, type-safe retrieval
- **Null/NaN Handling** - Drop rows with missing data
- **Shift Operations** - Lag/lead column values
- **I/O Operations** - Pretty printing and CSV export
- **Syntactic Sugar** - Method chaining with fluent API

## Installation

### NuGet Package Manager
```
Install-Package Dimension.DataFrame.Extensions
```

### .NET CLI
```
dotnet add package Dimension.DataFrame.Extensions
```

### PackageReference
```xml
<PackageReference Include="Dimension.DataFrame.Extensions" Version="1.0.0" />
```

## Quick Start

```csharp
using Dimension.DataFrame.Extensions;
using Microsoft.Data.Analysis;

// Create a DataFrame
var prices = new PrimitiveDataFrameColumn<double>("Price", new[] { 100.0, 105.0, 103.0, 108.0, 110.0 });
var volumes = new PrimitiveDataFrameColumn<int>("Volume", new[] { 1000, 1500, 1200, 1800, 2000 });
var df = new DataFrame(prices, volumes);

// Calculate price differences
var priceDiff = prices.Diff<double>();
priceDiff.AddTo(df, "PriceChange");

// Calculate rolling average (3-period)
var rollingAvg = prices.Rolling(3, values => values.Average(v => v!.Value));
rollingAvg.AddTo(df, "MA_3");

// Print the DataFrame
df.Print();
```

## Usage Examples

### Arithmetic Operations

```csharp
var col1 = new PrimitiveDataFrameColumn<int>("A", new[] { 1, 2, 3, 4, 5 });
var col2 = new PrimitiveDataFrameColumn<int>("B", new[] { 10, 20, 30, 40, 50 });

// Addition
var sum = col1.Plus(col2);  // [11, 22, 33, 44, 55]

// Subtraction
var diff = col1.Minus(col2);  // [-9, -18, -27, -36, -45]

// Multiplication
var product = col1.Times(col2);  // [10, 40, 90, 160, 250]

// Division
var quotient = col2.Divide(col1, "Quotient");  // [10.0, 10.0, 10.0, 10.0, 10.0]
```

### Cumulative Operations

```csharp
var data = new PrimitiveDataFrameColumn<int>("Data", new[] { 1, 2, 3, 4, 5 });

// Cumulative sum
var cumSum = data.Cumulate();  // [1, 3, 6, 10, 15]

// Cumulative absolute sum
var negData = new PrimitiveDataFrameColumn<int>("NegData", new[] { -1, 2, -3, 4, -5 });
var cumAbsSum = negData.CumulateAbs();  // [1, 3, 6, 10, 15]
```

### Shift Operations (Lag/Lead)

```csharp
var prices = new PrimitiveDataFrameColumn<double>("Price", new[] { 100.0, 105.0, 103.0, 108.0 });

// Lag by 1 period (shift forward)
var lag1 = prices.Shift(1);  // [null, 100.0, 105.0, 103.0]

// Lead by 1 period (shift backward)
var lead1 = prices.Shift(-1);  // [105.0, 103.0, 108.0, null]

// Custom fill value
var lagWithFill = prices.Shift(1, 0.0);  // [0.0, 100.0, 105.0, 103.0]
```

### Rolling Window Calculations

```csharp
var data = new PrimitiveDataFrameColumn<double>("Data", new[] { 1.0, 2.0, 3.0, 4.0, 5.0 });

// Rolling sum
var rollingSum = data.Rolling(3, values => values.Sum(v => v!.Value));
// [null, null, 6.0, 9.0, 12.0]

// Rolling average
var rollingAvg = data.Rolling(3, values => values.Average(v => v!.Value));
// [null, null, 2.0, 3.0, 4.0]

// Rolling maximum
var rollingMax = data.Rolling(3, values => values.Max(v => v!.Value));
// [null, null, 3.0, 4.0, 5.0]
```

### Apply Custom Functions

```csharp
var data = new PrimitiveDataFrameColumn<int>("Data", new[] { 1, 2, 3, 4, 5 });

// Square all values
var squared = data.Apply(x => x * x, "Squared");  // [1, 4, 9, 16, 25]

// Apply custom transformation
var transformed = data.Apply(x => x * 2 + 1, "Transformed");  // [3, 5, 7, 9, 11]
```

### Filtering

```csharp
var df = new DataFrame(
    new PrimitiveDataFrameColumn<int>("A", new[] { 1, 2, 3, 4, 5 }),
    new PrimitiveDataFrameColumn<double>("B", new[] { 1.5, 2.5, 3.5, 4.5, 5.5 })
);

// Filter by predicate
var filtered = df.Filter<int>("A", value => value > 3);
// Returns DataFrame with rows where A > 3

// Filter by row indices
var subset = df.Filter(new[] { 0, 2, 4 });
// Returns rows at indices 0, 2, and 4
```

### Null and NaN Handling

```csharp
var df = new DataFrame(
    new PrimitiveDataFrameColumn<int>("A", new int?[] { 1, null, 3, 4 }),
    new PrimitiveDataFrameColumn<double>("B", new[] { 1.0, 2.0, double.NaN, 4.0 })
);

// Drop rows with nulls
var noNulls = df.DropNulls();  // Rows 0 and 3 remain

// Drop rows with NaN values
var noNaNs = df.DropNAs();  // Rows 0, 1, and 3 remain

// Drop rows with either nulls or NaNs
var clean = df.DropNullsOrNAs();  // Only rows 0 and 3 remain
```

### Method Chaining (Fluent API)

```csharp
var df = new DataFrame();
var col1 = new PrimitiveDataFrameColumn<int>("A", new[] { 1, 2, 3 });
var col2 = new PrimitiveDataFrameColumn<int>("B", new[] { 10, 20, 30 });

// Chain operations together
col1.Plus(col2)
    .Pow(2)
    .WithName<int>("Sum_Squared")
    .AddTo(df);

// df now contains column "Sum_Squared" with values [121, 484, 1089]
```

### Column Operations

```csharp
var df = new DataFrame(
    new PrimitiveDataFrameColumn<int>("A", new[] { 1, 2, 3 }),
    new PrimitiveDataFrameColumn<int>("B", new[] { 10, 20, 30 }),
    new PrimitiveDataFrameColumn<int>("C", new[] { 100, 200, 300 })
);

// Select specific columns
var subset = df.SelectColumns("A", "C");

// Check if column exists
bool hasColumn = df.ColumnExists("B");  // true

// Try to get column with type safety
if (df.TryGetColumn<int>("A", out var columnA))
{
    // Use columnA
}
```

### I/O Operations

```csharp
var df = new DataFrame(
    new PrimitiveDataFrameColumn<int>("ID", new[] { 1, 2, 3 }),
    new PrimitiveDataFrameColumn<string>("Name", new[] { "Alice", "Bob", "Charlie" }),
    new PrimitiveDataFrameColumn<double>("Score", new[] { 95.5, 87.3, 92.1 })
);

// Print to debug output (aligned columns)
df.Print(numRows: 10, numberFormat: "F2");

// Save to CSV
df.SaveToCsv("output.csv", sep: ",", includeHeader: true);
```

## Requirements

- .NET 8.0 or later
- Microsoft.Data.Analysis 0.21.1 or later
- MathNet.Numerics 5.0.0 or later

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## Testing

```bash
dotnet test
```

## Building from Source

```bash
git clone https://github.com/dimension-zero/Dimension.Data.Extensions.DataFrame.git
cd Dimension.Data.Extensions.DataFrame
dotnet build
```

## Creating NuGet Package

```bash
dotnet pack --configuration Release
```

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Authors

**Dimension Technologies**

## Acknowledgments

- Built on top of [Microsoft.Data.Analysis](https://www.nuget.org/packages/Microsoft.Data.Analysis/)
- Inspired by pandas for Python
- Uses [MathNet.Numerics](https://numerics.mathdotnet.com/) for numerical operations

## Support

For issues, questions, or contributions, please visit the [GitHub repository](https://github.com/dimension-zero/Dimension.Data.Extensions.DataFrame).

---

**Issued under the MIT Licence by Dimension Technologies.**
