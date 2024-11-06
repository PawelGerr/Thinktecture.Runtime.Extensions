#nullable enable
using System;
using Thinktecture.Runtime.Tests.TestUnions;

namespace Thinktecture.Runtime.Tests.UnionTests;

public class AsValue
{
   [Fact]
   public void Should_return_correct_value_or_throw_exception_having_2_types()
   {
      new TestUnion_class_string_int("text").AsString.Should().Be("text");
      new TestUnion_class_string_int("text").Invoking(u => u.AsInt32.Should()).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_string_int' is not of type 'int'.");
      new TestUnion_class_string_int(1).Invoking(u => u.AsString.Should()).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_string_int' is not of type 'string'.");
      new TestUnion_class_string_int(1).AsInt32.Should().Be(1);

      new TestUnion_class_nullable_string_int(@string: null).AsString.Should().BeNull();
      new TestUnion_class_nullable_string_int(@string: null).Invoking(u => u.AsInt32.Should()).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_nullable_string_int' is not of type 'int'.");
      new TestUnion_class_nullable_string_int("text").AsString.Should().Be("text");
      new TestUnion_class_nullable_string_int("text").Invoking(u => u.AsInt32.Should()).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_nullable_string_int' is not of type 'int'.");
      new TestUnion_class_nullable_string_int(1).Invoking(u => u.AsString.Should()).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_nullable_string_int' is not of type 'string?'.");
      new TestUnion_class_nullable_string_int(1).AsInt32.Should().Be(1);

      new TestUnion_class_nullable_string_nullable_int(@string: null).AsString.Should().BeNull();
      new TestUnion_class_nullable_string_nullable_int(@string: null).Invoking(u => u.AsNullableInt32.Should()).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_nullable_string_nullable_int' is not of type 'int?'.");
      new TestUnion_class_nullable_string_nullable_int("text").AsString.Should().Be("text");
      new TestUnion_class_nullable_string_nullable_int("text").Invoking(u => u.AsNullableInt32.Should()).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_nullable_string_nullable_int' is not of type 'int?'.");
      new TestUnion_class_nullable_string_nullable_int(1).Invoking(u => u.AsString.Should()).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_nullable_string_nullable_int' is not of type 'string?'.");
      new TestUnion_class_nullable_string_nullable_int(1).AsNullableInt32.Should().Be(1);
      new TestUnion_class_nullable_string_nullable_int(nullableInt32: null).Invoking(u => u.AsString.Should()).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_nullable_string_nullable_int' is not of type 'string?'.");
      new TestUnion_class_nullable_string_nullable_int(nullableInt32: null).AsNullableInt32.Should().BeNull();

      new TestUnion_struct_string_int("text").AsString.Should().Be("text");
      new TestUnion_struct_string_int("text").Invoking(u => u.AsInt32.Should()).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_struct_string_int' is not of type 'int'.");
      new TestUnion_struct_string_int(1).Invoking(u => u.AsString.Should()).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_struct_string_int' is not of type 'string'.");
      new TestUnion_struct_string_int(1).AsInt32.Should().Be(1);

      new TestUnion_class_with_array(["text"]).AsStringArray.Should().BeEquivalentTo(["text"]);
      new TestUnion_class_with_array(["text"]).Invoking(u => u.AsInt32.Should()).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_with_array' is not of type 'int'.");
      new TestUnion_class_with_array(1).Invoking(u => u.AsStringArray.Should()).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_with_array' is not of type 'string[]'.");
      new TestUnion_class_with_array(1).AsInt32.Should().Be(1);
   }

   [Fact]
   public void Should_return_correct_value_or_throw_exception_having_3_types()
   {
      new TestUnion_class_string_int_bool("text").AsString.Should().Be("text");
      new TestUnion_class_string_int_bool("text").Invoking(u => u.AsInt32.Should()).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_string_int_bool' is not of type 'int'.");
      new TestUnion_class_string_int_bool("text").Invoking(u => u.AsBoolean.Should()).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_string_int_bool' is not of type 'bool'.");

      new TestUnion_class_string_int_bool(1).Invoking(u => u.AsString.Should()).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_string_int_bool' is not of type 'string'.");
      new TestUnion_class_string_int_bool(1).AsInt32.Should().Be(1);
      new TestUnion_class_string_int_bool(1).Invoking(u => u.AsBoolean.Should()).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_string_int_bool' is not of type 'bool'.");

      new TestUnion_class_string_int_bool(true).Invoking(u => u.AsString.Should()).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_string_int_bool' is not of type 'string'.");
      new TestUnion_class_string_int_bool(true).Invoking(u => u.AsInt32.Should()).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_string_int_bool' is not of type 'int'.");
      new TestUnion_class_string_int_bool(true).AsBoolean.Should().Be(true);
   }

   [Fact]
   public void Should_return_correct_value_or_throw_exception_having_4_types()
   {
      new TestUnion_class_string_int_bool_guid("text").AsString.Should().Be("text");
      new TestUnion_class_string_int_bool_guid("text").Invoking(u => u.AsInt32.Should()).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_string_int_bool_guid' is not of type 'int'.");
      new TestUnion_class_string_int_bool_guid("text").Invoking(u => u.AsBoolean.Should()).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_string_int_bool_guid' is not of type 'bool'.");
      new TestUnion_class_string_int_bool_guid("text").Invoking(u => u.AsGuid.Should()).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_string_int_bool_guid' is not of type 'Guid'.");

      new TestUnion_class_string_int_bool_guid(1).Invoking(u => u.AsString.Should()).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_string_int_bool_guid' is not of type 'string'.");
      new TestUnion_class_string_int_bool_guid(1).AsInt32.Should().Be(1);
      new TestUnion_class_string_int_bool_guid(1).Invoking(u => u.AsBoolean.Should()).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_string_int_bool_guid' is not of type 'bool'.");
      new TestUnion_class_string_int_bool_guid(1).Invoking(u => u.AsGuid.Should()).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_string_int_bool_guid' is not of type 'Guid'.");

      new TestUnion_class_string_int_bool_guid(true).Invoking(u => u.AsString.Should()).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_string_int_bool_guid' is not of type 'string'.");
      new TestUnion_class_string_int_bool_guid(true).Invoking(u => u.AsInt32.Should()).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_string_int_bool_guid' is not of type 'int'.");
      new TestUnion_class_string_int_bool_guid(true).AsBoolean.Should().Be(true);
      new TestUnion_class_string_int_bool_guid(true).Invoking(u => u.AsGuid.Should()).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_string_int_bool_guid' is not of type 'Guid'.");

      new TestUnion_class_string_int_bool_guid(new Guid("60AF342A-2A29-4F4B-AA26-B69E857CCF5C")).Invoking(u => u.AsString.Should()).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_string_int_bool_guid' is not of type 'string'.");
      new TestUnion_class_string_int_bool_guid(new Guid("60AF342A-2A29-4F4B-AA26-B69E857CCF5C")).Invoking(u => u.AsInt32.Should()).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_string_int_bool_guid' is not of type 'int'.");
      new TestUnion_class_string_int_bool_guid(new Guid("60AF342A-2A29-4F4B-AA26-B69E857CCF5C")).Invoking(u => u.AsBoolean.Should()).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_string_int_bool_guid' is not of type 'bool'.");
      new TestUnion_class_string_int_bool_guid(new Guid("60AF342A-2A29-4F4B-AA26-B69E857CCF5C")).AsGuid.Should().Be(new Guid("60AF342A-2A29-4F4B-AA26-B69E857CCF5C"));
   }

   [Fact]
   public void Should_return_correct_value_or_throw_exception_having_5_types()
   {
      new TestUnion_class_string_int_bool_guid_char("text").AsString.Should().Be("text");
      new TestUnion_class_string_int_bool_guid_char("text").Invoking(u => u.AsInt32.Should()).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_string_int_bool_guid_char' is not of type 'int'.");
      new TestUnion_class_string_int_bool_guid_char("text").Invoking(u => u.AsBoolean.Should()).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_string_int_bool_guid_char' is not of type 'bool'.");
      new TestUnion_class_string_int_bool_guid_char("text").Invoking(u => u.AsGuid.Should()).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_string_int_bool_guid_char' is not of type 'Guid'.");
      new TestUnion_class_string_int_bool_guid_char("text").Invoking(u => u.AsChar.Should()).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_string_int_bool_guid_char' is not of type 'char'.");

      new TestUnion_class_string_int_bool_guid_char(1).Invoking(u => u.AsString.Should()).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_string_int_bool_guid_char' is not of type 'string'.");
      new TestUnion_class_string_int_bool_guid_char(1).AsInt32.Should().Be(1);
      new TestUnion_class_string_int_bool_guid_char(1).Invoking(u => u.AsBoolean.Should()).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_string_int_bool_guid_char' is not of type 'bool'.");
      new TestUnion_class_string_int_bool_guid_char(1).Invoking(u => u.AsGuid.Should()).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_string_int_bool_guid_char' is not of type 'Guid'.");
      new TestUnion_class_string_int_bool_guid_char(1).Invoking(u => u.AsChar.Should()).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_string_int_bool_guid_char' is not of type 'char'.");

      new TestUnion_class_string_int_bool_guid_char(true).Invoking(u => u.AsString.Should()).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_string_int_bool_guid_char' is not of type 'string'.");
      new TestUnion_class_string_int_bool_guid_char(true).Invoking(u => u.AsInt32.Should()).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_string_int_bool_guid_char' is not of type 'int'.");
      new TestUnion_class_string_int_bool_guid_char(true).AsBoolean.Should().Be(true);
      new TestUnion_class_string_int_bool_guid_char(true).Invoking(u => u.AsGuid.Should()).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_string_int_bool_guid_char' is not of type 'Guid'.");
      new TestUnion_class_string_int_bool_guid_char(true).Invoking(u => u.AsChar.Should()).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_string_int_bool_guid_char' is not of type 'char'.");

      new TestUnion_class_string_int_bool_guid_char(new Guid("60AF342A-2A29-4F4B-AA26-B69E857CCF5C")).Invoking(u => u.AsString.Should()).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_string_int_bool_guid_char' is not of type 'string'.");
      new TestUnion_class_string_int_bool_guid_char(new Guid("60AF342A-2A29-4F4B-AA26-B69E857CCF5C")).Invoking(u => u.AsInt32.Should()).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_string_int_bool_guid_char' is not of type 'int'.");
      new TestUnion_class_string_int_bool_guid_char(new Guid("60AF342A-2A29-4F4B-AA26-B69E857CCF5C")).Invoking(u => u.AsBoolean.Should()).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_string_int_bool_guid_char' is not of type 'bool'.");
      new TestUnion_class_string_int_bool_guid_char(new Guid("60AF342A-2A29-4F4B-AA26-B69E857CCF5C")).AsGuid.Should().Be(new Guid("60AF342A-2A29-4F4B-AA26-B69E857CCF5C"));
      new TestUnion_class_string_int_bool_guid_char(new Guid("60AF342A-2A29-4F4B-AA26-B69E857CCF5C")).Invoking(u => u.AsChar.Should()).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_string_int_bool_guid_char' is not of type 'char'.");

      new TestUnion_class_string_int_bool_guid_char('A').Invoking(u => u.AsString.Should()).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_string_int_bool_guid_char' is not of type 'string'.");
      new TestUnion_class_string_int_bool_guid_char('A').Invoking(u => u.AsInt32.Should()).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_string_int_bool_guid_char' is not of type 'int'.");
      new TestUnion_class_string_int_bool_guid_char('A').Invoking(u => u.AsBoolean.Should()).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_string_int_bool_guid_char' is not of type 'bool'.");
      new TestUnion_class_string_int_bool_guid_char('A').Invoking(u => u.AsGuid.Should()).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_string_int_bool_guid_char' is not of type 'Guid'.");
      new TestUnion_class_string_int_bool_guid_char('A').AsChar.Should().Be('A');
   }

   [Fact]
   public void Should_return_correct_value_or_throw_exception_having_5_types_with_duplicates()
   {
      TestUnion_class_with_same_types.CreateText("text").AsText.Should().Be("text");
      TestUnion_class_with_same_types.CreateText("text").Invoking(u => u.AsInt32.Should()).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_with_same_types' is not of type 'int'.");
      TestUnion_class_with_same_types.CreateText("text").Invoking(u => u.AsString2.Should()).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_with_same_types' is not of type 'string'.");
      TestUnion_class_with_same_types.CreateText("text").Invoking(u => u.AsString3.Should()).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_with_same_types' is not of type 'string?'.");
      TestUnion_class_with_same_types.CreateText("text").Invoking(u => u.AsNullableInt32.Should()).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_with_same_types' is not of type 'int?'.");

      new TestUnion_class_with_same_types(1).Invoking(u => u.AsText.Should()).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_with_same_types' is not of type 'string'.");
      new TestUnion_class_with_same_types(1).AsInt32.Should().Be(1);
      new TestUnion_class_with_same_types(1).Invoking(u => u.AsString2.Should()).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_with_same_types' is not of type 'string'.");
      new TestUnion_class_with_same_types(1).Invoking(u => u.AsString3.Should()).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_with_same_types' is not of type 'string?'.");
      new TestUnion_class_with_same_types(1).Invoking(u => u.AsNullableInt32.Should()).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_with_same_types' is not of type 'int?'.");

      TestUnion_class_with_same_types.CreateString2("text").Invoking(u => u.AsText.Should()).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_with_same_types' is not of type 'string'.");
      TestUnion_class_with_same_types.CreateString2("text").Invoking(u => u.AsInt32.Should()).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_with_same_types' is not of type 'int'.");
      TestUnion_class_with_same_types.CreateString2("text").AsString2.Should().Be("text");
      TestUnion_class_with_same_types.CreateString2("text").Invoking(u => u.AsString3.Should()).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_with_same_types' is not of type 'string?'.");
      TestUnion_class_with_same_types.CreateString2("text").Invoking(u => u.AsNullableInt32.Should()).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_with_same_types' is not of type 'int?'.");

      TestUnion_class_with_same_types.CreateString3("text").Invoking(u => u.AsText.Should()).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_with_same_types' is not of type 'string'.");
      TestUnion_class_with_same_types.CreateString3("text").Invoking(u => u.AsInt32.Should()).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_with_same_types' is not of type 'int'.");
      TestUnion_class_with_same_types.CreateString3("text").Invoking(u => u.AsString2.Should()).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_with_same_types' is not of type 'string'.");
      TestUnion_class_with_same_types.CreateString3("text").AsString3.Should().Be("text");
      TestUnion_class_with_same_types.CreateString3("text").Invoking(u => u.AsNullableInt32.Should()).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_with_same_types' is not of type 'int?'.");

      new TestUnion_class_with_same_types((int?)1).Invoking(u => u.AsText.Should()).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_with_same_types' is not of type 'string'.");
      new TestUnion_class_with_same_types((int?)1).Invoking(u => u.AsInt32.Should()).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_with_same_types' is not of type 'int'.");
      new TestUnion_class_with_same_types((int?)1).Invoking(u => u.AsString2.Should()).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_with_same_types' is not of type 'string'.");
      new TestUnion_class_with_same_types((int?)1).Invoking(u => u.AsString3.Should()).Should().Throw<InvalidOperationException>().WithMessage("'TestUnion_class_with_same_types' is not of type 'string?'.");
      new TestUnion_class_with_same_types((int?)1).AsNullableInt32.Should().Be(1);
   }
}
