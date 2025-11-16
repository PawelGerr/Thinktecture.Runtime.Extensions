using Thinktecture.CodeAnalysis.ValueObjects;

namespace Thinktecture.CodeAnalysis;

public sealed class OperatorsGeneratorState
   : IEquatable<OperatorsGeneratorState>,
     IHasGenerics
{
   public ITypeInformation Type { get; }
   public IMemberInformation KeyMember { get; }
   public string CreateFactoryMethodName { get; }
   public OperatorsGeneration OperatorsGeneration { get; }
   public ImplementedOperators KeyMemberOperators { get; }
   public IOperatorsCodeGeneratorProvider GeneratorProvider { get; }
   public ImmutableArray<GenericTypeParameterState> GenericParameters { get; }

   public OperatorsGeneratorState(
      ITypeInformation type,
      IMemberInformation keyMember,
      string createFactoryMethodName,
      OperatorsGeneration operatorsGeneration,
      ImplementedOperators keyMemberOperators,
      IOperatorsCodeGeneratorProvider generatorProvider,
      ImmutableArray<GenericTypeParameterState> genericParameters)
   {
      Type = type;
      KeyMember = keyMember;
      CreateFactoryMethodName = createFactoryMethodName;
      OperatorsGeneration = operatorsGeneration;
      KeyMemberOperators = keyMemberOperators;
      GeneratorProvider = generatorProvider;
      GenericParameters = genericParameters;
   }

   public bool Equals(OperatorsGeneratorState other)
   {
      return TypeInformationComparer.Instance.Equals(Type, other.Type)
             && MemberInformationComparer.Instance.Equals(KeyMember, other.KeyMember)
             && CreateFactoryMethodName == other.CreateFactoryMethodName
             && OperatorsGeneration == other.OperatorsGeneration
             && KeyMemberOperators == other.KeyMemberOperators
             && ReferenceEquals(GeneratorProvider, other.GeneratorProvider)
             && GenericParameters.SequenceEqual(other.GenericParameters);
   }

   public override bool Equals(object? obj)
   {
      return obj is OperatorsGeneratorState state && Equals(state);
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
         hashCode = (hashCode * 397) ^ GeneratorProvider.GetHashCode();
         hashCode = (hashCode * 397) ^ GenericParameters.ComputeHashCode();

         return hashCode;
      }
   }
}
