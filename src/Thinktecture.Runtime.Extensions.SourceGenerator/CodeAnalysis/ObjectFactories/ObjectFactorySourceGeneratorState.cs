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
   public bool IsValueType { get; }
   public bool IsRefStruct { get; }
   public NullableAnnotation NullableAnnotation { get; }
   public bool IsNullableStruct { get; }
   public bool HasParsableKeyMember { get; }
   public bool SkipIParsable { get; }
   public bool SkipISpanParsable { get; }
   public ImmutableArray<ContainingTypeState> ContainingTypes { get; }
   public ImmutableArray<GenericTypeParameterState> GenericParameters { get; }

   public int NumberOfGenerics => GenericParameters.Length;
   public bool IsTypeParameter => false;

   public ObjectFactorySourceGeneratorState(
      INamedTypeSymbol type,
      AttributeInfo attributeInfo,
      AttributeData? thinktectureComponentAttribute,
      ITypedMemberState? keyMember)
   {
      AttributeInfo = attributeInfo;

      Name = type.Name;
      Namespace = type.ContainingNamespace?.IsGlobalNamespace == true ? null : type.ContainingNamespace?.ToString();
      TypeFullyQualified = type.ToFullyQualifiedDisplayString();
      IsRecord = type.IsRecord;
      IsReferenceType = type.IsReferenceType;
      IsValueType = type.IsValueType;
      IsRefStruct = type is { IsRefLikeType: true, IsReferenceType: false };
      NullableAnnotation = type.NullableAnnotation;
      IsNullableStruct = type.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T;
      ContainingTypes = type.GetContainingTypes();
      GenericParameters = type.GetGenericTypeParameters();
      SkipIParsable = thinktectureComponentAttribute?.FindSkipIParsable() ?? false;
      SkipISpanParsable = (thinktectureComponentAttribute?.FindSkipISpanParsable() ?? false) || SkipIParsable;
      HasParsableKeyMember = keyMember?.IsParsable ?? false;
   }

   public override bool Equals(object? obj)
   {
      return obj is ObjectFactorySourceGeneratorState enumSettings && Equals(enumSettings);
   }

   public bool Equals(ObjectFactorySourceGeneratorState? other)
   {
      if (other is null)
         return false;

      return TypeFullyQualified == other.TypeFullyQualified
             && IsRecord == other.IsRecord
             && IsReferenceType == other.IsReferenceType
             && IsValueType == other.IsValueType
             && IsRefStruct == other.IsRefStruct
             && NullableAnnotation == other.NullableAnnotation
             && HasParsableKeyMember == other.HasParsableKeyMember
             && SkipIParsable == other.SkipIParsable
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
         hashCode = (hashCode * 397) ^ IsValueType.GetHashCode();
         hashCode = (hashCode * 397) ^ IsRefStruct.GetHashCode();
         hashCode = (hashCode * 397) ^ (int)NullableAnnotation;
         hashCode = (hashCode * 397) ^ HasParsableKeyMember.GetHashCode();
         hashCode = (hashCode * 397) ^ SkipIParsable.GetHashCode();
         hashCode = (hashCode * 397) ^ AttributeInfo.GetHashCode();
         hashCode = (hashCode * 397) ^ GenericParameters.ComputeHashCode();
         hashCode = (hashCode * 397) ^ ContainingTypes.ComputeHashCode();

         return hashCode;
      }
   }
}
