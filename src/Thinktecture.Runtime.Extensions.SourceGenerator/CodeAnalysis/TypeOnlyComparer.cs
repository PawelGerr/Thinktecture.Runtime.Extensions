namespace Thinktecture.CodeAnalysis;

public class TypeOnlyComparer
   : IEqualityComparer<FormattableGeneratorState>,
     IEqualityComparer<ComparableGeneratorState>,
     IEqualityComparer<ParsableGeneratorState>,
     IEqualityComparer<ComparisonOperatorsGeneratorState>,
     IEqualityComparer<EqualityComparisonOperatorsGeneratorState>,
     IEqualityComparer<OperatorsGeneratorState>
{
   public static readonly TypeOnlyComparer Instance = new();

   public bool Equals(FormattableGeneratorState x, FormattableGeneratorState y) => x.Type.TypeFullyQualified == y.Type.TypeFullyQualified;
   public bool Equals(ComparableGeneratorState x, ComparableGeneratorState y) => x.Type.TypeFullyQualified == y.Type.TypeFullyQualified;
   public bool Equals(ParsableGeneratorState x, ParsableGeneratorState y) => x.Type.TypeFullyQualified == y.Type.TypeFullyQualified;
   public bool Equals(ComparisonOperatorsGeneratorState x, ComparisonOperatorsGeneratorState y) => x.Type.TypeFullyQualified == y.Type.TypeFullyQualified;
   public bool Equals(EqualityComparisonOperatorsGeneratorState x, EqualityComparisonOperatorsGeneratorState y) => x.Type.TypeFullyQualified == y.Type.TypeFullyQualified;
   public bool Equals(OperatorsGeneratorState x, OperatorsGeneratorState y) => x.Type.TypeFullyQualified == y.Type.TypeFullyQualified;

   public int GetHashCode(FormattableGeneratorState obj) => obj.Type.TypeFullyQualified.GetHashCode();
   public int GetHashCode(ComparableGeneratorState obj) => obj.Type.TypeFullyQualified.GetHashCode();
   public int GetHashCode(ParsableGeneratorState obj) => obj.Type.TypeFullyQualified.GetHashCode();
   public int GetHashCode(ComparisonOperatorsGeneratorState obj) => obj.Type.TypeFullyQualified.GetHashCode();
   public int GetHashCode(EqualityComparisonOperatorsGeneratorState obj) => obj.Type.TypeFullyQualified.GetHashCode();
   public int GetHashCode(OperatorsGeneratorState obj) => obj.Type.TypeFullyQualified.GetHashCode();
}

public class TypeOnlyComparer<T> : IEqualityComparer<T>
   where T : ITypeFullyQualified
{
   public static readonly IEqualityComparer<T> Instance = new TypeOnlyComparer<T>();

   public bool Equals(T? x, T? y)
   {
      if (x is null)
         return y is null;

      if (y is null)
         return false;

      return x.TypeFullyQualified == y.TypeFullyQualified;
   }

   public int GetHashCode(T obj)
   {
      return obj.TypeFullyQualified.GetHashCode();
   }
}
