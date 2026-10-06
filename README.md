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

## Regression case study

[`.github/workflows/case-study.yml`](.github/workflows/case-study.yml) builds Performance
Agent from source and measures a deliberate regression in one job, on one runner:

| Role | Commit | `BuildReport()` implementation |
| --- | --- | --- |
| Baseline | `f7d0e68` | `StringBuilder` |
| Candidate | `cf8c9ee` | repeated `string +=` concatenation |

Measurements are interleaved (baseline A → candidate → baseline B), then two checks run
against [`performance-budget.json`](performance-budget.json) (5% mean, 10% allocation):

- **Regression check** (A vs candidate) is expected to **FAIL** (exit 1).
- **No-change control** (A vs B, identical code) must **not** FAIL; PASS or INCONCLUSIVE is acceptable.

The job itself fails if either expectation is violated, so a green run is evidence that
the regression was caught without a false positive on unchanged code. Verdicts appear in
the run summary; raw evidence JSON is uploaded as the `case-study-results` artifact.
