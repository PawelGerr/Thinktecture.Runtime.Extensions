﻿// <auto-generated />
#nullable enable

namespace Thinktecture.Tests;

partial class TestEnum :
   global::System.IParsable<global::Thinktecture.Tests.TestEnum>
{
   private static global::Thinktecture.ValidationError? Validate<T>(string key, global::System.IFormatProvider? provider, out global::Thinktecture.Tests.TestEnum? result)
      where T : global::Thinktecture.IValueObjectFactory<global::Thinktecture.Tests.TestEnum, string, global::Thinktecture.ValidationError>
   {
      return T.Validate(key, provider, out result);
   }

   /// <inheritdoc />
   public static global::Thinktecture.Tests.TestEnum Parse(string s, global::System.IFormatProvider? provider)
   {
      var validationError = Validate<global::Thinktecture.Tests.TestEnum>(s, provider, out var result);

      if(validationError is null)
         return result!;

      throw new global::System.FormatException(validationError.ToString() ?? "Unable to parse \"TestEnum\".");
   }

   /// <inheritdoc />
   public static bool TryParse(
      string? s,
      global::System.IFormatProvider? provider,
      [global::System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)] out global::Thinktecture.Tests.TestEnum result)
   {
      if(s is null)
      {
         result = default;
         return false;
      }

      var validationError = Validate<global::Thinktecture.Tests.TestEnum>(s, provider, out result!);
      return validationError is null;
   }
}
