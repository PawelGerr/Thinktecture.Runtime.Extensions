using Microsoft.CodeAnalysis.CSharp.Syntax;
using Thinktecture.CodeAnalysis;

namespace Thinktecture.Logging;

public interface ILogger : IDisposable
{
   bool IsEnabled(LogLevel logLevel);
   void Log(LogLevel logLevel, string message);
   void LogTrace(string message, TypeDeclarationSyntax type);
   void LogDebug(string message, TypeDeclarationSyntax? type = null, ICodeGeneratorFactory? factory = null);
   void LogInformation(string message, TypeDeclarationSyntax? type = null, ICodeGeneratorFactory? factory = null);
   void LogError(string message, TypeDeclarationSyntax? type = null, Exception? exception = null);

   void LogDebug<T>(string message, TypeDeclarationSyntax? type, T namespaceAndName, ICodeGeneratorFactory? factory = null)
      where T : INamespaceAndName;

   void LogInformation<T>(string message, TypeDeclarationSyntax? type, T namespaceAndName, ICodeGeneratorFactory? factory = null)
      where T : INamespaceAndName;
}
