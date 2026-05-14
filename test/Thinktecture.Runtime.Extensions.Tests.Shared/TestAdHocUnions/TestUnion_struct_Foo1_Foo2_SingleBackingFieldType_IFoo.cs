namespace Thinktecture.Runtime.Tests.TestAdHocUnions;

// ReSharper disable once InconsistentNaming
[Union<Foo1, Foo2>(
   SingleBackingFieldType = typeof(IFoo),
   SwitchMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads,
   MapMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads)]
public readonly partial struct TestUnion_struct_Foo1_Foo2_SingleBackingFieldType_IFoo;
