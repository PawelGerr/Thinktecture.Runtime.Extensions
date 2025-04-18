namespace Thinktecture.Runtime.Tests.TestRegularUnions;

[Union(SwitchMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads,
       MapMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads,
       SwitchMapStateParameterName = "context")]
public partial record TestUnionWithCustomSwitchMapStateParameterName
{
   public string Name { get; set; }

   private TestUnionWithCustomSwitchMapStateParameterName(string name)
   {
      Name = name;
   }

   public sealed record Child1(string Name) : TestUnionWithCustomSwitchMapStateParameterName(Name);

   public sealed record Child2(string Name) : TestUnionWithCustomSwitchMapStateParameterName(Name);
}
