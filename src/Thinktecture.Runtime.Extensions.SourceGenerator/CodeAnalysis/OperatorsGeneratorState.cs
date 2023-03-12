using Thinktecture.CodeAnalysis.ValueObjects;

namespace Thinktecture.CodeAnalysis;

public readonly struct OperatorsGeneratorState : IEquatable<OperatorsGeneratorState>
{
   public ITypeInformation Type { get; }
   public IMemberInformation KeyMember { get; }
   public OperatorsGeneration OperatorsGeneration { get; }
   public bool HasKeyMemberOperators { get; }
   public IOperatorsCodeGeneratorProvider GeneratorProvider { get; }

   public OperatorsGeneratorState(
      ITypeInformation type,
      IMemberInformation keyMember,
      OperatorsGeneration operatorsGeneration,
      bool hasKeyMemberOperators,
      IOperatorsCodeGeneratorProvider generatorProvider)
   {
      Type = type;
      KeyMember = keyMember;
      OperatorsGeneration = operatorsGeneration;
      HasKeyMemberOperators = hasKeyMemberOperators;
      GeneratorProvider = generatorProvider;
   }

   public bool Equals(OperatorsGeneratorState other)
   {
      return TypeInformationComparer.Instance.Equals(Type, other.Type)
             && MemberInformationComparer.Instance.Equals(KeyMember, other.KeyMember)
             && OperatorsGeneration == other.OperatorsGeneration
             && HasKeyMemberOperators == other.HasKeyMemberOperators
             && ReferenceEquals(GeneratorProvider, other.GeneratorProvider);
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
         hashCode = (hashCode * 397) ^ (int)OperatorsGeneration;
         hashCode = (hashCode * 397) ^ HasKeyMemberOperators.GetHashCode();
         hashCode = (hashCode * 397) ^ GeneratorProvider.GetHashCode();

         return hashCode;
      }
   }
}
