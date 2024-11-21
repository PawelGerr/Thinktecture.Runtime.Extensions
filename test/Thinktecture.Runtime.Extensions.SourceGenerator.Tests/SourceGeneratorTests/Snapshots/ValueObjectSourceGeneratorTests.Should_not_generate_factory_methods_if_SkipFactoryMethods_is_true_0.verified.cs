﻿// <auto-generated />
#nullable enable

namespace Thinktecture.Tests
{
   sealed partial class TestValueObject : global::System.IEquatable<global::Thinktecture.Tests.TestValueObject?>,
      global::Thinktecture.IKeyedValueObject<int>,
      global::Thinktecture.IValueObjectConvertable<int>
   {
      [global::System.Runtime.CompilerServices.ModuleInitializer]
      internal static void ModuleInit()
      {
         global::System.Func<int, global::Thinktecture.Tests.TestValueObject>? convertFromKey = null;
         global::System.Linq.Expressions.Expression<global::System.Func<int, global::Thinktecture.Tests.TestValueObject>>? convertFromKeyExpression = null;
         global::System.Linq.Expressions.Expression<global::System.Func<int, global::Thinktecture.Tests.TestValueObject>> convertFromKeyExpressionViaCtor = static @value => new global::Thinktecture.Tests.TestValueObject(@value);

         var convertToKey = new global::System.Func<global::Thinktecture.Tests.TestValueObject, int>(static item => item._value);
         global::System.Linq.Expressions.Expression<global::System.Func<global::Thinktecture.Tests.TestValueObject, int>> convertToKeyExpression = static obj => obj._value;

         var type = typeof(global::Thinktecture.Tests.TestValueObject);
         var metadata = new global::Thinktecture.Internal.KeyedValueObjectMetadata(type, typeof(int), false, false, convertFromKey, convertFromKeyExpression, convertFromKeyExpressionViaCtor, convertToKey, convertToKeyExpression);

         global::Thinktecture.Internal.KeyedValueObjectMetadataLookup.AddMetadata(type, metadata);
      }

      private static readonly int _typeHashCode = typeof(global::Thinktecture.Tests.TestValueObject).GetHashCode();

      /// <summary>
      /// The identifier of this object.
      /// </summary>
      private readonly int _value;

      /// <summary>
      /// Gets the identifier of the item.
      /// </summary>
      [global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
      int global::Thinktecture.IValueObjectConvertable<int>.ToValue()
      {
         return this._value;
      }

      /// <summary>
      /// Implicit conversion to the type <see cref="int"/>.
      /// </summary>
      /// <param name="obj">Object to covert.</param>
      /// <returns>The <see cref="_value"/> of provided <paramref name="obj"/> or <c>default</c> if <paramref name="obj"/> is <c>null</c>.</returns>
      [return: global::System.Diagnostics.CodeAnalysis.NotNullIfNotNull("obj")]
      public static implicit operator int?(global::Thinktecture.Tests.TestValueObject? obj)
      {
         return obj?._value;
      }

      /// <summary>
      /// Explicit conversion to the type <see cref="int"/>.
      /// </summary>
      /// <param name="obj">Object to covert.</param>
      /// <returns>The <see cref="_value"/> of provided <paramref name="obj"/> or <c>default</c> if <paramref name="obj"/> is <c>null</c>.</returns>
      [return: global::System.Diagnostics.CodeAnalysis.NotNullIfNotNull("obj")]
      public static explicit operator int(global::Thinktecture.Tests.TestValueObject obj)
      {
         if(obj is null)
            throw new global::System.NullReferenceException();

         return obj._value;
      }

      private TestValueObject(int @value)
      {
         ValidateConstructorArguments(ref @value);

         this._value = @value;
      }

      static partial void ValidateConstructorArguments(ref int @value);

      /// <inheritdoc />
      public override bool Equals(object? other)
      {
         return other is global::Thinktecture.Tests.TestValueObject obj && Equals(obj);
      }

      /// <inheritdoc />
      public bool Equals(global::Thinktecture.Tests.TestValueObject? other)
      {
         if (other is null)
            return false;

         if (global::System.Object.ReferenceEquals(this, other))
            return true;

         return this._value.Equals(other._value);
      }

      /// <inheritdoc />
      public override int GetHashCode()
      {
         return global::System.HashCode.Combine(_typeHashCode, this._value);
      }

      /// <inheritdoc />
      public override string ToString()
      {
         return this._value.ToString();
      }
   }
}