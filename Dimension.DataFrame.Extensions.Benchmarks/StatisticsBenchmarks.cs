using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using Dimension.DataFrame.Extensions;
using Microsoft.Data.Analysis;

namespace Dimension.DataFrame.Extensions.Benchmarks;

[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[RankColumn]
public class StatisticsBenchmarks
{
    private PrimitiveDataFrameColumn<double> _column = null!;

    [Params(1000, 10000, 100000)]
    public int N;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(42);
        var data = Enumerable.Range(0, N).Select(_ => random.NextDouble() * 1000).ToArray();
        _column = new PrimitiveDataFrameColumn<double>("Data", data);
    }

    [Benchmark]
    public double? Mean()
    {
        return _column.Mean();
    }

    [Benchmark]
    public double? Median()
    {
        return _column.Median();
    }

    [Benchmark]
    public double? StdDev()
    {
        return _column.StdDev();
    }

    [Benchmark]
    public double? Variance()
    {
        return _column.Variance();
    }

    [Benchmark]
    public double? Min()
    {
        return _column.Min();
    }

    [Benchmark]
    public double? Max()
    {
        return _column.Max();
    }

    [Benchmark]
    public double Sum()
    {
        return _column.Sum();
    }

    [Benchmark]
    public (long, double?, double?, double?, double?, double?, double?, double?) Describe()
    {
        return _column.Describe();
    }
}
