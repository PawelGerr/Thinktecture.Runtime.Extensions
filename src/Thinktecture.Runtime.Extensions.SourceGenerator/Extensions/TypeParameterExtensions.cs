namespace Thinktecture;

public static class TypeParameterExtensions
{
   public static IReadOnlyList<string> GetConstraints(this ITypeParameterSymbol typeParam)
   {
      List<string>? constraints = null;

      if (typeParam.HasReferenceTypeConstraint)
      {
         (constraints ??= []).Add("class");
      }
      else if (typeParam.HasValueTypeConstraint)
      {
         (constraints ??= []).Add("struct");
      }

      foreach (var constraintType in typeParam.ConstraintTypes)
      {
         (constraints ??= []).Add(constraintType.ToFullyQualifiedDisplayString());
      }

      if (typeParam.HasConstructorConstraint)
         (constraints ??= []).Add("new()");

      return constraints ?? [];
   }
}
