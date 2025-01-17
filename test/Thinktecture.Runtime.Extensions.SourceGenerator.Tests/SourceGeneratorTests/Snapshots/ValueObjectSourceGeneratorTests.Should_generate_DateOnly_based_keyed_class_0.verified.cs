﻿// <auto-generated />
#nullable enable

namespace Thinktecture.Tests
{
   [global::System.ComponentModel.TypeConverter(typeof(global::Thinktecture.ValueObjectTypeConverter<global::Thinktecture.Tests.TestValueObject, global::System.DateOnly, global::Thinktecture.ValidationError>))]
   sealed partial class TestValueObject : global::System.IEquatable<global::Thinktecture.Tests.TestValueObject?>,
      global::Thinktecture.IKeyedValueObject<global::System.DateOnly>,
      global::Thinktecture.IValueObjectConvertable<global::System.DateOnly>,
      global::Thinktecture.IValueObjectFactory<global::Thinktecture.Tests.TestValueObject, global::System.DateOnly, global::Thinktecture.ValidationError>
   {
      [global::System.Runtime.CompilerServices.ModuleInitializer]
      internal static void ModuleInit()
      {
         global::System.Func<global::System.DateOnly, global::Thinktecture.Tests.TestValueObject> convertFromKey = new (global::Thinktecture.Tests.TestValueObject.Create);
         global::System.Linq.Expressions.Expression<global::System.Func<global::System.DateOnly, global::Thinktecture.Tests.TestValueObject>> convertFromKeyExpression = static @value => global::Thinktecture.Tests.TestValueObject.Create(@value);
         global::System.Linq.Expressions.Expression<global::System.Func<global::System.DateOnly, global::Thinktecture.Tests.TestValueObject>> convertFromKeyExpressionViaCtor = static @value => new global::Thinktecture.Tests.TestValueObject(@value);

         var convertToKey = new global::System.Func<global::Thinktecture.Tests.TestValueObject, global::System.DateOnly>(static item => item._value);
         global::System.Linq.Expressions.Expression<global::System.Func<global::Thinktecture.Tests.TestValueObject, global::System.DateOnly>> convertToKeyExpression = static obj => obj._value;

         var type = typeof(global::Thinktecture.Tests.TestValueObject);
         var metadata = new global::Thinktecture.Internal.KeyedValueObjectMetadata(type, typeof(global::System.DateOnly), false, false, convertFromKey, convertFromKeyExpression, convertFromKeyExpressionViaCtor, convertToKey, convertToKeyExpression);

         global::Thinktecture.Internal.KeyedValueObjectMetadataLookup.AddMetadata(type, metadata);
      }

      private static readonly int _typeHashCode = typeof(global::Thinktecture.Tests.TestValueObject).GetHashCode();

      /// <summary>
      /// The identifier of this object.
      /// </summary>
      private readonly global::System.DateOnly _value;

      public static global::Thinktecture.ValidationError? Validate(global::System.DateOnly @value, global::System.IFormatProvider? @provider, out global::Thinktecture.Tests.TestValueObject? obj)
      {
         global::Thinktecture.ValidationError? validationError = null;
         ValidateFactoryArguments(ref validationError, ref @value);

         if (validationError is null)
         {
            obj = new global::Thinktecture.Tests.TestValueObject(@value);
            obj.FactoryPostInit();
         }
         else
         {
            obj = default;
         }

         return validationError;
      }

      public static global::Thinktecture.Tests.TestValueObject Create(global::System.DateOnly @value)
      {
         var validationError = Validate(@value, null, out global::Thinktecture.Tests.TestValueObject? obj);

         if (validationError is not null)
            throw new global::System.ComponentModel.DataAnnotations.ValidationException(validationError.ToString() ?? "Validation failed.");

         return obj!;
      }

      public static bool TryCreate(global::System.DateOnly @value, [global::System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out global::Thinktecture.Tests.TestValueObject? obj)
      {
         return TryCreate(@value, out obj, out _);
      }

      public static bool TryCreate(
         global::System.DateOnly @value,
         [global::System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out global::Thinktecture.Tests.TestValueObject? obj,
         [global::System.Diagnostics.CodeAnalysis.NotNullWhen(false)] out global::Thinktecture.ValidationError? validationError)
      {
         validationError = Validate(@value, null, out obj);

         return validationError is null;
      }

      static partial void ValidateFactoryArguments(ref global::Thinktecture.ValidationError? validationError, ref global::System.DateOnly @value);

      partial void FactoryPostInit();

      /// <summary>
      /// Gets the identifier of the item.
      /// </summary>
      [global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
      global::System.DateOnly global::Thinktecture.IValueObjectConvertable<global::System.DateOnly>.ToValue()
      {
         return this._value;
      }

      /// <summary>
      /// Implicit conversion to the type <see cref="global::System.DateOnly"/>.
      /// </summary>
      /// <param name="obj">Object to covert.</param>
      /// <returns>The <see cref="_value"/> of provided <paramref name="obj"/> or <c>default</c> if <paramref name="obj"/> is <c>null</c>.</returns>
      [return: global::System.Diagnostics.CodeAnalysis.NotNullIfNotNull("obj")]
      public static implicit operator global::System.DateOnly?(global::Thinktecture.Tests.TestValueObject? obj)
      {
         return obj?._value;
      }

      /// <summary>
      /// Explicit conversion to the type <see cref="global::System.DateOnly"/>.
      /// </summary>
      /// <param name="obj">Object to covert.</param>
      /// <returns>The <see cref="_value"/> of provided <paramref name="obj"/>.</returns>
      /// <exception cref="System.NullReferenceException">If <paramref name="obj"/> is <c>null</c>.</exception>
      [return: global::System.Diagnostics.CodeAnalysis.NotNullIfNotNull("obj")]
      public static explicit operator global::System.DateOnly(global::Thinktecture.Tests.TestValueObject obj)
      {
         if(obj is null)
            throw new global::System.NullReferenceException();

         return obj._value;
      }

      /// <summary>
      /// Explicit conversion from the type <see cref="global::System.DateOnly"/>.
      /// </summary>
      /// <param name="value">Value to covert.</param>
      /// <returns>An instance of <see cref="TestValueObject"/>.</returns>
      public static explicit operator global::Thinktecture.Tests.TestValueObject(global::System.DateOnly @value)
      {
         return global::Thinktecture.Tests.TestValueObject.Create(@value);
      }

      private TestValueObject(global::System.DateOnly @value)
      {
         ValidateConstructorArguments(ref @value);

         this._value = @value;
      }

      static partial void ValidateConstructorArguments(ref global::System.DateOnly @value);

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
