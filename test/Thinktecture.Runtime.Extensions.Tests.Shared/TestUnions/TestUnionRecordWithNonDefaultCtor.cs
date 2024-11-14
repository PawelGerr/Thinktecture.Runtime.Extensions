namespace Thinktecture.Runtime.Tests.TestUnions;

[Union]
public partial record TestUnionRecordWithNonDefaultCtor
{
   public string Name { get; }

   private TestUnionRecordWithNonDefaultCtor(string name)
   {
      Name = name;
   }

   public sealed record Child1(string Name) : TestUnionRecordWithNonDefaultCtor(Name);

   public sealed record Child2(string Name) : TestUnionRecordWithNonDefaultCtor(Name);
}
