namespace Thinktecture.Runtime.Tests.TestEnums;

// ReSharper disable once InconsistentNaming
[SmartEnum<string>]
public partial class SmartEnum_StringBased_with_BaseClass : TestBaseClassWithNonEmptyCtor
{
   public static readonly SmartEnum_StringBased_with_BaseClass Item1 = new("Item1", "BaseClassValue1");
   public static readonly SmartEnum_StringBased_with_BaseClass Item2 = new("Item2", "BaseClassValue2");
}
