using Microsoft.CodeAnalysis;
using NSubstitute;
using Thinktecture.CodeAnalysis;

#nullable enable

namespace Thinktecture.Runtime.Tests.CodeAnalysis.ComparableCodeGeneratorTests;

/// <summary>
/// Builder for creating InterfaceCodeGeneratorState instances for testing ComparableCodeGenerator.
/// </summary>
public class ComparableStateBuilder
{
   private ITypeInformation? _type;
   private IMemberInformation? _keyMember;
   private string _createFactoryMethodName = "Create";

   public ComparableStateBuilder WithType(string typeFullyQualified, string name = "TestType", bool isReferenceType = true)
   {
      _type = CreateType(typeFullyQualified, name, isReferenceType);
      return this;
   }

   public ComparableStateBuilder WithReferenceType(string typeFullyQualified, string name = "TestType")
   {
      _type = CreateType(typeFullyQualified, name, isReferenceType: true);
      return this;
   }

   public ComparableStateBuilder WithValueType(string typeFullyQualified, string name = "TestType")
   {
      _type = CreateType(typeFullyQualified, name, isReferenceType: false);
      return this;
   }

   public ComparableStateBuilder WithStringKeyMember(string name = "Key")
   {
      _keyMember = CreateKeyMember(name, "global::System.String", SpecialType.System_String);
      return this;
   }

   public ComparableStateBuilder WithIntKeyMember(string name = "Key")
   {
      _keyMember = CreateKeyMember(name, "global::System.Int32", SpecialType.System_Int32);
      return this;
   }

   public ComparableStateBuilder WithGuidKeyMember(string name = "Key")
   {
      _keyMember = CreateKeyMember(name, "global::System.Guid", SpecialType.None);
      return this;
   }

   public ComparableStateBuilder WithKeyMember(string name, string typeFullyQualified, SpecialType specialType = SpecialType.None)
   {
      _keyMember = CreateKeyMember(name, typeFullyQualified, specialType);
      return this;
   }

   public ComparableStateBuilder WithCreateFactoryMethodName(string name)
   {
      _createFactoryMethodName = name;
      return this;
   }

   public InterfaceCodeGeneratorState Build()
   {
      _type ??= CreateType("global::Thinktecture.Tests.TestType", "TestType", isReferenceType: true);
      _keyMember ??= CreateKeyMember("Key", "global::System.Int32", SpecialType.System_Int32);

      return new InterfaceCodeGeneratorState(_type, _keyMember, _createFactoryMethodName);
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
}
