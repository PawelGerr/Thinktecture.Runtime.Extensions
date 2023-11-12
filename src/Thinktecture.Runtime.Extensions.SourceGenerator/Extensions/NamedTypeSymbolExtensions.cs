using Thinktecture.CodeAnalysis;
using Thinktecture.CodeAnalysis.SmartEnums;

namespace Thinktecture;

public static class NamedTypeSymbolExtensions
{
   public static BaseTypeState? GetBaseType(
      this INamedTypeSymbol type,
      TypedMemberStateFactory factory)
   {
      if (type.BaseType.IsNullOrObject() || type.BaseType.Kind == SymbolKind.ErrorType)
         return null;

      var isSameAssembly = SymbolEqualityComparer.Default.Equals(type.ContainingAssembly, type.BaseType.ContainingAssembly);
      return new BaseTypeState(factory, type.BaseType, isSameAssembly);
   }

   public static IReadOnlyList<ConstructorState> GetConstructors(
      this INamedTypeSymbol type,
      TypedMemberStateFactory factory)
   {
      return type.GetConstructors(factory, type.GetBaseType(factory));
   }

   private static IReadOnlyList<ConstructorState> GetConstructors(
      this INamedTypeSymbol type,
      TypedMemberStateFactory factory,
      BaseTypeState? baseType)
   {
      if (type.Constructors.IsDefaultOrEmpty)
         return Array.Empty<ConstructorState>();

      List<ConstructorState>? ctorStates = null;

      for (var i = 0; i < type.Constructors.Length; i++)
      {
         var ctor = type.Constructors[i];

         if (ctor.MethodKind == MethodKind.Constructor
             && (ctor.DeclaredAccessibility is Accessibility.Protected or Accessibility.Public || (ctor.DeclaredAccessibility == Accessibility.Internal && baseType?.IsSameAssembly != true))
             && (!ctor.IsImplicitlyDeclared || baseType?.IsSameAssembly != true)) // default-ctor will be replaced by ctor implemented by this generator
         {
            var parameters = ctor.Parameters.IsDefaultOrEmpty
                                ? ImmutableArray<DefaultMemberState>.Empty
                                : ImmutableArray.CreateRange(ctor.Parameters, static (p, f) => new DefaultMemberState(f.Create(p.Type), p.Name, new ArgumentName(p.Name, p.Name)), factory);

            var ctorState = new ConstructorState(parameters);
            (ctorStates ??= new List<ConstructorState>()).Add(ctorState);
         }
      }

      return ctorStates ?? (IReadOnlyList<ConstructorState>)Array.Empty<ConstructorState>();
   }
}
