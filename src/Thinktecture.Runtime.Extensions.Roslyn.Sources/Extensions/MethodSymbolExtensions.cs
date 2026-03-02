namespace Thinktecture;

public static class MethodSymbolExtensions
{
   public static bool IsUserDefinedComparisonOperator(this IMethodSymbol method, ITypeSymbol type)
   {
      return method is { MethodKind: MethodKind.UserDefinedOperator, IsStatic: true, ReturnType.SpecialType: SpecialType.System_Boolean, Parameters.Length: 2 }
             && method.Parameters[0] is { RefKind: RefKind.None } firstParam
             && method.Parameters[1] is { RefKind: RefKind.None } secondParam
             && SymbolEqualityComparer.Default.Equals(firstParam.Type, type)
             && SymbolEqualityComparer.Default.Equals(secondParam.Type, type);
   }

   public static bool IsUserDefinedArithmeticOperator(this IMethodSymbol method, ITypeSymbol type)
   {
      return method is { MethodKind: MethodKind.UserDefinedOperator, IsStatic: true, Parameters.Length: 2 }
             && SymbolEqualityComparer.Default.Equals(method.ReturnType, type)
             && method.Parameters[0] is { RefKind: RefKind.None } firstParam
             && method.Parameters[1] is { RefKind: RefKind.None } secondParam
             && SymbolEqualityComparer.Default.Equals(firstParam.Type, type)
             && SymbolEqualityComparer.Default.Equals(secondParam.Type, type);
   }
}
