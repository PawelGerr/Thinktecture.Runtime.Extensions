namespace Thinktecture.CodeAnalysis;

public readonly struct ParsableGeneratorState : IEquatable<ParsableGeneratorState>, ITypeInformationProvider<IParsableTypeInformation>
{
   public IParsableTypeInformation Type { get; }
   public IMemberInformation? KeyMember { get; }
   public ValidationErrorState ValidationError { get; }
   public bool SkipIParsable { get; }
   public bool IsKeyMemberParsable { get; }
   public bool IsEnum { get; }
   public bool IsValidatableEnum { get; }
   public bool HasStringBasedValidateMethod { get; }

   public ParsableGeneratorState(
      IParsableTypeInformation type,
      IMemberInformation? keyMember,
      ValidationErrorState validationError,
      bool skipIParsable,
      bool isKeyMemberParsable,
      bool isEnum,
      bool isValidatableEnum,
      bool hasStringBasedValidateMethod)
   {
      Type = type;
      KeyMember = keyMember;
      ValidationError = validationError;
      SkipIParsable = skipIParsable;
      IsKeyMemberParsable = isKeyMemberParsable;
      IsEnum = isEnum;
      IsValidatableEnum = isValidatableEnum;
      HasStringBasedValidateMethod = hasStringBasedValidateMethod;
   }

   public bool Equals(ParsableGeneratorState other)
   {
      return Type.Equals(other.Type)
             && MemberInformationComparer.Instance.Equals(KeyMember, other.KeyMember)
             && ValidationError.Equals(other.ValidationError)
             && SkipIParsable == other.SkipIParsable
             && IsKeyMemberParsable == other.IsKeyMemberParsable
             && IsEnum == other.IsEnum
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
         var hashCode = Type.GetHashCode();
         hashCode = (hashCode * 397) ^ (KeyMember is null ? 0 : MemberInformationComparer.Instance.GetHashCode(KeyMember));
         hashCode = (hashCode * 397) ^ ValidationError.GetHashCode();
         hashCode = (hashCode * 397) ^ SkipIParsable.GetHashCode();
         hashCode = (hashCode * 397) ^ IsKeyMemberParsable.GetHashCode();
         hashCode = (hashCode * 397) ^ IsEnum.GetHashCode();
         hashCode = (hashCode * 397) ^ IsValidatableEnum.GetHashCode();
         hashCode = (hashCode * 397) ^ HasStringBasedValidateMethod.GetHashCode();

         return hashCode;
      }
   }
}
