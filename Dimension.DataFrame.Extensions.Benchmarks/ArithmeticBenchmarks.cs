using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using Dimension.DataFrame.Extensions;
using Microsoft.Data.Analysis;

namespace Dimension.DataFrame.Extensions.Benchmarks;

[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[RankColumn]
public class ArithmeticBenchmarks
{
    private PrimitiveDataFrameColumn<int> _column1 = null!;
    private PrimitiveDataFrameColumn<int> _column2 = null!;
    private PrimitiveDataFrameColumn<double> _doubleColumn1 = null!;
    private PrimitiveDataFrameColumn<double> _doubleColumn2 = null!;

    [Params(1000, 10000, 100000)]
    public int N;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(42);
        var data1 = Enumerable.Range(0, N).Select(_ => random.Next(1, 1000)).ToArray();
        var data2 = Enumerable.Range(0, N).Select(_ => random.Next(1, 1000)).ToArray();

        _column1 = new PrimitiveDataFrameColumn<int>("A", data1);
        _column2 = new PrimitiveDataFrameColumn<int>("B", data2);

        var doubleData1 = data1.Select(x => (double)x).ToArray();
        var doubleData2 = data2.Select(x => (double)x).ToArray();

        _doubleColumn1 = new PrimitiveDataFrameColumn<double>("A", doubleData1);
        _doubleColumn2 = new PrimitiveDataFrameColumn<double>("B", doubleData2);
    }

    [Benchmark]
    public PrimitiveDataFrameColumn<int> Plus_Int()
    {
        return _column1.Plus(_column2);
    }

    [Benchmark]
    public PrimitiveDataFrameColumn<int> Minus_Int()
    {
        return _column1.Minus(_column2);
    }

    [Benchmark]
    public PrimitiveDataFrameColumn<int> Times_Int()
    {
        return _column1.Times(_column2);
    }

    [Benchmark]
    public PrimitiveDataFrameColumn<double> Divide_Double()
    {
        return _doubleColumn1.Divide(_doubleColumn2, "Result");
    }
}
