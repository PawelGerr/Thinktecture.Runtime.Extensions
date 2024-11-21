using Microsoft.CodeAnalysis.CSharp.Syntax;
using Thinktecture.CodeAnalysis;

namespace Thinktecture.Logging;

public sealed class InformationLogger : LoggerBase, ILogger
{
   public InformationLogger(LoggerFactory loggerFactory, ILoggingSink sink, string source)
      : base(loggerFactory, sink, source, LogLevel.Information)
   {
   }

   public void LogTrace(string message, TypeDeclarationSyntax type)
   {
   }

   public void LogDebug(string message, TypeDeclarationSyntax? type, ICodeGeneratorFactory? factory)
   {
   }

   public void LogDebug<T>(string message, TypeDeclarationSyntax? type, T namespaceAndName, ICodeGeneratorFactory? factory)
      where T : INamespaceAndName
   {
   }

   public void LogInformation(string message, TypeDeclarationSyntax? type, ICodeGeneratorFactory? factory)
   {
      Log(LogLevel.Information, message, type, factory);
   }

   public void LogInformation<T>(string message, TypeDeclarationSyntax? type, T namespaceAndName, ICodeGeneratorFactory? factory)
      where T : INamespaceAndName
   {
      Log(LogLevel.Information, message, type, namespaceAndName, factory);
   }

   public void LogError(string message, TypeDeclarationSyntax? type, Exception? exception) => Log(LogLevel.Error, message, type, exception: exception);
}
