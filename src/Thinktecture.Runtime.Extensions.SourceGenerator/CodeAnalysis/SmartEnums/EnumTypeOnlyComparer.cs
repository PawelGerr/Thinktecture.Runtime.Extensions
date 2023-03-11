namespace Thinktecture.CodeAnalysis.SmartEnums;

public class EnumTypeOnlyComparer
   : IEqualityComparer<FormattableGeneratorState>,
     IEqualityComparer<ComparableGeneratorState>,
     IEqualityComparer<ParsableGeneratorState>,
     IEqualityComparer<ComparisonOperatorsGeneratorState>
{
   public static readonly EnumTypeOnlyComparer Instance = new();

   public bool Equals(FormattableGeneratorState x, FormattableGeneratorState y) => x.Type.TypeFullyQualified == y.Type.TypeFullyQualified;
   public bool Equals(ComparableGeneratorState x, ComparableGeneratorState y) => x.Type.TypeFullyQualified == y.Type.TypeFullyQualified;
   public bool Equals(ParsableGeneratorState x, ParsableGeneratorState y) => x.Type.TypeFullyQualified == y.Type.TypeFullyQualified;
   public bool Equals(ComparisonOperatorsGeneratorState x, ComparisonOperatorsGeneratorState y) => x.Type.TypeFullyQualified == y.Type.TypeFullyQualified;

   public int GetHashCode(FormattableGeneratorState obj) => obj.Type.TypeFullyQualified.GetHashCode();
   public int GetHashCode(ComparableGeneratorState obj) => obj.Type.TypeFullyQualified.GetHashCode();
   public int GetHashCode(ParsableGeneratorState obj) => obj.Type.TypeFullyQualified.GetHashCode();
   public int GetHashCode(ComparisonOperatorsGeneratorState obj) => obj.Type.TypeFullyQualified.GetHashCode();
}

public class EnumTypeOnlyComparer<T> : IEqualityComparer<T>
   where T : ITypeFullyQualified
{
   public static readonly IEqualityComparer<T> Instance = new EnumTypeOnlyComparer<T>();

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
