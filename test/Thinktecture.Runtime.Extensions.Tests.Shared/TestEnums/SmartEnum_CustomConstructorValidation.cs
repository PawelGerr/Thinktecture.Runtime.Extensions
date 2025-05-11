using System;

namespace Thinktecture.Runtime.Tests.TestEnums;

// ReSharper disable once InconsistentNaming
[SmartEnum<string>]
public partial class SmartEnum_CustomConstructorValidation
{
   public static readonly SmartEnum_CustomConstructorValidation Item1 = new("");

   static partial void ValidateConstructorArguments(ref string key)
   {
      if (String.IsNullOrWhiteSpace(key))
         throw new ArgumentException("Key cannot be empty.");
   }
}
