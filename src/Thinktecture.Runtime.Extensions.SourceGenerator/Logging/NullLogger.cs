using Microsoft.CodeAnalysis.CSharp.Syntax;
using Thinktecture.CodeAnalysis;

namespace Thinktecture.Logging;

public class NullLogger : ILogger
{
   public static readonly ILogger Instance = new NullLogger();

   public bool IsEnabled(LogLevel logLevel)
   {
      return false;
   }

   public void Log(LogLevel logLevel, string message)
   {
   }

   public void LogTrace(string message, TypeDeclarationSyntax type)
   {
   }

   public void LogDebug(string message, TypeDeclarationSyntax? type = null, INamespaceAndName? namespaceAndName = null, ICodeGeneratorFactory? factory = null)
   {
   }

   public void LogInformation(string message, TypeDeclarationSyntax? type = null, INamespaceAndName? namespaceAndName = null, ICodeGeneratorFactory? factory = null)
   {
   }

   public void LogError(string message, TypeDeclarationSyntax? type, Exception? exception)
   {
   }

   public void Dispose()
   {
   }
}
