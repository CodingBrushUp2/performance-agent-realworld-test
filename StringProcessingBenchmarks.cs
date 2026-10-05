using BenchmarkDotNet.Attributes;

[MemoryDiagnoser]
public class StringProcessingBenchmarks
{
    private readonly string[] _values = Enumerable.Range(0, 1_000)
        .Select(i => $"order-{i:D6}")
        .ToArray();

    [Benchmark]
    public string BuildReport()
    {
        var builder = new System.Text.StringBuilder();
        foreach (var value in _values)
            builder.Append(value).Append('|');
        return builder.ToString();
    }
}
