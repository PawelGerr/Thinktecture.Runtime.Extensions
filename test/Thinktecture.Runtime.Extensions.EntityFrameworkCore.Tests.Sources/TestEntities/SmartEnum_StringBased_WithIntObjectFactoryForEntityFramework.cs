using System;

#nullable enable

namespace Thinktecture.Runtime.Tests.TestEntities;

// String-keyed Smart Enum whose object factory stores an int for Entity Framework Core.
// The value converter therefore persists an int, so the key-based (string) max-length strategy must not apply.
// ReSharper disable once InconsistentNaming
[SmartEnum<string>]
[ObjectFactory<int>(UseWithEntityFramework = true)]
public partial class SmartEnum_StringBased_WithIntObjectFactoryForEntityFramework
{
   public static readonly SmartEnum_StringBased_WithIntObjectFactoryForEntityFramework Item1 = new("Item1", 1);
   public static readonly SmartEnum_StringBased_WithIntObjectFactoryForEntityFramework Item2 = new("Item2", 2);

   public int Number { get; }

   public static ValidationError? Validate(int value, IFormatProvider? provider, out SmartEnum_StringBased_WithIntObjectFactoryForEntityFramework? item)
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
