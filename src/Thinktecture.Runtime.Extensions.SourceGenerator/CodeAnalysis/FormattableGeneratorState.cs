namespace Thinktecture.CodeAnalysis;

public sealed class FormattableGeneratorState
   : IEquatable<FormattableGeneratorState>,
     IHasGenerics
{
   public ITypeInformation Type { get; }
   public IMemberInformation KeyMember { get; }
   public string CreateFactoryMethodName { get; }
   public bool SkipIFormattable { get; }
   public bool IsKeyMemberFormattable { get; }
   public ImmutableArray<GenericTypeParameterState> GenericParameters { get; }

   public FormattableGeneratorState(
      ITypeInformation type,
      IMemberInformation keyMember,
      string createFactoryMethodName,
      bool skipIFormattable,
      bool isKeyMemberFormattable,
      ImmutableArray<GenericTypeParameterState> genericParameters)
   {
      Type = type;
      KeyMember = keyMember;
      CreateFactoryMethodName = createFactoryMethodName;
      SkipIFormattable = skipIFormattable;
      IsKeyMemberFormattable = isKeyMemberFormattable;
      GenericParameters = genericParameters;
   }

   public bool Equals(FormattableGeneratorState other)
   {
      return TypeInformationComparer.Instance.Equals(Type, other.Type)
             && MemberInformationComparer.Instance.Equals(KeyMember, other.KeyMember)
             && CreateFactoryMethodName == other.CreateFactoryMethodName
             && SkipIFormattable == other.SkipIFormattable
             && IsKeyMemberFormattable == other.IsKeyMemberFormattable
             && GenericParameters.SequenceEqual(other.GenericParameters);
   }

   public override bool Equals(object? obj)
   {
      return obj is FormattableGeneratorState state && Equals(state);
   }

   public override int GetHashCode()
   {
      unchecked
      {
         var hashCode = TypeInformationComparer.Instance.GetHashCode(Type);
         hashCode = (hashCode * 397) ^ MemberInformationComparer.Instance.GetHashCode(KeyMember);
         hashCode = (hashCode * 397) ^ CreateFactoryMethodName.GetHashCode();
         hashCode = (hashCode * 397) ^ SkipIFormattable.GetHashCode();
         hashCode = (hashCode * 397) ^ IsKeyMemberFormattable.GetHashCode();
         hashCode = (hashCode * 397) ^ GenericParameters.ComputeHashCode();

         return hashCode;
      }
   }
}
