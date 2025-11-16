using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using NSubstitute;
using Thinktecture.CodeAnalysis;

#nullable enable

namespace Thinktecture.Runtime.Tests.CodeAnalysis.AdditionOperatorsCodeGeneratorTests;

/// <summary>
/// Builder for creating InterfaceCodeGeneratorState instances for testing AdditionOperatorsCodeGenerator.
/// </summary>
public class AdditionOperatorsStateBuilder
{
   private ITypeInformation? _type;
   private IMemberInformation? _keyMember;
   private string _createFactoryMethodName = "Create";

   public AdditionOperatorsStateBuilder WithType(string typeFullyQualified, string name = "TestType", bool isReferenceType = true)
   {
      _type = CreateType(typeFullyQualified, name, isReferenceType);
      return this;
   }

   public AdditionOperatorsStateBuilder WithReferenceType(string typeFullyQualified, string name = "TestType")
   {
      _type = CreateType(typeFullyQualified, name, isReferenceType: true);
      return this;
   }

   public AdditionOperatorsStateBuilder WithValueType(string typeFullyQualified, string name = "TestType")
   {
      _type = CreateType(typeFullyQualified, name, isReferenceType: false);
      return this;
   }

   public AdditionOperatorsStateBuilder WithStringKeyMember(string name = "Value", bool isReferenceType = true)
   {
      _keyMember = CreateKeyMember(name, "global::System.String", SpecialType.System_String, isReferenceType);
      return this;
   }

   public AdditionOperatorsStateBuilder WithIntKeyMember(string name = "Value", bool isReferenceType = false)
   {
      _keyMember = CreateKeyMember(name, "global::System.Int32", SpecialType.System_Int32, isReferenceType);
      return this;
   }

   public AdditionOperatorsStateBuilder WithByteKeyMember(string name = "Value", bool isReferenceType = false)
   {
      _keyMember = CreateKeyMember(name, "global::System.Byte", SpecialType.System_Byte, isReferenceType);
      return this;
   }

   public AdditionOperatorsStateBuilder WithDecimalKeyMember(string name = "Value", bool isReferenceType = false)
   {
      _keyMember = CreateKeyMember(name, "global::System.Decimal", SpecialType.System_Decimal, isReferenceType);
      return this;
   }

   public AdditionOperatorsStateBuilder WithKeyMember(string name, string typeFullyQualified, SpecialType specialType = SpecialType.None, bool isReferenceType = false)
   {
      _keyMember = CreateKeyMember(name, typeFullyQualified, specialType, isReferenceType);
      return this;
   }

   public AdditionOperatorsStateBuilder WithCreateFactoryMethodName(string name)
   {
      _createFactoryMethodName = name;
      return this;
   }

   public InterfaceCodeGeneratorState Build()
   {
      _type ??= CreateType("global::Thinktecture.Tests.TestType", "TestType", isReferenceType: true);
      _keyMember ??= CreateKeyMember("Value", "global::System.Int32", SpecialType.System_Int32, isReferenceType: false);

      return new InterfaceCodeGeneratorState(_type, _keyMember, _createFactoryMethodName, []);
   }

   private static ITypeInformation CreateType(string typeFullyQualified, string name, bool isReferenceType)
   {
      var type = Substitute.For<ITypeInformation>();
      type.TypeFullyQualified.Returns(typeFullyQualified);
      type.TypeMinimallyQualified.Returns(name);
      type.Name.Returns(name);
      type.Namespace.Returns("Thinktecture.Tests");
      type.IsReferenceType.Returns(isReferenceType);
      type.NullableAnnotation.Returns(NullableAnnotation.None);
      type.IsEqualWithReferenceEquality.Returns(false);
      type.DisallowsDefaultValue.Returns(false);
      return type;
   }

   private static IMemberInformation CreateKeyMember(string name, string typeFullyQualified, SpecialType specialType, bool isReferenceType)
   {
      var member = Substitute.For<IMemberInformation>();
      member.Name.Returns(name);
      member.TypeFullyQualified.Returns(typeFullyQualified);
      member.SpecialType.Returns(specialType);
      member.IsReferenceType.Returns(isReferenceType);
      member.NullableAnnotation.Returns(NullableAnnotation.None);
      return member;
   }
}
