namespace Thinktecture.CodeAnalysis;

public interface ISourceGeneratorState
{
   string? Namespace { get; }
   string Name { get; }
}
