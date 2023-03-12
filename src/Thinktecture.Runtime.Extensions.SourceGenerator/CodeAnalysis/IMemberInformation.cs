using Microsoft.CodeAnalysis;

namespace Thinktecture.CodeAnalysis;

public interface IMemberInformation : ITypeFullyQualified
{
   string Name { get; }
   bool IsReferenceType { get; }
   SpecialType SpecialType { get; }
}
