using Microsoft.CodeAnalysis;
using NSubstitute;
using Thinktecture.CodeAnalysis;

#nullable enable

namespace Thinktecture.Runtime.Tests.CodeAnalysis.ParsableCodeGeneratorTests;

/// <summary>
/// Builder for creating ParsableGeneratorState instances for testing.
/// </summary>
public class ParsableStateBuilder
{
   private IParsableTypeInformation? _type;
   private IMemberInformation? _keyMember;
   private ValidationErrorState _validationError = ValidationErrorState.Default;
   private bool _skipIParsable;
   private bool _isKeyMemberParsable;
   private bool _isEnum;
   private bool _hasStringBasedValidateMethod;

   public ParsableStateBuilder WithType(string typeFullyQualified, string name = "TestType")
   {
      _type = CreateType(typeFullyQualified, name);
      return this;
   }

   public ParsableStateBuilder WithStringKeyMember(string name = "Key")
   {
      _keyMember = CreateKeyMember(name, "global::System.String", SpecialType.System_String);
      return this;
   }

   public ParsableStateBuilder WithIntKeyMember(string name = "Key")
   {
      _keyMember = CreateKeyMember(name, "global::System.Int32", SpecialType.System_Int32);
      return this;
   }

   public ParsableStateBuilder WithGuidKeyMember(string name = "Key")
   {
      _keyMember = CreateKeyMember(name, "global::System.Guid", SpecialType.None);
      _isKeyMemberParsable = true;
      return this;
   }

   public ParsableStateBuilder WithoutKeyMember()
   {
      _keyMember = null;
      return this;
   }

   public ParsableStateBuilder WithValidationError(string typeFullyQualified)
   {
      _validationError = new ValidationErrorState(typeFullyQualified);
      return this;
   }

   public ParsableStateBuilder WithSkipIParsable(bool skip = true)
   {
      _skipIParsable = skip;
      return this;
   }

   public ParsableStateBuilder WithIsEnum(bool isEnum = true)
   {
      _isEnum = isEnum;
      return this;
   }

   public ParsableStateBuilder WithHasStringBasedValidateMethod(bool hasStringBasedValidateMethod = true)
   {
      _hasStringBasedValidateMethod = hasStringBasedValidateMethod;
      return this;
   }

   public ParsableGeneratorState Build()
   {
      _type ??= CreateType("global::Thinktecture.Tests.TestType", "TestType");

      return new ParsableGeneratorState(
         _type,
         _keyMember,
         _validationError,
         _skipIParsable,
         _isKeyMemberParsable,
         _isEnum,
         _hasStringBasedValidateMethod);
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
