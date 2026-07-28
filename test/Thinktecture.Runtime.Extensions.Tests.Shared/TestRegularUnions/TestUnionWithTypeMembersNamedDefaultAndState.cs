namespace Thinktecture.Runtime.Tests.TestRegularUnions;

// The derived types are named so that their generated callback parameters render to "default" and "state", which are
// the names of the fixed Switch/Map parameters. This type exists to make the generated code fail to compile if the
// generator ever emits those fixed names without avoiding the collision.
[Union(SwitchMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads,
       MapMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads)]
public partial record TestUnionWithTypeMembersNamedDefaultAndState
{
   public sealed record Default(string Name) : TestUnionWithTypeMembersNamedDefaultAndState;

   public sealed record State(string Name) : TestUnionWithTypeMembersNamedDefaultAndState;
}
