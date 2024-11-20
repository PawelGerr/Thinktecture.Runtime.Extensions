﻿// <auto-generated />
#nullable enable

namespace Thinktecture.Tests
{
   sealed partial class TestValueObject : global::System.IEquatable<global::Thinktecture.Tests.TestValueObject?>,
      global::System.Numerics.IEqualityOperators<global::Thinktecture.Tests.TestValueObject, global::Thinktecture.Tests.TestValueObject, bool>,
      global::Thinktecture.IComplexValueObject
   {
      [global::System.Runtime.CompilerServices.ModuleInitializer]
      internal static void ModuleInit()
      {
         global::System.Linq.Expressions.Expression<global::System.Func<TestValueObject, object>> action = o => new
                                                                                                            {
                                                                                                            };

         var members = new global::System.Collections.Generic.List<global::System.Reflection.MemberInfo>();

         foreach (var arg in ((global::System.Linq.Expressions.NewExpression)action.Body).Arguments)
         {
            members.Add(((global::System.Linq.Expressions.MemberExpression)arg).Member);
         }

         var type = typeof(global::Thinktecture.Tests.TestValueObject);
         var metadata = new global::Thinktecture.Internal.ComplexValueObjectMetadata(type, members.AsReadOnly());

         global::Thinktecture.Internal.ComplexValueObjectMetadataLookup.AddMetadata(type, metadata);
      }

      private static readonly int _typeHashCode = typeof(global::Thinktecture.Tests.TestValueObject).GetHashCode();

      public static global::Thinktecture.ValidationError? Validate(
         out global::Thinktecture.Tests.TestValueObject? obj)
      {
         global::Thinktecture.ValidationError? validationError = null;
         var factoryArgumentsValidationError = ValidateFactoryArguments(
            ref validationError);

         if (validationError is null)
         {
            obj = new global::Thinktecture.Tests.TestValueObject();
            obj.FactoryPostInit(factoryArgumentsValidationError);
         }
         else
         {
            obj = default;
         }

         return validationError;
      }

      public static global::Thinktecture.Tests.TestValueObject Create()
      {
         var validationError = Validate(
            out global::Thinktecture.Tests.TestValueObject? obj);

         if (validationError is not null)
            throw new global::System.ComponentModel.DataAnnotations.ValidationException(validationError.ToString() ?? "Validation failed.");

         return obj!;
      }

      public static bool TryCreate(
         [global::System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out global::Thinktecture.Tests.TestValueObject? obj)
      {
         return TryCreate(
            out obj,
            out _);
      }

      public static bool TryCreate(
         [global::System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out global::Thinktecture.Tests.TestValueObject? obj,
         [global::System.Diagnostics.CodeAnalysis.NotNullWhen(false)] out global::Thinktecture.ValidationError? validationError)
      {
         validationError = Validate(
            out obj);

         return validationError is null;
      }

      private static partial int ValidateFactoryArguments(
         ref global::Thinktecture.ValidationError? validationError);

      partial void FactoryPostInit(int factoryArgumentsValidationError);

      private TestValueObject()
      {
      }

      /// <summary>
      /// Compares two instances of <see cref="TestValueObject"/>.
      /// </summary>
      /// <param name="obj">Instance to compare.</param>
      /// <param name="other">Another instance to compare.</param>
      /// <returns><c>true</c> if objects are equal; otherwise <c>false</c>.</returns>
      public static bool operator ==(global::Thinktecture.Tests.TestValueObject? obj, global::Thinktecture.Tests.TestValueObject? other)
      {
         if (obj is null)
            return other is null;

         return obj.Equals(other);
      }

      /// <summary>
      /// Compares two instances of <see cref="TestValueObject"/>.
      /// </summary>
      /// <param name="obj">Instance to compare.</param>
      /// <param name="other">Another instance to compare.</param>
      /// <returns><c>false</c> if objects are equal; otherwise <c>true</c>.</returns>
      public static bool operator !=(global::Thinktecture.Tests.TestValueObject? obj, global::Thinktecture.Tests.TestValueObject? other)
      {
         return !(obj == other);
      }

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

         return true;
      }

      /// <inheritdoc />
      public override int GetHashCode()
      {
         return _typeHashCode;
      }

      /// <inheritdoc />
      public override string ToString()
      {
         return "TestValueObject";
      }
   }
}
