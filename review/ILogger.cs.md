# Review: src/Thinktecture.Runtime.Extensions.SourceGenerator/Logging/ILogger.cs

Status: Reviewed

Summary:
Public logging abstraction for the source generator. Exposes level checks, a generic Log method, and convenience methods for Trace/Debug/Information/Error with Roslyn context (TypeDeclarationSyntax) and optional CodeGen factory context. Provides generic overloads for Debug/Information that accept an INamespaceAndName to enrich messages. IDisposable indicates ownership of sinks/resources.

API:
- bool IsEnabled(LogLevel logLevel)
- void Log(LogLevel logLevel, string message)
- void LogTrace(string message, TypeDeclarationSyntax type)
- void LogDebug(string message, TypeDeclarationSyntax? type = null, ICodeGeneratorFactory? factory = null)
- void LogInformation(string message, TypeDeclarationSyntax? type = null, ICodeGeneratorFactory? factory = null)
- void LogError(string message, TypeDeclarationSyntax? type = null, Exception? exception = null)
- void LogDebug<T>(string message, TypeDeclarationSyntax? type, T namespaceAndName, ICodeGeneratorFactory? factory = null) where T : INamespaceAndName
- void LogInformation<T>(string message, TypeDeclarationSyntax? type, T namespaceAndName, ICodeGeneratorFactory? factory = null) where T : INamespaceAndName

Strengths:
- Roslyn-aware context: accepts TypeDeclarationSyntax to tie log entries to a concrete type, useful for mapping back to source during diagnostics.
- Enrichment hooks: factory and namespace/name allow consistent enrichment of log records across the pipeline.
- IsEnabled guard available for expensive message creation or conditional logging.
- IDisposable suggests implementations can manage file handles/sinks and flush/cleanup deterministically.

Issues, Risks, and Suggestions:

1) Completeness and parity with LogLevel
- There is no LogWarning(...) convenience API despite Warning being present in LogLevel.
- LogTrace requires a non-null type whereas Debug/Information accept optional type. This asymmetry may be intentional, but reads inconsistent.
- Critical level is absent in LogLevel; consequently, no LogCritical. If adapters to Microsoft.Extensions.Logging are planned, consider parity.
Recommendations:
- Add LogWarning(...) overloads mirroring Debug/Information signatures.
- Decide on Trace’s required Type parameter and document rationale or align signatures.
- Reconsider LogLevel alignment with MEL (add Critical and shift None) or document mapping expectations.

2) Structured logging and message formatting
- All methods take string message only. For high-volume code generation workflows, consider:
  - Interpolated string handler support to avoid allocation when disabled (like MEL’s LoggerMessage pattern).
  - Overloads accepting Exception for Debug/Information as well (currently only Error has Exception).
  - Key-value pairs or state objects to enable structured sinks (e.g., message template + args).
Examples (optional future API):
```
void Log(LogLevel level, [InterpolatedStringHandlerArgument("level")] ref GeneratorLogHandler handler);

void LogInformation(string message, Exception? ex = null, TypeDeclarationSyntax? type = null, ICodeGeneratorFactory? factory = null);
```

3) Thread-safety contract
- Interface doesn’t specify thread-safety. Source generators can run in parallel; implementations likely need to be thread-safe.
Action:
- Document that ILogger implementations must be thread-safe, or scope instances per-thread and document lifetime semantics.

4) Disposal semantics
- IDisposable is good, but usage patterns should be documented: who owns the lifetime, when to dispose (end of generation, end of build?), and whether Dispose flushes buffered logs.
Action:
- Add XML docs clarifying ownership and disposal responsibility.

5) Generic overload consistency and symmetry
- Debug/Information have generic overloads with INamespaceAndName. Consider:
  - Adding the same for Trace/Error for completeness, if useful.
  - Alternatively, document why only Debug/Information need the generic overloads (e.g., verbosity vs. errors carry exception and fewer enrichment needs).

6) Public vs internal
- If ILogger is purely an internal abstraction for the generator infrastructure, consider making it internal to avoid exposing a second “LogLevel/ILogger” into consumers’ public surface, which may conflict or confuse alongside Microsoft.Extensions.Logging.
- If public by design, add XML docs and clarify how this logger relates to external frameworks and adapters.

7) Naming conflicts and adapter guidance
- With two LogLevel types (Thinktecture.Logging and MEL), consuming code may face ambiguity. Provide guidance or namespaces/aliases in docs and ensure any adapter maps values explicitly.

8) Minor nullability/documentation improvements
- Ensure nullable annotations reflect guarantees, e.g., message must be non-null.
- Provide XML docs for parameters (type, factory, namespaceAndName) to ensure consistent enrichment usage across implementations.

Overall Assessment:
- Practical, Roslyn-aware logging abstraction suitable for generator diagnostics. To harden the design: fill the Warning gap, decide on MEL parity (including Critical), clarify thread-safety/disposal expectations, and consider structured logging or interpolated string handlers for performance. Document the enrichment fields and when to use which overload to keep logs consistent across the codebase.
