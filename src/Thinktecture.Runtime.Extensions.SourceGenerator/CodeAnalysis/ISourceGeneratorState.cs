using Microsoft.CodeAnalysis;

namespace Thinktecture.CodeAnalysis;

public interface ISourceGeneratorState
{
   Location GetFirstLocation();
}
