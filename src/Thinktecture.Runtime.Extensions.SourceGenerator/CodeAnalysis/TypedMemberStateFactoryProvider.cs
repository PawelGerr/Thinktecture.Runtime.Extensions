namespace Thinktecture.CodeAnalysis;

public class TypedMemberStateFactoryProvider
{
   private static readonly object _lock7 = new();
   private static readonly object _lock8 = new();
   private static readonly object _lock9 = new();
   private static readonly object _lock10 = new();
   private static readonly object _lock = new();

   private static TypedMemberStateFactory? _dotnet7;
   private static TypedMemberStateFactory? _dotnet8;
   private static TypedMemberStateFactory? _dotnet9;
   private static TypedMemberStateFactory? _dotnet10;

   private static readonly Dictionary<int, TypedMemberStateFactory> _factoriesByVersion = new();

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
         7 => GetFactory(compilation, _lock7, ref _dotnet7),
         8 => GetFactory(compilation, _lock8, ref _dotnet8),
         9 => GetFactory(compilation, _lock9, ref _dotnet9),
         10 => GetFactory(compilation, _lock10, ref _dotnet10),
         _ => GetFactoryFromDictionary(version, compilation)
      };
   }

   private static TypedMemberStateFactory GetFactoryFromDictionary(int version, Compilation compilation)
   {
      lock (_lock)
      {
         if (_factoriesByVersion.TryGetValue(version, out var factory))
            return factory;

         factory = TypedMemberStateFactory.Create(compilation);
         _factoriesByVersion.Add(version, factory);

         return factory;
      }
   }

   private static TypedMemberStateFactory GetFactory(
      Compilation compilation,
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

         return cachedFactory = TypedMemberStateFactory.Create(compilation);
      }
   }
}
