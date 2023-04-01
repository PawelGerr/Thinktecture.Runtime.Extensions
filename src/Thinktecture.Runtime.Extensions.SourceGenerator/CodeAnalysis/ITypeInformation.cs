namespace Thinktecture.CodeAnalysis;

public interface ITypeInformation : INamespaceAndName, ITypeFullyQualified
{
   string TypeMinimallyQualified { get; }
   string TypeFullyQualifiedNullAnnotated { get; }
   bool IsReferenceType { get; }
   bool IsEqualWithReferenceEquality { get; }
}
