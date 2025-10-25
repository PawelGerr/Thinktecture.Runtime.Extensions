using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Thinktecture.Logging;

public sealed class WarningLogger : LoggerBase, ILogger
{
   public WarningLogger(LoggerFactory loggerFactory, ILoggingSink sink, string source)
      : base(loggerFactory, sink, source, LogLevel.Warning)
   {
   }

   public void LogError(string message, TypeDeclarationSyntax? type, Exception? exception) => Log(LogLevel.Error, message, type, exception: exception);
}
