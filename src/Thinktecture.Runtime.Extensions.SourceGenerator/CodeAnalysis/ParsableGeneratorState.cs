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

   /// <summary>
   /// Whether empty or whitespace-only input yields a <c>null</c> instance in the generated <c>Validate</c>.
   /// The generated Parse/TryParse then reject such input, because IParsable cannot express null on success.
   /// </summary>
   public bool EmptyStringYieldsNull { get; }

   public ImmutableArray<GenericTypeParameterState> GenericParameters { get; }

   public ParsableGeneratorState(
      IParsableTypeInformation type,
      IParsableMemberInformation? keyMember,
      ValidationErrorState validationError,
      bool skipIParsable,
      bool hasStringBasedValidateMethod,
      bool emptyStringYieldsNull,
      ImmutableArray<GenericTypeParameterState> genericParameters)
   {
      Type = type;
      KeyMember = keyMember;
      ValidationError = validationError;
      SkipIParsable = skipIParsable;
      HasStringBasedValidateMethod = hasStringBasedValidateMethod;
      EmptyStringYieldsNull = emptyStringYieldsNull;
      GenericParameters = genericParameters;
   }

   public bool Equals(ParsableGeneratorState other)
   {
      return Type.Equals(other.Type)
             && ParsableMemberInformationComparer.Instance.Equals(KeyMember, other.KeyMember)
             && ValidationError.Equals(other.ValidationError)
             && SkipIParsable == other.SkipIParsable
             && HasStringBasedValidateMethod == other.HasStringBasedValidateMethod
             && EmptyStringYieldsNull == other.EmptyStringYieldsNull
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
         hashCode = (hashCode * 397) ^ EmptyStringYieldsNull.GetHashCode();
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
