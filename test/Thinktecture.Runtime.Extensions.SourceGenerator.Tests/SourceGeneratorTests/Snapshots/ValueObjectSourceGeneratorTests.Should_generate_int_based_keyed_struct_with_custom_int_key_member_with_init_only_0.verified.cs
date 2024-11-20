﻿// <auto-generated />
#nullable enable

namespace Thinktecture.Tests
{
   [global::System.Runtime.InteropServices.StructLayout(global::System.Runtime.InteropServices.LayoutKind.Auto)]
   [global::System.ComponentModel.TypeConverter(typeof(global::Thinktecture.ValueObjectTypeConverter<global::Thinktecture.Tests.TestValueObject, int, global::Thinktecture.ValidationError>))]
   readonly partial struct TestValueObject : global::System.IEquatable<global::Thinktecture.Tests.TestValueObject>,
      global::Thinktecture.IKeyedValueObject<int>,
      global::Thinktecture.IValueObjectConvertable<int>,
      global::Thinktecture.IValueObjectFactory<global::Thinktecture.Tests.TestValueObject, int, global::Thinktecture.ValidationError>
   {
      [global::System.Runtime.CompilerServices.ModuleInitializer]
      internal static void ModuleInit()
      {
         global::System.Func<int, global::Thinktecture.Tests.TestValueObject> convertFromKey = new (global::Thinktecture.Tests.TestValueObject.Create);
         global::System.Linq.Expressions.Expression<global::System.Func<int, global::Thinktecture.Tests.TestValueObject>> convertFromKeyExpression = static @value => global::Thinktecture.Tests.TestValueObject.Create(@value);
         global::System.Linq.Expressions.Expression<global::System.Func<int, global::Thinktecture.Tests.TestValueObject>> convertFromKeyExpressionViaCtor = static @value => new global::Thinktecture.Tests.TestValueObject(@value);

         var convertToKey = new global::System.Func<global::Thinktecture.Tests.TestValueObject, int>(static item => item._value);
         global::System.Linq.Expressions.Expression<global::System.Func<global::Thinktecture.Tests.TestValueObject, int>> convertToKeyExpression = static obj => obj._value;

         var type = typeof(global::Thinktecture.Tests.TestValueObject);
         var metadata = new global::Thinktecture.Internal.KeyedValueObjectMetadata(type, typeof(int), false, false, convertFromKey, convertFromKeyExpression, convertFromKeyExpressionViaCtor, convertToKey, convertToKeyExpression);

         global::Thinktecture.Internal.KeyedValueObjectMetadataLookup.AddMetadata(type, metadata);
      }

      private static readonly int _typeHashCode = typeof(global::Thinktecture.Tests.TestValueObject).GetHashCode();

      public static global::Thinktecture.ValidationError? Validate(int @value, global::System.IFormatProvider? @provider, out global::Thinktecture.Tests.TestValueObject obj)
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

      public static global::Thinktecture.Tests.TestValueObject Create(int @value)
      {
         var validationError = Validate(@value, null, out global::Thinktecture.Tests.TestValueObject obj);

         if (validationError is not null)
            throw new global::System.ComponentModel.DataAnnotations.ValidationException(validationError.ToString() ?? "Validation failed.");

         return obj!;
      }

      public static bool TryCreate(int @value, [global::System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out global::Thinktecture.Tests.TestValueObject obj)
      {
         return TryCreate(@value, out obj, out _);
      }

      public static bool TryCreate(
         int @value,
         [global::System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out global::Thinktecture.Tests.TestValueObject obj,
         [global::System.Diagnostics.CodeAnalysis.NotNullWhen(false)] out global::Thinktecture.ValidationError? validationError)
      {
         validationError = Validate(@value, null, out obj);

         return validationError is null;
      }

      static partial void ValidateFactoryArguments(ref global::Thinktecture.ValidationError? validationError, ref int @value);

      partial void FactoryPostInit();

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
      /// Implicit conversion to the type <see cref="int"/>.
      /// </summary>
      /// <param name="obj">Object to covert.</param>
      /// <returns>The <see cref="_value"/> of provided <paramref name="obj"/>.</returns>
      public static implicit operator int(global::Thinktecture.Tests.TestValueObject obj)
      {
         return obj._value;
      }

      /// <summary>
      /// Explicit conversion from the type <see cref="int"/>.
      /// </summary>
      /// <param name="value">Value to covert.</param>
      /// <returns>An instance of <see cref="TestValueObject"/>.</returns>
      public static explicit operator global::Thinktecture.Tests.TestValueObject(int @value)
      {
         return global::Thinktecture.Tests.TestValueObject.Create(@value);
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
      public bool Equals(global::Thinktecture.Tests.TestValueObject other)
      {
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
