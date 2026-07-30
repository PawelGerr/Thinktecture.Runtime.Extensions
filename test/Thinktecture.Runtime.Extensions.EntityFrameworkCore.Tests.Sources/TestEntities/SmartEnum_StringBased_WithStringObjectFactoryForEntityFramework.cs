namespace Thinktecture.Runtime.Tests.TestEntities;

// String-keyed Smart Enum whose object factory has the same value type as the key and is flagged for Entity
// Framework Core. The factory controls the persisted value, which may differ from the item keys, so the key-based
// max-length strategy must not be applied even though the value type and the key type match.
// ReSharper disable once InconsistentNaming
[SmartEnum<string>]
[ObjectFactory<string>(UseWithEntityFramework = true)]
public partial class SmartEnum_StringBased_WithStringObjectFactoryForEntityFramework
{
   public static readonly SmartEnum_StringBased_WithStringObjectFactoryForEntityFramework Item1 = new("Item1");
   public static readonly SmartEnum_StringBased_WithStringObjectFactoryForEntityFramework Item2 = new("Item2");
}
