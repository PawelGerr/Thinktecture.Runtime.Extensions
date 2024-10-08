using System;
using Thinktecture.Runtime.Tests.TestUnions;

namespace Thinktecture.Runtime.Tests.UnionTests;

// ReSharper disable once InconsistentNaming
public class Map
{
   public class HavingClass
   {
      [Theory]
      [InlineData(1, "text")]
      [InlineData(2, 42)]
      public void Should_use_correct_arg_having_2_values(int index, object expected)
      {
         var value = index switch
         {
            1 => new TestUnion_class_string_int("text"),
            2 => new TestUnion_class_string_int(42),
            _ => throw new Exception()
         };

         var calledActionOn = value.Map(@string: (object)"text",
                                        int32: 42);

         calledActionOn.Should().Be(expected);
      }

      [Theory]
      [InlineData(1, "text")]
      [InlineData(2, 42)]
      [InlineData(3, true)]
      public void Should_use_correct_arg_having_3_values(int index, object expected)
      {
         var value = index switch
         {
            1 => new TestUnion_class_string_int_bool("text"),
            2 => new TestUnion_class_string_int_bool(42),
            3 => new TestUnion_class_string_int_bool(true),
            _ => throw new Exception()
         };

         var calledActionOn = value.Map(@string: (object)"text",
                                        int32: 42,
                                        boolean: true);

         calledActionOn.Should().Be(expected);
      }

      [Theory]
      [InlineData(1, "text")]
      [InlineData(2, 42)]
      [InlineData(3, true)]
      [InlineData(4, "4CB8C761-434B-4E34-83E0-C2E1BD4FAA0B")]
      public void Should_use_correct_arg_having_4_values(int index, object expected)
      {
         var value = index switch
         {
            1 => new TestUnion_class_string_int_bool_guid("text"),
            2 => new TestUnion_class_string_int_bool_guid(42),
            3 => new TestUnion_class_string_int_bool_guid(true),
            4 => new TestUnion_class_string_int_bool_guid(new Guid("4CB8C761-434B-4E34-83E0-C2E1BD4FAA0B")),
            _ => throw new Exception()
         };

         var calledActionOn = value.Map(@string: (object)"text",
                                        int32: 42,
                                        boolean: true,
                                        guid: new Guid("4CB8C761-434B-4E34-83E0-C2E1BD4FAA0B"));

         calledActionOn.Should().Be(index == 4 ? new Guid((string)expected) : expected);
      }

      [Theory]
      [InlineData(1, "text")]
      [InlineData(2, 42)]
      [InlineData(3, true)]
      [InlineData(4, "4CB8C761-434B-4E34-83E0-C2E1BD4FAA0B")]
      [InlineData(5, 'A')]
      public void Should_use_correct_arg_having_5_values(int index, object expected)
      {
         var value = index switch
         {
            1 => new TestUnion_class_string_int_bool_guid_char("text"),
            2 => new TestUnion_class_string_int_bool_guid_char(42),
            3 => new TestUnion_class_string_int_bool_guid_char(true),
            4 => new TestUnion_class_string_int_bool_guid_char(new Guid("4CB8C761-434B-4E34-83E0-C2E1BD4FAA0B")),
            5 => new TestUnion_class_string_int_bool_guid_char('A'),
            _ => throw new Exception()
         };

         var calledActionOn = value.Map(@string: (object)"text",
                                        int32: 42,
                                        boolean: true,
                                        guid: new Guid("4CB8C761-434B-4E34-83E0-C2E1BD4FAA0B"),
                                        @char: 'A');

         calledActionOn.Should().Be(index == 4 ? new Guid((string)expected) : expected);
      }
   }

   public class HavingStruct
   {
      [Theory]
      [InlineData(1, "text")]
      [InlineData(2, 42)]
      public void Should_use_correct_arg_having_2_values(int index, object expected)
      {
         var value = index switch
         {
            1 => new TestUnion_struct_string_int("text"),
            2 => new TestUnion_struct_string_int(42),
            _ => throw new Exception()
         };

         var calledActionOn = value.Map(@string: (object)"text",
                                        int32: 42);

         calledActionOn.Should().Be(expected);
      }
   }
}
