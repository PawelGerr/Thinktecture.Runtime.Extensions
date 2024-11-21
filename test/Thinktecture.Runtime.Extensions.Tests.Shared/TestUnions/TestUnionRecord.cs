namespace Thinktecture.Runtime.Tests.TestUnions;

[Union(SwitchMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads,
       MapMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads)]
public partial record TestUnionRecord
{
   public string Name { get; set; }

   private TestUnionRecord(string name)
   {
      Name = name;
   }

   public sealed record Child1(string Name) : TestUnionRecord(Name);

   public sealed record Child2(string Name) : TestUnionRecord(Name);
}
