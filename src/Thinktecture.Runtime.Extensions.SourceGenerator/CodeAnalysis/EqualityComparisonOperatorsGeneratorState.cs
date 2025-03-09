namespace Thinktecture.CodeAnalysis;

public readonly struct EqualityComparisonOperatorsGeneratorState : IEquatable<EqualityComparisonOperatorsGeneratorState>, ITypeInformationProvider
{
   public ITypeInformation Type { get; }
   public IMemberInformation? KeyMember { get; }
   public OperatorsGeneration OperatorsGeneration { get; }
   public ComparerInfo? EqualityComparer { get; }

   public EqualityComparisonOperatorsGeneratorState(
      ITypeInformation type,
      IMemberInformation? keyMember,
      OperatorsGeneration operatorsGeneration,
      ComparerInfo? equalityComparer)
   {
      Type = type;
      KeyMember = keyMember;
      OperatorsGeneration = operatorsGeneration;
      EqualityComparer = equalityComparer;
   }

   public bool Equals(EqualityComparisonOperatorsGeneratorState other)
   {
      return TypeInformationComparer.Instance.Equals(Type, other.Type)
             && MemberInformationComparer.Instance.Equals(KeyMember, other.KeyMember)
             && OperatorsGeneration == other.OperatorsGeneration
             && EqualityComparer == other.EqualityComparer;
   }

   public override bool Equals(object? obj)
   {
      return obj is EqualityComparisonOperatorsGeneratorState state && Equals(state);
   }

   public override int GetHashCode()
   {
      unchecked
      {
         var hashCode = TypeInformationComparer.Instance.GetHashCode(Type);
         hashCode = (hashCode * 397) ^ (KeyMember is null ? 0 : MemberInformationComparer.Instance.GetHashCode(KeyMember));
         hashCode = (hashCode * 397) ^ (int)OperatorsGeneration;
         hashCode = (hashCode * 397) ^ (EqualityComparer?.GetHashCode() ?? 0);

         return hashCode;
      }
   }
}
