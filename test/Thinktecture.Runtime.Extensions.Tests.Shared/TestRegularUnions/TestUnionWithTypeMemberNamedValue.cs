namespace Thinktecture.Runtime.Tests.TestRegularUnions;

// The derived type is named so that its generated callback parameter renders to "value", which is the name of the
// pattern local of the generated switch bodies. This type exists to make the generated code fail to compile if the
// generator ever emits that local without avoiding the collision.
[Union(SwitchMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads,
       MapMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads)]
public partial record TestUnionWithTypeMemberNamedValue
{
   public sealed record Value(string Name) : TestUnionWithTypeMemberNamedValue;

   public sealed record Other(string Name) : TestUnionWithTypeMemberNamedValue;
}
