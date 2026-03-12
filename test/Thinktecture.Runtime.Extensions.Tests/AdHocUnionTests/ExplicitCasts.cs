using System;
using Thinktecture.Runtime.Tests.TestAdHocUnions;

namespace Thinktecture.Runtime.Tests.AdHocUnionTests;

public class ExplicitCasts
{
   [Fact]
   public void Should_have_explicit_casts_to_value_having_2_types()
   {
      ((string)new TestUnion_class_string_int("text")).Should().Be("text");
      FluentActions.Invoking(() => (int)new TestUnion_class_string_int("text")).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_string_int' is not of type 'int' but of type 'string'.");

      ((int)new TestUnion_class_string_int(1)).Should().Be(1);
      FluentActions.Invoking(() => (string)new TestUnion_class_string_int(1)).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_string_int' is not of type 'string' but of type 'int'.");

      ((string)new TestUnion_struct_string_int("text")).Should().Be("text");
      FluentActions.Invoking(() => (int)new TestUnion_struct_string_int("text")).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_struct_string_int' is not of type 'int' but of type 'string'.");

      ((int)new TestUnion_struct_string_int(1)).Should().Be(1);
      FluentActions.Invoking(() => (string)new TestUnion_struct_string_int(1)).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_struct_string_int' is not of type 'string' but of type 'int'.");

      ((string[])new TestUnion_class_with_array(["text"])).Should().BeEquivalentTo(["text"]);
      FluentActions.Invoking(() => (int)new TestUnion_class_with_array(["text"])).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_with_array' is not of type 'int' but of type 'string[]'.");
   }

   [Fact]
   public void Should_have_explicit_casts_to_value_having_3_types()
   {
      ((string)new TestUnion_class_string_int_bool("text")).Should().Be("text");
      FluentActions.Invoking(() => (int)new TestUnion_class_string_int_bool("text")).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_string_int_bool' is not of type 'int' but of type 'string'.");
      FluentActions.Invoking(() => (bool)new TestUnion_class_string_int_bool("text")).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_string_int_bool' is not of type 'bool' but of type 'string'.");

      ((int)new TestUnion_class_string_int_bool(1)).Should().Be(1);
      FluentActions.Invoking(() => (string)new TestUnion_class_string_int_bool(1)).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_string_int_bool' is not of type 'string' but of type 'int'.");
      FluentActions.Invoking(() => (bool)new TestUnion_class_string_int_bool(1)).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_string_int_bool' is not of type 'bool' but of type 'int'.");

      ((bool)new TestUnion_class_string_int_bool(true)).Should().Be(true);
      FluentActions.Invoking(() => (string)new TestUnion_class_string_int_bool(true)).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_string_int_bool' is not of type 'string' but of type 'bool'.");
      FluentActions.Invoking(() => (int)new TestUnion_class_string_int_bool(true)).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_string_int_bool' is not of type 'int' but of type 'bool'.");
   }

   [Fact]
   public void Should_have_explicit_casts_to_value_having_4_types()
   {
      ((string)new TestUnion_class_string_int_bool_guid("text")).Should().Be("text");
      FluentActions.Invoking(() => (int)new TestUnion_class_string_int_bool_guid("text")).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_string_int_bool_guid' is not of type 'int' but of type 'string'.");
      FluentActions.Invoking(() => (bool)new TestUnion_class_string_int_bool_guid("text")).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_string_int_bool_guid' is not of type 'bool' but of type 'string'.");
      FluentActions.Invoking(() => (Guid)new TestUnion_class_string_int_bool_guid("text")).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_string_int_bool_guid' is not of type 'Guid' but of type 'string'.");

      ((int)new TestUnion_class_string_int_bool_guid(1)).Should().Be(1);
      FluentActions.Invoking(() => (string)new TestUnion_class_string_int_bool_guid(1)).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_string_int_bool_guid' is not of type 'string' but of type 'int'.");
      FluentActions.Invoking(() => (bool)new TestUnion_class_string_int_bool_guid(1)).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_string_int_bool_guid' is not of type 'bool' but of type 'int'.");
      FluentActions.Invoking(() => (Guid)new TestUnion_class_string_int_bool_guid(1)).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_string_int_bool_guid' is not of type 'Guid' but of type 'int'.");

      ((bool)new TestUnion_class_string_int_bool_guid(true)).Should().Be(true);
      FluentActions.Invoking(() => (string)new TestUnion_class_string_int_bool_guid(true)).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_string_int_bool_guid' is not of type 'string' but of type 'bool'.");
      FluentActions.Invoking(() => (int)new TestUnion_class_string_int_bool_guid(true)).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_string_int_bool_guid' is not of type 'int' but of type 'bool'.");
      FluentActions.Invoking(() => (Guid)new TestUnion_class_string_int_bool_guid(true)).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_string_int_bool_guid' is not of type 'Guid' but of type 'bool'.");

      ((Guid)new TestUnion_class_string_int_bool_guid(new Guid("9265932D-D3EB-4204-A027-7876F72BD66E"))).Should().Be(new Guid("9265932D-D3EB-4204-A027-7876F72BD66E"));
      FluentActions.Invoking(() => (string)new TestUnion_class_string_int_bool_guid(new Guid("9265932D-D3EB-4204-A027-7876F72BD66E"))).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_string_int_bool_guid' is not of type 'string' but of type 'Guid'.");
      FluentActions.Invoking(() => (int)new TestUnion_class_string_int_bool_guid(new Guid("9265932D-D3EB-4204-A027-7876F72BD66E"))).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_string_int_bool_guid' is not of type 'int' but of type 'Guid'.");
      FluentActions.Invoking(() => (bool)new TestUnion_class_string_int_bool_guid(new Guid("9265932D-D3EB-4204-A027-7876F72BD66E"))).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_string_int_bool_guid' is not of type 'bool' but of type 'Guid'.");
   }

   [Fact]
   public void Should_have_explicit_casts_to_value_having_5_types()
   {
      ((string)new TestUnion_class_string_int_bool_guid_char("text")).Should().Be("text");
      FluentActions.Invoking(() => (int)new TestUnion_class_string_int_bool_guid_char("text")).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_string_int_bool_guid_char' is not of type 'int' but of type 'string'.");
      FluentActions.Invoking(() => (bool)new TestUnion_class_string_int_bool_guid_char("text")).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_string_int_bool_guid_char' is not of type 'bool' but of type 'string'.");
      FluentActions.Invoking(() => (Guid)new TestUnion_class_string_int_bool_guid_char("text")).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_string_int_bool_guid_char' is not of type 'Guid' but of type 'string'.");
      FluentActions.Invoking(() => (char)new TestUnion_class_string_int_bool_guid_char("text")).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_string_int_bool_guid_char' is not of type 'char' but of type 'string'.");

      ((int)new TestUnion_class_string_int_bool_guid_char(1)).Should().Be(1);
      FluentActions.Invoking(() => (string)new TestUnion_class_string_int_bool_guid_char(1)).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_string_int_bool_guid_char' is not of type 'string' but of type 'int'.");
      FluentActions.Invoking(() => (bool)new TestUnion_class_string_int_bool_guid_char(1)).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_string_int_bool_guid_char' is not of type 'bool' but of type 'int'.");
      FluentActions.Invoking(() => (Guid)new TestUnion_class_string_int_bool_guid_char(1)).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_string_int_bool_guid_char' is not of type 'Guid' but of type 'int'.");
      FluentActions.Invoking(() => (char)new TestUnion_class_string_int_bool_guid_char(1)).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_string_int_bool_guid_char' is not of type 'char' but of type 'int'.");

      ((bool)new TestUnion_class_string_int_bool_guid_char(true)).Should().Be(true);
      FluentActions.Invoking(() => (string)new TestUnion_class_string_int_bool_guid_char(true)).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_string_int_bool_guid_char' is not of type 'string' but of type 'bool'.");
      FluentActions.Invoking(() => (int)new TestUnion_class_string_int_bool_guid_char(true)).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_string_int_bool_guid_char' is not of type 'int' but of type 'bool'.");
      FluentActions.Invoking(() => (Guid)new TestUnion_class_string_int_bool_guid_char(true)).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_string_int_bool_guid_char' is not of type 'Guid' but of type 'bool'.");
      FluentActions.Invoking(() => (char)new TestUnion_class_string_int_bool_guid_char(true)).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_string_int_bool_guid_char' is not of type 'char' but of type 'bool'.");

      ((Guid)new TestUnion_class_string_int_bool_guid_char(new Guid("9265932D-D3EB-4204-A027-7876F72BD66E"))).Should().Be(new Guid("9265932D-D3EB-4204-A027-7876F72BD66E"));
      FluentActions.Invoking(() => (string)new TestUnion_class_string_int_bool_guid_char(new Guid("9265932D-D3EB-4204-A027-7876F72BD66E"))).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_string_int_bool_guid_char' is not of type 'string' but of type 'Guid'.");
      FluentActions.Invoking(() => (int)new TestUnion_class_string_int_bool_guid_char(new Guid("9265932D-D3EB-4204-A027-7876F72BD66E"))).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_string_int_bool_guid_char' is not of type 'int' but of type 'Guid'.");
      FluentActions.Invoking(() => (bool)new TestUnion_class_string_int_bool_guid_char(new Guid("9265932D-D3EB-4204-A027-7876F72BD66E"))).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_string_int_bool_guid_char' is not of type 'bool' but of type 'Guid'.");
      FluentActions.Invoking(() => (char)new TestUnion_class_string_int_bool_guid_char(new Guid("9265932D-D3EB-4204-A027-7876F72BD66E"))).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_string_int_bool_guid_char' is not of type 'char' but of type 'Guid'.");

      ((char)new TestUnion_class_string_int_bool_guid_char('A')).Should().Be('A');
      FluentActions.Invoking(() => (string)new TestUnion_class_string_int_bool_guid_char('A')).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_string_int_bool_guid_char' is not of type 'string' but of type 'char'.");
      FluentActions.Invoking(() => (int)new TestUnion_class_string_int_bool_guid_char('A')).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_string_int_bool_guid_char' is not of type 'int' but of type 'char'.");
      FluentActions.Invoking(() => (bool)new TestUnion_class_string_int_bool_guid_char('A')).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_string_int_bool_guid_char' is not of type 'bool' but of type 'char'.");
      FluentActions.Invoking(() => (Guid)new TestUnion_class_string_int_bool_guid_char('A')).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_string_int_bool_guid_char' is not of type 'Guid' but of type 'char'.");
   }

   [Fact]
   public void Should_have_explicit_casts_to_value_having_5_types_with_duplicates()
   {
      ((int)new TestUnion_class_with_same_types(1)).Should().Be(1);
      FluentActions.Invoking(() => (int?)new TestUnion_class_with_same_types(1)).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_with_same_types' is not of type 'int?' but of type 'int'.");
   }
}
