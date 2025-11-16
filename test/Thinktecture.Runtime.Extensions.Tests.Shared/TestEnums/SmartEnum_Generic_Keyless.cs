namespace Thinktecture.Runtime.Tests.TestEnums;

// ReSharper disable once InconsistentNaming
[SmartEnum(SwitchMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads,
           MapMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads)]
public partial class SmartEnum_Generic_Keyless<T>
   where T : class
{
   public static readonly SmartEnum_Generic_Keyless<T> Item1 = new(null);
   public static readonly SmartEnum_Generic_Keyless<T> Item2 = new(null);

   public T? Value { get; }
}
