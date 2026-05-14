namespace Thinktecture.Runtime.Tests.TestAdHocUnions;

// ReSharper disable once InconsistentNaming
[Union<EmptyFooStruct, Foo1>(
   SingleBackingFieldType = typeof(IFoo),
   T1IsStateless = true,
   SwitchMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads,
   MapMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads)]
public readonly partial struct TestUnion_struct_with_StatelessStruct_member_SingleBackingFieldType;
