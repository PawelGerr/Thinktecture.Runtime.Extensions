# Review: Logging/InformationLogger.cs

Path: `src/Thinktecture.Runtime.Extensions.SourceGenerator/Logging/InformationLogger.cs`

## Summary
`InformationLogger` is a sealed logger that inherits `LoggerBase` and implements `ILogger`. It sets its minimum level to `LogLevel.Information` via the base constructor. It intentionally ignores Trace and Debug messages (no-op methods) while delegating Information and Error to `LoggerBase.Log`.

## API and Behavior
- Constructor: `InformationLogger(LoggerFactory, ILoggingSink, string source)` calls `base(..., LogLevel.Information)`, establishing the min level for this logger instance.
- `LogTrace(string message, TypeDeclarationSyntax type)`: no-op — trace is suppressed.
- `LogDebug(...)` (both generic and non-generic): no-op — debug is suppressed.
- `LogInformation(...)` (both generic and non-generic): delegates to `Log(LogLevel.Information, ...)`.
- `LogError(...)`: delegates to `Log(LogLevel.Error, ...)` including `Exception?`.

Interface compliance:
- All `ILogger` members are implemented, with optional parameters provided on the interface not required on the implementation signatures. Members inherited from `LoggerBase` (e.g., `IsEnabled`, `Log(LogLevel, string)`) satisfy the interface.

## Potential Issues and Warnings
- Unused parameters:
  - `type` in `LogTrace`, and `type`, `namespaceAndName`, `factory` in both `LogDebug` overloads are not used. Analyzers (e.g., IDE0060) may warn. Options:
    - Add `_ = type; _ = factory; _ = namespaceAndName;` as applicable to acknowledge parameters; or
    - Delegate to `base.Log(LogLevel.Trace|Debug, ...)` and let the base filter by level; or
    - Suppress with justification per project conventions.
- Consistency consideration:
  - `DebugLogger` delegates Debug/Information/Error to base while `InformationLogger` no-ops for Debug. If uniform routing through `LoggerBase` is desired (for consistent formatting/enrichment even when later level policies change), consider delegating Debug/Trace calls to base and rely on its level gating.
- Documentation:
  - Public API has no XML docs. If documentation is enforced, add summaries clarifying intentional suppression of Trace/Debug.

## Nullability
- Signatures match `ILogger`: non-nullable `TypeDeclarationSyntax` for Trace, nullable for non-trace overloads. Delegations pass through `null` safely to `LoggerBase`.

## Recommendations
- Either keep no-ops and add comments indicating intentional suppression (plus `_ =` for unused params), or delegate Trace/Debug to `base.Log(...)` for consistency and analyzer satisfaction.
- Optionally add XML docs summarizing behavior.

## Verdict
No functional errors. Minor warnings due to unused parameters and a consistency choice regarding whether to delegate suppressed levels to the base logger.
