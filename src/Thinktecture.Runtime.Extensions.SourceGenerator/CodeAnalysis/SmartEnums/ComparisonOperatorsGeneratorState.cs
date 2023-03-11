namespace Thinktecture.CodeAnalysis.SmartEnums;

public readonly struct ComparisonOperatorsGeneratorState : IEquatable<ComparisonOperatorsGeneratorState>
{
   public ITypeInformation Type { get; }
   public IMemberInformation KeyMember { get; }
   public OperatorsGeneration ComparisonOperators { get; }
   public bool HasKeyMemberComparisonOperators { get; }

   public ComparisonOperatorsGeneratorState(
      ITypeInformation type,
      IMemberInformation keyMember,
      OperatorsGeneration comparisonOperators,
      bool hasKeyMemberComparisonOperators)
   {
      Type = type;
      KeyMember = keyMember;
      ComparisonOperators = comparisonOperators;
      HasKeyMemberComparisonOperators = hasKeyMemberComparisonOperators;
   }

   public bool Equals(ComparisonOperatorsGeneratorState other)
   {
      return TypeInformationComparer.Instance.Equals(Type, other.Type)
             && MemberInformationComparer.Instance.Equals(KeyMember, other.KeyMember)
             && ComparisonOperators == other.ComparisonOperators
             && HasKeyMemberComparisonOperators == other.HasKeyMemberComparisonOperators;
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
         hashCode = (hashCode * 397) ^ (int)ComparisonOperators;
         hashCode = (hashCode * 397) ^ HasKeyMemberComparisonOperators.GetHashCode();

         return hashCode;
      }
   }
}
