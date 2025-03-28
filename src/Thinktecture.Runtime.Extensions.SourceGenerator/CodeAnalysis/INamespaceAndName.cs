namespace Thinktecture.CodeAnalysis;

public interface INamespaceAndName
{
   string? Namespace { get; }
   string Name { get; }
   IReadOnlyList<ContainingTypeState> ContainingTypes { get; }
   IReadOnlyList<string> GenericsFullyQualified { get; }
}
