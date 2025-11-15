using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using Dimension.DataFrame.Extensions;
using Microsoft.Data.Analysis;

namespace Dimension.DataFrame.Extensions.Benchmarks;

[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[RankColumn]
public class RollingWindowBenchmarks
{
    private PrimitiveDataFrameColumn<double> _column = null!;

    [Params(1000, 10000)]
    public int N;

    [Params(3, 10, 50)]
    public int WindowSize;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(42);
        var data = Enumerable.Range(0, N).Select(_ => random.NextDouble() * 1000).ToArray();
        _column = new PrimitiveDataFrameColumn<double>("Data", data);
    }

    [Benchmark]
    public PrimitiveDataFrameColumn<double> RollingSum()
    {
        return _column.Rolling(WindowSize, values => values.Sum(v => v!.Value));
    }

    [Benchmark]
    public PrimitiveDataFrameColumn<double> RollingAverage()
    {
        return _column.Rolling(WindowSize, values => values.Average(v => v!.Value));
    }

    [Benchmark]
    public PrimitiveDataFrameColumn<double> RollingMax()
    {
        return _column.Rolling(WindowSize, values => values.Max(v => v!.Value));
    }
}
