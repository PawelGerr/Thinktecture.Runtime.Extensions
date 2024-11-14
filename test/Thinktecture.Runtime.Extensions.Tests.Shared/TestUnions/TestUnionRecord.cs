namespace Thinktecture.Runtime.Tests.TestUnions;

[Union]
public partial record TestUnionRecord
{
   public sealed record Child1(string Name) : TestUnionRecord;

   public sealed record Child2(string Name) : TestUnionRecord;
}
