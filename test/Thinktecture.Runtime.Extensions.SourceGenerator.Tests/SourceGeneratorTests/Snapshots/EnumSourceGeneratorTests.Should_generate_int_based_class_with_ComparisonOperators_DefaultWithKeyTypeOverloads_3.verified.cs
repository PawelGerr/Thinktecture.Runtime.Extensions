﻿// <auto-generated />
#nullable enable

namespace Thinktecture.Tests;

partial class TestEnum :
   global::System.Numerics.IComparisonOperators<global::Thinktecture.Tests.TestEnum, global::Thinktecture.Tests.TestEnum, bool>,
   global::System.Numerics.IComparisonOperators<global::Thinktecture.Tests.TestEnum, int, bool>
{
   /// <inheritdoc cref="global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_LessThan(TSelf, TOther)" />
   public static bool operator <(global::Thinktecture.Tests.TestEnum left, global::Thinktecture.Tests.TestEnum right)
   {
      global::System.ArgumentNullException.ThrowIfNull(nameof(left));
      global::System.ArgumentNullException.ThrowIfNull(nameof(right));
      return left.Key < right.Key;
   }

   /// <inheritdoc cref="global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_LessThanOrEqual(TSelf, TOther)" />
   public static bool operator <=(global::Thinktecture.Tests.TestEnum left, global::Thinktecture.Tests.TestEnum right)
   {
      global::System.ArgumentNullException.ThrowIfNull(nameof(left));
      global::System.ArgumentNullException.ThrowIfNull(nameof(right));
      return left.Key <= right.Key;
   }

   /// <inheritdoc cref="global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_GreaterThan(TSelf, TOther)" />
   public static bool operator >(global::Thinktecture.Tests.TestEnum left, global::Thinktecture.Tests.TestEnum right)
   {
      global::System.ArgumentNullException.ThrowIfNull(nameof(left));
      global::System.ArgumentNullException.ThrowIfNull(nameof(right));
      return left.Key > right.Key;
   }

   /// <inheritdoc cref="global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_GreaterThanOrEqual(TSelf, TOther)" />
   public static bool operator >=(global::Thinktecture.Tests.TestEnum left, global::Thinktecture.Tests.TestEnum right)
   {
      global::System.ArgumentNullException.ThrowIfNull(nameof(left));
      global::System.ArgumentNullException.ThrowIfNull(nameof(right));
      return left.Key >= right.Key;
   }

   /// <inheritdoc cref="global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_LessThan(TSelf, TOther)" />
   public static bool operator <(global::Thinktecture.Tests.TestEnum left, int right)
   {
      global::System.ArgumentNullException.ThrowIfNull(nameof(left));
      return left.Key < right;
   }

   /// <inheritdoc cref="global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_LessThan(TSelf, TOther)" />
   public static bool operator <(int left, global::Thinktecture.Tests.TestEnum right)
   {
      global::System.ArgumentNullException.ThrowIfNull(nameof(right));
      return left < right.Key;
   }

   /// <inheritdoc cref="global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_LessThanOrEqual(TSelf, TOther)" />
   public static bool operator <=(global::Thinktecture.Tests.TestEnum left, int right)
   {
      global::System.ArgumentNullException.ThrowIfNull(nameof(left));
      return left.Key <= right;
   }

   /// <inheritdoc cref="global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_LessThanOrEqual(TSelf, TOther)" />
   public static bool operator <=(int left, global::Thinktecture.Tests.TestEnum right)
   {
      global::System.ArgumentNullException.ThrowIfNull(nameof(right));
      return left <= right.Key;
   }

   /// <inheritdoc cref="global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_GreaterThan(TSelf, TOther)" />
   public static bool operator >(global::Thinktecture.Tests.TestEnum left, int right)
   {
      global::System.ArgumentNullException.ThrowIfNull(nameof(left));
      return left.Key > right;
   }

   /// <inheritdoc cref="global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_GreaterThan(TSelf, TOther)" />
   public static bool operator >(int left, global::Thinktecture.Tests.TestEnum right)
   {
      global::System.ArgumentNullException.ThrowIfNull(nameof(right));
      return left > right.Key;
   }

   /// <inheritdoc cref="global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_GreaterThanOrEqual(TSelf, TOther)" />
   public static bool operator >=(global::Thinktecture.Tests.TestEnum left, int right)
   {
      global::System.ArgumentNullException.ThrowIfNull(nameof(left));
      return left.Key >= right;
   }

   /// <inheritdoc cref="global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_GreaterThanOrEqual(TSelf, TOther)" />
   public static bool operator >=(int left, global::Thinktecture.Tests.TestEnum right)
   {
      global::System.ArgumentNullException.ThrowIfNull(nameof(right));
      return left >= right.Key;
   }
}