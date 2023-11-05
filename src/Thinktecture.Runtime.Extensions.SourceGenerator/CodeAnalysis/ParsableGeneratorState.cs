namespace Thinktecture.CodeAnalysis;

public readonly struct ParsableGeneratorState : IEquatable<ParsableGeneratorState>, ITypeInformationProvider
{
   public ITypeInformation Type { get; }
   public IMemberInformation? KeyMember { get; }
   public bool SkipIParsable { get; }
   public bool IsKeyMemberParsable { get; }
   public bool IsValidatableEnum { get; }
   public bool HasStringBasedValidateMethod { get; }

   public ParsableGeneratorState(
      ITypeInformation type,
      IMemberInformation? keyMember,
      bool skipIParsable,
      bool isKeyMemberParsable,
      bool isValidatableEnum,
      bool hasStringBasedValidateMethod)
   {
      Type = type;
      KeyMember = keyMember;
      SkipIParsable = skipIParsable;
      IsKeyMemberParsable = isKeyMemberParsable;
      IsValidatableEnum = isValidatableEnum;
      HasStringBasedValidateMethod = hasStringBasedValidateMethod;
   }

   public bool Equals(ParsableGeneratorState other)
   {
      return TypeInformationComparer.Instance.Equals(Type, other.Type)
             && MemberInformationComparer.Instance.Equals(KeyMember, other.KeyMember)
             && SkipIParsable == other.SkipIParsable
             && IsKeyMemberParsable == other.IsKeyMemberParsable
             && IsValidatableEnum == other.IsValidatableEnum
             && HasStringBasedValidateMethod == other.HasStringBasedValidateMethod;
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
         hashCode = (hashCode * 397) ^ (KeyMember is null ? 0 : MemberInformationComparer.Instance.GetHashCode(KeyMember));
         hashCode = (hashCode * 397) ^ SkipIParsable.GetHashCode();
         hashCode = (hashCode * 397) ^ IsKeyMemberParsable.GetHashCode();
         hashCode = (hashCode * 397) ^ IsValidatableEnum.GetHashCode();
         hashCode = (hashCode * 397) ^ HasStringBasedValidateMethod.GetHashCode();

         return hashCode;
      }
   }
}
