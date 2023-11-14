using System.Collections.Concurrent;
using Thinktecture.Logging;

namespace Thinktecture.CodeAnalysis;

public class TypedMemberStateFactoryProvider
{
   private static readonly object _lock7 = new();
   private static readonly object _lock8 = new();

   private static Version _version7 = new(7, 0, 0, 0);
   private static Version _version8 = new(8, 0, 0, 0);

   private static TypedMemberStateFactory? _dotnet7;
   private static TypedMemberStateFactory? _dotnet8;

   private static readonly ConcurrentDictionary<Version, TypedMemberStateFactory> _factoriesByVersion = new();

   public static TypedMemberStateFactory? GetFactoryOrNull(
      Compilation compilation,
      ILogger logger)
   {
      var objSymbol = compilation.GetSpecialType(SpecialType.System_Object);

      if (objSymbol.TypeKind == TypeKind.Error)
         return null;

      Version? version = null;

      if (objSymbol.ContainingAssembly is { Identity.Version: { } dotnetVersion })
         version = dotnetVersion;

      if (version == _version7)
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

      if (version == _version8)
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

      if (version is null)
      {
         logger.LogError("Could not determine current .NET version");

         return TypedMemberStateFactory.Create(compilation);
      }

      return _factoriesByVersion.GetOrAdd(version, _ =>
                                                   {
                                                      logger.LogError($"Unexpected .NET version '{version}'");

                                                      return TypedMemberStateFactory.Create(compilation);
                                                   });
   }
}
