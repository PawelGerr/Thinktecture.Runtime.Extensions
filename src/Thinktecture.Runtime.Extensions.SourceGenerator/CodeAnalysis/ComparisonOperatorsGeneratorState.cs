namespace Thinktecture.CodeAnalysis;

public readonly struct ComparisonOperatorsGeneratorState : IEquatable<ComparisonOperatorsGeneratorState>
{
   public ITypeInformation Type { get; }
   public IMemberInformation KeyMember { get; }
   public OperatorsGeneration OperatorsGeneration { get; }
   public ImplementedComparisonOperators KeyMemberOperators { get; }
   public string? ComparerAccessor { get; }

   public ComparisonOperatorsGeneratorState(
      ITypeInformation type,
      IMemberInformation keyMember,
      OperatorsGeneration operatorsGeneration,
      ImplementedComparisonOperators keyMemberOperators,
      string? comparerAccessor)
   {
      Type = type;
      KeyMember = keyMember;
      OperatorsGeneration = operatorsGeneration;
      KeyMemberOperators = keyMemberOperators;
      ComparerAccessor = comparerAccessor;
   }

   public bool Equals(ComparisonOperatorsGeneratorState other)
   {
      return TypeInformationComparer.Instance.Equals(Type, other.Type)
             && MemberInformationComparer.Instance.Equals(KeyMember, other.KeyMember)
             && OperatorsGeneration == other.OperatorsGeneration
             && KeyMemberOperators == other.KeyMemberOperators
             && ComparerAccessor == other.ComparerAccessor;
   }

   public override bool Equals(object? obj)
   {
      return obj is ComparisonOperatorsGeneratorState state && Equals(state);
   }

   public override int GetHashCode()
   {
      unchecked
      {
         var hashCode = TypeInformationComparer.Instance.GetHashCode(Type);
         hashCode = (hashCode * 397) ^ MemberInformationComparer.Instance.GetHashCode(KeyMember);
         hashCode = (hashCode * 397) ^ (int)OperatorsGeneration;
         hashCode = (hashCode * 397) ^ (int)KeyMemberOperators;
         hashCode = (hashCode * 397) ^ ComparerAccessor?.GetHashCode() ?? 0;

         return hashCode;
      }
   }
}
