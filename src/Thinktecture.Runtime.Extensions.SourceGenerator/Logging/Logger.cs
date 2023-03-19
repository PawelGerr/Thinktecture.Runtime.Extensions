using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Thinktecture.Logging;

public abstract class LoggerBase : IDisposable
{
   private readonly LoggerFactory _loggerFactory;
   private readonly ILoggingSink _sink;
   private readonly string _source;
   private readonly object _lock;

   private bool _isDisposed;

   protected LoggerBase(LoggerFactory loggerFactory, ILoggingSink sink, string source)
   {
      _loggerFactory = loggerFactory;
      _sink = sink;
      _source = source;
      _lock = new object();
   }

   protected void Log(LogLevel logLevel,
                      string message,
                      TypeDeclarationSyntax? type = null,
                      Exception? exception = null)
   {
      if (type is not null)
         message = $"{message}. Type: {type.Identifier}";

      if (exception is not null)
         message = $"{message}. Exception: {exception}";

      _sink.Write(_source, logLevel, DateTime.Now, message);
   }

   public void Dispose()
   {
      if (_isDisposed)
         return;

      lock (_lock)
      {
         if (_isDisposed)
            return;

         _isDisposed = true;

         _loggerFactory.ReleaseSink(_sink);
      }
   }
}

public sealed class TraceLogger : LoggerBase, ILogger
{
   public TraceLogger(LoggerFactory loggerFactory, ILoggingSink sink, string source)
      : base(loggerFactory, sink, source)
   {
   }

   public void LogTrace(string message, TypeDeclarationSyntax type) => Log(LogLevel.Trace, message, type);
   public void LogDebug(string message, TypeDeclarationSyntax type) => Log(LogLevel.Debug, message, type);
   public void LogInformation(string message) => Log(LogLevel.Information, message);
   public void LogError(string message, Exception exception) => Log(LogLevel.Error, message, exception: exception);
}

public sealed class DebugLogger : LoggerBase, ILogger
{
   public DebugLogger(LoggerFactory loggerFactory, ILoggingSink sink, string source)
      : base(loggerFactory, sink, source)
   {
   }

   public void LogTrace(string message, TypeDeclarationSyntax type)
   {
   }

   public void LogDebug(string message, TypeDeclarationSyntax type) => Log(LogLevel.Debug, message, type);
   public void LogInformation(string message) => Log(LogLevel.Information, message);
   public void LogError(string message, Exception exception) => Log(LogLevel.Error, message, exception: exception);
}

public sealed class InformationLogger : LoggerBase, ILogger
{
   public InformationLogger(LoggerFactory loggerFactory, ILoggingSink sink, string source)
      : base(loggerFactory, sink, source)
   {
   }

   public void LogTrace(string message, TypeDeclarationSyntax type)
   {
   }

   public void LogDebug(string message, TypeDeclarationSyntax type)
   {
   }

   public void LogInformation(string message) => Log(LogLevel.Information, message);
   public void LogError(string message, Exception exception) => Log(LogLevel.Error, message, exception: exception);
}

public sealed class WarningLogger : LoggerBase, ILogger
{
   public WarningLogger(LoggerFactory loggerFactory, ILoggingSink sink, string source)
      : base(loggerFactory, sink, source)
   {
   }

   public void LogTrace(string message, TypeDeclarationSyntax type)
   {
   }

   public void LogDebug(string message, TypeDeclarationSyntax type)
   {
   }

   public void LogInformation(string message)
   {
   }

   public void LogError(string message, Exception exception) => Log(LogLevel.Error, message, exception: exception);
}

public sealed class ErrorLogger : LoggerBase, ILogger
{
   public ErrorLogger(LoggerFactory loggerFactory, ILoggingSink sink, string source)
      : base(loggerFactory, sink, source)
   {
   }

   public void LogTrace(string message, TypeDeclarationSyntax type)
   {
   }

   public void LogDebug(string message, TypeDeclarationSyntax type)
   {
   }

   public void LogInformation(string message)
   {
   }

   public void LogError(string message, Exception exception) => Log(LogLevel.Error, message, exception: exception);
}
