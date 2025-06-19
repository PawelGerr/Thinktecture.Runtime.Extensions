#nullable enable
using System;

namespace Thinktecture.Runtime.Tests.TestEntities;

[ComplexValueObject(DefaultStringComparison = StringComparison.OrdinalIgnoreCase)]
[ObjectFactory<string>(
   UseWithEntityFramework = true,
   HasCorrespondingConstructor = true)]
// ReSharper disable once InconsistentNaming
public partial class TestComplexValueObject_ObjectFactory_and_Constructor
{
   public string Property1 { get; }
   public string Property2 { get; }

   private TestComplexValueObject_ObjectFactory_and_Constructor(string value)
   {
      // Ctor bypasses validation, "Validate" of the ObjectFactory does not.
      (Property1, Property2) = SplitValue(value);
   }

   static partial void ValidateFactoryArguments(
      ref ValidationError? validationError,
      ref string property1,
      ref string property2)
   {
      if (String.IsNullOrWhiteSpace(property1) || String.IsNullOrWhiteSpace(property2))
      {
         validationError = new ValidationError("Both properties must be non-empty.");
      }
   }

   public static ValidationError? Validate(
      string? value,
      IFormatProvider? provider,
      out TestComplexValueObject_ObjectFactory_and_Constructor? item)
   {
      if (value is null)
      {
         item = null;
         return new ValidationError("Value cannot be null.");
      }

      var values = SplitValue(value);

      return Validate(values.Property1, values.Property2, out item);
   }

   private static (string Property1, string Property2) SplitValue(string value)
   {
      var parts = value.Split('|');

      return parts.Length switch
      {
         0 => (String.Empty, String.Empty),
         1 => (parts[0], string.Empty),
         _ => (parts[0], parts[1]),
      };
   }

   public string ToValue()
   {
      return $"{Property1}|{Property2}";
   }
}
