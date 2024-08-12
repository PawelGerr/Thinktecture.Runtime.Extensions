namespace Thinktecture.Runtime.Tests.TestEnums;

[SmartEnum<string>(SwitchMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads,
                   MapMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads)]
public sealed partial class ValidTestEnum
{
   public static readonly ValidTestEnum Item1 = new("item1");
   public static readonly ValidTestEnum Item2 = new("item2");
}
