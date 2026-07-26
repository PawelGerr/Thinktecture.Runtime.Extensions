namespace Thinktecture.Runtime.Tests.TestAdHocUnions;

public readonly struct FooStruct : IFoo
{
   public string Bar => "fooStruct";
}

// ReSharper disable once InconsistentNaming
[Union<TypeParamRef1, Foo1>(
   SingleBackingFieldType = typeof(IFoo),
   T1IsStateless = true,
   // Without a nullable member the typed backing field is non-nullable, and the constructor of the
   // stateless member leaves it unassigned, which produces CS8618 in the generated code.
   T2IsNullableReferenceType = true,
   SwitchMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads,
   MapMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads)]
public partial class TestUnion_generic_class_stateless_structconstrained_TypeParamRef1_Foo1_SingleBackingFieldType<T>
   where T : struct, IFoo;
