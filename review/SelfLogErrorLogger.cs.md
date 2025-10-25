SelfLogErrorLogger.cs — Review

Summary
- Purpose: Minimal logger that only emits Error-level entries to SelfLog; all other levels are effectively no-ops.
- Scope reviewed: Correctness of error logging path, completeness of ILogger implementation, time source/format consistency, nullability, API design.

Warnings
1) ILogger.Log(LogLevel, string) is a no-op
- Details: The generic Log(LogLevel, string) method is empty. If callers use the general API (e.g., Log(LogLevel.Error, ...)), errors will be dropped silently.
- Impact: Error events may be lost depending on call sites, reducing diagnosability.
- Recommendation: Implement Log to route Error-level to SelfLog (and ignore others), e.g.:
  public void Log(LogLevel logLevel, string message)
  {
     if (logLevel == LogLevel.Error)
        SelfLog.Write(DateTime.UtcNow, nameof(LogLevel.Error), _source, message);
  }

2) Local time and inconsistent timestamp format
- Details: Uses DateTime.Now and passes a DateTime to SelfLog.Write, which formats using InvariantCulture (not ISO-8601). Other paths use "O" or proposed UTC.
- Impact: Inconsistent and local timestamps complicate cross-machine debugging and parsing.
- Recommendation: Prefer DateTimeOffset.UtcNow and a consistent "O" format throughout (align with SelfLog improvements).

3) Missing type/factory context parity
- Details: LogError appends Type (identifier) and Exception but has no way to include factory/code generator context. Other loggers include factory info when available.
- Impact: Error entries from SelfLogErrorLogger may be harder to correlate to specific generators compared to other loggers.
- Recommendation: Consider an overload that accepts ICodeGeneratorFactory to include CodeGeneratorName, or centralize message composition logic to a shared helper to keep parity.

4) No argument validation
- Details: Constructor accepts source without validation; null/whitespace sources are allowed.
- Impact: Source in logs could be empty, reducing usefulness; may hide misconfiguration.
- Recommendation: Add ArgumentException.ThrowIfNullOrWhiteSpace(source).

5) No-ops for non-error level methods
- Details: Trace/Debug/Information methods do nothing. This is likely intentional to keep SelfLog quiet except for errors.
- Recommendation: Optionally document this explicitly in XML docs so consumers understand intentional no-op behavior.

6) Minor: Redundant .ToString() on type.Identifier
- Details: string interpolation will call ToString() implicitly on SyntaxToken.
- Recommendation: Use $"{type.Identifier}" for brevity.

No errors found
- Implementation compiles; class is simple and fulfills the “only error” logging design.

Suggested patch (illustrative)

public class SelfLogErrorLogger : ILogger
{
   private readonly string _source;

   public SelfLogErrorLogger(string source)
   {
      ArgumentException.ThrowIfNullOrWhiteSpace(source);
      _source = source;
   }

   public bool IsEnabled(LogLevel logLevel) => logLevel == LogLevel.Error;

   public void Log(LogLevel logLevel, string message)
   {
      if (logLevel == LogLevel.Error)
      {
         // Keep timestamp construction consistent with SelfLog
         SelfLog.Write(DateTime.UtcNow, nameof(LogLevel.Error), _source, message);
      }
   }

   public void LogTrace(string message, TypeDeclarationSyntax type) { }
   public void LogDebug(string message, TypeDeclarationSyntax? type = null, ICodeGeneratorFactory? factory = null) { }
   public void LogDebug<T>(string message, TypeDeclarationSyntax? type, T namespaceAndName, ICodeGeneratorFactory? factory = null)
      where T : INamespaceAndName { }
   public void LogInformation(string message, TypeDeclarationSyntax? type = null, ICodeGeneratorFactory? factory = null) { }
   public void LogInformation<T>(string message, TypeDeclarationSyntax? type, T namespaceAndName, ICodeGeneratorFactory? factory = null)
      where T : INamespaceAndName { }

   public void LogError(string message, TypeDeclarationSyntax? type, Exception? exception)
   {
      if (type is not null)
         message = $"{message}. Type: {type.Identifier}";

      if (exception is not null)
         message = $"{message}. Exception: {exception}";

      // Prefer UTC; adjust SelfLog signature if updated to DateTimeOffset
      SelfLog.Write(DateTime.UtcNow, nameof(LogLevel.Error), _source, message);
   }

   public void Dispose() { }
}

Optional improvements
- Consider making the class sealed and internal if not intended for extension or public consumption.
- If parity with other loggers is important, add overloads for namespace/name and factory in LogError for richer context.
