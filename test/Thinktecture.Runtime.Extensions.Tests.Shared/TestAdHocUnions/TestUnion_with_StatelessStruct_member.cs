namespace Thinktecture.Runtime.Tests.TestAdHocUnions;

public readonly struct EmptyFooStruct : IFoo
{
   public string Bar => "empty-struct";
}

// ReSharper disable once InconsistentNaming
[Union<EmptyFooStruct, Foo1>(
   SingleBackingFieldType = typeof(IFoo),
   T1IsStateless = true,
   SwitchMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads,
   MapMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads)]
public partial class TestUnion_with_StatelessStruct_member;
