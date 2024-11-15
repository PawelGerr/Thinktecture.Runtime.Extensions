using System;
using Thinktecture.Runtime.Tests.TestAdHocUnions;

namespace Thinktecture.Runtime.Tests.AdHocUnionTests;

// ReSharper disable once InconsistentNaming
public class MapPartially
{
   public class HavingClass
   {
      [Theory]
      [InlineData(1, "text")]
      [InlineData(2, 42)]
      public void Should_use_correct_arg_having_2_types(int index, object expected)
      {
         var value = index switch
         {
            1 => new TestUnion_class_string_int("text"),
            2 => new TestUnion_class_string_int(42),
            _ => throw new Exception()
         };

         var calledActionOn = value.MapPartially(@default: (object)"default",
                                                 @string: "text",
                                                 int32: 42);

         calledActionOn.Should().Be(expected);
      }

      [Theory]
      [InlineData(1, "default")]
      [InlineData(2, 42)]
      public void Should_use_default_having_2_types(int index, object expected)
      {
         var value = index switch
         {
            1 => new TestUnion_class_string_int("text"),
            2 => new TestUnion_class_string_int(42),
            _ => throw new Exception()
         };

         var calledActionOn = value.MapPartially(@default: (object)"default",
                                                 int32: 42);

         calledActionOn.Should().Be(expected);
      }

      [Theory]
      [InlineData(1, "text")]
      [InlineData(2, 42)]
      [InlineData(3, true)]
      public void Should_use_correct_arg_having_3_types(int index, object expected)
      {
         var value = index switch
         {
            1 => new TestUnion_class_string_int_bool("text"),
            2 => new TestUnion_class_string_int_bool(42),
            3 => new TestUnion_class_string_int_bool(true),
            _ => throw new Exception()
         };

         var calledActionOn = value.MapPartially(@default: (object)"default",
                                                 @string: "text",
                                                 int32: 42,
                                                 boolean: true);

         calledActionOn.Should().Be(expected);
      }

      [Theory]
      [InlineData(1, "default")]
      [InlineData(2, 42)]
      [InlineData(3, true)]
      public void Should_use_default_having_3_types(int index, object expected)
      {
         var value = index switch
         {
            1 => new TestUnion_class_string_int_bool("text"),
            2 => new TestUnion_class_string_int_bool(42),
            3 => new TestUnion_class_string_int_bool(true),
            _ => throw new Exception()
         };

         var calledActionOn = value.MapPartially(@default: (object)"default",
                                                 int32: 42,
                                                 boolean: true);

         calledActionOn.Should().Be(expected);
      }

      [Theory]
      [InlineData(1, "text")]
      [InlineData(2, 42)]
      [InlineData(3, true)]
      [InlineData(4, "FCF57B31-DA82-475A-8418-5161AA1A4280")]
      public void Should_use_correct_arg_having_4_types(int index, object expected)
      {
         var value = index switch
         {
            1 => new TestUnion_class_string_int_bool_guid("text"),
            2 => new TestUnion_class_string_int_bool_guid(42),
            3 => new TestUnion_class_string_int_bool_guid(true),
            4 => new TestUnion_class_string_int_bool_guid(new Guid("FCF57B31-DA82-475A-8418-5161AA1A4280")),
            _ => throw new Exception()
         };

         var calledActionOn = value.MapPartially(@default: (object)"default",
                                                 @string: "text",
                                                 int32: 42,
                                                 boolean: true,
                                                 guid: new Guid("FCF57B31-DA82-475A-8418-5161AA1A4280"));

         calledActionOn.Should().Be(index == 4 ? new Guid((string)expected) : expected);
      }

      [Theory]
      [InlineData(1, "default")]
      [InlineData(2, 42)]
      [InlineData(3, true)]
      [InlineData(4, "FCF57B31-DA82-475A-8418-5161AA1A4280")]
      public void Should_use_default_having_4_types(int index, object expected)
      {
         var value = index switch
         {
            1 => new TestUnion_class_string_int_bool_guid("text"),
            2 => new TestUnion_class_string_int_bool_guid(42),
            3 => new TestUnion_class_string_int_bool_guid(true),
            4 => new TestUnion_class_string_int_bool_guid(new Guid("FCF57B31-DA82-475A-8418-5161AA1A4280")),
            _ => throw new Exception()
         };

         var calledActionOn = value.MapPartially(@default: (object)"default",
                                                 int32: 42,
                                                 boolean: true,
                                                 guid: new Guid("FCF57B31-DA82-475A-8418-5161AA1A4280"));

         calledActionOn.Should().Be(index == 4 ? new Guid((string)expected) : expected);
      }

      [Theory]
      [InlineData(1, "text")]
      [InlineData(2, 42)]
      [InlineData(3, true)]
      [InlineData(4, "FCF57B31-DA82-475A-8418-5161AA1A4280")]
      [InlineData(5, 'A')]
      public void Should_use_correct_arg_having_5_types(int index, object expected)
      {
         var value = index switch
         {
            1 => new TestUnion_class_string_int_bool_guid_char("text"),
            2 => new TestUnion_class_string_int_bool_guid_char(42),
            3 => new TestUnion_class_string_int_bool_guid_char(true),
            4 => new TestUnion_class_string_int_bool_guid_char(new Guid("FCF57B31-DA82-475A-8418-5161AA1A4280")),
            5 => new TestUnion_class_string_int_bool_guid_char('A'),
            _ => throw new Exception()
         };

         var calledActionOn = value.MapPartially(@default: (object)"default",
                                                 @string: "text",
                                                 int32: 42,
                                                 boolean: true,
                                                 guid: new Guid("FCF57B31-DA82-475A-8418-5161AA1A4280"),
                                                 @char: 'A');

         calledActionOn.Should().Be(index == 4 ? new Guid((string)expected) : expected);
      }

      [Theory]
      [InlineData(1, "default")]
      [InlineData(2, 42)]
      [InlineData(3, true)]
      [InlineData(4, "FCF57B31-DA82-475A-8418-5161AA1A4280")]
      [InlineData(5, 'A')]
      public void Should_use_default_having_5_types(int index, object expected)
      {
         var value = index switch
         {
            1 => new TestUnion_class_string_int_bool_guid_char("text"),
            2 => new TestUnion_class_string_int_bool_guid_char(42),
            3 => new TestUnion_class_string_int_bool_guid_char(true),
            4 => new TestUnion_class_string_int_bool_guid_char(new Guid("FCF57B31-DA82-475A-8418-5161AA1A4280")),
            5 => new TestUnion_class_string_int_bool_guid_char('A'),
            _ => throw new Exception()
         };

         var calledActionOn = value.MapPartially(@default: (object)"default",
                                                 int32: 42,
                                                 boolean: true,
                                                 guid: new Guid("FCF57B31-DA82-475A-8418-5161AA1A4280"),
                                                 @char: 'A');

         calledActionOn.Should().Be(index == 4 ? new Guid((string)expected) : expected);
      }

      [Theory]
      [InlineData(1, "text")]
      [InlineData(2, 42)]
      [InlineData(3, "text2")]
      [InlineData(4, "text3")]
      [InlineData(5, 43)]
      public void Should_use_correct_arg_having_5_types_with_duplicates(int index, object expected)
      {
         var value = index switch
         {
            1 => TestUnion_class_with_same_types.CreateText("text"),
            2 => new TestUnion_class_with_same_types(42),
            3 => TestUnion_class_with_same_types.CreateString2("text2"),
            4 => TestUnion_class_with_same_types.CreateString3("text3"),
            5 => new TestUnion_class_with_same_types((int?)43),
            _ => throw new Exception()
         };

         var calledActionOn = value.MapPartially(@default: (object)"default",
                                                 text: (object)"text",
                                                 int32: 42,
                                                 string2: "text2",
                                                 string3: "text3",
                                                 nullableInt32: 43);

         calledActionOn.Should().Be(expected);
      }

      [Theory]
      [InlineData(1, "default")]
      [InlineData(2, 42)]
      [InlineData(3, "text2")]
      [InlineData(4, "text3")]
      [InlineData(5, 43)]
      public void Should_use_default_having_5_types_with_duplicates(int index, object expected)
      {
         var value = index switch
         {
            1 => TestUnion_class_with_same_types.CreateText("text"),
            2 => new TestUnion_class_with_same_types(42),
            3 => TestUnion_class_with_same_types.CreateString2("text2"),
            4 => TestUnion_class_with_same_types.CreateString3("text3"),
            5 => new TestUnion_class_with_same_types((int?)43),
            _ => throw new Exception()
         };

         var calledActionOn = value.MapPartially(@default: (object)"default",
                                                 int32: 42,
                                                 string2: "text2",
                                                 string3: "text3",
                                                 nullableInt32: 43);

         calledActionOn.Should().Be(expected);
      }
   }

   public class HavingStruct
   {
      [Theory]
      [InlineData(1, "text")]
      [InlineData(2, 42)]
      public void Should_use_correct_arg_having_2_types(int index, object expected)
      {
         var value = index switch
         {
            1 => new TestUnion_struct_string_int("text"),
            2 => new TestUnion_struct_string_int(42),
            _ => throw new Exception()
         };

         var calledActionOn = value.MapPartially(@default: (object)"default",
                                                 @string: "text",
                                                 int32: 42);

         calledActionOn.Should().Be(expected);
      }

      [Theory]
      [InlineData(1, "default")]
      [InlineData(2, 42)]
      public void Should_use_default_having_2_types(int index, object expected)
      {
         var value = index switch
         {
            1 => new TestUnion_struct_string_int("text"),
            2 => new TestUnion_struct_string_int(42),
            _ => throw new Exception()
         };

         var calledActionOn = value.MapPartially(@default: (object)"default",
                                                 int32: 42);

         calledActionOn.Should().Be(expected);
      }
   }
}
