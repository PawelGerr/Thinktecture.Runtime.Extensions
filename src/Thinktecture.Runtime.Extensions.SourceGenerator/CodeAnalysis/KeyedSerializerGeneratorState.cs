namespace Thinktecture.CodeAnalysis;

public readonly struct KeyedSerializerGeneratorState : IEquatable<KeyedSerializerGeneratorState>, INamespaceAndName, IHasGenerics
{
   public IKeyedSerializerGeneratorTypeInformation Type { get; }
   public IMemberInformation? KeyMember { get; }
   public AttributeInfo AttributeInfo { get; }
   public SerializationFrameworks SerializationFrameworks { get; }
   public ImmutableArray<GenericTypeParameterState> GenericParameters { get; }
   public int NumberOfGenerics => GenericParameters.Length;

   public string? Namespace => Type.Namespace;
   public ImmutableArray<ContainingTypeState> ContainingTypes => Type.ContainingTypes;
   public string Name => Type.Name;

   public KeyedSerializerGeneratorState(
      IKeyedSerializerGeneratorTypeInformation type,
      IMemberInformation? keyMember,
      AttributeInfo attributeInfo,
      SerializationFrameworks serializationFrameworks,
      ImmutableArray<GenericTypeParameterState> genericParameters)
   {
      Type = type;
      KeyMember = keyMember;
      AttributeInfo = attributeInfo;
      SerializationFrameworks = serializationFrameworks;
      GenericParameters = genericParameters;
   }

   public bool Equals(KeyedSerializerGeneratorState other)
   {
      return Type.Equals(other.Type)
             && MemberInformationComparer.Instance.Equals(KeyMember, other.KeyMember)
             && SerializationFrameworks == other.SerializationFrameworks
             && ContainingTypes.SequenceEqual(other.ContainingTypes)
             && AttributeInfo.Equals(other.AttributeInfo)
             && GenericParameters.SequenceEqual(other.GenericParameters);
   }

   public override bool Equals(object? obj)
   {
      return obj is KeyedSerializerGeneratorState other && Equals(other);
   }

   public override int GetHashCode()
   {
      unchecked
      {
         var hashCode = Type.GetHashCode();
         hashCode = (hashCode * 397) ^ (KeyMember is null ? 0 : MemberInformationComparer.Instance.GetHashCode(KeyMember));
         hashCode = (hashCode * 397) ^ (int)SerializationFrameworks;
         hashCode = (hashCode * 397) ^ ContainingTypes.ComputeHashCode();
         hashCode = (hashCode * 397) ^ AttributeInfo.GetHashCode();
         hashCode = (hashCode * 397) ^ GenericParameters.ComputeHashCode();

         return hashCode;
      }
   }

   public static bool operator ==(KeyedSerializerGeneratorState left, KeyedSerializerGeneratorState right)
   {
      return left.Equals(right);
   }

   public static bool operator !=(KeyedSerializerGeneratorState left, KeyedSerializerGeneratorState right)
   {
      return !(left == right);
   }
}
