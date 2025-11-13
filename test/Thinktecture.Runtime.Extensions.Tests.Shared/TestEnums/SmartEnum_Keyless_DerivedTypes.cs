namespace Thinktecture.Runtime.Tests.TestEnums;

// ReSharper disable once InconsistentNaming
[SmartEnum]
public abstract partial class SmartEnum_Keyless_DerivedTypes
{
   public static readonly SmartEnum_Keyless_DerivedTypes Item1 = new DerivedEnum1();
   public static readonly SmartEnum_Keyless_DerivedTypes Item2 = new DerivedEnum2();

   private sealed class DerivedEnum1 : SmartEnum_Keyless_DerivedTypes;

   private sealed class DerivedEnum2 : SmartEnum_Keyless_DerivedTypes;
}
