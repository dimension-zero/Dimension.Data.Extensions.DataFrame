# Performance Benchmarks

This project contains performance benchmarks for the Dimension.DataFrame.Extensions library using BenchmarkDotNet.

## Running Benchmarks

### Run all benchmarks
```bash
dotnet run -c Release
```

### Run specific benchmark
```bash
dotnet run -c Release -- --filter *ArithmeticBenchmarks*
```

### Run with specific parameters
```bash
dotnet run -c Release -- --filter *StatisticsBenchmarks.Mean*
```

## Benchmark Categories

### ArithmeticBenchmarks
Tests the performance of arithmetic operations (Plus, Minus, Times, Divide) on DataFrame columns of varying sizes.

### StatisticsBenchmarks
Tests statistical calculations (Mean, Median, StdDev, Variance, Min, Max, Sum, Describe) across different dataset sizes.

### MathBenchmarks
Tests mathematical functions (Abs, Log, Log10, Exp, Sqrt, Pow, Sin, Cos) for various column sizes.

### RollingWindowBenchmarks
Tests rolling window operations with different window sizes and dataset sizes.

## Output

Benchmarks produce detailed reports including:
- Execution time (mean, median, std dev)
- Memory allocation
- Relative performance rankings
- Statistical significance

Results are saved to `BenchmarkDotNet.Artifacts/` directory.

## Tips

- Always run in Release mode (`-c Release`)
- Close other applications to minimize interference
- Run multiple times to ensure consistent results
- Use `--filter` to run specific benchmarks
- Export results: `dotnet run -c Release -- --exporters json,html`
