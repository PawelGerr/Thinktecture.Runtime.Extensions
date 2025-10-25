# Review: Logging/WarningLogger.cs

Path: `src/Thinktecture.Runtime.Extensions.SourceGenerator/Logging/WarningLogger.cs`

## Summary
`WarningLogger` is a sealed logger inheriting `LoggerBase` and implementing `ILogger`. It sets its minimum level to `LogLevel.Warning`. It intentionally ignores Trace, Debug, and Information messages (no-ops) while delegating Error to `LoggerBase.Log`.

## API and Behavior
- Constructor: `WarningLogger(LoggerFactory, ILoggingSink, string source)` calls `base(..., LogLevel.Warning)`, establishing the minimum level.
- `LogTrace(string message, TypeDeclarationSyntax type)`: no-op — trace suppressed.
- `LogDebug(...)` (generic and non-generic): no-op — debug suppressed.
- `LogInformation(...)` (generic and non-generic): no-op — information suppressed.
- `LogError(...)`: delegates to `Log(LogLevel.Error, ...)` including `Exception?`.

Interface compliance:
- All `ILogger` members are implemented. Optional parameters are defined at interface level; implementation signatures are compatible. Members from `LoggerBase` (e.g., `IsEnabled`, `Log(LogLevel, string)`) cover additional interface methods.

## Potential Issues and Warnings
- Unused parameters (analyzers like IDE0060):
  - `type` in `LogTrace`
  - `type`, `factory` in non-generic `LogDebug`
  - `type`, `namespaceAndName`, `factory` in generic `LogDebug`
  - `type`, `factory` in non-generic `LogInformation`
  - `type`, `namespaceAndName`, `factory` in generic `LogInformation`
  Options:
  - Add `_ = param;` for each to acknowledge usage; or
  - Delegate to `base.Log(LogLevel.Trace|Debug|Information, ...)` and let base perform level gating; or
  - Add suppression with justification per project policy.
- Consistency:
  - Other logger variants either delegate or no-op for lower levels. If uniform routing through the base (for formatting/enrichment) is desired, delegate and rely on base-level filtering. If strict suppression is intended, leave as no-op but document intent.
- Documentation:
  - No XML docs. If enforced, add brief summaries clarifying that this logger suppresses Trace/Debug/Information.

## Nullability
- Matches `ILogger` expectations: non-nullable `TypeDeclarationSyntax` for Trace; nullable for other overloads. Delegation for Error passes nullable values to `LoggerBase` which should handle nulls appropriately.

## Recommendations
- Either keep no-ops and add comments plus `_ =` assignments to avoid analyzer warnings, or delegate suppressed levels to `base.Log(...)` for consistency and future-proofing.
- Optionally add XML docs summarizing suppression behavior.

## Verdict
No functional errors. Minor warnings about unused parameters and a design choice regarding delegation vs suppression for Trace/Debug/Information.
