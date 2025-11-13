using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Thinktecture.Logging;

public sealed class SelfLogErrorLogger(string source) : ILogger
{
   public bool IsEnabled(LogLevel logLevel)
   {
      return logLevel == LogLevel.Error;
   }

   public void Log(LogLevel logLevel, string message)
   {
      if (logLevel != LogLevel.Error)
         return;

      LogError(message, null, null);
   }

   public void LogError(string message, TypeDeclarationSyntax? type, Exception? exception)
   {
      try
      {
         if (type is not null)
            message = $"{message}. Type: {type.Identifier}";

         if (exception is not null)
            message = $"{message}. Exception: {exception}";

         SelfLog.Write(DateTime.Now, nameof(LogLevel.Error), source, message);
      }
      catch (Exception ex)
      {
         SelfLog.Write(ex.ToString());
      }
   }

   public void Dispose()
   {
   }
}
