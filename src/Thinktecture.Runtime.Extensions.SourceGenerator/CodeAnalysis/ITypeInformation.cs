namespace Thinktecture.CodeAnalysis;

public interface ITypeInformation
{
   string? Namespace { get; }
   string Name { get; }

   string TypeFullyQualified { get; }
   string TypeMinimallyQualified { get; }
   string TypeFullyQualifiedNullAnnotated { get; }
   bool IsReferenceType { get; }
}
