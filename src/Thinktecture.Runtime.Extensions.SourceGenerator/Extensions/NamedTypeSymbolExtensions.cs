using Thinktecture.CodeAnalysis;
using Thinktecture.CodeAnalysis.SmartEnums;

namespace Thinktecture;

public static class NamedTypeSymbolExtensions
{
   public static BaseTypeState? GetBaseType(
      this INamedTypeSymbol type,
      TypedMemberStateFactory factory)
   {
      if (type.BaseType.IsNullOrDotnetBaseType() || type.BaseType.Kind == SymbolKind.ErrorType)
         return null;

      var constructors = type.BaseType.GetAccessibleConstructors(type, factory);
      return new BaseTypeState(constructors);
   }

#pragma warning disable CA1859
   private static IReadOnlyList<ConstructorState> GetAccessibleConstructors(
      this INamedTypeSymbol type,
      INamedTypeSymbol derivedType,
      TypedMemberStateFactory factory)
#pragma warning restore CA1859
   {
      if (type.Constructors.IsDefaultOrEmpty)
         return [];

      var isSameAssembly = SymbolEqualityComparer.Default.Equals(type.ContainingAssembly, derivedType.ContainingAssembly);

      bool? isNestedInside = null;
      bool GetIsNestedInside() => isNestedInside ??= derivedType.IsNestedInside(type);

      bool? hasInternalsVisibleTo = null;
      bool GetHasInternalsVisibleTo() => hasInternalsVisibleTo ??= type.ContainingAssembly.HasInternalsVisibleToFor(derivedType.ContainingAssembly);

      List<ConstructorState>? ctorStates = null;

      for (var i = 0; i < type.Constructors.Length; i++)
      {
         var ctor = type.Constructors[i];

         if (ctor.DeclaredAccessibility is Accessibility.Protected or Accessibility.Public or Accessibility.ProtectedOrInternal // always accessible
             || (ctor.DeclaredAccessibility is Accessibility.Internal or Accessibility.ProtectedAndInternal && (isSameAssembly || GetHasInternalsVisibleTo()))
             || (ctor.DeclaredAccessibility is Accessibility.Private && GetIsNestedInside()) // private but derived type is nested inside
            )
         {
            var parameters = ctor.Parameters.IsDefaultOrEmpty
                                ? ImmutableArray<DefaultMemberState>.Empty
                                : ImmutableArray.CreateRange(ctor.Parameters, static (p, f) => new DefaultMemberState(f.Create(p.Type), p.Name, ArgumentName.Create(p.Name, renderAsIs: true)), factory);

            var ctorState = new ConstructorState(parameters);
            (ctorStates ??= []).Add(ctorState);
         }
      }

      return ctorStates ?? (IReadOnlyList<ConstructorState>)[];
   }

   private static bool IsNestedInside(this INamedTypeSymbol nestedType, INamedTypeSymbol type)
   {
      var containingType = nestedType.ContainingType;

      while (containingType != null)
      {
         if (SymbolEqualityComparer.Default.Equals(containingType, type))
         {
            return true;
         }

         containingType = containingType.ContainingType;
      }

      return false;
   }

   public static ImmutableArray<GenericTypeParameterState> GetGenericTypeParameters(this INamedTypeSymbol type)
   {
      return type.TypeParameters.GetGenericTypeParameters();
   }

   public static ImmutableArray<ContainingTypeState> GetContainingTypes(
      this INamedTypeSymbol type)
   {
      if (type.ContainingType is null)
         return [];

      var types = ImmutableArray.CreateBuilder<ContainingTypeState>();
      var containingType = type.ContainingType;

      while (containingType != null)
      {
         var typeState = new ContainingTypeState(
            containingType.Name,
            containingType.IsReferenceType,
            containingType.IsRecord,
            containingType.GetGenericTypeParameters());
         types.Add(typeState);
         containingType = containingType.ContainingType;
      }

      types.Reverse();

      return types.DrainToImmutable();
   }

   public static bool IsNestedInGenericClass(this INamedTypeSymbol type)
   {
      var containingType = type.ContainingType;

      while (containingType is not null)
      {
         if (containingType.Arity > 0)
            return true;

         containingType = containingType.ContainingType;
      }

      return false;
   }

   public static bool HasLowerAccessibility(
      this INamedTypeSymbol type,
      Accessibility accessibility,
      INamedTypeSymbol stopType)
   {
      var containingType = type;

      while (containingType is not null
             && !SymbolEqualityComparer.Default.Equals(containingType, stopType))
      {
         if (containingType.DeclaredAccessibility < accessibility)
            return true;

         containingType = containingType.ContainingType;
      }

      return false;
   }
}
