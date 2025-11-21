using Thinktecture.CodeAnalysis.AdHocUnions;
using Thinktecture.CodeAnalysis.ObjectFactories;
using Thinktecture.CodeAnalysis.RegularUnions;
using Thinktecture.CodeAnalysis.SmartEnums;
using Thinktecture.CodeAnalysis.ValueObjects;

namespace Thinktecture.CodeAnalysis;

public sealed class TypeOnlyComparer
   : IEqualityComparer<FormattableGeneratorState>,
     IEqualityComparer<ComparableGeneratorState>,
     IEqualityComparer<ParsableGeneratorState>,
     IEqualityComparer<SpanParsableGeneratorState>,
     IEqualityComparer<ComparisonOperatorsGeneratorState>,
     IEqualityComparer<EqualityComparisonOperatorsGeneratorState>,
     IEqualityComparer<OperatorsGeneratorState>,
     IEqualityComparer<SmartEnumSourceGeneratorState>,
     IEqualityComparer<KeyedValueObjectSourceGeneratorState>,
     IEqualityComparer<ComplexValueObjectSourceGeneratorState>,
     IEqualityComparer<AdHocUnionSourceGenState>,
     IEqualityComparer<RegularUnionSourceGenState>,
     IEqualityComparer<ObjectFactorySourceGeneratorState>
{
   public static readonly TypeOnlyComparer Instance = new();

   public bool Equals(FormattableGeneratorState x, FormattableGeneratorState y) => x.Type.TypeFullyQualified == y.Type.TypeFullyQualified;
   public bool Equals(ComparableGeneratorState x, ComparableGeneratorState y) => x.Type.TypeFullyQualified == y.Type.TypeFullyQualified;
   public bool Equals(ParsableGeneratorState x, ParsableGeneratorState y) => x.Type.TypeFullyQualified == y.Type.TypeFullyQualified;
   public bool Equals(SpanParsableGeneratorState x, SpanParsableGeneratorState y) => x.Type.TypeFullyQualified == y.Type.TypeFullyQualified;
   public bool Equals(ComparisonOperatorsGeneratorState x, ComparisonOperatorsGeneratorState y) => x.Type.TypeFullyQualified == y.Type.TypeFullyQualified;
   public bool Equals(EqualityComparisonOperatorsGeneratorState x, EqualityComparisonOperatorsGeneratorState y) => x.Type.TypeFullyQualified == y.Type.TypeFullyQualified;
   public bool Equals(OperatorsGeneratorState x, OperatorsGeneratorState y) => x.Type.TypeFullyQualified == y.Type.TypeFullyQualified;
   public bool Equals(SmartEnumSourceGeneratorState x, SmartEnumSourceGeneratorState y) => x.TypeFullyQualified == y.TypeFullyQualified;
   public bool Equals(KeyedValueObjectSourceGeneratorState x, KeyedValueObjectSourceGeneratorState y) => x.TypeFullyQualified == y.TypeFullyQualified;
   public bool Equals(ComplexValueObjectSourceGeneratorState x, ComplexValueObjectSourceGeneratorState y) => x.TypeFullyQualified == y.TypeFullyQualified;
   public bool Equals(AdHocUnionSourceGenState x, AdHocUnionSourceGenState y) => x.TypeFullyQualified == y.TypeFullyQualified;
   public bool Equals(RegularUnionSourceGenState x, RegularUnionSourceGenState y) => x.TypeFullyQualified == y.TypeFullyQualified;
   public bool Equals(ObjectFactorySourceGeneratorState x, ObjectFactorySourceGeneratorState y) => x.TypeFullyQualified == y.TypeFullyQualified;

   public int GetHashCode(FormattableGeneratorState obj) => obj.Type.TypeFullyQualified.GetHashCode();
   public int GetHashCode(ComparableGeneratorState obj) => obj.Type.TypeFullyQualified.GetHashCode();
   public int GetHashCode(ParsableGeneratorState obj) => obj.Type.TypeFullyQualified.GetHashCode();
   public int GetHashCode(SpanParsableGeneratorState obj) => obj.Type.TypeFullyQualified.GetHashCode();
   public int GetHashCode(ComparisonOperatorsGeneratorState obj) => obj.Type.TypeFullyQualified.GetHashCode();
   public int GetHashCode(EqualityComparisonOperatorsGeneratorState obj) => obj.Type.TypeFullyQualified.GetHashCode();
   public int GetHashCode(OperatorsGeneratorState obj) => obj.Type.TypeFullyQualified.GetHashCode();
   public int GetHashCode(SmartEnumSourceGeneratorState obj) => obj.TypeFullyQualified.GetHashCode();
   public int GetHashCode(KeyedValueObjectSourceGeneratorState obj) => obj.TypeFullyQualified.GetHashCode();
   public int GetHashCode(ComplexValueObjectSourceGeneratorState obj) => obj.TypeFullyQualified.GetHashCode();
   public int GetHashCode(AdHocUnionSourceGenState obj) => obj.TypeFullyQualified.GetHashCode();
   public int GetHashCode(RegularUnionSourceGenState obj) => obj.TypeFullyQualified.GetHashCode();
   public int GetHashCode(ObjectFactorySourceGeneratorState obj) => obj.TypeFullyQualified.GetHashCode();

   private TypeOnlyComparer()
   {
   }
}
