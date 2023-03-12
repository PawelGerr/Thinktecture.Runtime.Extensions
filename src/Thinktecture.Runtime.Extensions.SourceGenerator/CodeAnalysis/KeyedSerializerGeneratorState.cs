namespace Thinktecture.CodeAnalysis;

public readonly struct KeyedSerializerGeneratorState : IEquatable<KeyedSerializerGeneratorState>, INamespaceAndName
{
   public ITypeInformation Type { get; }
   public ITypeFullyQualified KeyMember { get; }
   public AttributeInfo AttributeInfo { get; }

   public string? Namespace => Type.Namespace;
   public string Name => Type.Name;

   public KeyedSerializerGeneratorState(ITypeInformation type, ITypeFullyQualified keyMember, AttributeInfo attributeInfo)
   {
      Type = type;
      KeyMember = keyMember;
      AttributeInfo = attributeInfo;
   }

   public bool Equals(KeyedSerializerGeneratorState other)
   {
      return TypeInformationComparer.Instance.Equals(Type, other.Type)
             && KeyMember.TypeFullyQualified == other.KeyMember.TypeFullyQualified
             && AttributeInfo.Equals(other.AttributeInfo);
   }

   public override bool Equals(object? obj)
   {
      return obj is KeyedSerializerGeneratorState other && Equals(other);
   }

   public override int GetHashCode()
   {
      unchecked
      {
         var hashCode = TypeInformationComparer.Instance.GetHashCode(Type);
         hashCode = (hashCode * 397) ^ KeyMember.TypeFullyQualified.GetHashCode();
         hashCode = (hashCode * 397) ^ AttributeInfo.GetHashCode();

         return hashCode;
      }
   }
}
