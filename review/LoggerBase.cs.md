LoggerBase.cs — Review

Summary
- Purpose: Base logger used by source generators; wraps an ILoggingSink with minimal formatting helpers and disposal logic.
- Scope reviewed: Thread-safety, filtering, message composition, nullability, time source, and disposal.

Warnings
1) Protected Log methods bypass IsEnabled filtering
- Details: Both protected Log(...) overloads call _sink.Write(...) unconditionally. Only the public Log(LogLevel, string) checks IsEnabled. Derived classes using the protected overloads may emit logs even when below the configured minimum level.
- Impact: Unnecessary computations/allocations and writes when logging is disabled; inconsistent filtering behavior across call sites.
- Recommendation: Add an IsEnabled(logLevel) check at the start of both protected overloads to short-circuit early.

2) Loss of type/factory context when exception is present
- Details: In both protected Log overloads, if exception is not null the method sets message = $"{message}. Exception: {exception}" and skips appending Type and Code Generator context.
- Impact: Loses valuable diagnostic context when logging exceptions, making it harder to correlate errors to a specific type/generator.
- Recommendation: Include both exception and type/factory context when available.

3) Nullability risk for namespaceAndName in generic overload
- Details: The generic overload expects a T : INamespaceAndName, but does not guard against null. If a null reference is passed at runtime, the else branch will throw NullReferenceException when accessing Namespace/Name.
- Impact: Possible runtime NRE depending on call sites.
- Recommendation: Add an ArgumentNullException.ThrowIfNull(namespaceAndName); or if nullable annotations are enabled, make this parameter non-nullable and validate at runtime for safety.

4) Time source uses DateTime.Now (local time)
- Details: _sink.Write(..., DateTime.Now, ...)
- Impact: Local time is sensitive to time zone/DST changes and less consistent across machines/build agents.
- Recommendation: Prefer DateTimeOffset.UtcNow (or delegate timestamping to the sink).

5) No argument validation in constructor
- Details: loggerFactory, sink, and source are stored without null/empty checks.
- Impact: Potential for later NREs or invalid state.
- Recommendation: Add null/empty validation (e.g., ArgumentNullException.ThrowIfNull(...), ArgumentException for empty source).

6) Potential thread-safety concern on sink writes
- Details: Dispose uses a lock to guard ReleaseSink once, but writes are not synchronized here.
- Impact: If the ILoggingSink implementation is not thread-safe, concurrent writes could race.
- Recommendation: Document that sinks must be thread-safe, or consider synchronization within LoggerBase (prefer documenting/thread-safe sinks to avoid contention).

No errors found
- Code compiles conceptually; disposal pattern is correct without finalizer; double-checked lock in Dispose prevents duplicate ReleaseSink.

Suggested patch (illustrative)
Note: Example only — adapt to project conventions.

protected void Log(LogLevel logLevel,
                   string message,
                   TypeDeclarationSyntax? type,
                   ICodeGeneratorFactory? factory = null,
                   Exception? exception = null)
{
   if (!IsEnabled(logLevel))
      return;

   if (type is not null)
   {
      message = factory is not null
                  ? $"{message}. Type: {type.Identifier}. Code Generator: {factory.CodeGeneratorName}."
                  : $"{message}. Type: {type.Identifier}";
   }

   if (exception is not null)
   {
      message = $"{message}. Exception: {exception}";
   }

   _sink.Write(_source, logLevel, DateTimeOffset.UtcNow, message);
}

protected void Log<T>(LogLevel logLevel,
                        string message,
                        TypeDeclarationSyntax? type,
                        T namespaceAndName,
                        ICodeGeneratorFactory? factory = null,
                        Exception? exception = null)
   where T : INamespaceAndName
{
   if (!IsEnabled(logLevel))
      return;

   if (type is not null)
   {
      message = factory is not null
                  ? $"{message}. Type: {type.Identifier}. Code Generator: {factory.CodeGeneratorName}."
                  : $"{message}. Type: {type.Identifier}";
   }
   else
   {
      ArgumentNullException.ThrowIfNull(namespaceAndName);
      message = factory is not null
                  ? $"{message}. Type: {namespaceAndName.Namespace}.{namespaceAndName.Name}. Code Generator: {factory.CodeGeneratorName}."
                  : $"{message}. Type: {namespaceAndName.Namespace}.{namespaceAndName.Name}";
   }

   if (exception is not null)
   {
      message = $"{message}. Exception: {exception}";
   }

   _sink.Write(_source, logLevel, DateTimeOffset.UtcNow, message);
}

Constructor guard example:

protected LoggerBase(LoggerFactory loggerFactory,
                     ILoggingSink sink,
                     string source,
                     LogLevel minimumLogLevel)
{
   ArgumentNullException.ThrowIfNull(loggerFactory);
   ArgumentNullException.ThrowIfNull(sink);
   ArgumentException.ThrowIfNullOrWhiteSpace(source);

   _loggerFactory = loggerFactory;
   _sink = sink;
   _source = source;
   _minimumLogLevel = minimumLogLevel;
   _lock = new object();
}
