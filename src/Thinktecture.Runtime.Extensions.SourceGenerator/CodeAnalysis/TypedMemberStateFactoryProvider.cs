using System.Collections.Concurrent;
using Thinktecture.Logging;

namespace Thinktecture.CodeAnalysis;

public class TypedMemberStateFactoryProvider
{
   private static readonly object _lock7 = new();
   private static readonly object _lock8 = new();
   private static readonly object _lock9 = new();

   private static readonly Version _version7 = new(7, 0, 0, 0);
   private static readonly Version _version8 = new(8, 0, 0, 0);
   private static readonly Version _version9 = new(9, 0, 0, 0);

   private static TypedMemberStateFactory? _dotnet7;
   private static TypedMemberStateFactory? _dotnet8;
   private static TypedMemberStateFactory? _dotnet9;

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
         return GetFactory(compilation, logger, _version7, _lock7, ref _dotnet7);

      if (version == _version8)
         return GetFactory(compilation, logger, _version8, _lock8, ref _dotnet8);

      if (version == _version9)
         return GetFactory(compilation, logger, _version9, _lock9, ref _dotnet9);

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

   private static TypedMemberStateFactory GetFactory(
      Compilation compilation,
      ILogger logger,
      Version dotnetVersion,
      object lockObj,
      ref TypedMemberStateFactory? cachedFactory)
   {
      var factory = cachedFactory;

      if (factory is not null)
         return factory;

      lock (lockObj)
      {
         if (cachedFactory is not null)
            return cachedFactory;

         logger.LogDebug($"Create TypedMemberStateFactory for .NET {dotnetVersion}");

         return cachedFactory = TypedMemberStateFactory.Create(compilation);
      }
   }
}
