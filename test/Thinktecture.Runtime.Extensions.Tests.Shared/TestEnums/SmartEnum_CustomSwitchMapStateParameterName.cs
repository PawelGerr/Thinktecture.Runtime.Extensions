namespace Thinktecture.Runtime.Tests.TestEnums;

// ReSharper disable once InconsistentNaming
[SmartEnum<string>(SwitchMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads,
                   MapMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads,
                   SwitchMapStateParameterName = "context")]
public partial class SmartEnum_CustomSwitchMapStateParameterName
{
   public static readonly SmartEnum_CustomSwitchMapStateParameterName Item1 = new("item1");
   public static readonly SmartEnum_CustomSwitchMapStateParameterName Item2 = new("item2");
}
