# Review: src/Thinktecture.Runtime.Extensions.SourceGenerator/Logging/ILoggingSink.cs

Status: Reviewed

Summary:
Defines a minimal sink abstraction to persist log events:
- void Write(string source, LogLevel logLevel, DateTime datetime, string message);

Intended to be implemented by concrete sinks (file system, debug output, etc.) behind the generator’s logging pipeline.

Strengths:
- Small and focused contract that is easy to implement.
- Includes a “source” parameter to categorize the origin (e.g., logger name or component), improving log filtering.
- Provides timestamp and level, so sinks don’t need to add these themselves.

Issues, Risks, and Suggestions:

1) DateTime vs DateTimeOffset and Kind semantics
- DateTime lacks offset/time zone context; consumers may misinterpret local vs UTC times.
- If logs may be correlated across machines/processes, prefer DateTimeOffset or enforce UTC.
Recommendations:
- Either change to DateTimeOffset timestamp, or document that datetime must be UTC (DateTime.Kind == Utc) and enforce/normalize in the logger before calling sinks.

2) Structured logging extensibility
- Only a single formatted message string is provided; no key-value state, exception, or scope data.
- If you need richer logs or downstream structured sinks (e.g., JSON), consider extending the contract.
Options:
- Add optional Exception? exception parameter.
- Add IReadOnlyList<KeyValuePair<string, object?>>? properties or a small LogEvent type carrying message template, args, and properties.
- Alternatively, keep sink minimal and do enrichment/formatting in the logger, passing already-rendered text.

3) Consistency with LogLevel design
- Current LogLevel enum omits Critical and assigns None = 5 (diverges from MEL). Any adapter sink mapping to Microsoft.Extensions.Logging must explicitly translate.
Action:
- If MEL parity is desired long-term, align values in LogLevel; otherwise, ensure all sinks performing cross-framework mapping use a switch with tests.

4) Thread-safety and async I/O
- Interface doesn’t state thread-safety. Generators can run work in parallel; implementations should be safe for concurrent Write calls.
- Synchronous Write may block on I/O; consider batching or async paths in implementations.
Recommendations:
- Document that Write must be thread-safe.
- Optionally introduce an async variant:
  - ValueTask WriteAsync(string source, LogLevel level, DateTimeOffset timestamp, string message, CancellationToken ct = default);
  Keep the sync method for simple sinks.

5) Source semantics and validation
- Clarify what “source” represents (logger name, category, generator/subsystem) and whether null/empty is allowed.
- Consider using string? plus docs, or non-nullable with argument validation higher up. Add XML docs.

6) Culture and formatting guidance
- If the message contains formatted values, specify recommended culture (e.g., invariant for machine-readable logs).
- If sinks format datetime themselves, define the standard format (ISO 8601) to ensure consistent output.

7) Newline and injection handling
- Messages may contain newlines; file sinks should normalize line breaks and prevent partial record interleaving in concurrent scenarios.
- Consider documenting that message may be multi-line and sinks must write atomically per record when possible.

8) Public vs internal surface
- If this is internal plumbing for the generator package, consider internal to avoid expanding public API. If public by design, add XML docs to reduce misuse.

Suggested improved signature (optional, if broadening scope in future):
```
public interface ILoggingSink
{
    void Write(
        string source,
        LogLevel level,
        DateTimeOffset timestamp,
        string message,
        Exception? exception = null,
        IReadOnlyList<KeyValuePair<string, object?>>? properties = null);
}
```

Overall Assessment:
- Minimal and adequate for simple text sinks. To improve interoperability and reliability: prefer DateTimeOffset or enforce UTC, document thread-safety, define source semantics, and consider optional exception/properties for structured logging scenarios. If staying minimal by design, ensure the higher-level logger performs normalization and enrichment before invoking the sink.
