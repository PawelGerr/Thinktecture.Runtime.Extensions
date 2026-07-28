namespace Thinktecture.Runtime.Tests.TestAdHocUnions;

public static class CollidingMemberTypes
{
   public sealed class Default;

   public sealed class State;
}

// The first member type is named so that its generated callback parameter renders to "default", which is the name of
// the fixed Switch/Map parameter. This type exists to make the generated code fail to compile if the generator ever
// emits that fixed name without avoiding the collision.
// ReSharper disable once InconsistentNaming
[Union<CollidingMemberTypes.Default, int>(
   SwitchMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads,
   MapMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads)]
public partial class TestUnion_class_memberNamedDefault_int;
