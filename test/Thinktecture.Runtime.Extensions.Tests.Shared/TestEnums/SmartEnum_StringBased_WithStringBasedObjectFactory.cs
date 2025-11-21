namespace Thinktecture.Runtime.Tests.TestEnums;

// ReSharper disable once InconsistentNaming
[SmartEnum<string>]
[ObjectFactory<string>]
public partial class SmartEnum_StringBased_WithStringBasedObjectFactory
{
   public static readonly SmartEnum_StringBased_WithStringBasedObjectFactory Item1 = new("Item1");
   public static readonly SmartEnum_StringBased_WithStringBasedObjectFactory Item2 = new("Item2");
}
