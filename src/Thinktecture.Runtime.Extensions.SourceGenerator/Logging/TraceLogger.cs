using Microsoft.CodeAnalysis.CSharp.Syntax;
using Thinktecture.CodeAnalysis;

namespace Thinktecture.Logging;

public sealed class TraceLogger : LoggerBase, ILogger
{
   public TraceLogger(LoggerFactory loggerFactory, ILoggingSink sink, string source)
      : base(loggerFactory, sink, source, LogLevel.Trace)
   {
   }

   public void LogTrace(string message, TypeDeclarationSyntax type) => Log(LogLevel.Trace, message, type: type);

   public void LogDebug(string message, TypeDeclarationSyntax? type, ICodeGeneratorFactory? factory)
   {
      Log(LogLevel.Debug, message, type, factory);
   }

   public void LogDebug<T>(string message, TypeDeclarationSyntax? type, T namespaceAndName, ICodeGeneratorFactory? factory)
      where T : INamespaceAndName
   {
      Log(LogLevel.Debug, message, type, namespaceAndName, factory);
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
