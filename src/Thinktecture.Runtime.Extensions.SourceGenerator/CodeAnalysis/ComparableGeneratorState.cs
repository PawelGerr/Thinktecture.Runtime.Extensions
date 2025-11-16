namespace Thinktecture.CodeAnalysis;

public sealed class ComparableGeneratorState
   : IEquatable<ComparableGeneratorState>,
     IHasGenerics
{
   public ITypeInformation Type { get; }
   public IMemberInformation KeyMember { get; }
   public string CreateFactoryMethodName { get; }
   public bool SkipIComparable { get; }
   public bool IsKeyMemberComparable { get; }
   public string? ComparerAccessor { get; }
   public ImmutableArray<GenericTypeParameterState> GenericParameters { get; }

   public ComparableGeneratorState(
      ITypeInformation type,
      IMemberInformation keyMember,
      string createFactoryMethodName,
      bool skipIComparable,
      bool isKeyMemberComparable,
      string? comparerAccessor,
      ImmutableArray<GenericTypeParameterState> genericParameters)
   {
      Type = type;
      KeyMember = keyMember;
      CreateFactoryMethodName = createFactoryMethodName;
      SkipIComparable = skipIComparable;
      IsKeyMemberComparable = isKeyMemberComparable;
      ComparerAccessor = comparerAccessor;
      GenericParameters = genericParameters;
   }

   public bool Equals(ComparableGeneratorState other)
   {
      return TypeInformationComparer.Instance.Equals(Type, other.Type)
             && MemberInformationComparer.Instance.Equals(KeyMember, other.KeyMember)
             && CreateFactoryMethodName == other.CreateFactoryMethodName
             && SkipIComparable == other.SkipIComparable
             && IsKeyMemberComparable == other.IsKeyMemberComparable
             && ComparerAccessor == other.ComparerAccessor
             && GenericParameters.SequenceEqual(other.GenericParameters);
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
         hashCode = (hashCode * 397) ^ CreateFactoryMethodName.GetHashCode();
         hashCode = (hashCode * 397) ^ SkipIComparable.GetHashCode();
         hashCode = (hashCode * 397) ^ IsKeyMemberComparable.GetHashCode();
         hashCode = (hashCode * 397) ^ (ComparerAccessor?.GetHashCode() ?? 0);
         hashCode = (hashCode * 397) ^ GenericParameters.ComputeHashCode();

         return hashCode;
      }
   }
}
