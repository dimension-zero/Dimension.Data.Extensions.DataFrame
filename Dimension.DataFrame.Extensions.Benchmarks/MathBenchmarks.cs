using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using Dimension.DataFrame.Extensions;
using Microsoft.Data.Analysis;

namespace Dimension.DataFrame.Extensions.Benchmarks;

[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[RankColumn]
public class MathBenchmarks
{
    private PrimitiveDataFrameColumn<double> _column = null!;

    [Params(1000, 10000, 100000)]
    public int N;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(42);
        var data = Enumerable.Range(0, N).Select(_ => random.NextDouble() * 100 + 1).ToArray();
        _column = new PrimitiveDataFrameColumn<double>("Data", data);
    }

    [Benchmark]
    public PrimitiveDataFrameColumn<double> Abs()
    {
        return _column.Abs();
    }

    [Benchmark]
    public PrimitiveDataFrameColumn<double> Log()
    {
        return _column.Log();
    }

    [Benchmark]
    public PrimitiveDataFrameColumn<double> Log10()
    {
        return _column.Log10();
    }

    [Benchmark]
    public PrimitiveDataFrameColumn<double> Exp()
    {
        return _column.Exp();
    }

    [Benchmark]
    public PrimitiveDataFrameColumn<double> Sqrt()
    {
        return _column.Sqrt();
    }

    [Benchmark]
    public PrimitiveDataFrameColumn<double> Pow()
    {
        return _column.Pow(2);
    }

    [Benchmark]
    public PrimitiveDataFrameColumn<double> Sin()
    {
        return _column.Sin();
    }

    [Benchmark]
    public PrimitiveDataFrameColumn<double> Cos()
    {
        return _column.Cos();
    }
}
