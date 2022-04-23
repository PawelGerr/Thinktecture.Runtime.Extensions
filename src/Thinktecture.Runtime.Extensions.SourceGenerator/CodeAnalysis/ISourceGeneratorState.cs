using Microsoft.CodeAnalysis;

namespace Thinktecture.CodeAnalysis;

public interface ISourceGeneratorState
{
   string? Namespace { get; }
   string Name { get; }

   Location GetFirstLocation();
}
