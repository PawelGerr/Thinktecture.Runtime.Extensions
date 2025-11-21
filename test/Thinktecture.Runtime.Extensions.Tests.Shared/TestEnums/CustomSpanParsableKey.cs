using System;
using System.Globalization;

namespace Thinktecture.Runtime.Tests.TestEnums;

// ReSharper disable once InconsistentNaming
public readonly struct CustomSpanParsableKey : ISpanParsable<CustomSpanParsableKey>, IEquatable<CustomSpanParsableKey>
{
   public int Value { get; }

   public CustomSpanParsableKey(int value) => Value = value;

   public static CustomSpanParsableKey Parse(string s, IFormatProvider? provider)
      => new(int.Parse(s, provider));

   public static bool TryParse(string? s, IFormatProvider? provider, out CustomSpanParsableKey result)
   {
      if (int.TryParse(s, provider, out var value))
      {
         result = new CustomSpanParsableKey(value);
         return true;
      }

      result = default;
      return false;
   }

   public static CustomSpanParsableKey Parse(ReadOnlySpan<char> s, IFormatProvider? provider)
      => new(int.Parse(s, provider));

   public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out CustomSpanParsableKey result)
   {
      if (int.TryParse(s, provider, out var value))
      {
         result = new CustomSpanParsableKey(value);
         return true;
      }

      result = default;
      return false;
   }

   public bool Equals(CustomSpanParsableKey other) => Value == other.Value;
   public override bool Equals(object? obj) => obj is CustomSpanParsableKey other && Equals(other);
   public override int GetHashCode() => Value.GetHashCode();
   public override string ToString() => Value.ToString(CultureInfo.InvariantCulture);
}
