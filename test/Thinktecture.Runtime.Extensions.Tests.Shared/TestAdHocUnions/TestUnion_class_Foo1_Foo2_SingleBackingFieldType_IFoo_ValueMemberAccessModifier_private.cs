namespace Thinktecture.Runtime.Tests.TestAdHocUnions;

// ReSharper disable once InconsistentNaming
[Union<Foo1, Foo2>(
   SingleBackingFieldType = typeof(IFoo),
   ValueMemberAccessModifier = AccessModifier.Private,
   SwitchMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads,
   MapMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads)]
public partial class TestUnion_class_Foo1_Foo2_SingleBackingFieldType_IFoo_ValueMemberAccessModifier_private
{
   // Gates the typed-value getter (Value typed as IFoo) under a non-public access modifier.
   public IFoo? ReadValueInternally() => Value;
}
