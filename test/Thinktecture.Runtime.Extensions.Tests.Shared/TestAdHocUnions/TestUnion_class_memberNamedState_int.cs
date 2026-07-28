namespace Thinktecture.Runtime.Tests.TestAdHocUnions;

// The first member type is named so that its generated callback parameter renders to "state", which is the name of
// the fixed Switch/Map state parameter. This type exists to make the generated code fail to compile if the generator
// ever emits that fixed name without avoiding the collision.
// ReSharper disable once InconsistentNaming
[Union<CollidingMemberTypes.State, int>(
   SwitchMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads,
   MapMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads)]
public partial class TestUnion_class_memberNamedState_int;
