using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using NSubstitute;
using Thinktecture.CodeAnalysis;

#nullable enable

namespace Thinktecture.Runtime.Tests.CodeAnalysis.KeyedMessagePackCodeGeneratorTests;

/// <summary>
/// Builder for creating KeyedSerializerGeneratorState instances for testing KeyedMessagePackCodeGenerator.
/// </summary>
public class KeyedMessagePackCodeGeneratorStateBuilder
{
   private IKeyedSerializerGeneratorTypeInformation? _type;
   private IMemberInformation? _keyMember;
   private ImmutableArray<ObjectFactoryState> _objectFactories = ImmutableArray<ObjectFactoryState>.Empty;
   private ValidationErrorState _validationError = ValidationErrorState.Default;
   private global::Thinktecture.CodeAnalysis.SerializationFrameworks _serializationFrameworks = global::Thinktecture.CodeAnalysis.SerializationFrameworks.All;
   private bool _isReferenceType = true; // Default to class
   private bool _isRecord;

   public KeyedMessagePackCodeGeneratorStateBuilder WithType(
      string typeFullyQualified,
      string name = "TestType",
      string? @namespace = "Thinktecture.Tests",
      ImmutableArray<ContainingTypeState>? containingTypes = null)
   {
      _type = CreateType(typeFullyQualified, name, @namespace, containingTypes ?? ImmutableArray<ContainingTypeState>.Empty, _isReferenceType, _isRecord);
      return this;
   }

   public KeyedMessagePackCodeGeneratorStateBuilder AsStruct()
   {
      _isReferenceType = false;
      return this;
   }

   public KeyedMessagePackCodeGeneratorStateBuilder AsClass()
   {
      _isReferenceType = true;
      return this;
   }

   public KeyedMessagePackCodeGeneratorStateBuilder AsRecord()
   {
      _isRecord = true;
      return this;
   }

   public KeyedMessagePackCodeGeneratorStateBuilder WithTypeWithoutNamespace(
      string typeFullyQualified = "global::TestType",
      string name = "TestType")
   {
      _type = CreateType(typeFullyQualified, name, null, ImmutableArray<ContainingTypeState>.Empty, _isReferenceType, _isRecord);
      return this;
   }

   public KeyedMessagePackCodeGeneratorStateBuilder WithNestedType(
      string typeFullyQualified = "global::Thinktecture.Tests.OuterClass.TestType",
      string name = "TestType",
      string? @namespace = "Thinktecture.Tests",
      params ContainingTypeState[] containingTypes)
   {
      _type = CreateType(typeFullyQualified, name, @namespace, containingTypes.ToImmutableArray(), _isReferenceType, _isRecord);
      return this;
   }

   public KeyedMessagePackCodeGeneratorStateBuilder WithStringKeyMember(string name = "Key")
   {
      _keyMember = CreateKeyMember(name, "global::System.String", SpecialType.System_String);
      return this;
   }

   public KeyedMessagePackCodeGeneratorStateBuilder WithIntKeyMember(string name = "Key")
   {
      _keyMember = CreateKeyMember(name, "global::System.Int32", SpecialType.System_Int32);
      return this;
   }

   public KeyedMessagePackCodeGeneratorStateBuilder WithGuidKeyMember(string name = "Key")
   {
      _keyMember = CreateKeyMember(name, "global::System.Guid", SpecialType.None);
      return this;
   }

   public KeyedMessagePackCodeGeneratorStateBuilder WithDecimalKeyMember(string name = "Key")
   {
      _keyMember = CreateKeyMember(name, "global::System.Decimal", SpecialType.System_Decimal);
      return this;
   }

   public KeyedMessagePackCodeGeneratorStateBuilder WithByteKeyMember(string name = "Key")
   {
      _keyMember = CreateKeyMember(name, "global::System.Byte", SpecialType.System_Byte);
      return this;
   }

   public KeyedMessagePackCodeGeneratorStateBuilder WithoutKeyMember()
   {
      _keyMember = null;
      return this;
   }

   public KeyedMessagePackCodeGeneratorStateBuilder WithCustomFactory(
      string typeFullyQualified,
      SpecialType specialType = SpecialType.None,
      global::Thinktecture.CodeAnalysis.SerializationFrameworks useForSerialization = global::Thinktecture.CodeAnalysis.SerializationFrameworks.MessagePack)
   {
      var factory = CreateObjectFactory(typeFullyQualified, specialType, useForSerialization);
      _objectFactories = _objectFactories.Add(factory);
      return this;
   }

   public KeyedMessagePackCodeGeneratorStateBuilder WithStringCustomFactory(
      global::Thinktecture.CodeAnalysis.SerializationFrameworks useForSerialization = global::Thinktecture.CodeAnalysis.SerializationFrameworks.MessagePack)
   {
      var factory = CreateObjectFactory("global::System.String", SpecialType.System_String, useForSerialization);
      _objectFactories = _objectFactories.Add(factory);
      return this;
   }

   public KeyedMessagePackCodeGeneratorStateBuilder WithValidationError(string typeFullyQualified)
   {
      _validationError = new ValidationErrorState(typeFullyQualified);
      return this;
   }

   public KeyedMessagePackCodeGeneratorStateBuilder WithSerializationFrameworks(global::Thinktecture.CodeAnalysis.SerializationFrameworks frameworks)
   {
      _serializationFrameworks = frameworks;
      return this;
   }

   public KeyedSerializerGeneratorState Build()
   {
      _type ??= CreateType("global::Thinktecture.Tests.TestType", "TestType", "Thinktecture.Tests", ImmutableArray<ContainingTypeState>.Empty, _isReferenceType, _isRecord);

      var attributeInfo = CreateAttributeInfo(_objectFactories, _validationError);

      return new KeyedSerializerGeneratorState(
         _type,
         _keyMember,
         attributeInfo,
         _serializationFrameworks,
         []);
   }

   private static IKeyedSerializerGeneratorTypeInformation CreateType(
      string typeFullyQualified,
      string name,
      string? @namespace,
      ImmutableArray<ContainingTypeState> containingTypes,
      bool isReferenceType,
      bool isRecord)
   {
      var type = Substitute.For<IKeyedSerializerGeneratorTypeInformation>();
      type.TypeFullyQualified.Returns(typeFullyQualified);
      type.Name.Returns(name);
      type.Namespace.Returns(@namespace);
      type.ContainingTypes.Returns(containingTypes);
      type.IsReferenceType.Returns(isReferenceType);
      type.IsValueType.Returns(!isReferenceType);
      type.IsRecord.Returns(isRecord);
      type.IsTypeParameter.Returns(false);
      return type;
   }

   private static IMemberInformation CreateKeyMember(string name, string typeFullyQualified, SpecialType specialType)
   {
      var member = Substitute.For<IMemberInformation>();
      member.Name.Returns(name);
      member.TypeFullyQualified.Returns(typeFullyQualified);
      member.SpecialType.Returns(specialType);
      member.IsReferenceType.Returns(specialType == SpecialType.System_String);
      member.NullableAnnotation.Returns(NullableAnnotation.None);
      return member;
   }

   private static ObjectFactoryState CreateObjectFactory(
      string typeFullyQualified,
      SpecialType specialType,
      global::Thinktecture.CodeAnalysis.SerializationFrameworks useForSerialization)
   {
      // Create a mock ITypeSymbol to pass to the constructor
      var typeSymbol = Substitute.For<ITypeSymbol>();
      typeSymbol.SpecialType.Returns(specialType);

      // Mock the ToDisplayString method to return our fully qualified string
      typeSymbol.ToDisplayString(Arg.Any<SymbolDisplayFormat>()).Returns(typeFullyQualified);

      return new ObjectFactoryState(
         typeSymbol,
         useForSerialization,
         useWithEntityFramework: false,
         useForModelBinding: false,
         hasCorrespondingConstructor: false);
   }

   private static AttributeInfo CreateAttributeInfo(
      ImmutableArray<ObjectFactoryState> objectFactories,
      ValidationErrorState validationError)
   {
      // Use reflection to create AttributeInfo since it has a private constructor
      var constructor = typeof(AttributeInfo).GetConstructors(
         System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)[0];

      return (AttributeInfo)constructor.Invoke(
      [
         false, // hasStructLayoutAttribute
         false, // hasJsonConverterAttribute
         false, // hasNewtonsoftJsonConverterAttribute
         false, // hasMessagePackFormatterAttribute
         objectFactories,
         validationError,
         null, // keyMemberComparerAccessor
         null  // keyMemberEqualityComparerAccessor
      ]);
   }
}
