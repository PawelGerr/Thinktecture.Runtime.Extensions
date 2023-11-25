using Thinktecture.CodeAnalysis.SmartEnums;
using Thinktecture.CodeAnalysis.ValueObjects;

namespace Thinktecture.CodeAnalysis;

public class TypeOnlyComparer
   : IEqualityComparer<FormattableGeneratorState>,
     IEqualityComparer<ComparableGeneratorState>,
     IEqualityComparer<ParsableGeneratorState>,
     IEqualityComparer<ComparisonOperatorsGeneratorState>,
     IEqualityComparer<EqualityComparisonOperatorsGeneratorState>,
     IEqualityComparer<OperatorsGeneratorState>,
     IEqualityComparer<EnumSourceGeneratorState>,
     IEqualityComparer<SmartEnumDerivedTypes>,
     IEqualityComparer<KeyedValueObjectSourceGeneratorState>,
     IEqualityComparer<ComplexValueObjectSourceGeneratorState>
{
   public static readonly TypeOnlyComparer Instance = new();

   public bool Equals(FormattableGeneratorState x, FormattableGeneratorState y) => x.Type.TypeFullyQualified == y.Type.TypeFullyQualified;
   public bool Equals(ComparableGeneratorState x, ComparableGeneratorState y) => x.Type.TypeFullyQualified == y.Type.TypeFullyQualified;
   public bool Equals(ParsableGeneratorState x, ParsableGeneratorState y) => x.Type.TypeFullyQualified == y.Type.TypeFullyQualified;
   public bool Equals(ComparisonOperatorsGeneratorState x, ComparisonOperatorsGeneratorState y) => x.Type.TypeFullyQualified == y.Type.TypeFullyQualified;
   public bool Equals(EqualityComparisonOperatorsGeneratorState x, EqualityComparisonOperatorsGeneratorState y) => x.Type.TypeFullyQualified == y.Type.TypeFullyQualified;
   public bool Equals(OperatorsGeneratorState x, OperatorsGeneratorState y) => x.Type.TypeFullyQualified == y.Type.TypeFullyQualified;
   public bool Equals(EnumSourceGeneratorState x, EnumSourceGeneratorState y) => x.TypeFullyQualified == y.TypeFullyQualified;
   public bool Equals(SmartEnumDerivedTypes x, SmartEnumDerivedTypes y) => x.TypeFullyQualified == y.TypeFullyQualified;
   public bool Equals(KeyedValueObjectSourceGeneratorState x, KeyedValueObjectSourceGeneratorState y) => x.TypeFullyQualified == y.TypeFullyQualified;
   public bool Equals(ComplexValueObjectSourceGeneratorState x, ComplexValueObjectSourceGeneratorState y) => x.TypeFullyQualified == y.TypeFullyQualified;

   public int GetHashCode(FormattableGeneratorState obj) => obj.Type.TypeFullyQualified.GetHashCode();
   public int GetHashCode(ComparableGeneratorState obj) => obj.Type.TypeFullyQualified.GetHashCode();
   public int GetHashCode(ParsableGeneratorState obj) => obj.Type.TypeFullyQualified.GetHashCode();
   public int GetHashCode(ComparisonOperatorsGeneratorState obj) => obj.Type.TypeFullyQualified.GetHashCode();
   public int GetHashCode(EqualityComparisonOperatorsGeneratorState obj) => obj.Type.TypeFullyQualified.GetHashCode();
   public int GetHashCode(OperatorsGeneratorState obj) => obj.Type.TypeFullyQualified.GetHashCode();
   public int GetHashCode(EnumSourceGeneratorState obj) => obj.TypeFullyQualified.GetHashCode();
   public int GetHashCode(SmartEnumDerivedTypes obj) => obj.TypeFullyQualified.GetHashCode();
   public int GetHashCode(KeyedValueObjectSourceGeneratorState obj) => obj.TypeFullyQualified.GetHashCode();
   public int GetHashCode(ComplexValueObjectSourceGeneratorState obj) => obj.TypeFullyQualified.GetHashCode();

   private TypeOnlyComparer()
   {
   }
}
