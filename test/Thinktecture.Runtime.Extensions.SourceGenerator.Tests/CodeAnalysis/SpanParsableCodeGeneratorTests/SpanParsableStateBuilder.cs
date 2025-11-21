using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using NSubstitute;
using Thinktecture.CodeAnalysis;

#nullable enable

namespace Thinktecture.Runtime.Tests.CodeAnalysis.SpanParsableCodeGeneratorTests;

/// <summary>
/// Builder for creating ParsableGeneratorState instances for ISpanParsable testing.
/// This builder focuses on types that support span-based parsing.
/// </summary>
public class SpanParsableStateBuilder
{
   private IParsableTypeInformation? _type;
   private IParsableMemberInformation? _keyMember;
   private ValidationErrorState _validationError = ValidationErrorState.Default;
   private bool _isEnum;
   private bool _hasStringBasedValidateMethod;
   private bool _hasReadOnlySpanOfCharBasedValidateMethod;
   private string? _genericTypeParameters; // e.g. "<T>" or "<TKey, TValue>"
   private string? _genericConstraints;    // e.g. "where T : IComparable<T>"

   public SpanParsableStateBuilder WithType(string typeFullyQualified, string name = "TestType")
   {
      _type = CreateType(typeFullyQualified, name);
      return this;
   }

   public SpanParsableStateBuilder WithStringKeyMember(string name = "Key")
   {
      _keyMember = CreateKeyMember(name, "global::System.String", SpecialType.System_String);
      return this;
   }

   public SpanParsableStateBuilder WithIntKeyMember(string name = "Key")
   {
      _keyMember = CreateKeyMember(name, "global::System.Int32", SpecialType.System_Int32);
      return this;
   }

   public SpanParsableStateBuilder WithGuidKeyMember(string name = "Key")
   {
      _keyMember = CreateKeyMember(name, "global::System.Guid", SpecialType.None);
      return this;
   }

   public SpanParsableStateBuilder WithDecimalKeyMember(string name = "Key")
   {
      _keyMember = CreateKeyMember(name, "global::System.Decimal", SpecialType.System_Decimal);
      return this;
   }

   public SpanParsableStateBuilder WithDateTimeKeyMember(string name = "Key")
   {
      _keyMember = CreateKeyMember(name, "global::System.DateTime", SpecialType.System_DateTime);
      return this;
   }

   public SpanParsableStateBuilder WithLongKeyMember(string name = "Key")
   {
      _keyMember = CreateKeyMember(name, "global::System.Int64", SpecialType.System_Int64);
      return this;
   }

   public SpanParsableStateBuilder WithValidationError(string typeFullyQualified)
   {
      _validationError = new ValidationErrorState(typeFullyQualified);
      return this;
   }

   public SpanParsableStateBuilder WithIsEnum(bool isEnum = true)
   {
      _isEnum = isEnum;
      return this;
   }

   public SpanParsableStateBuilder WithIsKeyMemberParsable(bool isParsable = true)
   {
      if (_keyMember is null)
         throw new InvalidOperationException("Key member not defined yet");

      _keyMember.IsParsable.Returns(isParsable);
      return this;
   }

   public SpanParsableStateBuilder WithHasStringBasedValidateMethod(bool hasStringBasedValidateMethod = true)
   {
      _hasStringBasedValidateMethod = hasStringBasedValidateMethod;
      return this;
   }

   public SpanParsableStateBuilder WithHasReadOnlySpanOfCharBasedValidateMethod(bool hasReadOnlySpanOfCharBasedValidateMethod = true)
   {
      _hasReadOnlySpanOfCharBasedValidateMethod = hasReadOnlySpanOfCharBasedValidateMethod;
      return this;
   }

   public SpanParsableStateBuilder WithGenericType(string typeFullyQualified, string name, string genericParameters, string? genericConstraints = null)
   {
      _type = CreateType(typeFullyQualified + genericParameters, name + genericParameters);
      _genericTypeParameters = genericParameters;
      _genericConstraints = genericConstraints;
      return this;
   }

   public SpanParsableGeneratorState Build()
   {
      _type ??= CreateType("global::Thinktecture.Tests.TestType", "TestType");

      var genericParameters = ParseGenericParameters(_genericTypeParameters, _genericConstraints);

      // For ISpanParsable, create SpanParsableGeneratorState
      return new SpanParsableGeneratorState(
         _type,
         _keyMember,
         _validationError,
         skipIParsable: false,     // We're testing ISpanParsable, so don't skip
         skipISpanParsable: false, // We're testing ISpanParsable, so don't skip
         _isEnum,
         _hasStringBasedValidateMethod,
         _hasReadOnlySpanOfCharBasedValidateMethod,
         genericParameters);
   }

   private static ImmutableArray<GenericTypeParameterState> ParseGenericParameters(string? genericTypeParameters, string? genericConstraints)
   {
      if (string.IsNullOrWhiteSpace(genericTypeParameters))
         return ImmutableArray<GenericTypeParameterState>.Empty;

      // Parse generic parameter names from "<T>" or "<TKey, TValue>"
      var trimmed = genericTypeParameters.Trim();
      if (!trimmed.StartsWith("<") || !trimmed.EndsWith(">"))
         return ImmutableArray<GenericTypeParameterState>.Empty;

      var parameterNames = trimmed.Substring(1, trimmed.Length - 2)
                                  .Split(',')
                                  .Select(p => p.Trim())
                                  .Where(p => !string.IsNullOrWhiteSpace(p))
                                  .ToList();

      if (parameterNames.Count == 0)
         return ImmutableArray<GenericTypeParameterState>.Empty;

      // Parse constraints from "where T : class" or "where T : class where TValue : IEquatable<TValue>"
      var constraintsMap = new Dictionary<string, List<string>>();

      if (!string.IsNullOrWhiteSpace(genericConstraints))
      {
         // Split by "where" keyword
         var whereClauses = genericConstraints.Split(new[] { "where" }, StringSplitOptions.RemoveEmptyEntries);

         foreach (var whereClause in whereClauses)
         {
            var colonIndex = whereClause.IndexOf(':');
            if (colonIndex == -1)
               continue;

            var paramName = whereClause.Substring(0, colonIndex).Trim();
            var constraintsStr = whereClause.Substring(colonIndex + 1).Trim();

            // Split constraints by comma (handling nested generics is complex, so we keep it simple for now)
            var constraints = constraintsStr.Split(',')
                                            .Select(c => c.Trim())
                                            .Where(c => !string.IsNullOrWhiteSpace(c))
                                            .ToList();

            constraintsMap[paramName] = constraints;
         }
      }

      // Build GenericTypeParameterState for each parameter
      var builder = ImmutableArray.CreateBuilder<GenericTypeParameterState>(parameterNames.Count);

      foreach (var paramName in parameterNames)
      {
         var constraints = constraintsMap.TryGetValue(paramName, out var list)
                              ? list.ToImmutableArray()
                              : ImmutableArray<string>.Empty;

         builder.Add(new GenericTypeParameterState(paramName, constraints));
      }

      return builder.ToImmutable();
   }

   private static IParsableTypeInformation CreateType(string typeFullyQualified, string name)
   {
      var type = Substitute.For<IParsableTypeInformation>();
      type.TypeFullyQualified.Returns(typeFullyQualified);
      type.Name.Returns(name);
      type.Namespace.Returns("Thinktecture.Tests");
      type.IsReferenceType.Returns(true);
      type.NullableAnnotation.Returns(NullableAnnotation.None);

      return type;
   }

   private static IParsableMemberInformation CreateKeyMember(string name, string typeFullyQualified, SpecialType specialType)
   {
      var member = Substitute.For<IParsableMemberInformation>();
      member.Name.Returns(name);
      member.TypeFullyQualified.Returns(typeFullyQualified);
      member.SpecialType.Returns(specialType);
      member.IsReferenceType.Returns(specialType == SpecialType.System_String);
      member.NullableAnnotation.Returns(NullableAnnotation.None);
      member.IsParsable.Returns(true);
      member.IsSpanParsable.Returns(true);

      return member;
   }
}
