namespace Thinktecture.Runtime.Tests.TestEnums;

// ReSharper disable once InconsistentNaming
[SmartEnum(SwitchMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads,
           MapMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads)]
public partial class SmartEnum_Keyless
{
   public static readonly SmartEnum_Keyless Item1 = new(1);
   public static readonly SmartEnum_Keyless Item2 = new(2);

   public int IntProperty { get; }
}
