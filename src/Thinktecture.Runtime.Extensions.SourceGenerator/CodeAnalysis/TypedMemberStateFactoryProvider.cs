using System.Collections.Concurrent;

namespace Thinktecture.CodeAnalysis;

public static class TypedMemberStateFactoryProvider
{
   private static Lazy<TypedMemberStateFactory>? _dotnet8;
   private static Lazy<TypedMemberStateFactory>? _dotnet9;
   private static Lazy<TypedMemberStateFactory>? _dotnet10;
   private static readonly ConcurrentDictionary<int, TypedMemberStateFactory> _factoriesByVersion = new();

   public static TypedMemberStateFactory? GetFactoryOrNull(Compilation compilation)
   {
      var objSymbol = compilation.GetSpecialType(SpecialType.System_Object);

      if (objSymbol.TypeKind == TypeKind.Error)
         return null;

      var version = 0;

      if (objSymbol.ContainingAssembly is { Identity.Version: { } dotnetVersion })
         version = dotnetVersion.Major;

      return version switch
      {
         8 => GetFactory(compilation, ref _dotnet8),
         9 => GetFactory(compilation, ref _dotnet9),
         10 => GetFactory(compilation, ref _dotnet10),
         _ => GetFactoryFromDictionary(version, compilation)
      };
   }

   private static TypedMemberStateFactory GetFactoryFromDictionary(int version, Compilation compilation)
   {
      return _factoriesByVersion.GetOrAdd(version, _ => TypedMemberStateFactory.Create(compilation));
   }

   private static TypedMemberStateFactory GetFactory(
      Compilation compilation,
      ref Lazy<TypedMemberStateFactory>? lazyFactory)
   {
      // Fast-path: already created
      var existing = Volatile.Read(ref lazyFactory);

      if (existing is not null)
         return existing.Value;

      // Create a new Lazy capturing the first compilation provided
      var newLazy = new Lazy<TypedMemberStateFactory>(
         () => TypedMemberStateFactory.Create(compilation),
         LazyThreadSafetyMode.ExecutionAndPublication);

      // Ensure only one Lazy instance wins
      existing = Interlocked.CompareExchange(ref lazyFactory, newLazy, null);

      return (existing ?? newLazy).Value;
   }
}
