namespace Thinktecture.Runtime.Tests.TestEnums;

// ReSharper disable once InconsistentNaming
[SmartEnum<string>(MapMethods = SwitchMapMethodsGeneration.None)]
public partial class SmartEnum_without_MapMethods
{
   public static readonly SmartEnum_without_MapMethods Item1 = new("Item1");
   public static readonly SmartEnum_without_MapMethods Item2 = new("Item2");
}
