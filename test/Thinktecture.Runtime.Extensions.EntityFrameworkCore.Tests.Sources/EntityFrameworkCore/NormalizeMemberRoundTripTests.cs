#nullable enable
using System;
using Thinktecture.EntityFrameworkCore.Storage.ValueConversion;

namespace Thinktecture.Runtime.Tests.EntityFrameworkCore;

// ReSharper disable InconsistentNaming
public class NormalizeMemberRoundTripTests
{
   [Fact]
   public void Should_normalize_value_when_reading_back_via_EF_value_converter()
   {
      // The ThinktectureValueConverter routes deserialization through T.Validate(),
      // which constructs the union via ctor / factory. Normalize fires on that path.
      // The plan asserts: "EF Core's useConstructorForRead=false skip-path does not apply
      // to ad-hoc unions (no ConvertFromKeyExpressionViaConstructor is generated for them),
      // so Normalize always fires on EF read."
      var converter = ThinktectureValueConverterFactory.Create<NormalizingEfUnion, string>();

      var convertFromProvider = converter.ConvertFromProvider;

      var converted = (NormalizingEfUnion?)convertFromProvider("  Hello  ");

      converted.Should().NotBeNull();
      converted!.AsString.Should().Be("hello");
   }

   [Fact]
   public void Should_normalize_value_when_reading_back_with_useConstructorForRead_false()
   {
      // Explicitly verifies that the useConstructorForRead=false path also normalizes
      // for ad-hoc unions (no ConvertFromKeyExpressionViaConstructor is generated for them).
      var converter = ThinktectureValueConverterFactory.Create<NormalizingEfUnion, string>(useConstructorForRead: false);

      var converted = (NormalizingEfUnion?)converter.ConvertFromProvider("  Hello  ");

      converted.Should().NotBeNull();
      converted!.AsString.Should().Be("hello");
   }
}

[Union<string, int>]
[ObjectFactory<string>(UseForSerialization = SerializationFrameworks.All)]
public partial class NormalizingEfUnion
{
   public string ToValue()
   {
      return Switch(@string: t => t,
                    @int32: n => n.ToString(System.Globalization.CultureInfo.InvariantCulture));
   }

   public static ValidationError? Validate(string? value, IFormatProvider? provider, out NormalizingEfUnion? item)
   {
      if (String.IsNullOrWhiteSpace(value))
      {
         item = null;
         return null;
      }

      item = value!;
      return null;
   }

   static partial void NormalizeString(ref string @string)
   {
      @string = @string.Trim().ToLowerInvariant();
   }
}
