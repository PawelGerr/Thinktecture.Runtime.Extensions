using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Thinktecture.Logging;

public sealed class InformationLogger : LoggerBase, ILogger
{
   public InformationLogger(LoggerFactory loggerFactory, ILoggingSink sink, string source)
      : base(loggerFactory, sink, source, LogLevel.Information)
   {
   }

   public void LogError(string message, TypeDeclarationSyntax? type, Exception? exception) => Log(LogLevel.Error, message, type, exception: exception);
}
