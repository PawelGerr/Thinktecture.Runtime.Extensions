using System;

namespace Thinktecture.ValueTypes
{
   [ValueType]
   public partial class ProductName
   {
      [ValueTypeEqualityMember(Comparer = nameof(StringComparer.OrdinalIgnoreCase))]
      private string Value { get; }

      static partial void ValidateFactoryArguments(ref string value)
      {
         if (String.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Product name cannot be empty.");

         if (value.Length == 1)
            throw new FormatException("Product name cannot be 1 character long.");

         value = value.Trim();
      }
   }
}
