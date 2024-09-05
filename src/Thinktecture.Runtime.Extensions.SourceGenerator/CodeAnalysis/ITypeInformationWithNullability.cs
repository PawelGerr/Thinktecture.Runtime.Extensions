namespace Thinktecture.CodeAnalysis;

public interface ITypeInformationWithNullability : ITypeFullyQualified
{
   bool IsReferenceType { get; }
   NullableAnnotation NullableAnnotation { get; }
   bool IsNullableStruct { get; }
}
