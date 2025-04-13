namespace Thinktecture.CodeAnalysis;

public interface ITypeInformation : INamespaceAndName, ITypeInformationWithNullability, ITypeMinimallyQualified
{
   bool IsEqualWithReferenceEquality { get; }
   bool DisallowsDefaultValue { get; }
}
