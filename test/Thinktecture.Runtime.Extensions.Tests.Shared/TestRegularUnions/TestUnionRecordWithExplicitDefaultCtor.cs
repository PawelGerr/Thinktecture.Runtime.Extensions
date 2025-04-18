namespace Thinktecture.Runtime.Tests.TestRegularUnions;

[Union]
public partial record TestUnionRecordWithExplicitDefaultCtor
{
   private TestUnionRecordWithExplicitDefaultCtor()
   {
   }

   public sealed record Child1(string Name) : TestUnionRecordWithExplicitDefaultCtor;

   public sealed record Child2(string Name) : TestUnionRecordWithExplicitDefaultCtor;
}
