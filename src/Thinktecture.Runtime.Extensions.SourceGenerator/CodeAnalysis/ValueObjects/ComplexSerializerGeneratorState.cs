namespace Thinktecture.CodeAnalysis.ValueObjects;

public readonly struct ComplexSerializerGeneratorState : IEquatable<ComplexSerializerGeneratorState>, INamespaceAndName
{
   public ITypeInformation Type { get; }
   public IReadOnlyList<InstanceMemberInfo> AssignableInstanceFieldsAndProperties { get; }
   public AttributeInfo AttributeInfo { get; }
   public SerializationFrameworks SerializationFrameworks { get; }

   public string? Namespace => Type.Namespace;
   public IReadOnlyList<ContainingTypeState> ContainingTypes => Type.ContainingTypes;
   public string Name => Type.Name;
   public int NumberOfGenerics => 0;

   public ComplexSerializerGeneratorState(
      ITypeInformation type,
      IReadOnlyList<InstanceMemberInfo> assignableInstanceFieldsAndProperties,
      AttributeInfo attributeInfo,
      SerializationFrameworks serializationFrameworks)
   {
      Type = type;
      AssignableInstanceFieldsAndProperties = assignableInstanceFieldsAndProperties;
      AttributeInfo = attributeInfo;
      SerializationFrameworks = serializationFrameworks;
   }

   public bool Equals(ComplexSerializerGeneratorState other)
   {
      return TypeInformationComparer.Instance.Equals(Type, other.Type)
             && SerializationFrameworks == other.SerializationFrameworks
             && AssignableInstanceFieldsAndProperties.SequenceEqual(other.AssignableInstanceFieldsAndProperties)
             && ContainingTypes.SequenceEqual(other.ContainingTypes)
             && AttributeInfo.Equals(other.AttributeInfo);
   }

   public override bool Equals(object? obj)
   {
      return obj is ComplexSerializerGeneratorState other && Equals(other);
   }

   public override int GetHashCode()
   {
      unchecked
      {
         var hashCode = TypeInformationComparer.Instance.GetHashCode(Type);
         hashCode = (hashCode * 397) ^ (int)SerializationFrameworks;
         hashCode = (hashCode * 397) ^ AssignableInstanceFieldsAndProperties.ComputeHashCode();
         hashCode = (hashCode * 397) ^ ContainingTypes.ComputeHashCode();
         hashCode = (hashCode * 397) ^ AttributeInfo.GetHashCode();
         return hashCode;
      }
   }
}
