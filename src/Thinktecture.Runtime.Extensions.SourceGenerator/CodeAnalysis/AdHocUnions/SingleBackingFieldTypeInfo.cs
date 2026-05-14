namespace Thinktecture.CodeAnalysis.AdHocUnions;

public readonly record struct SingleBackingFieldTypeInfo(
   string FullyQualified,
   bool IsReferenceType,
   bool IsNullableStruct);
