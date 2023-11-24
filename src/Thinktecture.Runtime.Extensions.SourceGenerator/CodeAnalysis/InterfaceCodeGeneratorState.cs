namespace Thinktecture.CodeAnalysis;

public readonly struct InterfaceCodeGeneratorState : IEquatable<InterfaceCodeGeneratorState>, ITypeInformationProvider
{
   public ITypeInformation Type { get; }
   public IMemberInformation KeyMember { get; }
   public string CreateFactoryMethodName { get; }

   public InterfaceCodeGeneratorState(
      ITypeInformation type,
      IMemberInformation keyMember,
      string createFactoryMethodName)
   {
      Type = type;
      KeyMember = keyMember;
      CreateFactoryMethodName = createFactoryMethodName;
   }

   public override bool Equals(object? obj)
   {
      return obj is InterfaceCodeGeneratorState state && Equals(state);
   }

   public bool Equals(InterfaceCodeGeneratorState other)
   {
      return Type.Equals(other.Type)
             && KeyMember.Equals(other.KeyMember)
             && CreateFactoryMethodName == other.CreateFactoryMethodName;
   }

   public override int GetHashCode()
   {
      unchecked
      {
         var hashCode = Type.GetHashCode();
         hashCode = (hashCode * 397) ^ KeyMember.GetHashCode();
         hashCode = (hashCode * 397) ^ CreateFactoryMethodName.GetHashCode();

         return hashCode;
      }
   }
}
