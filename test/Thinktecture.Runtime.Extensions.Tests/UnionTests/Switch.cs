using System;
using Thinktecture.Runtime.Tests.TestUnions;

namespace Thinktecture.Runtime.Tests.UnionTests;

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
                         nullableInt32: v =>
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
                         @string: (ctx, v) =>
                                  {
                                     ctx.Should().Be(state);
                                     calledActionOn = v;
                                  },
                         int32: (ctx, v) =>
                                {
                                   ctx.Should().Be(state);
                                   calledActionOn = v;
                                },
                         boolean: (ctx, v) =>
                                  {
                                     ctx.Should().Be(state);
                                     calledActionOn = v;
                                  },
                         guid: (ctx, v) =>
                               {
                                  ctx.Should().Be(state);
                                  calledActionOn = v;
                               },
                         @char: (ctx, v) =>
                                {
                                   ctx.Should().Be(state);
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
                                              @string: (ctx, v) =>
                                                       {
                                                          ctx.Should().Be(state);
                                                          return (object)v;
                                                       },
                                              int32: (ctx, v) =>
                                                     {
                                                        ctx.Should().Be(state);
                                                        return v;
                                                     });

            calledActionOn.Should().Be(expected);
         }

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
                                              @string: (ctx, v) =>
                                                       {
                                                          ctx.Should().Be(state);
                                                          return (object)v;
                                                       },
                                              int32: (ctx, v) =>
                                                     {
                                                        ctx.Should().Be(state);
                                                        return v;
                                                     },
                                              boolean: (ctx, v) =>
                                                       {
                                                          ctx.Should().Be(state);
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
                                              @string: (ctx, v) =>
                                                       {
                                                          ctx.Should().Be(state);
                                                          return (object)v;
                                                       },
                                              int32: (ctx, v) =>
                                                     {
                                                        ctx.Should().Be(state);
                                                        return v;
                                                     },
                                              boolean: (ctx, v) =>
                                                       {
                                                          ctx.Should().Be(state);
                                                          return v;
                                                       },
                                              guid: (ctx, v) =>
                                                    {
                                                       ctx.Should().Be(state);
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
                                              @string: (ctx, v) =>
                                                       {
                                                          ctx.Should().Be(state);
                                                          return (object)v;
                                                       },
                                              int32: (ctx, v) =>
                                                     {
                                                        ctx.Should().Be(state);
                                                        return v;
                                                     },
                                              boolean: (ctx, v) =>
                                                       {
                                                          ctx.Should().Be(state);
                                                          return v;
                                                       },
                                              guid: (ctx, v) =>
                                                    {
                                                       ctx.Should().Be(state);
                                                       return v;
                                                    },
                                              @char: (ctx, v) =>
                                                     {
                                                        ctx.Should().Be(state);
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
                         @string: (ctx, v) =>
                                  {
                                     ctx.Should().Be(state);
                                     calledActionOn = v;
                                  },
                         int32: (ctx, v) =>
                                {
                                   ctx.Should().Be(state);
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
                                              @string: (ctx, v) =>
                                                       {
                                                          ctx.Should().Be(state);
                                                          return (object)v;
                                                       },
                                              int32: (ctx, v) =>
                                                     {
                                                        ctx.Should().Be(state);
                                                        return v;
                                                     });

            calledActionOn.Should().Be(expected);
         }
      }
   }
}
