using System;

namespace Thinktecture.Runtime.Tests.TestValueObjects;

// Plain, non-sealed class whose only Thinktecture feature is a standalone object factory. It implements
// IObjectFactoryOwner but not IMetadataOwner. A derived class inherits the interface but not the private static
// factory property, so MetadataLookup must walk the base types to resolve the object factories for the derived type.
// ReSharper disable once InconsistentNaming
[ObjectFactory<string>(UseForSerialization = SerializationFrameworks.All)]
public partial class ClassWithStringObjectFactory
{
   public string Value { get; }

   protected ClassWithStringObjectFactory(string value)
   {
      Value = value;
   }

   public static ValidationError? Validate(string? value, IFormatProvider? provider, out ClassWithStringObjectFactory? item)
   {
      if (String.IsNullOrWhiteSpace(value))
      {
         item = null;
         return new ValidationError("Value cannot be empty.");
      }

      item = new ClassWithStringObjectFactory(value);
      return null;
   }

   public string ToValue()
   {
      return Value;
   }
}

// ReSharper disable once InconsistentNaming
public sealed class DerivedClassWithStringObjectFactory : ClassWithStringObjectFactory
{
   public DerivedClassWithStringObjectFactory(string value)
      : base(value)
   {
   }
}
