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

### Latest result

[Run 37526050859](https://github.com/CodingBrushUp2/performance-agent-realworld-test/actions/runs/37526050859)
(GitHub-hosted `ubuntu-24.04`, times in ns, allocations in bytes per operation):

```text
regression check (exit 1)
StringProcessingBenchmarks.BuildReport: FAIL
  Mean: 3558.11 -> 289144.7 (+8026.35%) (budget +5%) FAIL
  Allocation: 59200 -> 13038000 (+21923.65%) (budget +10%) FAIL
Overall: FAIL

no-change control (exit 0)
StringProcessingBenchmarks.BuildReport: PASS
  Mean: 3558.11 -> 3534.2 (-0.67%) (budget +5%) PASS
  Allocation: 59200 -> 59200 (0%) (budget +10%) PASS
Overall: PASS
```

### What it found

The first runs did not pass. They exposed two Performance Agent bugs, both fixed in
[performance-agent#119](https://github.com/CodingBrushUp2/performance-agent/pull/119):

1. **Wrong project measured.** The benchmark host inherited the caller's working
   directory, and BenchmarkDotNet locates the project by name from there. With the
   fixture checked out in the workspace, both baseline and candidate runs silently
   built the workspace copy, and the regression was reported as PASS. The workflow keeps
   that layout, so it now guards against a recurrence.
2. **Unclear failure.** When BenchmarkDotNet could not build a benchmark, perfagent
   crashed with a stack trace instead of naming the failed benchmark and its log.
