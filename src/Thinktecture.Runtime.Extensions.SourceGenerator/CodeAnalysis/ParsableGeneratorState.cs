namespace Thinktecture.CodeAnalysis;

public sealed class ParsableGeneratorState :
   IEquatable<ParsableGeneratorState>,
   ITypeInformationProvider<IParsableTypeInformation>,
   IHasGenerics,
   IParsableState
{
   public IParsableTypeInformation Type { get; }
   public IParsableMemberInformation? KeyMember { get; }
   public ValidationErrorState ValidationError { get; }
   public bool SkipIParsable { get; }
   public bool HasStringBasedValidateMethod { get; }
   public ImmutableArray<GenericTypeParameterState> GenericParameters { get; }

   public ParsableGeneratorState(
      IParsableTypeInformation type,
      IParsableMemberInformation? keyMember,
      ValidationErrorState validationError,
      bool skipIParsable,
      bool hasStringBasedValidateMethod,
      ImmutableArray<GenericTypeParameterState> genericParameters)
   {
      Type = type;
      KeyMember = keyMember;
      ValidationError = validationError;
      SkipIParsable = skipIParsable;
      HasStringBasedValidateMethod = hasStringBasedValidateMethod;
      GenericParameters = genericParameters;
   }

   public bool Equals(ParsableGeneratorState other)
   {
      return Type.Equals(other.Type)
             && ParsableMemberInformationComparer.Instance.Equals(KeyMember, other.KeyMember)
             && ValidationError.Equals(other.ValidationError)
             && SkipIParsable == other.SkipIParsable
             && HasStringBasedValidateMethod == other.HasStringBasedValidateMethod
             && GenericParameters.SequenceEqual(other.GenericParameters);
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
         hashCode = (hashCode * 397) ^ (KeyMember is null ? 0 : ParsableMemberInformationComparer.Instance.GetHashCode(KeyMember));
         hashCode = (hashCode * 397) ^ ValidationError.GetHashCode();
         hashCode = (hashCode * 397) ^ SkipIParsable.GetHashCode();
         hashCode = (hashCode * 397) ^ HasStringBasedValidateMethod.GetHashCode();
         hashCode = (hashCode * 397) ^ GenericParameters.ComputeHashCode();

         return hashCode;
      }
   }

   public static bool operator ==(ParsableGeneratorState left, ParsableGeneratorState right)
   {
      return left.Equals(right);
   }

   public static bool operator !=(ParsableGeneratorState left, ParsableGeneratorState right)
   {
      return !(left == right);
   }
}
