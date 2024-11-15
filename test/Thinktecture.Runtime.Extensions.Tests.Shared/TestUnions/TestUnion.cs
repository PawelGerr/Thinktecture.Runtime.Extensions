namespace Thinktecture.Runtime.Tests.TestUnions;

[Union(SwitchMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads,
       MapMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads)]
public partial record TestUnion
{
   public string Name { get; set; }

   private TestUnion(string name)
   {
      Name = name;
   }

   public sealed record Child1(string Name) : TestUnion(Name);

   public sealed record Child2(string Name) : TestUnion(Name);
}
