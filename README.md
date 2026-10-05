# Performance Agent Real-World Test

Independent BenchmarkDotNet project used to validate the installed `perfagent` tool outside the Performance Agent source repository.

This repository is intentionally small. It is a consumer fixture, not part of Performance Agent itself.

## Run directly

```powershell
dotnet run -c Release
```

## Validate Performance Agent

From this repository root:

```powershell
perfagent run .\PerformanceAgent.RealWorldBenchmarks.csproj
perfagent history
```

Then select a run as the Current baseline and use a later run as the candidate for regression checks and AI analysis.
