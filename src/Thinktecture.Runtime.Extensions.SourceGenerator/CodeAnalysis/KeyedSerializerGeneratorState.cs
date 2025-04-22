namespace Thinktecture.CodeAnalysis;

public readonly struct KeyedSerializerGeneratorState : IEquatable<KeyedSerializerGeneratorState>, INamespaceAndName
{
   public ITypeInformation Type { get; }
   public IMemberInformation? KeyMember { get; }
   public AttributeInfo AttributeInfo { get; }
   public SerializationFrameworks SerializationFrameworks { get; }

   public string? Namespace => Type.Namespace;
   public IReadOnlyList<ContainingTypeState> ContainingTypes => Type.ContainingTypes;
   public string Name => Type.Name;
   public int NumberOfGenerics => 0;

   public KeyedSerializerGeneratorState(
      ITypeInformation type,
      IMemberInformation? keyMember,
      AttributeInfo attributeInfo,
      SerializationFrameworks serializationFrameworks)
   {
      Type = type;
      KeyMember = keyMember;
      AttributeInfo = attributeInfo;
      SerializationFrameworks = serializationFrameworks;
   }

   public bool Equals(KeyedSerializerGeneratorState other)
   {
      return TypeInformationComparer.Instance.Equals(Type, other.Type)
             && KeyMember?.TypeFullyQualified == other.KeyMember?.TypeFullyQualified
             && SerializationFrameworks == other.SerializationFrameworks
             && ContainingTypes.SequenceEqual(other.ContainingTypes)
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
         hashCode = (hashCode * 397) ^ (KeyMember is null ? 0 : KeyMember.TypeFullyQualified.GetHashCode());
         hashCode = (hashCode * 397) ^ (int)SerializationFrameworks;
         hashCode = (hashCode * 397) ^ ContainingTypes.ComputeHashCode();
         hashCode = (hashCode * 397) ^ AttributeInfo.GetHashCode();

         return hashCode;
      }
   }
}
