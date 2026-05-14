namespace Thinktecture.Runtime.Tests.TestAdHocUnions;

public class NullFoo : IFoo
{
   public string Bar => "null-foo";
}

// ReSharper disable once InconsistentNaming
[Union<NullFoo, Foo1>(
   SingleBackingFieldType = typeof(IFoo),
   T1IsStateless = true,
   SwitchMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads,
   MapMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads)]
public partial class TestUnion_with_StatelessRefType_member;
