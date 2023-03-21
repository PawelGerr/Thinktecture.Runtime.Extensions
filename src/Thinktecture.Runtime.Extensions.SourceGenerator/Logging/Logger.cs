using Microsoft.CodeAnalysis.CSharp.Syntax;
using Thinktecture.CodeAnalysis;

namespace Thinktecture.Logging;

public abstract class LoggerBase : IDisposable
{
   private readonly LoggerFactory _loggerFactory;
   private readonly ILoggingSink _sink;
   private readonly string _source;
   private readonly LogLevel _minimumLogLevel;
   private readonly object _lock;

   private bool _isDisposed;

   protected LoggerBase(
      LoggerFactory loggerFactory,
      ILoggingSink sink,
      string source,
      LogLevel minimumLogLevel)
   {
      _loggerFactory = loggerFactory;
      _sink = sink;
      _source = source;
      _minimumLogLevel = minimumLogLevel;
      _lock = new object();
   }

   public bool IsEnabled(LogLevel logLevel)
   {
      return logLevel >= _minimumLogLevel && logLevel < LogLevel.None;
   }

   protected void Log(LogLevel logLevel,
                      string message,
                      TypeDeclarationSyntax? type,
                      INamespaceAndName? namespaceAndName = null,
                      ICodeGeneratorFactory? factory = null,
                      Exception? exception = null)
   {
      if (type is not null)
         message = $"{message}. Type: {type.Identifier}";

      if (namespaceAndName is not null)
         message = $"{message}. Type: {namespaceAndName.Namespace}.{namespaceAndName.Name}";

      if (exception is not null)
         message = $"{message}. Exception: {exception}";

      if (factory is not null)
         message = $"{message}. Code Generator: {factory.CodeGeneratorName}";

      _sink.Write(_source, logLevel, DateTime.Now, message);
   }

   public void Log(LogLevel logLevel, string message)
   {
      if (IsEnabled(logLevel))
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
      : base(loggerFactory, sink, source, LogLevel.Trace)
   {
   }

   public void LogTrace(string message, TypeDeclarationSyntax type) => Log(LogLevel.Trace, message, type: type);
   public void LogDebug(string message, TypeDeclarationSyntax? type, INamespaceAndName? namespaceAndName, ICodeGeneratorFactory? factory) => Log(LogLevel.Debug, message, type, namespaceAndName, factory);
   public void LogInformation(string message, TypeDeclarationSyntax? type, INamespaceAndName? namespaceAndName, ICodeGeneratorFactory? factory) => Log(LogLevel.Information, message, type, namespaceAndName, factory);
   public void LogError(string message, TypeDeclarationSyntax? type, Exception? exception) => Log(LogLevel.Error, message, type, exception: exception);
}

public sealed class DebugLogger : LoggerBase, ILogger
{
   public DebugLogger(LoggerFactory loggerFactory, ILoggingSink sink, string source)
      : base(loggerFactory, sink, source, LogLevel.Debug)
   {
   }

   public void LogTrace(string message, TypeDeclarationSyntax type)
   {
   }

   public void LogDebug(string message, TypeDeclarationSyntax? type, INamespaceAndName? namespaceAndName, ICodeGeneratorFactory? factory) => Log(LogLevel.Debug, message, type, namespaceAndName, factory);
   public void LogInformation(string message, TypeDeclarationSyntax? type, INamespaceAndName? namespaceAndName, ICodeGeneratorFactory? factory) => Log(LogLevel.Information, message, type, namespaceAndName, factory);
   public void LogError(string message, TypeDeclarationSyntax? type, Exception? exception) => Log(LogLevel.Error, message, type, exception: exception);
}

public sealed class InformationLogger : LoggerBase, ILogger
{
   public InformationLogger(LoggerFactory loggerFactory, ILoggingSink sink, string source)
      : base(loggerFactory, sink, source, LogLevel.Information)
   {
   }

   public void LogTrace(string message, TypeDeclarationSyntax type)
   {
   }

   public void LogDebug(string message, TypeDeclarationSyntax? type, INamespaceAndName? namespaceAndName, ICodeGeneratorFactory? factory)
   {
   }

   public void LogInformation(string message, TypeDeclarationSyntax? type, INamespaceAndName? namespaceAndName, ICodeGeneratorFactory? factory) => Log(LogLevel.Information, message, type, namespaceAndName, factory);
   public void LogError(string message, TypeDeclarationSyntax? type, Exception? exception) => Log(LogLevel.Error, message, type, exception: exception);
}

public sealed class WarningLogger : LoggerBase, ILogger
{
   public WarningLogger(LoggerFactory loggerFactory, ILoggingSink sink, string source)
      : base(loggerFactory, sink, source, LogLevel.Warning)
   {
   }

   public void LogTrace(string message, TypeDeclarationSyntax type)
   {
   }

   public void LogDebug(string message, TypeDeclarationSyntax? type, INamespaceAndName? namespaceAndName, ICodeGeneratorFactory? factory)
   {
   }

   public void LogInformation(string message, TypeDeclarationSyntax? type, INamespaceAndName? namespaceAndName, ICodeGeneratorFactory? factory)
   {
   }

   public void LogError(string message, TypeDeclarationSyntax? type, Exception? exception) => Log(LogLevel.Error, message, type, exception: exception);
}

public sealed class ErrorLogger : LoggerBase, ILogger
{
   public ErrorLogger(LoggerFactory loggerFactory, ILoggingSink sink, string source)
      : base(loggerFactory, sink, source, LogLevel.Error)
   {
   }

   public void LogTrace(string message, TypeDeclarationSyntax type)
   {
   }

   public void LogDebug(string message, TypeDeclarationSyntax? type, INamespaceAndName? namespaceAndName, ICodeGeneratorFactory? factory)
   {
   }

   public void LogInformation(string message, TypeDeclarationSyntax? type, INamespaceAndName? namespaceAndName, ICodeGeneratorFactory? factory)
   {
   }

   public void LogError(string message, TypeDeclarationSyntax? type, Exception? exception) => Log(LogLevel.Error, message, type, exception: exception);
}
