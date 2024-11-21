using Microsoft.CodeAnalysis.CSharp.Syntax;

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
}
