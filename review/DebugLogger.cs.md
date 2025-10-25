# Review: Logging/DebugLogger.cs

Path: `src/Thinktecture.Runtime.Extensions.SourceGenerator/Logging/DebugLogger.cs`

## Summary
`DebugLogger` is a sealed logger implementation that inherits `LoggerBase` and implements `ILogger`. It sets its minimum level to `LogLevel.Debug` via the base constructor and delegates Debug/Information/Error to `LoggerBase.Log`. `LogTrace` is intentionally a no-op.

## API and Behavior
- Constructor: `DebugLogger(LoggerFactory, ILoggingSink, string source)` calls `base(..., LogLevel.Debug)` which establishes the effective minimum level for this logger instance.
- `LogTrace(string message, TypeDeclarationSyntax type)`: empty method body — trace messages are ignored by this logger.
- `LogDebug(...)` and `LogInformation(...)`: forward to `Log(LogLevel.Debug|Information, ...)` on `LoggerBase`.
- Generic overloads for Debug/Information accept `T : INamespaceAndName` to include symbol identity context. The calls forward to base with the same constraints.
- `LogError(...)`: forwards to `Log(LogLevel.Error, ...)` including the `Exception?`.

Interface compliance:
- Implements all members of `ILogger`. Optional parameters on the interface are compile-time features and not required on the implementing method; the provided signatures are compatible.

## Potential Issues and Warnings
- Behavior consistency: `LogTrace` is a no-op while other levels delegate to `LoggerBase.Log`. If the intent is to completely suppress trace logs for this logger type, this is correct. If the intent is to let the base decide (e.g., to allow sinks with independent level gating), consider delegating to `base.Log(LogLevel.Trace, ...)` for consistency.
- Unused parameter: `type` in `LogTrace` is not used. Some analyzers (e.g., IDE0060) may flag “Remove unused parameter.” Because the signature is dictated by the interface, removal is not possible. If analyzer noise is a concern:
  - Add `_ = type;` to explicitly acknowledge the parameter; or
  - Call into `base.Log(LogLevel.Trace, ...)` as suggested above; or
  - Add a justification comment or suppression attribute as per project conventions.
- Documentation: Public API lacks XML doc comments. If the project enforces documentation, consider adding short summaries clarifying that this logger deliberately suppresses trace logs.
- Nullability: Signatures align with the interface (`TypeDeclarationSyntax?` for non-trace overloads; non-nullable for `LogTrace`). Passing through `null` where allowed is handled by `LoggerBase`.

## Recommendations
- Decide on the intended semantics for trace in `DebugLogger` and either:
  - Keep as no-op and add a brief comment, e.g., “Debug logger intentionally ignores trace-level messages,” and optionally `_ = type;` to avoid IDE0060; or
  - Delegate to `base.Log(LogLevel.Trace, message, type)` for uniform handling (base will filter by level anyway).
- Consider adding minimal XML docs to clarify intended filtering.
- No functional errors detected.

## Verdict
No errors. One minor warning around an unused parameter and a small consistency consideration for `LogTrace` handling. The implementation is otherwise correct and consistent with the logging design used across the project.
