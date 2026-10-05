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
        // Deliberate regression fixture: repeated immutable concatenation creates
        // substantially more work and allocations than the baseline StringBuilder version.
        var report = string.Empty;
        foreach (var value in _values)
            report += value + "|";
        return report;
    }
}
