using Microsoft.CodeAnalysis;

namespace Thinktecture.CodeAnalysis;

public interface IMemberInformation
{
   string Name { get; }
   string TypeFullyQualified { get; }
   bool IsReferenceType { get; }
   SpecialType SpecialType { get; }
}
