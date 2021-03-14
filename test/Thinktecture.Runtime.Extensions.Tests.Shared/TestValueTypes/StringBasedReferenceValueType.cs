using System;
using System.ComponentModel.DataAnnotations;

namespace Thinktecture.Runtime.Tests.TestValueTypes
{
   [ValueType]
   public partial class StringBasedReferenceValueType
   {
      public string Property { get; }

      static partial void ValidateFactoryArguments(ref ValidationResult? validationResult, ref string property)
      {
         if (String.IsNullOrWhiteSpace(property))
         {
            validationResult = new ValidationResult("Property cannot be empty.");
            return;
         }

         if (property.Length == 1)
         {
            validationResult = new ValidationResult("Property cannot be 1 character long.");
            return;
         }
      }
   }
}
