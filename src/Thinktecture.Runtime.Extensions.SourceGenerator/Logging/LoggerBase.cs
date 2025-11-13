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
                      ICodeGeneratorFactory? factory = null,
                      Exception? exception = null)
   {
      try
      {
         if (type is not null)
         {
            message += $" Type: {type.Identifier.ToString()}.";
         }

         if (factory is not null)
         {
            message += $" Code Generator: {factory.CodeGeneratorName}.";
         }

         if (exception is not null)
         {
            message += $" Exception: {exception}";
         }

         _sink.Write(_source, logLevel, DateTime.Now, message);
      }
      catch (Exception ex)
      {
         SelfLog.Write(ex.ToString());
      }
   }

   public void Log(LogLevel logLevel, string message)
   {
      if (IsEnabled(logLevel))
         _sink.Write(_source, logLevel, DateTime.Now, message);
   }

   public void Dispose()
   {
      Dispose(true);
      GC.SuppressFinalize(this);
   }

   private void Dispose(bool disposing)
   {
      if (_isDisposed)
         return;

      lock (_lock)
      {
         if (_isDisposed)
            return;

         if (disposing)
            _loggerFactory.ReleaseSink(_sink);

         _isDisposed = true;
      }
   }
}
