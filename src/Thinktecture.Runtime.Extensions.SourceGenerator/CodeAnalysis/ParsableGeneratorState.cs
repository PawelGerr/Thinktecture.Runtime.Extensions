namespace Thinktecture.CodeAnalysis;

public readonly struct ParsableGeneratorState : IEquatable<ParsableGeneratorState>
{
   public ITypeInformation Type { get; }
   public IMemberInformation KeyMember { get; }
   public bool SkipIParsable { get; }
   public bool IsKeyMemberParsable { get; }
   public bool IsValidatableEnum { get; }

   public ParsableGeneratorState(
      ITypeInformation type,
      IMemberInformation keyMember,
      bool skipIParsable,
      bool isKeyMemberParsable,
      bool isValidatableEnum)
   {
      Type = type;
      KeyMember = keyMember;
      SkipIParsable = skipIParsable;
      IsKeyMemberParsable = isKeyMemberParsable;
      IsValidatableEnum = isValidatableEnum;
   }

   public bool Equals(ParsableGeneratorState other)
   {
      return TypeInformationComparer.Instance.Equals(Type, other.Type)
             && MemberInformationComparer.Instance.Equals(KeyMember, other.KeyMember)
             && SkipIParsable == other.SkipIParsable
             && IsKeyMemberParsable == other.IsKeyMemberParsable
             && IsValidatableEnum == other.IsValidatableEnum;
   }

   public override bool Equals(object? obj)
   {
      return obj is ParsableGeneratorState state && Equals(state);
   }

   public override int GetHashCode()
   {
      unchecked
      {
         var hashCode = TypeInformationComparer.Instance.GetHashCode(Type);
         hashCode = (hashCode * 397) ^ MemberInformationComparer.Instance.GetHashCode(KeyMember);
         hashCode = (hashCode * 397) ^ SkipIParsable.GetHashCode();
         hashCode = (hashCode * 397) ^ IsKeyMemberParsable.GetHashCode();
         hashCode = (hashCode * 397) ^ IsValidatableEnum.GetHashCode();

         return hashCode;
      }
   }
}
