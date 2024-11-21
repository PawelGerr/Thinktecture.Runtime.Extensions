﻿// <auto-generated />
#nullable enable

namespace Thinktecture.Tests;

partial class TestValueObject :
   global::System.IParsable<global::Thinktecture.Tests.TestValueObject>
{
   private static global::Thinktecture.ValidationError? Validate<T>(string key, global::System.IFormatProvider? provider, out global::Thinktecture.Tests.TestValueObject? result)
      where T : global::Thinktecture.IValueObjectFactory<global::Thinktecture.Tests.TestValueObject, string, global::Thinktecture.ValidationError>
   {
      return T.Validate(key, provider, out result);
   }

   /// <inheritdoc />
   public static global::Thinktecture.Tests.TestValueObject Parse(string s, global::System.IFormatProvider? provider)
   {
      var validationError = Validate<global::Thinktecture.Tests.TestValueObject>(s, provider, out var result);

      if(validationError is null)
         return result!;

      throw new global::System.FormatException(validationError.ToString() ?? "Unable to parse \"TestValueObject\".");
   }

   /// <inheritdoc />
   public static bool TryParse(
      string? s,
      global::System.IFormatProvider? provider,
      [global::System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)] out global::Thinktecture.Tests.TestValueObject result)
   {
      if(s is null)
      {
         result = default;
         return false;
      }

      var validationError = Validate<global::Thinktecture.Tests.TestValueObject>(s, provider, out result!);
      return validationError is null;
   }
}