namespace Thinktecture;

public static class TypeParameterExtensions
{
   public static ImmutableArray<string> GetConstraints(this ITypeParameterSymbol typeParam)
   {
      ImmutableArray<string>.Builder? constraints = null;

      if (typeParam.HasReferenceTypeConstraint)
      {
         (constraints = ImmutableArray.CreateBuilder<string>()).Add("class");
      }
      else if (typeParam.HasValueTypeConstraint)
      {
         (constraints = ImmutableArray.CreateBuilder<string>()).Add("struct");
      }

      foreach (var constraintType in typeParam.ConstraintTypes)
      {
         (constraints ??= ImmutableArray.CreateBuilder<string>()).Add(constraintType.ToFullyQualifiedDisplayString());
      }

      if (typeParam.HasNotNullConstraint)
         (constraints ??= ImmutableArray.CreateBuilder<string>()).Add("notnull");

      if (typeParam.HasConstructorConstraint)
         (constraints ??= ImmutableArray.CreateBuilder<string>()).Add("new()");

      if (typeParam.HasUnmanagedTypeConstraint)
         (constraints ??= ImmutableArray.CreateBuilder<string>()).Add("unmanaged");

      return constraints?.DrainToImmutable() ?? [];
   }
}
