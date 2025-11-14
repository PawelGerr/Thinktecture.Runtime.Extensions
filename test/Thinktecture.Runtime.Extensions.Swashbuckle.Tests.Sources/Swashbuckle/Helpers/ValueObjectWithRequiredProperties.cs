using System;
using System.ComponentModel.DataAnnotations;
using Thinktecture.Runtime.Tests.TestValueObjects;

namespace Thinktecture.Runtime.Tests.Swashbuckle.Helpers;

[ComplexValueObject(DefaultStringComparison = StringComparison.OrdinalIgnoreCase)]
public partial class ValueObjectWithRequiredProperties
{
   public IntBasedStructValueObjectDoesNotAllowDefaultStructs KeyedStruct { get; }   // required
   public ComplexValueObjectDoesNotAllowDefaultStructsWithInt ComplexStruct { get; } // required
   public string NonNullableReferenceType { get; }                                   // required
   public string? NullableReferenceType { get; }                                     // not required
   public int ValueType { get; }                                                     // not required
   public int? NullableValueType { get; }                                            // not required

   [Required]
   public IntBasedStructValueObjectDoesNotAllowDefaultStructs KeyedStructWithRequiredAttribute { get; } // required

   [Required]
   public ComplexValueObjectDoesNotAllowDefaultStructsWithInt ComplexStructWithRequiredAttribute { get; } // required

   [Required]
   public string NonNullableReferenceTypeWithRequiredAttribute { get; } // required

   [Required]
   public string? NullableReferenceTypeWithRequiredAttribute { get; } // not required

   [Required]
   public int ValueTypeWithRequiredAttribute { get; } // not required

   [Required]
   public int? NullableValueTypeWithRequiredAttribute { get; } // not required
}
