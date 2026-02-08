#if NET9_0_OR_GREATER
using System;

namespace Thinktecture.Runtime.Tests.TestValueObjects;

[ComplexValueObject(DefaultStringComparison = StringComparison.OrdinalIgnoreCase)]
[ObjectFactory<ReadOnlySpan<char>>(UseForSerialization = SerializationFrameworks.SystemTextJson)]
public partial class ComplexValueObjectWithReadOnlySpanBasedObjectFactoryForJson
{
   public string Property1 { get; }
   public string Property2 { get; }

   public static ValidationError? Validate(
      ReadOnlySpan<char> value,
      IFormatProvider? provider,
      out ComplexValueObjectWithReadOnlySpanBasedObjectFactoryForJson? item)
   {
      if (value.IsEmpty)
      {
         item = null;
         return null;
      }

      var separatorIndex = value.IndexOf('|');

      if (separatorIndex < 0)
      {
         item = null;
         return new ValidationError("Invalid format.");
      }

      var property1 = value.Slice(0, separatorIndex).ToString();
      var property2 = value.Slice(separatorIndex + 1).ToString();

      item = new(property1, property2);
      return null;
   }

   public ReadOnlySpan<char> ToValue()
   {
      return $"{Property1}|{Property2}";
   }
}
#endif
