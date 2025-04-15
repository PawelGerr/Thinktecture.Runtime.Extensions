using System.Collections.Generic;

namespace Thinktecture.Runtime.Tests.TestUnions;

[Union]
public partial record TestUnionWithInterface
{
   public sealed record Child1(IReadOnlyList<string> List) : TestUnionWithInterface;
}
