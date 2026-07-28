namespace Thinktecture.Runtime.Tests.TestEnums;

// The items are named so that their generated callback parameters render to "default" and "state", which are the
// names of the fixed Switch/Map parameters. This type exists to make the generated code fail to compile if the
// generator ever emits those fixed names without avoiding the collision.
// ReSharper disable once InconsistentNaming
[SmartEnum<int>(SwitchMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads,
                MapMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads)]
public partial class SmartEnum_ItemsCollidingWithSwitchMapParameters
{
   public static readonly SmartEnum_ItemsCollidingWithSwitchMapParameters Default = new(1);
   public static readonly SmartEnum_ItemsCollidingWithSwitchMapParameters State = new(2);
}
