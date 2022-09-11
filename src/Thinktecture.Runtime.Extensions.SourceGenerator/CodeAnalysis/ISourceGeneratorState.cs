using Microsoft.CodeAnalysis;

namespace Thinktecture.CodeAnalysis;

public interface ISourceGeneratorState : ITypeInformation
{
   Location GetFirstLocation(CancellationToken cancellationToken);
}
