using Microsoft.CodeAnalysis.CSharp.Syntax;
using Thinktecture.CodeAnalysis;

namespace Thinktecture;

public static class MethodSymbolExtensions
{
   public static SyntaxToken GetIdentifier(this IMethodSymbol method, CancellationToken cancellationToken)
   {
      var syntax = (MethodDeclarationSyntax)method.DeclaringSyntaxReferences.Single().GetSyntax(cancellationToken);
      return syntax.Identifier;
   }

   public static bool IsComparisonOperator(this IMethodSymbol method, ITypeSymbol type)
   {
      return method.ReturnType.SpecialType == SpecialType.System_Boolean
             && method.Parameters.Length == 2
             && SymbolEqualityComparer.IncludeNullability.Equals(method.Parameters[0].Type, type)
             && SymbolEqualityComparer.IncludeNullability.Equals(method.Parameters[1].Type, type);
   }

   public static bool IsArithmeticOperator(this IMethodSymbol method, ITypeSymbol type)
   {
      return SymbolEqualityComparer.IncludeNullability.Equals(method.ReturnType, type)
             && method.Parameters.Length == 2
             && SymbolEqualityComparer.IncludeNullability.Equals(method.Parameters[0].Type, type)
             && SymbolEqualityComparer.IncludeNullability.Equals(method.Parameters[1].Type, type);
   }

   public static IReadOnlyList<GenericTypeParameterState> GetGenericTypeParameters(this IMethodSymbol method)
   {
      if (method.TypeParameters.Length <= 0)
         return [];

      var genericTypeParameters = new List<GenericTypeParameterState>(method.TypeParameters.Length);

      for (var i = 0; i < method.TypeParameters.Length; i++)
      {
         var typeParam = method.TypeParameters[i];
         var constraints = typeParam.GetConstraints();

         genericTypeParameters.Add(new GenericTypeParameterState(
                                      typeParam.Name,
                                      constraints));
      }

      return genericTypeParameters ?? [];
   }
}
