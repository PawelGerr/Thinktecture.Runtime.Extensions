using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Thinktecture.Logging;

public class NullLogger : ILogger
{
   public static readonly ILogger Instance = new NullLogger();

   public void LogTrace(string message, TypeDeclarationSyntax type)
   {
   }

   public void LogDebug(string message, TypeDeclarationSyntax type)
   {
   }

   public void LogInformation(string message)
   {
   }

   public void LogError(string message, Exception exception)
   {
   }

   public void Dispose()
   {
   }
}
