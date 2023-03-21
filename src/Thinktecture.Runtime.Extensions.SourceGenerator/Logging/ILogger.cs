using Microsoft.CodeAnalysis.CSharp.Syntax;
using Thinktecture.CodeAnalysis;

namespace Thinktecture.Logging;

public interface ILogger : IDisposable
{
   bool IsEnabled(LogLevel logLevel);
   void Log(LogLevel logLevel, string message);

   void LogTrace(string message, TypeDeclarationSyntax type);
   void LogDebug(string message, TypeDeclarationSyntax? type = null, INamespaceAndName? namespaceAndName = null, ICodeGeneratorFactory? factory = null);
   void LogInformation(string message, TypeDeclarationSyntax? type = null, INamespaceAndName? namespaceAndName = null, ICodeGeneratorFactory? factory = null);
   void LogError(string message, TypeDeclarationSyntax? type = null, Exception? exception = null);
}
