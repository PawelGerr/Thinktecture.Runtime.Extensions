namespace Thinktecture.CodeAnalysis;

public readonly struct SpanParsableGeneratorState
   : IEquatable<SpanParsableGeneratorState>,
     ITypeInformationProvider<IParsableTypeInformation>,
     IHasGenerics,
     IParsableState
{
   public IParsableTypeInformation Type { get; }
   public IParsableMemberInformation? KeyMember { get; }
   public ValidationErrorState ValidationError { get; }
   public bool SkipIParsable { get; }
   public bool SkipISpanParsable { get; }
   public bool IsEnum { get; }
   public bool HasStringBasedValidateMethod { get; }
   public bool HasReadOnlySpanOfCharBasedValidateMethod { get; }

   /// <summary>
   /// Whether empty or whitespace-only input yields a <c>null</c> instance in the generated <c>Validate</c>.
   /// The generated Parse/TryParse then reject such input, because ISpanParsable cannot express null on success.
   /// </summary>
   public bool EmptyStringYieldsNull { get; }

   public ImmutableArray<GenericTypeParameterState> GenericParameters { get; }

   public SpanParsableGeneratorState(
      IParsableTypeInformation type,
      IParsableMemberInformation? keyMember,
      ValidationErrorState validationError,
      bool skipIParsable,
      bool skipISpanParsable,
      bool isEnum,
      bool hasStringBasedValidateMethod,
      bool hasReadOnlySpanOfCharBasedValidateMethod,
      bool emptyStringYieldsNull,
      ImmutableArray<GenericTypeParameterState> genericParameters)
   {
      Type = type;
      KeyMember = keyMember;
      ValidationError = validationError;
      SkipISpanParsable = skipISpanParsable;
      IsEnum = isEnum;
      HasReadOnlySpanOfCharBasedValidateMethod = hasReadOnlySpanOfCharBasedValidateMethod;
      EmptyStringYieldsNull = emptyStringYieldsNull;
      GenericParameters = genericParameters;
      SkipIParsable = skipIParsable;
      HasStringBasedValidateMethod = hasStringBasedValidateMethod;
   }

   public bool Equals(SpanParsableGeneratorState other)
   {
      return Type.Equals(other.Type)
             && ParsableMemberInformationComparer.Instance.Equals(KeyMember, other.KeyMember)
             && ValidationError.Equals(other.ValidationError)
             && SkipIParsable == other.SkipIParsable
             && SkipISpanParsable == other.SkipISpanParsable
             && IsEnum == other.IsEnum
             && HasStringBasedValidateMethod == other.HasStringBasedValidateMethod
             && HasReadOnlySpanOfCharBasedValidateMethod == other.HasReadOnlySpanOfCharBasedValidateMethod
             && EmptyStringYieldsNull == other.EmptyStringYieldsNull
             && GenericParameters.SequenceEqual(other.GenericParameters);
   }

   public override bool Equals(object? obj)
   {
      return obj is SpanParsableGeneratorState state && Equals(state);
   }

   public override int GetHashCode()
   {
      unchecked
      {
         var hashCode = Type.GetHashCode();
         hashCode = (hashCode * 397) ^ (KeyMember is null ? 0 : ParsableMemberInformationComparer.Instance.GetHashCode(KeyMember));
         hashCode = (hashCode * 397) ^ ValidationError.GetHashCode();
         hashCode = (hashCode * 397) ^ SkipIParsable.GetHashCode();
         hashCode = (hashCode * 397) ^ SkipISpanParsable.GetHashCode();
         hashCode = (hashCode * 397) ^ IsEnum.GetHashCode();
         hashCode = (hashCode * 397) ^ HasStringBasedValidateMethod.GetHashCode();
         hashCode = (hashCode * 397) ^ HasReadOnlySpanOfCharBasedValidateMethod.GetHashCode();
         hashCode = (hashCode * 397) ^ EmptyStringYieldsNull.GetHashCode();
         hashCode = (hashCode * 397) ^ GenericParameters.ComputeHashCode();

         return hashCode;
      }
   }

   public static bool operator ==(SpanParsableGeneratorState left, SpanParsableGeneratorState right)
   {
      return left.Equals(right);
   }

   public static bool operator !=(SpanParsableGeneratorState left, SpanParsableGeneratorState right)
   {
      return !(left == right);
   }
}
