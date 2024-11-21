using Microsoft.CodeAnalysis.CSharp.Syntax;
using Thinktecture.CodeAnalysis;

namespace Thinktecture.Logging;

public class SelfLogErrorLogger : ILogger
{
   private readonly string _source;

   public SelfLogErrorLogger(string source)
   {
      _source = source;
   }

   public bool IsEnabled(LogLevel logLevel)
   {
      return logLevel == LogLevel.Error;
   }

   public void Log(LogLevel logLevel, string message)
   {
   }

   public void LogTrace(string message, TypeDeclarationSyntax type)
   {
   }

   public void LogDebug(string message, TypeDeclarationSyntax? type = null, ICodeGeneratorFactory? factory = null)
   {
   }

   public void LogDebug<T>(string message, TypeDeclarationSyntax? type, T namespaceAndName, ICodeGeneratorFactory? factory = null)
      where T : INamespaceAndName
   {
   }

   public void LogInformation(string message, TypeDeclarationSyntax? type = null, ICodeGeneratorFactory? factory = null)
   {
   }

   public void LogInformation<T>(string message, TypeDeclarationSyntax? type, T namespaceAndName, ICodeGeneratorFactory? factory = null)
      where T : INamespaceAndName
   {
   }

   public void LogError(string message, TypeDeclarationSyntax? type, Exception? exception)
   {
      if (type is not null)
         message = $"{message}. Type: {type.Identifier.ToString()}";

      if (exception is not null)
         message = $"{message}. Exception: {exception}";

      SelfLog.Write(DateTime.Now, nameof(LogLevel.Error), _source, message);
   }

   public void Dispose()
   {
   }
}
