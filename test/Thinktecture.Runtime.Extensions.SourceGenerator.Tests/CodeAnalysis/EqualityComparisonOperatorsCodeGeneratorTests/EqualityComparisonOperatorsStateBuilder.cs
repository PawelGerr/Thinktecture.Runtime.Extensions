using Microsoft.CodeAnalysis;
using NSubstitute;
using Thinktecture.CodeAnalysis;

#nullable enable

namespace Thinktecture.Runtime.Tests.CodeAnalysis.EqualityComparisonOperatorsCodeGeneratorTests;

/// <summary>
/// Builder for creating EqualityComparisonOperatorsGeneratorState instances for testing.
/// </summary>
public class EqualityComparisonOperatorsStateBuilder
{
   private ITypeInformation? _type;
   private IMemberInformation? _keyMember;
   private Thinktecture.CodeAnalysis.OperatorsGeneration _operatorsGeneration = Thinktecture.CodeAnalysis.OperatorsGeneration.Default;
   private ComparerInfo? _equalityComparer;

   public EqualityComparisonOperatorsStateBuilder WithType(
      string typeFullyQualified,
      string name = "TestType",
      bool isReferenceType = true,
      bool isEqualWithReferenceEquality = false)
   {
      _type = CreateType(typeFullyQualified, name, isReferenceType, isEqualWithReferenceEquality);
      return this;
   }

   public EqualityComparisonOperatorsStateBuilder WithReferenceType(
      string typeFullyQualified,
      string name = "TestType",
      bool isEqualWithReferenceEquality = false)
   {
      _type = CreateType(typeFullyQualified, name, isReferenceType: true, isEqualWithReferenceEquality);
      return this;
   }

   public EqualityComparisonOperatorsStateBuilder WithValueType(
      string typeFullyQualified,
      string name = "TestType")
   {
      _type = CreateType(typeFullyQualified, name, isReferenceType: false, isEqualWithReferenceEquality: false);
      return this;
   }

   public EqualityComparisonOperatorsStateBuilder WithStringKeyMember(string name = "Key", bool isReferenceType = true)
   {
      _keyMember = CreateKeyMember(name, "global::System.String", SpecialType.System_String, isReferenceType);
      return this;
   }

   public EqualityComparisonOperatorsStateBuilder WithIntKeyMember(string name = "Key")
   {
      _keyMember = CreateKeyMember(name, "global::System.Int32", SpecialType.System_Int32, isReferenceType: false);
      return this;
   }

   public EqualityComparisonOperatorsStateBuilder WithGuidKeyMember(string name = "Key")
   {
      _keyMember = CreateKeyMember(name, "global::System.Guid", SpecialType.None, isReferenceType: false);
      return this;
   }

   public EqualityComparisonOperatorsStateBuilder WithKeyMember(
      string name,
      string typeFullyQualified,
      SpecialType specialType = SpecialType.None,
      bool isReferenceType = false)
   {
      _keyMember = CreateKeyMember(name, typeFullyQualified, specialType, isReferenceType);
      return this;
   }

   public EqualityComparisonOperatorsStateBuilder WithOperatorsGeneration(Thinktecture.CodeAnalysis.OperatorsGeneration operatorsGeneration)
   {
      _operatorsGeneration = operatorsGeneration;
      return this;
   }

   public EqualityComparisonOperatorsStateBuilder WithEqualityComparer(string comparer, bool isAccessor = true)
   {
      _equalityComparer = new ComparerInfo(comparer, isAccessor);
      return this;
   }

   public EqualityComparisonOperatorsStateBuilder WithoutKeyMember()
   {
      _keyMember = null;
      return this;
   }

   public EqualityComparisonOperatorsGeneratorState Build()
   {
      _type ??= CreateType("global::Thinktecture.Tests.TestType", "TestType", isReferenceType: true, isEqualWithReferenceEquality: false);

      return new EqualityComparisonOperatorsGeneratorState(_type, _keyMember, _operatorsGeneration, _equalityComparer, []);
   }

   private static ITypeInformation CreateType(
      string typeFullyQualified,
      string name,
      bool isReferenceType,
      bool isEqualWithReferenceEquality)
   {
      var type = Substitute.For<ITypeInformation>();
      type.TypeFullyQualified.Returns(typeFullyQualified);
      type.TypeMinimallyQualified.Returns(name);
      type.Name.Returns(name);
      type.Namespace.Returns("Thinktecture.Tests");
      type.IsReferenceType.Returns(isReferenceType);
      type.NullableAnnotation.Returns(NullableAnnotation.None);
      type.IsEqualWithReferenceEquality.Returns(isEqualWithReferenceEquality);
      return type;
   }

   private static IMemberInformation CreateKeyMember(
      string name,
      string typeFullyQualified,
      SpecialType specialType,
      bool isReferenceType)
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
