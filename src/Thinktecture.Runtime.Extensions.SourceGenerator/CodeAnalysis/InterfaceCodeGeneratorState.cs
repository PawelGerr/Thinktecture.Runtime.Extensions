namespace Thinktecture.CodeAnalysis;

public readonly struct InterfaceCodeGeneratorState
   : IEquatable<InterfaceCodeGeneratorState>,
     ITypeInformationProvider<ITypeInformation>,
     IHasGenerics
{
   public ITypeInformation Type { get; }
   public IMemberInformation KeyMember { get; }
   public string CreateFactoryMethodName { get; }
   public ImmutableArray<GenericTypeParameterState> GenericParameters { get; }

   public InterfaceCodeGeneratorState(
      ITypeInformation type,
      IMemberInformation keyMember,
      string createFactoryMethodName,
      ImmutableArray<GenericTypeParameterState> genericParameters)
   {
      Type = type;
      KeyMember = keyMember;
      CreateFactoryMethodName = createFactoryMethodName;
      GenericParameters = genericParameters;
   }

   public override bool Equals(object? obj)
   {
      return obj is InterfaceCodeGeneratorState state && Equals(state);
   }

   public bool Equals(InterfaceCodeGeneratorState other)
   {
      return Type.Equals(other.Type)
             && KeyMember.Equals(other.KeyMember)
             && CreateFactoryMethodName == other.CreateFactoryMethodName
             && GenericParameters.SequenceEqual(other.GenericParameters);
   }

   public override int GetHashCode()
   {
      unchecked
      {
         var hashCode = Type.GetHashCode();
         hashCode = (hashCode * 397) ^ KeyMember.GetHashCode();
         hashCode = (hashCode * 397) ^ CreateFactoryMethodName.GetHashCode();
         hashCode = (hashCode * 397) ^ GenericParameters.ComputeHashCode();

         return hashCode;
      }
   }

   public static bool operator ==(InterfaceCodeGeneratorState left, InterfaceCodeGeneratorState right)
   {
      return left.Equals(right);
   }

   public static bool operator !=(InterfaceCodeGeneratorState left, InterfaceCodeGeneratorState right)
   {
      return !(left == right);
   }
}
