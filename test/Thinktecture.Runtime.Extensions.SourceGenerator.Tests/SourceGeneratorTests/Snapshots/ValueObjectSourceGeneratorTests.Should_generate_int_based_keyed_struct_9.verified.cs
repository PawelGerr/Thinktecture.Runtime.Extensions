﻿// <auto-generated />
#nullable enable

namespace Thinktecture.Tests;

partial struct TestValueObject :
   global::System.Numerics.IDivisionOperators<global::Thinktecture.Tests.TestValueObject, global::Thinktecture.Tests.TestValueObject, global::Thinktecture.Tests.TestValueObject>
{
   /// <inheritdoc cref="global::System.Numerics.IDivisionOperators{TSelf, TOther, TResult}.op_Division(TSelf, TOther)" />
   public static global::Thinktecture.Tests.TestValueObject operator /(global::Thinktecture.Tests.TestValueObject left, global::Thinktecture.Tests.TestValueObject right)
   {
      return Create((left._value / right._value));
   }

   /// <inheritdoc cref="global::System.Numerics.IDivisionOperators{TSelf, TOther, TResult}.op_Division(TSelf, TOther)" />
   public static global::Thinktecture.Tests.TestValueObject operator checked /(global::Thinktecture.Tests.TestValueObject left, global::Thinktecture.Tests.TestValueObject right)
   {
      return Create(checked((left._value / right._value)));
   }
}
