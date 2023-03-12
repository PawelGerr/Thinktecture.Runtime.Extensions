using Microsoft.CodeAnalysis;
using Thinktecture.CodeAnalysis;
using Thinktecture.CodeAnalysis.SmartEnums;

namespace Thinktecture;

public static class NamedTypeSymbolExtensions
{
   public static BaseTypeState? GetBaseType(
      this INamedTypeSymbol type,
      TypedMemberStateFactory factory)
   {
      if (type.BaseType.IsNullOrObject())
         return null;

      var isSameAssembly = SymbolEqualityComparer.Default.Equals(type.ContainingAssembly, type.BaseType.ContainingAssembly);
      return new BaseTypeState(factory, type.BaseType, isSameAssembly);
   }

   public static IReadOnlyList<ConstructorState> GetConstructors(
      this INamedTypeSymbol type,
      TypedMemberStateFactory factory
   )
   {
      return type.GetConstructors(factory, type.GetBaseType(factory));
   }

   public static IReadOnlyList<ConstructorState> GetConstructors(
      this INamedTypeSymbol type,
      TypedMemberStateFactory factory,
      BaseTypeState? baseType)
   {
      var ctors = type.Constructors
                      .Where(c => c.MethodKind == MethodKind.Constructor
                                  && (c.DeclaredAccessibility is Accessibility.Protected or Accessibility.Public || (c.DeclaredAccessibility == Accessibility.Internal && baseType?.IsSameAssembly != true))
                                  && (!c.IsImplicitlyDeclared || baseType?.IsSameAssembly != true)); // default-ctor will be replaced by ctor implemented by this generator

      return ctors.Select(ctor =>
                          {
                             var parameters = ctor.Parameters
                                                  .Select(p => new DefaultMemberState(factory.Create(p.Type), p.Name, p.Name, false))
                                                  .ToList();

                             return new ConstructorState(parameters);
                          })
                  .ToList();
   }
}
