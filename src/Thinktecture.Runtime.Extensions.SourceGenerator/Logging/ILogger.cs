using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Thinktecture.Logging;

public interface ILogger : IDisposable
{
   void LogTrace(string message, TypeDeclarationSyntax type);
   void LogDebug(string message, TypeDeclarationSyntax type);
   void LogInformation(string message);
   void LogError(string message, Exception exception);
}
