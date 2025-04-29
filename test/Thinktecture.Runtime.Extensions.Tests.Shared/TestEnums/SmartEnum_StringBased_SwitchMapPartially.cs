namespace Thinktecture.Runtime.Tests.TestEnums;

// ReSharper disable once InconsistentNaming
[SmartEnum<string>(
   SwitchMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads,
   MapMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads)]
public partial class SmartEnum_StringBased_SwitchMapPartially
{
   public static readonly SmartEnum_StringBased_SwitchMapPartially Item1 = new("Item1");
   public static readonly SmartEnum_StringBased_SwitchMapPartially Item2 = new("Item2");
}
