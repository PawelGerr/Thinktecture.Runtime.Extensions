# Review: Logging/ErrorLogger.cs

Path: `src/Thinktecture.Runtime.Extensions.SourceGenerator/Logging/ErrorLogger.cs`

## Summary
`ErrorLogger` is a sealed logger inheriting `LoggerBase` and implementing `ILogger`. It sets its minimum level to `LogLevel.Error`. It intentionally ignores Trace, Debug, and Information (no-ops) while delegating Error to `LoggerBase.Log`.

## API and Behavior
- Constructor: `ErrorLogger(LoggerFactory, ILoggingSink, string source)` calls `base(..., LogLevel.Error)`, setting the min level to Error.
- `LogTrace(string message, TypeDeclarationSyntax type)`: no-op — trace suppressed.
- `LogDebug(...)` (generic and non-generic): no-op — debug suppressed.
- `LogInformation(...)` (generic and non-generic): no-op — information suppressed.
- `LogError(...)`: delegates to `Log(LogLevel.Error, ...)` including `Exception?`.

Interface compliance:
- Implements all `ILogger` members. Optional parameters on the interface are not required on the implementation signatures. Members coming from `LoggerBase` (e.g., `IsEnabled`, `Log(LogLevel, string)`) satisfy the interface.

## Potential Issues and Warnings
- Unused parameters (analyzers like IDE0060):
  - `type` in `LogTrace`
  - `type`, `factory` in non-generic `LogDebug`
  - `type`, `namespaceAndName`, `factory` in generic `LogDebug`
  - `type`, `factory` in non-generic `LogInformation`
  - `type`, `namespaceAndName`, `factory` in generic `LogInformation`
  Options:
  - Add `_ = param;` for each; or
  - Delegate to `base.Log(LogLevel.Trace|Debug|Information, ...)` and let base perform level gating; or
  - Suppress with justification per project conventions.
- Consistency:
  - Other logger variants either delegate suppressed levels to base (e.g., `DebugLogger` for Debug/Info) or no-op. If uniform routing through the base (for consistent formatting/enrichment and future-proofing) is desired, delegate and rely on base’s level filtering. If strict suppression is desired, keep no-ops and document intent.
- Documentation:
  - No XML docs. If the project enforces documentation, add summaries clarifying suppression of Trace/Debug/Information.

## Nullability
- Matches `ILogger`: non-nullable `TypeDeclarationSyntax` for Trace; nullable for other overloads. Delegation for Error should handle nulls in `LoggerBase`.

## Recommendations
- Either keep no-ops and add comments plus `_ =` assignments to silence analyzers, or delegate suppressed levels to `base.Log(...)` for consistency.
- Optionally add XML docs describing behavior.

## Verdict
No functional errors. Minor warnings regarding unused parameters and a design choice on suppression vs delegation for Trace/Debug/Information.
