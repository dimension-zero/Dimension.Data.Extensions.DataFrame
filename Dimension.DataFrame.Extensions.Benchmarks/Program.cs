using BenchmarkDotNet.Running;
using Dimension.DataFrame.Extensions.Benchmarks;

Console.WriteLine("Dimension.DataFrame.Extensions Performance Benchmarks");
Console.WriteLine("=====================================================");
Console.WriteLine();

var switcher = new BenchmarkSwitcher(new[]
{
    typeof(ArithmeticBenchmarks),
    typeof(StatisticsBenchmarks),
    typeof(MathBenchmarks),
    typeof(RollingWindowBenchmarks)
});

switcher.Run(args);
