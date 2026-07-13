using System;

namespace Thinktecture.Runtime.Tests.TestEnums;

// String-keyed Smart Enum whose object factory serializes via a non-string (int) value type for System.Text.Json.
// This models the configuration in which an object factory with a value type different from the key type has
// serialization priority. The runtime converter selection must honor that priority instead of using the
// span-based (string) converter that a string-keyed Smart Enum would otherwise get.
// ReSharper disable once InconsistentNaming
[SmartEnum<string>]
[ObjectFactory<int>(UseForSerialization = SerializationFrameworks.SystemTextJson)]
public partial class SmartEnum_StringBased_WithIntObjectFactoryForJson
{
   public static readonly SmartEnum_StringBased_WithIntObjectFactoryForJson Item1 = new("Item1", 1);
   public static readonly SmartEnum_StringBased_WithIntObjectFactoryForJson Item2 = new("Item2", 2);

   public int Number { get; }

   public static ValidationError? Validate(int value, IFormatProvider? provider, out SmartEnum_StringBased_WithIntObjectFactoryForJson? item)
   {
      item = value switch
      {
         1 => Item1,
         2 => Item2,
         _ => null
      };

      return item is null ? new ValidationError($"Unknown item '{value}'") : null;
   }

   public int ToValue()
   {
      return Number;
   }
}
