namespace Thinktecture.CodeAnalysis;

public interface INamespaceAndName
{
   string? Namespace { get; }
   string Name { get; }
   ImmutableArray<ContainingTypeState> ContainingTypes { get; }
   int NumberOfGenerics { get; }
}
