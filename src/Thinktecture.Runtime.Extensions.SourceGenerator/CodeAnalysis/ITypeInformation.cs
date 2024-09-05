namespace Thinktecture.CodeAnalysis;

public interface ITypeInformation : INamespaceAndName, ITypeInformationWithNullability
{
   string TypeMinimallyQualified { get; }
   bool IsEqualWithReferenceEquality { get; }
}
