using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Thinktecture.Logging;

public interface ILogger : IDisposable
{
   bool IsEnabled(LogLevel logLevel);
   void Log(LogLevel logLevel, string message);
   void LogError(string message, TypeDeclarationSyntax? type = null, Exception? exception = null);
}
