namespace Thinktecture.CodeAnalysis;

public readonly struct ComparableGeneratorState : IEquatable<ComparableGeneratorState>
{
   public ITypeInformation Type { get; }
   public IMemberInformation KeyMember { get; }
   public bool SkipIComparable { get; }
   public bool IsKeyMemberComparable { get; }
   public string? ComparerAccessor { get; }

   public ComparableGeneratorState(
      ITypeInformation type,
      IMemberInformation keyMember,
      bool skipIComparable,
      bool isKeyMemberComparable,
      string? comparerAccessor)
   {
      Type = type;
      KeyMember = keyMember;
      SkipIComparable = skipIComparable;
      IsKeyMemberComparable = isKeyMemberComparable;
      ComparerAccessor = comparerAccessor;
   }

   public bool Equals(ComparableGeneratorState other)
   {
      return TypeInformationComparer.Instance.Equals(Type, other.Type)
             && MemberInformationComparer.Instance.Equals(KeyMember, other.KeyMember)
             && SkipIComparable == other.SkipIComparable
             && IsKeyMemberComparable == other.IsKeyMemberComparable
             && ComparerAccessor == other.ComparerAccessor;
   }

   public override bool Equals(object? obj)
   {
      return obj is ComparableGeneratorState state && Equals(state);
   }

   public override int GetHashCode()
   {
      unchecked
      {
         var hashCode = TypeInformationComparer.Instance.GetHashCode(Type);
         hashCode = (hashCode * 397) ^ MemberInformationComparer.Instance.GetHashCode(KeyMember);
         hashCode = (hashCode * 397) ^ SkipIComparable.GetHashCode();
         hashCode = (hashCode * 397) ^ IsKeyMemberComparable.GetHashCode();
         hashCode = (hashCode * 397) ^ ComparerAccessor?.GetHashCode() ?? 0;

         return hashCode;
      }
   }
}
