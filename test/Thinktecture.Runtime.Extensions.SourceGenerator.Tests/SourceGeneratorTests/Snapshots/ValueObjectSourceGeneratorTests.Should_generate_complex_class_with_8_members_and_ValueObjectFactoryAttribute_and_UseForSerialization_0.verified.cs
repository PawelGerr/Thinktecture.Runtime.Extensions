﻿// <auto-generated />
#nullable enable

namespace Thinktecture.Tests
{
   sealed partial class TestValueObject : global::System.IEquatable<global::Thinktecture.Tests.TestValueObject?>,
      global::System.Numerics.IEqualityOperators<global::Thinktecture.Tests.TestValueObject, global::Thinktecture.Tests.TestValueObject, bool>,
      global::Thinktecture.IComplexValueObject,
      global::Thinktecture.IValueObjectFactory<global::Thinktecture.Tests.TestValueObject, string, global::Thinktecture.ValidationError>,
      global::Thinktecture.IValueObjectConvertable<string>
   {
      [global::System.Runtime.CompilerServices.ModuleInitializer]
      internal static void ModuleInit()
      {
         global::System.Linq.Expressions.Expression<global::System.Func<TestValueObject, object>> action = o => new
                                                                                                            {
                                                                                                               o._stringValue,
                                                                                                               o._intValue,
                                                                                                               o.ReferenceProperty,
                                                                                                               o.NullableReferenceProperty,
                                                                                                               o.StructProperty,
                                                                                                               o.NullableStructProperty
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
         string @stringValue,
         int @intValue,
         string @referenceProperty,
         string? @nullableReferenceProperty,
         int @structProperty,
         int? @nullableStructProperty,
         out global::Thinktecture.Tests.TestValueObject? obj)
      {
         global::Thinktecture.ValidationError? validationError = null;
         ValidateFactoryArguments(
            ref validationError,
            ref @stringValue,
            ref @intValue,
            ref @referenceProperty,
            ref @nullableReferenceProperty,
            ref @structProperty,
            ref @nullableStructProperty);

         if (validationError is null)
         {
            obj = new global::Thinktecture.Tests.TestValueObject(
               @stringValue,
               @intValue,
               @referenceProperty,
               @nullableReferenceProperty,
               @structProperty,
               @nullableStructProperty);
            obj.FactoryPostInit();
         }
         else
         {
            obj = default;
         }

         return validationError;
      }

      public static global::Thinktecture.Tests.TestValueObject Create(
         string @stringValue,
         int @intValue,
         string @referenceProperty,
         string? @nullableReferenceProperty,
         int @structProperty,
         int? @nullableStructProperty)
      {
         var validationError = Validate(
            @stringValue,
            @intValue,
            @referenceProperty,
            @nullableReferenceProperty,
            @structProperty,
            @nullableStructProperty,
            out global::Thinktecture.Tests.TestValueObject? obj);

         if (validationError is not null)
            throw new global::System.ComponentModel.DataAnnotations.ValidationException(validationError.ToString() ?? "Validation failed.");

         return obj!;
      }

      public static bool TryCreate(
         string @stringValue,
         int @intValue,
         string @referenceProperty,
         string? @nullableReferenceProperty,
         int @structProperty,
         int? @nullableStructProperty,
         [global::System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out global::Thinktecture.Tests.TestValueObject? obj)
      {
         return TryCreate(
            @stringValue,
            @intValue,
            @referenceProperty,
            @nullableReferenceProperty,
            @structProperty,
            @nullableStructProperty,
            out obj,
            out _);
      }

      public static bool TryCreate(
         string @stringValue,
         int @intValue,
         string @referenceProperty,
         string? @nullableReferenceProperty,
         int @structProperty,
         int? @nullableStructProperty,
         [global::System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out global::Thinktecture.Tests.TestValueObject? obj,
         [global::System.Diagnostics.CodeAnalysis.NotNullWhen(false)] out global::Thinktecture.ValidationError? validationError)
      {
         validationError = Validate(
            @stringValue,
            @intValue,
            @referenceProperty,
            @nullableReferenceProperty,
            @structProperty,
            @nullableStructProperty,
            out obj);

         return validationError is null;
      }

      static partial void ValidateFactoryArguments(
         ref global::Thinktecture.ValidationError? validationError,
         [global::System.Diagnostics.CodeAnalysis.AllowNullAttribute, global::System.Diagnostics.CodeAnalysis.NotNullAttribute] ref string @stringValue,
         ref int @intValue,
         [global::System.Diagnostics.CodeAnalysis.AllowNullAttribute, global::System.Diagnostics.CodeAnalysis.NotNullAttribute] ref string @referenceProperty,
         ref string? @nullableReferenceProperty,
         ref int @structProperty,
         ref int? @nullableStructProperty);

      partial void FactoryPostInit();

      private TestValueObject(
         string @stringValue,
         int @intValue,
         string @referenceProperty,
         string? @nullableReferenceProperty,
         int @structProperty,
         int? @nullableStructProperty)
      {
         ValidateConstructorArguments(
            ref @stringValue,
            ref @intValue,
            ref @referenceProperty,
            ref @nullableReferenceProperty,
            ref @structProperty,
            ref @nullableStructProperty);

         this._stringValue = @stringValue;
         this._intValue = @intValue;
         this.ReferenceProperty = @referenceProperty;
         this.NullableReferenceProperty = @nullableReferenceProperty;
         this.StructProperty = @structProperty;
         this.NullableStructProperty = @nullableStructProperty;
      }

      static partial void ValidateConstructorArguments(
         ref string @stringValue,
         ref int @intValue,
         ref string @referenceProperty,
         ref string? @nullableReferenceProperty,
         ref int @structProperty,
         ref int? @nullableStructProperty);

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

         return global::Thinktecture.ComparerAccessors.StringOrdinalIgnoreCase.EqualityComparer.Equals(this._stringValue, other._stringValue)
             && global::Thinktecture.ComparerAccessors.Default<int>.EqualityComparer.Equals(this._intValue, other._intValue);
      }

      /// <inheritdoc />
      public override int GetHashCode()
      {
         var hashCode = new global::System.HashCode();
         hashCode.Add(_typeHashCode);
         hashCode.Add(this._stringValue, global::Thinktecture.ComparerAccessors.StringOrdinalIgnoreCase.EqualityComparer);
         hashCode.Add(this._intValue, global::Thinktecture.ComparerAccessors.Default<int>.EqualityComparer);
         return hashCode.ToHashCode();
      }

      /// <inheritdoc />
      public override string ToString()
      {
         return $"{{ _stringValue = {this._stringValue}, _intValue = {this._intValue} }}";
      }
   }
}