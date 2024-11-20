﻿// <auto-generated />
#nullable enable

partial class TestEnum :
   global::System.Numerics.IComparisonOperators<global::TestEnum, global::TestEnum, bool>
{

   /// <inheritdoc cref="global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_LessThan(TSelf, TOther)" />
   public static bool operator <(global::TestEnum left, global::TestEnum right)
   {
      global::System.ArgumentNullException.ThrowIfNull(nameof(left));
      global::System.ArgumentNullException.ThrowIfNull(nameof(right));
      return global::System.StringComparer.OrdinalIgnoreCase.Compare(left.Key, right.Key) < 0;
   }

   /// <inheritdoc cref="global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_LessThanOrEqual(TSelf, TOther)" />
   public static bool operator <=(global::TestEnum left, global::TestEnum right)
   {
      global::System.ArgumentNullException.ThrowIfNull(nameof(left));
      global::System.ArgumentNullException.ThrowIfNull(nameof(right));
      return global::System.StringComparer.OrdinalIgnoreCase.Compare(left.Key, right.Key) <= 0;
   }

   /// <inheritdoc cref="global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_GreaterThan(TSelf, TOther)" />
   public static bool operator >(global::TestEnum left, global::TestEnum right)
   {
      global::System.ArgumentNullException.ThrowIfNull(nameof(left));
      global::System.ArgumentNullException.ThrowIfNull(nameof(right));
      return global::System.StringComparer.OrdinalIgnoreCase.Compare(left.Key, right.Key) > 0;
   }

   /// <inheritdoc cref="global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_GreaterThanOrEqual(TSelf, TOther)" />
   public static bool operator >=(global::TestEnum left, global::TestEnum right)
   {
      global::System.ArgumentNullException.ThrowIfNull(nameof(left));
      global::System.ArgumentNullException.ThrowIfNull(nameof(right));
      return global::System.StringComparer.OrdinalIgnoreCase.Compare(left.Key, right.Key) >= 0;
   }
}
