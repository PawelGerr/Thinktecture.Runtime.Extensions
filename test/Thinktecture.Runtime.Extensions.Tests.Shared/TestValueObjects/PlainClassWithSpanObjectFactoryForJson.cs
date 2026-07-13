#if NET9_0_OR_GREATER
using System;

namespace Thinktecture.Runtime.Tests.TestValueObjects;

// Plain class (not a Smart Enum, Value Object, or Union) whose only System.Text.Json mechanism is a
// ReadOnlySpan<char> object factory. Such a type implements IObjectFactoryOwner but not IMetadataOwner, so
// MetadataLookup.Find returns null while FindMetadataForConversion resolves it through the object-factory
// fallback. Used to verify the runtime JSON factory keeps the span-based converter for such a type.
// ReSharper disable once InconsistentNaming
[ObjectFactory<ReadOnlySpan<char>>(UseForSerialization = SerializationFrameworks.SystemTextJson)]
public partial class PlainClassWithSpanObjectFactoryForJson
{
   public string Value { get; }

   private PlainClassWithSpanObjectFactoryForJson(string value)
   {
      Value = value;
   }

   public static ValidationError? Validate(
      ReadOnlySpan<char> value,
      IFormatProvider? provider,
      out PlainClassWithSpanObjectFactoryForJson? item)
   {
      if (value.IsEmpty)
      {
         item = null;
         return new ValidationError("Value cannot be empty.");
      }

      item = new PlainClassWithSpanObjectFactoryForJson(value.ToString());
      return null;
   }

   public ReadOnlySpan<char> ToValue()
   {
      return Value;
   }
}
#endif
