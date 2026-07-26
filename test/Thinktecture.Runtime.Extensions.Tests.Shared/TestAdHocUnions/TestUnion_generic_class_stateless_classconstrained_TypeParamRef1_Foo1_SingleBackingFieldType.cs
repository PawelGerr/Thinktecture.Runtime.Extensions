namespace Thinktecture.Runtime.Tests.TestAdHocUnions;

// ReSharper disable once InconsistentNaming
[Union<TypeParamRef1, Foo1>(
   SingleBackingFieldType = typeof(IFoo),
   T1IsStateless = true,
   SwitchMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads,
   MapMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads)]
public partial class TestUnion_generic_class_stateless_classconstrained_TypeParamRef1_Foo1_SingleBackingFieldType<T>
   where T : class;
