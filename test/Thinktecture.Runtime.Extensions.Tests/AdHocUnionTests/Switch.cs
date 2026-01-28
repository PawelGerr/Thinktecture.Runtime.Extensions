using System;
using Thinktecture.Runtime.Tests.TestAdHocUnions;

namespace Thinktecture.Runtime.Tests.AdHocUnionTests;

// ReSharper disable once InconsistentNaming
public class Switch
{
   public class HavingClass
   {
      public class WithAction
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
            object calledActionOn = null;

            value.Switch(@string: v =>
                         {
                            calledActionOn = v;
                         },
                         int32: v =>
                         {
                            calledActionOn = v;
                         });

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
            object calledActionOn = null;

            value.Switch(@string: v =>
                         {
                            calledActionOn = v;
                         },
                         int32: v =>
                         {
                            calledActionOn = v;
                         },
                         boolean: v =>
                         {
                            calledActionOn = v;
                         });

            calledActionOn.Should().Be(expected);
         }

         [Theory]
         [InlineData(1, "text")]
         [InlineData(2, 42)]
         [InlineData(3, true)]
         [InlineData(4, "2A986EEB-1B82-46F8-A7F3-401ADC22BE33")]
         public void Should_use_correct_arg_having_4_values(int index, object expected)
         {
            var value = index switch
            {
               1 => new TestUnion_class_string_int_bool_guid("text"),
               2 => new TestUnion_class_string_int_bool_guid(42),
               3 => new TestUnion_class_string_int_bool_guid(true),
               4 => new TestUnion_class_string_int_bool_guid(new Guid("2A986EEB-1B82-46F8-A7F3-401ADC22BE33")),
               _ => throw new Exception()
            };
            object calledActionOn = null;

            value.Switch(@string: v =>
                         {
                            calledActionOn = v;
                         },
                         int32: v =>
                         {
                            calledActionOn = v;
                         },
                         boolean: v =>
                         {
                            calledActionOn = v;
                         },
                         guid: v =>
                         {
                            calledActionOn = v;
                         });

            calledActionOn.Should().Be(index == 4 ? new Guid((string)expected) : expected);
         }

         [Theory]
         [InlineData(1, "text")]
         [InlineData(2, 42)]
         [InlineData(3, true)]
         [InlineData(4, "2A986EEB-1B82-46F8-A7F3-401ADC22BE33")]
         [InlineData(5, 'A')]
         public void Should_use_correct_arg_having_5_values(int index, object expected)
         {
            var value = index switch
            {
               1 => new TestUnion_class_string_int_bool_guid_char("text"),
               2 => new TestUnion_class_string_int_bool_guid_char(42),
               3 => new TestUnion_class_string_int_bool_guid_char(true),
               4 => new TestUnion_class_string_int_bool_guid_char(new Guid("2A986EEB-1B82-46F8-A7F3-401ADC22BE33")),
               5 => new TestUnion_class_string_int_bool_guid_char('A'),
               _ => throw new Exception()
            };
            object calledActionOn = null;

            value.Switch(@string: v =>
                         {
                            calledActionOn = v;
                         },
                         int32: v =>
                         {
                            calledActionOn = v;
                         },
                         boolean: v =>
                         {
                            calledActionOn = v;
                         },
                         guid: v =>
                         {
                            calledActionOn = v;
                         },
                         @char: v =>
                         {
                            calledActionOn = v;
                         });

            calledActionOn.Should().Be(index == 4 ? new Guid((string)expected) : expected);
         }

         [Theory]
         [InlineData(1, "text")]
         [InlineData(2, 42)]
         [InlineData(3, "text2")]
         [InlineData(4, "text3")]
         [InlineData(5, 43)]
         public void Should_use_correct_arg_having_5_values_with_duplicates(int index, object expected)
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
            object calledActionOn = null;

            value.Switch(text: v =>
                         {
                            calledActionOn = v;
                         },
                         int32: v =>
                         {
                            calledActionOn = v;
                         },
                         string2: v =>
                         {
                            calledActionOn = v;
                         },
                         string3: v =>
                         {
                            calledActionOn = v;
                         },
                         nullableOfInt32: v =>
                         {
                            calledActionOn = v;
                         });

            calledActionOn.Should().Be(expected);
         }
      }

      public class WithActionAndState
      {
         [Theory]
         [InlineData(1, "text")]
         [InlineData(2, 42)]
         public void Should_pass_context_having_2_types(int index, object expected)
         {
            var value = index switch
            {
               1 => new TestUnion_class_string_int("text"),
               2 => new TestUnion_class_string_int(42),
               _ => throw new Exception()
            };

            var state = new object();

            object calledActionOn = null;

            value.Switch(state,
                         @string: (s, v) =>
                         {
                            s.Should().Be(s);
                            calledActionOn = v;
                         },
                         int32: (s, v) =>
                         {
                            s.Should().Be(state);
                            calledActionOn = v;
                         });

            calledActionOn.Should().Be(expected);
         }

#if NET9_0_OR_GREATER
         [Theory]
         [InlineData(1, "text")]
         [InlineData(2, 42)]
         public void Should_pass_context_having_2_types_and_ref_struct(int index, object expected)
         {
            var value = index switch
            {
               1 => new TestUnion_class_string_int("text"),
               2 => new TestUnion_class_string_int(42),
               _ => throw new Exception()
            };

            var state = new TestRefStruct(42);

            object calledActionOn = null;

            value.Switch(state,
                         @string: (s, v) =>
                                  {
                                     s.Value.Should().Be(42);
                                     calledActionOn = v;
                                  },
                         int32: (s, v) =>
                                {
                                   s.Value.Should().Be(42);
                                   calledActionOn = v;
                                });

            calledActionOn.Should().Be(expected);
         }
#endif

         [Theory]
         [InlineData(1, "text")]
         [InlineData(2, 42)]
         [InlineData(3, true)]
         public void Should_pass_context_having_3_types(int index, object expected)
         {
            var value = index switch
            {
               1 => new TestUnion_class_string_int_bool("text"),
               2 => new TestUnion_class_string_int_bool(42),
               3 => new TestUnion_class_string_int_bool(true),
               _ => throw new Exception()
            };

            var state = new object();

            object calledActionOn = null;

            value.Switch(state,
                         @string: (s, v) =>
                         {
                            s.Should().Be(state);
                            calledActionOn = v;
                         },
                         int32: (s, v) =>
                         {
                            s.Should().Be(state);
                            calledActionOn = v;
                         },
                         boolean: (s, v) =>
                         {
                            s.Should().Be(state);
                            calledActionOn = v;
                         });

            calledActionOn.Should().Be(expected);
         }

         [Theory]
         [InlineData(1, "text")]
         [InlineData(2, 42)]
         [InlineData(3, true)]
         [InlineData(4, "3E85ABD4-621A-4F58-8926-A842D71BB230")]
         public void Should_pass_context_having_4_types(int index, object expected)
         {
            var value = index switch
            {
               1 => new TestUnion_class_string_int_bool_guid("text"),
               2 => new TestUnion_class_string_int_bool_guid(42),
               3 => new TestUnion_class_string_int_bool_guid(true),
               4 => new TestUnion_class_string_int_bool_guid(new Guid("3E85ABD4-621A-4F58-8926-A842D71BB230")),
               _ => throw new Exception()
            };

            var state = new object();

            object calledActionOn = null;

            value.Switch(state,
                         @string: (s, v) =>
                         {
                            s.Should().Be(state);
                            calledActionOn = v;
                         },
                         int32: (s, v) =>
                         {
                            s.Should().Be(state);
                            calledActionOn = v;
                         },
                         boolean: (s, v) =>
                         {
                            s.Should().Be(state);
                            calledActionOn = v;
                         },
                         guid: (s, v) =>
                         {
                            s.Should().Be(state);
                            calledActionOn = v;
                         });

            calledActionOn.Should().Be(index == 4 ? new Guid((string)expected) : expected);
         }

         [Theory]
         [InlineData(1, "text")]
         [InlineData(2, 42)]
         [InlineData(3, true)]
         [InlineData(4, "3E85ABD4-621A-4F58-8926-A842D71BB230")]
         [InlineData(5, 'A')]
         public void Should_pass_context_having_5_types(int index, object expected)
         {
            var value = index switch
            {
               1 => new TestUnion_class_string_int_bool_guid_char("text"),
               2 => new TestUnion_class_string_int_bool_guid_char(42),
               3 => new TestUnion_class_string_int_bool_guid_char(true),
               4 => new TestUnion_class_string_int_bool_guid_char(new Guid("3E85ABD4-621A-4F58-8926-A842D71BB230")),
               5 => new TestUnion_class_string_int_bool_guid_char('A'),
               _ => throw new Exception()
            };

            var state = new object();

            object calledActionOn = null;

            value.Switch(state,
                         @string: (s, v) =>
                         {
                            s.Should().Be(state);
                            calledActionOn = v;
                         },
                         int32: (s, v) =>
                         {
                            s.Should().Be(state);
                            calledActionOn = v;
                         },
                         boolean: (s, v) =>
                         {
                            s.Should().Be(state);
                            calledActionOn = v;
                         },
                         guid: (s, v) =>
                         {
                            s.Should().Be(state);
                            calledActionOn = v;
                         },
                         @char: (s, v) =>
                         {
                            s.Should().Be(state);
                            calledActionOn = v;
                         });

            calledActionOn.Should().Be(index == 4 ? new Guid((string)expected) : expected);
         }
      }

      public class WithFunc
      {
         [Theory]
         [InlineData(1, "text")]
         [InlineData(2, 42)]
         public void Should_call_correct_arg_having_2_types(int index, object expected)
         {
            var value = index switch
            {
               1 => new TestUnion_class_string_int("text"),
               2 => new TestUnion_class_string_int(42),
               _ => throw new Exception()
            };

            var calledActionOn = value.Switch(@string: v => (object)v,
                                              int32: v => v);

            calledActionOn.Should().Be(expected);
         }

#if NET9_0_OR_GREATER
         [Theory]
         [InlineData(1, "text")]
         [InlineData(2, 42)]
         public void Should_call_correct_arg_having_2_types_returning_ref_struct(int index, object expected)
         {
            var value = index switch
            {
               1 => new TestUnion_class_string_int("text"),
               2 => new TestUnion_class_string_int(42),
               _ => throw new Exception()
            };

            var calledActionOn = value.Switch(@string: v => new TestRefStruct(v),
                                              int32: v => new TestRefStruct(v));

            calledActionOn.Value.Should().Be(expected);
         }
#endif

         [Theory]
         [InlineData(1, "text")]
         [InlineData(2, 42)]
         [InlineData(3, true)]
         public void Should_call_correct_arg_having_3_types(int index, object expected)
         {
            var value = index switch
            {
               1 => new TestUnion_class_string_int_bool("text"),
               2 => new TestUnion_class_string_int_bool(42),
               3 => new TestUnion_class_string_int_bool(true),
               _ => throw new Exception()
            };

            var calledActionOn = value.Switch(@string: v => (object)v,
                                              int32: v => v,
                                              boolean: v => v);

            calledActionOn.Should().Be(expected);
         }

         [Theory]
         [InlineData(1, "text")]
         [InlineData(2, 42)]
         [InlineData(3, true)]
         [InlineData(4, "F8002E79-5465-4797-AD3F-A6503ADF066E")]
         public void Should_call_correct_arg_having_4_types(int index, object expected)
         {
            var value = index switch
            {
               1 => new TestUnion_class_string_int_bool_guid("text"),
               2 => new TestUnion_class_string_int_bool_guid(42),
               3 => new TestUnion_class_string_int_bool_guid(true),
               4 => new TestUnion_class_string_int_bool_guid(new Guid("F8002E79-5465-4797-AD3F-A6503ADF066E")),
               _ => throw new Exception()
            };

            var calledActionOn = value.Switch(@string: v => (object)v,
                                              int32: v => v,
                                              boolean: v => v,
                                              guid: v => v);

            calledActionOn.Should().Be(index == 4 ? new Guid((string)expected) : expected);
         }

         [Theory]
         [InlineData(1, "text")]
         [InlineData(2, 42)]
         [InlineData(3, true)]
         [InlineData(4, "F8002E79-5465-4797-AD3F-A6503ADF066E")]
         [InlineData(5, 'A')]
         public void Should_call_correct_arg_having_5_types(int index, object expected)
         {
            var value = index switch
            {
               1 => new TestUnion_class_string_int_bool_guid_char("text"),
               2 => new TestUnion_class_string_int_bool_guid_char(42),
               3 => new TestUnion_class_string_int_bool_guid_char(true),
               4 => new TestUnion_class_string_int_bool_guid_char(new Guid("F8002E79-5465-4797-AD3F-A6503ADF066E")),
               5 => new TestUnion_class_string_int_bool_guid_char('A'),
               _ => throw new Exception()
            };

            var calledActionOn = value.Switch(@string: v => (object)v,
                                              int32: v => v,
                                              boolean: v => v,
                                              guid: v => v,
                                              @char: v => v);

            calledActionOn.Should().Be(index == 4 ? new Guid((string)expected) : expected);
         }

         [Fact]
         public void Should_call_correct_arg_with_stateless_type_T1()
         {
            var union1 = new TestUnion_class_stateless_nullvaluestruct_string(new NullValueStruct());
            var result1 = union1.Switch(
               nullValueStruct: _ => "marker",
               @string: v => v);
            result1.Should().Be("marker");

            var union2 = new TestUnion_class_stateless_nullvaluestruct_string("text");
            var result2 = union2.Switch(
               nullValueStruct: _ => "marker",
               @string: v => v);
            result2.Should().Be("text");
         }

         [Fact]
         public void Should_call_correct_arg_with_stateless_type_T2()
         {
            var union1 = new TestUnion_class_string_stateless_emptystatestruct("text");
            var result1 = union1.Switch(
               @string: v => v,
               emptyStateStruct: _ => "marker");
            result1.Should().Be("text");

            var union2 = new TestUnion_class_string_stateless_emptystatestruct(new EmptyStateStruct());
            var result2 = union2.Switch(
               @string: v => v,
               emptyStateStruct: _ => "marker");
            result2.Should().Be("marker");
         }

         [Fact]
         public void Should_call_correct_arg_with_multiple_stateless_types()
         {
            var union1 = new TestUnion_class_stateless_nullvaluestruct_stateless_emptystatestruct_string(new NullValueStruct());
            var result1 = union1.Switch(
               nullValueStruct: _ => "null",
               emptyStateStruct: _ => "empty",
               @string: v => v);
            result1.Should().Be("null");

            var union2 = new TestUnion_class_stateless_nullvaluestruct_stateless_emptystatestruct_string(new EmptyStateStruct());
            var result2 = union2.Switch(
               nullValueStruct: _ => "null",
               emptyStateStruct: _ => "empty",
               @string: v => v);
            result2.Should().Be("empty");

            var union3 = new TestUnion_class_stateless_nullvaluestruct_stateless_emptystatestruct_string("text");
            var result3 = union3.Switch(
               nullValueStruct: _ => "null",
               emptyStateStruct: _ => "empty",
               @string: v => v);
            result3.Should().Be("text");
         }

         [Fact]
         public void Should_call_correct_arg_with_nullable_struct_stateless()
         {
            var union1 = new TestUnion_class_string_stateless_emptystatestruct_nullable("text");
            var result1 = union1.Switch(
               @string: v => v,
               nullableOfEmptyStateStruct: _ => "marker");
            result1.Should().Be("text");

            var union2 = new TestUnion_class_string_stateless_emptystatestruct_nullable((EmptyStateStruct?)null);
            var result2 = union2.Switch(
               @string: v => v,
               nullableOfEmptyStateStruct: _ => "marker");
            result2.Should().Be("marker");

            var union3 = new TestUnion_class_string_stateless_emptystatestruct_nullable((EmptyStateStruct?)new EmptyStateStruct());
            var result3 = union3.Switch(
               @string: v => v,
               nullableOfEmptyStateStruct: _ => "marker");
            result3.Should().Be("marker");
         }

         [Fact]
         public void Should_call_correct_arg_with_reference_type_stateless()
         {
            var union1 = new TestUnion_class_stateless_nullvalueclass_string(new NullValueClass());
            var result1 = union1.Switch(
               nullValueClass: _ => "marker",
               @string: v => v);
            result1.Should().Be("marker");

            var union2 = new TestUnion_class_stateless_nullvalueclass_string("text");
            var result2 = union2.Switch(
               nullValueClass: _ => "marker",
               @string: v => v);
            result2.Should().Be("text");
         }

         [Fact]
         public void Should_call_correct_arg_with_multiple_reference_type_stateless()
         {
            var union1 = new TestUnion_class_stateless_nullvalueclass_stateless_emptystateclass_string(new NullValueClass());
            var result1 = union1.Switch(
               nullValueClass: _ => "null",
               emptyStateClass: _ => "empty",
               @string: v => v);
            result1.Should().Be("null");

            var union2 = new TestUnion_class_stateless_nullvalueclass_stateless_emptystateclass_string(new EmptyStateClass());
            var result2 = union2.Switch(
               nullValueClass: _ => "null",
               emptyStateClass: _ => "empty",
               @string: v => v);
            result2.Should().Be("empty");

            var union3 = new TestUnion_class_stateless_nullvalueclass_stateless_emptystateclass_string("text");
            var result3 = union3.Switch(
               nullValueClass: _ => "null",
               emptyStateClass: _ => "empty",
               @string: v => v);
            result3.Should().Be("text");
         }

         [Fact]
         public void Should_switch_correctly_with_duplicate_value_struct_stateless()
         {
            var union1 = TestUnion_class_stateless_nullvaluestruct_stateless_nullvaluestruct_string.CreateNullValue1(default);
            var result1 = union1.Switch(
               nullValue1: _ => "marker1",
               nullValue2: _ => "marker2",
               @string: _ => "text");
            result1.Should().Be("marker1");

            var union2 = TestUnion_class_stateless_nullvaluestruct_stateless_nullvaluestruct_string.CreateNullValue2(default);
            var result2 = union2.Switch(
               nullValue1: _ => "marker1",
               nullValue2: _ => "marker2",
               @string: _ => "text");
            result2.Should().Be("marker2");

            var union3 = new TestUnion_class_stateless_nullvaluestruct_stateless_nullvaluestruct_string("actual");
            var result3 = union3.Switch(
               nullValue1: _ => "marker1",
               nullValue2: _ => "marker2",
               @string: _ => "text");
            result3.Should().Be("text");
         }

         [Fact]
         public void Should_switch_correctly_with_duplicate_reference_type_stateless()
         {
            var union1 = TestUnion_class_stateless_nullvalueclass_stateless_nullvalueclass_int.CreateNullValueClass1(null);
            var result1 = union1.Switch(
               nullValueClass1: _ => "marker1",
               nullValueClass2: _ => "marker2",
               int32: v => v.ToString());
            result1.Should().Be("marker1");

            var union2 = TestUnion_class_stateless_nullvalueclass_stateless_nullvalueclass_int.CreateNullValueClass2(null);
            var result2 = union2.Switch(
               nullValueClass1: _ => "marker1",
               nullValueClass2: _ => "marker2",
               int32: v => v.ToString());
            result2.Should().Be("marker2");

            var union3 = new TestUnion_class_stateless_nullvalueclass_stateless_nullvalueclass_int(42);
            var result3 = union3.Switch(
               nullValueClass1: _ => "marker1",
               nullValueClass2: _ => "marker2",
               int32: v => v.ToString());
            result3.Should().Be("42");
         }
      }

      public class WithFuncAndContext
      {
         [Theory]
         [InlineData(1, "text")]
         [InlineData(2, 42)]
         public void Should_pass_context_having_2_types(int index, object expected)
         {
            var value = index switch
            {
               1 => new TestUnion_class_string_int("text"),
               2 => new TestUnion_class_string_int(42),
               _ => throw new Exception()
            };

            var state = new object();
            var calledActionOn = value.Switch(state,
                                              @string: (s, v) =>
                                              {
                                                 s.Should().Be(state);
                                                 return (object)v;
                                              },
                                              int32: (s, v) =>
                                              {
                                                 s.Should().Be(state);
                                                 return v;
                                              });

            calledActionOn.Should().Be(expected);
         }

#if NET9_0_OR_GREATER
         [Theory]
         [InlineData(1, "text")]
         [InlineData(2, 42)]
         public void Should_pass_context_having_2_types_and_ref_struct(int index, object expected)
         {
            var value = index switch
            {
               1 => new TestUnion_class_string_int("text"),
               2 => new TestUnion_class_string_int(42),
               _ => throw new Exception()
            };

            var state = new TestRefStruct(42);

            var calledActionOn = value.Switch(state,
                                              @string: (s, v) =>
                                                       {
                                                          s.Value.Should().Be(42);
                                                          return new TestRefStruct(v);
                                                       },
                                              int32: (s, v) =>
                                                     {
                                                        s.Value.Should().Be(42);
                                                        return new TestRefStruct(v);
                                                     });

            calledActionOn.Value.Should().Be(expected);
         }
#endif

         [Theory]
         [InlineData(1, "text")]
         [InlineData(2, 42)]
         [InlineData(3, true)]
         public void Should_pass_context_having_3_types(int index, object expected)
         {
            var value = index switch
            {
               1 => new TestUnion_class_string_int_bool("text"),
               2 => new TestUnion_class_string_int_bool(42),
               3 => new TestUnion_class_string_int_bool(true),
               _ => throw new Exception()
            };

            var state = new object();
            var calledActionOn = value.Switch(state,
                                              @string: (s, v) =>
                                              {
                                                 s.Should().Be(state);
                                                 return (object)v;
                                              },
                                              int32: (s, v) =>
                                              {
                                                 s.Should().Be(state);
                                                 return v;
                                              },
                                              boolean: (s, v) =>
                                              {
                                                 s.Should().Be(state);
                                                 return v;
                                              });

            calledActionOn.Should().Be(expected);
         }

         [Theory]
         [InlineData(1, "text")]
         [InlineData(2, 42)]
         [InlineData(3, true)]
         [InlineData(4, "6EF10862-7FC4-4AEB-BC92-21E798AC54D0")]
         public void Should_pass_context_having_4_types(int index, object expected)
         {
            var value = index switch
            {
               1 => new TestUnion_class_string_int_bool_guid("text"),
               2 => new TestUnion_class_string_int_bool_guid(42),
               3 => new TestUnion_class_string_int_bool_guid(true),
               4 => new TestUnion_class_string_int_bool_guid(new Guid("6EF10862-7FC4-4AEB-BC92-21E798AC54D0")),
               _ => throw new Exception()
            };

            var state = new object();
            var calledActionOn = value.Switch(state,
                                              @string: (s, v) =>
                                              {
                                                 s.Should().Be(state);
                                                 return (object)v;
                                              },
                                              int32: (s, v) =>
                                              {
                                                 s.Should().Be(state);
                                                 return v;
                                              },
                                              boolean: (s, v) =>
                                              {
                                                 s.Should().Be(state);
                                                 return v;
                                              },
                                              guid: (s, v) =>
                                              {
                                                 s.Should().Be(state);
                                                 return v;
                                              });

            calledActionOn.Should().Be(index == 4 ? new Guid((string)expected) : expected);
         }

         [Theory]
         [InlineData(1, "text")]
         [InlineData(2, 42)]
         [InlineData(3, true)]
         [InlineData(4, "6EF10862-7FC4-4AEB-BC92-21E798AC54D0")]
         [InlineData(5, 'A')]
         public void Should_pass_context_having_5_types(int index, object expected)
         {
            var value = index switch
            {
               1 => new TestUnion_class_string_int_bool_guid_char("text"),
               2 => new TestUnion_class_string_int_bool_guid_char(42),
               3 => new TestUnion_class_string_int_bool_guid_char(true),
               4 => new TestUnion_class_string_int_bool_guid_char(new Guid("6EF10862-7FC4-4AEB-BC92-21E798AC54D0")),
               5 => new TestUnion_class_string_int_bool_guid_char('A'),
               _ => throw new Exception()
            };

            var state = new object();
            var calledActionOn = value.Switch(state,
                                              @string: (s, v) =>
                                              {
                                                 s.Should().Be(state);
                                                 return (object)v;
                                              },
                                              int32: (s, v) =>
                                              {
                                                 s.Should().Be(state);
                                                 return v;
                                              },
                                              boolean: (s, v) =>
                                              {
                                                 s.Should().Be(state);
                                                 return v;
                                              },
                                              guid: (s, v) =>
                                              {
                                                 s.Should().Be(state);
                                                 return v;
                                              },
                                              @char: (s, v) =>
                                              {
                                                 s.Should().Be(state);
                                                 return v;
                                              });

            calledActionOn.Should().Be(index == 4 ? new Guid((string)expected) : expected);
         }
      }
   }

   public class HavingStruct
   {
      public class WithAction
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
            object calledActionOn = null;

            value.Switch(@string: v =>
                         {
                            calledActionOn = v;
                         },
                         int32: v =>
                         {
                            calledActionOn = v;
                         });

            calledActionOn.Should().Be(expected);
         }
      }

      public class WithActionAndState
      {
         [Theory]
         [InlineData(1, "text")]
         [InlineData(2, 42)]
         public void Should_pass_context_having_2_types(int index, object expected)
         {
            var value = index switch
            {
               1 => new TestUnion_struct_string_int("text"),
               2 => new TestUnion_struct_string_int(42),
               _ => throw new Exception()
            };

            var state = new object();

            object calledActionOn = null;

            value.Switch(state,
                         @string: (s, v) =>
                         {
                            s.Should().Be(state);
                            calledActionOn = v;
                         },
                         int32: (s, v) =>
                         {
                            s.Should().Be(state);
                            calledActionOn = v;
                         });

            calledActionOn.Should().Be(expected);
         }
      }

      public class WithFunc
      {
         [Theory]
         [InlineData(1, "text")]
         [InlineData(2, 42)]
         public void Should_call_correct_arg_having_2_types(int index, object expected)
         {
            var value = index switch
            {
               1 => new TestUnion_struct_string_int("text"),
               2 => new TestUnion_struct_string_int(42),
               _ => throw new Exception()
            };

            var calledActionOn = value.Switch(@string: v => (object)v,
                                              int32: v => v);

            calledActionOn.Should().Be(expected);
         }

         [Fact]
         public void Should_call_correct_arg_with_stateless_type_in_struct_union()
         {
            var union1 = new TestUnion_struct_stateless_nullvaluestruct_int(new NullValueStruct());
            var result1 = union1.Switch(
               nullValueStruct: _ => 0,
               int32: v => v);
            result1.Should().Be(0);

            var union2 = new TestUnion_struct_stateless_nullvaluestruct_int(42);
            var result2 = union2.Switch(
               nullValueStruct: _ => 0,
               int32: v => v);
            result2.Should().Be(42);
         }

         [Fact]
         public void Should_switch_correctly_with_duplicate_markers_in_struct_union()
         {
            var union1 = TestUnion_struct_stateless_nullvaluestruct_stateless_nullvaluestruct_int.CreateNullValue1(default);
            var result1 = union1.Switch(
               nullValue1: _ => 0,
               nullValue2: _ => -1,
               int32: v => v);
            result1.Should().Be(0);

            var union2 = TestUnion_struct_stateless_nullvaluestruct_stateless_nullvaluestruct_int.CreateNullValue2(default);
            var result2 = union2.Switch(
               nullValue1: _ => 0,
               nullValue2: _ => -1,
               int32: v => v);
            result2.Should().Be(-1);

            var union3 = new TestUnion_struct_stateless_nullvaluestruct_stateless_nullvaluestruct_int(42);
            var result3 = union3.Switch(
               nullValue1: _ => 0,
               nullValue2: _ => -1,
               int32: v => v);
            result3.Should().Be(42);
         }
      }

      public class WithFuncAndContext
      {
         [Theory]
         [InlineData(1, "text")]
         [InlineData(2, 42)]
         public void Should_pass_context_having_2_types(int index, object expected)
         {
            var value = index switch
            {
               1 => new TestUnion_struct_string_int("text"),
               2 => new TestUnion_struct_string_int(42),
               _ => throw new Exception()
            };

            var state = new object();
            var calledActionOn = value.Switch(state,
                                              @string: (s, v) =>
                                              {
                                                 s.Should().Be(state);
                                                 return (object)v;
                                              },
                                              int32: (s, v) =>
                                              {
                                                 s.Should().Be(state);
                                                 return v;
                                              });

            calledActionOn.Should().Be(expected);
         }
      }
   }
}
