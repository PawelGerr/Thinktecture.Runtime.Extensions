using Microsoft.CodeAnalysis;

namespace Thinktecture.CodeAnalysis;

public interface IMemberState : IEquatable<IMemberState>
{
   string Name { get; }
   string ArgumentName { get; }
   SpecialType SpecialType { get; }
   string TypeFullyQualified { get; }
   string TypeFullyQualifiedNullAnnotated { get; }
   string TypeFullyQualifiedWithNullability { get; }
   bool IsReferenceType { get; }
}
