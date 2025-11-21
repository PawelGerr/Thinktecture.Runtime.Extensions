using System;

namespace Thinktecture.Runtime.Tests.TestValueObjects;

// ReSharper disable once InconsistentNaming
[ValueObject<int>]
[ObjectFactory<string>]
public partial class IntBasedReferenceValueObject_With_StringBasedObjectFactory
{
   public static ValidationError? Validate(string? value, IFormatProvider? provider, out IntBasedReferenceValueObject_With_StringBasedObjectFactory? item)
   {
      return Validate(Int32.Parse(value!), provider, out item);
   }
}
