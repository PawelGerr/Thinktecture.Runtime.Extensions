using Thinktecture.Runtime.Tests.TestEnums;

namespace Thinktecture.Runtime.Tests.EnumTypeConverterTests;

public class CanConvertTo : TypeConverterTestsBase
{
   [Fact]
   public void Should_return_true_if_type_matches_the_key()
   {
      StringBasedTypeConverter.CanConvertTo(typeof(string)).Should().BeTrue();

      ExtensibleTestEnumTypeConverter.CanConvertTo(typeof(string)).Should().BeTrue();
      ExtendedTestEnumTypeConverter.CanConvertTo(typeof(string)).Should().BeTrue();
      DifferentAssemblyExtendedTestEnumTypeConverter.CanConvertTo(typeof(string)).Should().BeTrue();
   }

   [Fact]
   public void Should_return_true_if_type_matches_the_enum()
   {
      StringBasedTypeConverter.CanConvertTo(typeof(TestEnum)).Should().BeTrue();

      ExtensibleTestEnumTypeConverter.CanConvertTo(typeof(ExtensibleTestEnum)).Should().BeTrue();
      ExtendedTestEnumTypeConverter.CanConvertTo(typeof(ExtendedTestEnum)).Should().BeTrue();
      DifferentAssemblyExtendedTestEnumTypeConverter.CanConvertTo(typeof(DifferentAssemblyExtendedTestEnum)).Should().BeTrue();
   }

   [Fact]
   public void Should_return_false_if_type_doesnt_match_the_enum_and_key()
   {
      StringBasedTypeConverter.CanConvertTo(typeof(Guid)).Should().BeFalse();

      ExtensibleTestEnumTypeConverter.CanConvertTo(typeof(Guid)).Should().BeFalse();
      ExtendedTestEnumTypeConverter.CanConvertTo(typeof(Guid)).Should().BeFalse();
      DifferentAssemblyExtendedTestEnumTypeConverter.CanConvertTo(typeof(Guid)).Should().BeFalse();
   }

   [Fact]
   public void Should_return_true_if_type_doesnt_matches_the_key_but_there_is_default_conversion_of_key_to_string()
   {
      IntBasedTypeConverter.CanConvertTo(typeof(string)).Should().BeTrue();
   }

   [Fact]
   public void Should_return_true_if_type_is_nullable_struct()
   {
      IntBasedTypeConverter.CanConvertTo(typeof(int?)).Should().BeTrue();
   }

   [Fact]
   public void Should_return_true_for_structs()
   {
      IntBasedStructEnumTypeConverter.CanConvertTo(typeof(StructIntegerEnum)).Should().BeTrue();
   }

   [Fact]
   public void Should_return_true_for_nullable_structs()
   {
      IntBasedStructEnumTypeConverter.CanConvertTo(typeof(StructIntegerEnum?)).Should().BeTrue();
   }
}