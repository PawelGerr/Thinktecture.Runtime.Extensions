namespace Thinktecture.CodeAnalysis;

public interface ITypeInformationWithNullability : ITypeFullyQualified, ITypeKindInformation
{
   NullableAnnotation NullableAnnotation { get; }
   bool IsNullableStruct { get; }
}
