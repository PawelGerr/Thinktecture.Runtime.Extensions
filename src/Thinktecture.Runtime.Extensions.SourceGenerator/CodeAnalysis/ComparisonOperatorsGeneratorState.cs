namespace Thinktecture.CodeAnalysis;

public sealed class ComparisonOperatorsGeneratorState
   : IEquatable<ComparisonOperatorsGeneratorState>,
     IHasGenerics
{
   public ITypeInformation Type { get; }
   public IMemberInformation KeyMember { get; }
   public string CreateFactoryMethodName { get; }
   public OperatorsGeneration OperatorsGeneration { get; }
   public ImplementedComparisonOperators KeyMemberOperators { get; }
   public string? ComparerAccessor { get; }
   public ImmutableArray<GenericTypeParameterState> GenericParameters { get; }

   public ComparisonOperatorsGeneratorState(
      ITypeInformation type,
      IMemberInformation keyMember,
      string createFactoryMethodName,
      OperatorsGeneration operatorsGeneration,
      ImplementedComparisonOperators keyMemberOperators,
      string? comparerAccessor,
      ImmutableArray<GenericTypeParameterState> genericParameters)
   {
      Type = type;
      KeyMember = keyMember;
      CreateFactoryMethodName = createFactoryMethodName;
      OperatorsGeneration = operatorsGeneration;
      KeyMemberOperators = keyMemberOperators;
      ComparerAccessor = comparerAccessor;
      GenericParameters = genericParameters;
   }

   public bool Equals(ComparisonOperatorsGeneratorState other)
   {
      return TypeInformationComparer.Instance.Equals(Type, other.Type)
             && MemberInformationComparer.Instance.Equals(KeyMember, other.KeyMember)
             && CreateFactoryMethodName == other.CreateFactoryMethodName
             && OperatorsGeneration == other.OperatorsGeneration
             && KeyMemberOperators == other.KeyMemberOperators
             && ComparerAccessor == other.ComparerAccessor
             && GenericParameters.SequenceEqual(other.GenericParameters);
   }

   public override bool Equals(object? obj)
   {
      return obj is ComparisonOperatorsGeneratorState state && Equals(state);
   }

   public override int GetHashCode()
   {
      unchecked
      {
         var hashCode = TypeInformationComparer.Instance.GetHashCode(Type);
         hashCode = (hashCode * 397) ^ MemberInformationComparer.Instance.GetHashCode(KeyMember);
         hashCode = (hashCode * 397) ^ CreateFactoryMethodName.GetHashCode();
         hashCode = (hashCode * 397) ^ (int)OperatorsGeneration;
         hashCode = (hashCode * 397) ^ (int)KeyMemberOperators;
         hashCode = (hashCode * 397) ^ (ComparerAccessor?.GetHashCode() ?? 0);
         hashCode = (hashCode * 397) ^ GenericParameters.ComputeHashCode();

         return hashCode;
      }
   }

   public static bool operator ==(ComparisonOperatorsGeneratorState left, ComparisonOperatorsGeneratorState right)
   {
      return left.Equals(right);
   }

   public static bool operator !=(ComparisonOperatorsGeneratorState left, ComparisonOperatorsGeneratorState right)
   {
      return !(left == right);
   }
}
