namespace Thinktecture.Runtime.Tests.TestEnums;

[SmartEnum<string>(SwitchMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads,
                   MapMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads,
                   SwitchMapStateParameterName = "context")]
public partial class TestEnumWithCustomSwitchMapStateParameterName
{
   public static readonly TestEnumWithCustomSwitchMapStateParameterName Item1 = new("item1");
   public static readonly TestEnumWithCustomSwitchMapStateParameterName Item2 = new("item2");
}
