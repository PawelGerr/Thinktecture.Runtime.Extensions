using Microsoft.CodeAnalysis;
using Thinktecture.Logging;

namespace Thinktecture.CodeAnalysis;

public class TypedMemberStateFactoryProvider
{
   private static readonly object _lock7 = new();
   private static readonly object _lock8 = new();

   private static TypedMemberStateFactory? _dotnet7;
   private static TypedMemberStateFactory? _dotnet8;

   public static TypedMemberStateFactory? GetFactoryOrNull(
      Compilation compilation,
      ILogger logger)
   {
      var objSymbol = compilation.GetSpecialType(SpecialType.System_Object);

      if (objSymbol.TypeKind == TypeKind.Error)
         return null;

      var majorVersion = 7;

      if (objSymbol.ContainingAssembly is { Identity.Version: { } version })
         majorVersion = version.Major;

      if (majorVersion == 8)
      {
         var factory = _dotnet8;

         if (factory is not null)
            return factory;

         lock (_lock8)
         {
            if (_dotnet8 is not null)
               return _dotnet8;

            logger.LogDebug("Create TypedMemberStateFactory for .NET 8");

            return _dotnet8 = TypedMemberStateFactory.Create(compilation);
         }
      }
      else
      {
         var factory = _dotnet7;

         if (factory is not null)
            return factory;

         lock (_lock7)
         {
            if (_dotnet7 is not null)
               return _dotnet7;

            logger.LogDebug("Create TypedMemberStateFactory for .NET 7");

            return _dotnet7 = TypedMemberStateFactory.Create(compilation);
         }
      }
   }
}
