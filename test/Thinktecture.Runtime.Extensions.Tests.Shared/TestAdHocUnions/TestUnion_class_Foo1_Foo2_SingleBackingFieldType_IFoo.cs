namespace Thinktecture.Runtime.Tests.TestAdHocUnions;

public interface IFoo
{
   string Bar { get; }
}

public class Foo1 : IFoo
{
   public string Bar => "foo1";
   public int ExtraInt { get; init; }
}

public class Foo2 : IFoo
{
   public string Bar => "foo2";
   public string ExtraString { get; init; } = "";
}

// ReSharper disable once InconsistentNaming
[Union<Foo1, Foo2>(
   SingleBackingFieldType = typeof(IFoo),
   SwitchMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads,
   MapMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads)]
public partial class TestUnion_class_Foo1_Foo2_SingleBackingFieldType_IFoo;
