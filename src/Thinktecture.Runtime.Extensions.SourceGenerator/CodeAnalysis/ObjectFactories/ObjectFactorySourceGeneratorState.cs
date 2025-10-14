namespace Thinktecture.CodeAnalysis.ObjectFactories;

public sealed class ObjectFactorySourceGeneratorState :
   IParsableTypeInformation,
   IKeyedSerializerGeneratorTypeInformation,
   IHasGenerics,
   IEquatable<ObjectFactorySourceGeneratorState>
{
   public AttributeInfo AttributeInfo { get; }
   public string? Namespace { get; }
   public string Name { get; }
   public string TypeFullyQualified { get; }
   public bool IsRecord { get; }
   public bool IsReferenceType { get; }
   public bool IsStruct { get; }
   public bool IsRefStruct { get; }
   public NullableAnnotation NullableAnnotation { get; }
   public bool IsNullableStruct { get; }
   public bool SkipIParsable { get; }
   public IReadOnlyList<ContainingTypeState> ContainingTypes { get; }
   public IReadOnlyList<GenericTypeParameterState> GenericParameters { get; }

   public int NumberOfGenerics => GenericParameters.Count;

   public bool IsTypeParameter => false;

   public ObjectFactorySourceGeneratorState(
      INamedTypeSymbol type,
      AttributeInfo attributeInfo,
      AttributeData? thinktectureComponentAttribute)
   {
      AttributeInfo = attributeInfo;

      Name = type.Name;
      Namespace = type.ContainingNamespace?.IsGlobalNamespace == true ? null : type.ContainingNamespace?.ToString();
      TypeFullyQualified = type.ToFullyQualifiedDisplayString();
      IsRecord = type.IsRecord;
      IsReferenceType = type.IsReferenceType;
      IsStruct = type.IsValueType;
      IsRefStruct = type is { IsRefLikeType: true, IsReferenceType: false };
      NullableAnnotation = type.NullableAnnotation;
      IsNullableStruct = type.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T;
      ContainingTypes = type.GetContainingTypes();
      GenericParameters = type.GetGenericTypeParameters();
      SkipIParsable = thinktectureComponentAttribute?.FindSkipIParsable() ?? false;
   }

   public override bool Equals(object? obj)
   {
      return obj is ObjectFactorySourceGeneratorState enumSettings && Equals(enumSettings);
   }

   public bool Equals(ObjectFactorySourceGeneratorState other)
   {
      return TypeFullyQualified == other.TypeFullyQualified
             && IsRecord == other.IsRecord
             && IsReferenceType == other.IsReferenceType
             && IsStruct == other.IsStruct
             && IsRefStruct == other.IsRefStruct
             && AttributeInfo.Equals(other.AttributeInfo)
             && GenericParameters.SequenceEqual(other.GenericParameters)
             && ContainingTypes.SequenceEqual(other.ContainingTypes);
   }

   public override int GetHashCode()
   {
      unchecked
      {
         var hashCode = TypeFullyQualified.GetHashCode();
         hashCode = (hashCode * 397) ^ IsRecord.GetHashCode();
         hashCode = (hashCode * 397) ^ IsReferenceType.GetHashCode();
         hashCode = (hashCode * 397) ^ IsStruct.GetHashCode();
         hashCode = (hashCode * 397) ^ IsRefStruct.GetHashCode();
         hashCode = (hashCode * 397) ^ AttributeInfo.GetHashCode();
         hashCode = (hashCode * 397) ^ GenericParameters.ComputeHashCode();
         hashCode = (hashCode * 397) ^ ContainingTypes.ComputeHashCode();

         return hashCode;
      }
   }
}
