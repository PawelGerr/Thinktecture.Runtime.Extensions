namespace Thinktecture.CodeAnalysis;

public readonly struct InterfaceCodeGeneratorState : IEquatable<InterfaceCodeGeneratorState>, ITypeInformationProvider
{
   public ITypeInformation Type { get; }
   public IMemberInformation KeyMember { get; }

   public InterfaceCodeGeneratorState(
      ITypeInformation type,
      IMemberInformation keyMember)
   {
      Type = type;
      KeyMember = keyMember;
   }

   public override bool Equals(object? obj)
   {
      return obj is InterfaceCodeGeneratorState state && Equals(state);
   }

   public bool Equals(InterfaceCodeGeneratorState other)
   {
      return Type.Equals(other.Type)
             && KeyMember.Equals(other.KeyMember);
   }

   public override int GetHashCode()
   {
      unchecked
      {
         var hashCode = Type.GetHashCode();
         hashCode = (hashCode * 397) ^ KeyMember.GetHashCode();

         return hashCode;
      }
   }
}
