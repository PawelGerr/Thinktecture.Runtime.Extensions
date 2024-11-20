﻿// <auto-generated />
#nullable enable

namespace Thinktecture.Tests;

partial class TestValueObject :
   global::System.Numerics.IAdditionOperators<global::Thinktecture.Tests.TestValueObject, global::Thinktecture.Tests.TestValueObject, global::Thinktecture.Tests.TestValueObject>,
   global::System.Numerics.IAdditionOperators<global::Thinktecture.Tests.TestValueObject, int, global::Thinktecture.Tests.TestValueObject>
{
   /// <inheritdoc cref="global::System.Numerics.IAdditionOperators{TSelf, TOther, TResult}.op_Addition(TSelf, TOther)" />
   public static global::Thinktecture.Tests.TestValueObject operator +(global::Thinktecture.Tests.TestValueObject left, global::Thinktecture.Tests.TestValueObject right)
   {
      global::System.ArgumentNullException.ThrowIfNull(nameof(left));
      global::System.ArgumentNullException.ThrowIfNull(nameof(right));
      return Create((left._value + right._value));
   }

   /// <inheritdoc cref="global::System.Numerics.IAdditionOperators{TSelf, TOther, TResult}.op_Addition(TSelf, TOther)" />
   public static global::Thinktecture.Tests.TestValueObject operator checked +(global::Thinktecture.Tests.TestValueObject left, global::Thinktecture.Tests.TestValueObject right)
   {
      global::System.ArgumentNullException.ThrowIfNull(nameof(left));
      global::System.ArgumentNullException.ThrowIfNull(nameof(right));
      return Create(checked((left._value + right._value)));
   }

   /// <inheritdoc cref="global::System.Numerics.IAdditionOperators{TSelf, TOther, TResult}.op_Addition(TSelf, TOther)" />
   public static global::Thinktecture.Tests.TestValueObject operator +(global::Thinktecture.Tests.TestValueObject left, int right)
   {
      global::System.ArgumentNullException.ThrowIfNull(nameof(left));
      return Create((left._value + right));
   }

   /// <inheritdoc cref="global::System.Numerics.IAdditionOperators{TSelf, TOther, TResult}.op_Addition(TSelf, TOther)" />
   public static global::Thinktecture.Tests.TestValueObject operator +(int left, global::Thinktecture.Tests.TestValueObject right)
   {
      global::System.ArgumentNullException.ThrowIfNull(nameof(right));
      return Create((left + right._value));
   }

   /// <inheritdoc cref="global::System.Numerics.IAdditionOperators{TSelf, TOther, TResult}.op_Addition(TSelf, TOther)" />
   public static global::Thinktecture.Tests.TestValueObject operator checked +(global::Thinktecture.Tests.TestValueObject left, int right)
   {
      global::System.ArgumentNullException.ThrowIfNull(nameof(left));
      return Create(checked((left._value + right)));
   }

   /// <inheritdoc cref="global::System.Numerics.IAdditionOperators{TSelf, TOther, TResult}.op_Addition(TSelf, TOther)" />
   public static global::Thinktecture.Tests.TestValueObject operator checked +(int left, global::Thinktecture.Tests.TestValueObject right)
   {
      global::System.ArgumentNullException.ThrowIfNull(nameof(right));
      return Create(checked((left + right._value)));
   }
}
