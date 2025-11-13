namespace Thinktecture.CodeAnalysis.ValueObjects;

public readonly struct ComplexSerializerGeneratorState<T> : IEquatable<ComplexSerializerGeneratorState<T>>, INamespaceAndName
   where T : ITypeInformation, IHasGenerics, IEquatable<T>
{
   public T Type { get; }
   public ImmutableArray<InstanceMemberInfo> AssignableInstanceFieldsAndProperties { get; }
   public AttributeInfo AttributeInfo { get; }
   public SerializationFrameworks SerializationFrameworks { get; }

   public string? Namespace => Type.Namespace;
   public ImmutableArray<ContainingTypeState> ContainingTypes => Type.ContainingTypes;
   public string Name => Type.Name;
   public int NumberOfGenerics => Type.NumberOfGenerics;

   public ComplexSerializerGeneratorState(
      T type,
      ImmutableArray<InstanceMemberInfo> assignableInstanceFieldsAndProperties,
      AttributeInfo attributeInfo,
      SerializationFrameworks serializationFrameworks)
   {
      Type = type;
      AssignableInstanceFieldsAndProperties = assignableInstanceFieldsAndProperties;
      AttributeInfo = attributeInfo;
      SerializationFrameworks = serializationFrameworks;
   }

   public bool Equals(ComplexSerializerGeneratorState<T> other)
   {
      return Type.Equals(other.Type)
             && SerializationFrameworks == other.SerializationFrameworks
             && AssignableInstanceFieldsAndProperties.SequenceEqual(other.AssignableInstanceFieldsAndProperties)
             && ContainingTypes.SequenceEqual(other.ContainingTypes)
             && AttributeInfo.Equals(other.AttributeInfo);
   }

   public override bool Equals(object? obj)
   {
      return obj is ComplexSerializerGeneratorState<T> other && Equals(other);
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

   public static bool operator ==(ComplexSerializerGeneratorState<T> left, ComplexSerializerGeneratorState<T> right)
   {
      return left.Equals(right);
   }

   public static bool operator !=(ComplexSerializerGeneratorState<T> left, ComplexSerializerGeneratorState<T> right)
   {
      return !(left == right);
   }
}
