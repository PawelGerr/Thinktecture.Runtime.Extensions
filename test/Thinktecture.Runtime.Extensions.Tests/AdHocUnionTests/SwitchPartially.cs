using System;
using Thinktecture.Runtime.Tests.TestAdHocUnions;

namespace Thinktecture.Runtime.Tests.AdHocUnionTests;

// ReSharper disable once InconsistentNaming
public class SwitchPartially
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

            value.SwitchPartially(@string: v =>
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

            value.SwitchPartially(@string: v =>
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

            value.SwitchPartially(@string: v =>
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

            value.SwitchPartially(@string: v =>
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

            value.SwitchPartially(text: v =>
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

         [Theory]
         [InlineData(1, "default:text")]
         [InlineData(2, 42)]
         public void Should_use_default_arg_having_2_types(int index, object expected)
         {
            var value = index switch
            {
               1 => new TestUnion_class_string_int("text"),
               2 => new TestUnion_class_string_int(42),
               _ => throw new Exception()
            };
            object calledActionOn = null;

            value.SwitchPartially(@default: v =>
                                            {
                                               calledActionOn = $"default:{v}";
                                            },
                                  int32: v =>
                                         {
                                            calledActionOn = v;
                                         });

            calledActionOn.Should().Be(expected);
         }

         [Theory]
         [InlineData(1, "default:text")]
         [InlineData(2, 42)]
         [InlineData(3, true)]
         public void Should_use_default_arg_having_3_types(int index, object expected)
         {
            var value = index switch
            {
               1 => new TestUnion_class_string_int_bool("text"),
               2 => new TestUnion_class_string_int_bool(42),
               3 => new TestUnion_class_string_int_bool(true),
               _ => throw new Exception()
            };
            object calledActionOn = null;

            value.SwitchPartially(@default: v =>
                                            {
                                               calledActionOn = $"default:{v}";
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
         [InlineData(1, "default:text")]
         [InlineData(2, 42)]
         [InlineData(3, true)]
         [InlineData(4, "DA9B90DD-AF66-4856-B084-1B1BB21DEA9B")]
         public void Should_use_default_arg_having_4_types(int index, object expected)
         {
            var value = index switch
            {
               1 => new TestUnion_class_string_int_bool_guid("text"),
               2 => new TestUnion_class_string_int_bool_guid(42),
               3 => new TestUnion_class_string_int_bool_guid(true),
               4 => new TestUnion_class_string_int_bool_guid(new Guid("DA9B90DD-AF66-4856-B084-1B1BB21DEA9B")),
               _ => throw new Exception()
            };
            object calledActionOn = null;

            value.SwitchPartially(@default: v =>
                                            {
                                               calledActionOn = $"default:{v}";
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
         [InlineData(1, "default:text")]
         [InlineData(2, 42)]
         [InlineData(3, true)]
         [InlineData(4, "DA9B90DD-AF66-4856-B084-1B1BB21DEA9B")]
         [InlineData(5, 'A')]
         public void Should_use_default_arg_having_5_types(int index, object expected)
         {
            var value = index switch
            {
               1 => new TestUnion_class_string_int_bool_guid_char("text"),
               2 => new TestUnion_class_string_int_bool_guid_char(42),
               3 => new TestUnion_class_string_int_bool_guid_char(true),
               4 => new TestUnion_class_string_int_bool_guid_char(new Guid("DA9B90DD-AF66-4856-B084-1B1BB21DEA9B")),
               5 => new TestUnion_class_string_int_bool_guid_char('A'),
               _ => throw new Exception()
            };
            object calledActionOn = null;

            value.SwitchPartially(@default: v =>
                                            {
                                               calledActionOn = $"default:{v}";
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
         [InlineData(1, "default:text")]
         [InlineData(2, 42)]
         [InlineData(3, "text2")]
         [InlineData(4, "text3")]
         [InlineData(5, 43)]
         public void Should_use_default_arg_having_5_types_with_duplicates(int index, object expected)
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

            value.SwitchPartially(@default: v =>
                                            {
                                               calledActionOn = $"default:{v}";
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

            value.SwitchPartially(state,
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

            value.SwitchPartially(state,
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
                                           });

            calledActionOn.Should().Be(expected);
         }

         [Theory]
         [InlineData(1, "text")]
         [InlineData(2, 42)]
         [InlineData(3, true)]
         [InlineData(4, "9FCEBE8E-AEED-4ADE-B597-562AFB9C9733")]
         public void Should_pass_context_having_5_types(int index, object expected)
         {
            var value = index switch
            {
               1 => new TestUnion_class_string_int_bool_guid("text"),
               2 => new TestUnion_class_string_int_bool_guid(42),
               3 => new TestUnion_class_string_int_bool_guid(true),
               4 => new TestUnion_class_string_int_bool_guid(new Guid("9FCEBE8E-AEED-4ADE-B597-562AFB9C9733")),
               _ => throw new Exception()
            };

            var state = new object();
            object calledActionOn = null;

            value.SwitchPartially(state,
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
                                        });

            calledActionOn.Should().Be(index == 4 ? new Guid((string)expected) : expected);
         }

         [Theory]
         [InlineData(1, "default:text")]
         [InlineData(2, 42)]
         public void Should_pass_context_to_default_having_2_types(int index, object expected)
         {
            var value = index switch
            {
               1 => new TestUnion_class_string_int("text"),
               2 => new TestUnion_class_string_int(42),
               _ => throw new Exception()
            };

            var state = new object();
            object calledActionOn = null;

            value.SwitchPartially(state,
                                  @default: (ctx, v) =>
                                            {
                                               ctx.Should().Be(state);
                                               calledActionOn = $"default:{v}";
                                            },
                                  int32: (ctx, v) =>
                                         {
                                            ctx.Should().Be(state);
                                            calledActionOn = v;
                                         });

            calledActionOn.Should().Be(expected);
         }

         [Theory]
         [InlineData(1, "default:text")]
         [InlineData(2, 42)]
         [InlineData(3, true)]
         public void Should_pass_context_to_default_having_3_types(int index, object expected)
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

            value.SwitchPartially(state,
                                  @default: (ctx, v) =>
                                            {
                                               ctx.Should().Be(state);
                                               calledActionOn = $"default:{v}";
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
                                           });

            calledActionOn.Should().Be(expected);
         }

         [Theory]
         [InlineData(1, "default:text")]
         [InlineData(2, 42)]
         [InlineData(3, true)]
         [InlineData(4, "81F697B0-5B9B-4441-89EB-2970A85C1069")]
         public void Should_pass_context_to_default_having_4_types(int index, object expected)
         {
            var value = index switch
            {
               1 => new TestUnion_class_string_int_bool_guid("text"),
               2 => new TestUnion_class_string_int_bool_guid(42),
               3 => new TestUnion_class_string_int_bool_guid(true),
               4 => new TestUnion_class_string_int_bool_guid(new Guid("81F697B0-5B9B-4441-89EB-2970A85C1069")),
               _ => throw new Exception()
            };

            var state = new object();
            object calledActionOn = null;

            value.SwitchPartially(state,
                                  @default: (ctx, v) =>
                                            {
                                               ctx.Should().Be(state);
                                               calledActionOn = $"default:{v}";
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
                                        });

            calledActionOn.Should().Be(index == 4 ? new Guid((string)expected) : expected);
         }

         [Theory]
         [InlineData(1, "default:text")]
         [InlineData(2, 42)]
         [InlineData(3, true)]
         [InlineData(4, "81F697B0-5B9B-4441-89EB-2970A85C1069")]
         [InlineData(5, 'A')]
         public void Should_pass_context_to_default_having_5_types(int index, object expected)
         {
            var value = index switch
            {
               1 => new TestUnion_class_string_int_bool_guid_char("text"),
               2 => new TestUnion_class_string_int_bool_guid_char(42),
               3 => new TestUnion_class_string_int_bool_guid_char(true),
               4 => new TestUnion_class_string_int_bool_guid_char(new Guid("81F697B0-5B9B-4441-89EB-2970A85C1069")),
               5 => new TestUnion_class_string_int_bool_guid_char('A'),
               _ => throw new Exception()
            };

            var state = new object();
            object calledActionOn = null;

            value.SwitchPartially(state,
                                  @default: (ctx, v) =>
                                            {
                                               ctx.Should().Be(state);
                                               calledActionOn = $"default:{v}";
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
         public void Should_use_correct_arg_having_2_types(int index, object expected)
         {
            var value = index switch
            {
               1 => new TestUnion_class_string_int("text"),
               2 => new TestUnion_class_string_int(42),
               _ => throw new Exception()
            };

            var calledActionOn = value.SwitchPartially(@default: v => $"default:{v}",
                                                       @string: v => (object)v,
                                                       int32: v => v);

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

            var calledActionOn = value.SwitchPartially(@default: v => $"default:{v}",
                                                       @string: v => (object)v,
                                                       int32: v => v,
                                                       boolean: v => v);

            calledActionOn.Should().Be(expected);
         }

         [Theory]
         [InlineData(1, "text")]
         [InlineData(2, 42)]
         [InlineData(3, true)]
         [InlineData(4, "D4EF64BB-730B-4D5C-94A1-C019F83EF945")]
         public void Should_use_correct_arg_having_4_types(int index, object expected)
         {
            var value = index switch
            {
               1 => new TestUnion_class_string_int_bool_guid("text"),
               2 => new TestUnion_class_string_int_bool_guid(42),
               3 => new TestUnion_class_string_int_bool_guid(true),
               4 => new TestUnion_class_string_int_bool_guid(new Guid("D4EF64BB-730B-4D5C-94A1-C019F83EF945")),
               _ => throw new Exception()
            };

            var calledActionOn = value.SwitchPartially(@default: v => $"default:{v}",
                                                       @string: v => (object)v,
                                                       int32: v => v,
                                                       boolean: v => v,
                                                       guid: v => v);

            calledActionOn.Should().Be(index == 4 ? new Guid((string)expected) : expected);
         }

         [Theory]
         [InlineData(1, "text")]
         [InlineData(2, 42)]
         [InlineData(3, true)]
         [InlineData(4, "D4EF64BB-730B-4D5C-94A1-C019F83EF945")]
         [InlineData(5, 'A')]
         public void Should_use_correct_arg_having_5_types(int index, object expected)
         {
            var value = index switch
            {
               1 => new TestUnion_class_string_int_bool_guid_char("text"),
               2 => new TestUnion_class_string_int_bool_guid_char(42),
               3 => new TestUnion_class_string_int_bool_guid_char(true),
               4 => new TestUnion_class_string_int_bool_guid_char(new Guid("D4EF64BB-730B-4D5C-94A1-C019F83EF945")),
               5 => new TestUnion_class_string_int_bool_guid_char('A'),
               _ => throw new Exception()
            };

            var calledActionOn = value.SwitchPartially(@default: v => $"default:{v}",
                                                       @string: v => (object)v,
                                                       int32: v => v,
                                                       boolean: v => v,
                                                       guid: v => v,
                                                       @char: v => v);

            calledActionOn.Should().Be(index == 4 ? new Guid((string)expected) : expected);
         }

         [Theory]
         [InlineData(1, "default:text")]
         [InlineData(2, 42)]
         public void Should_use_default_arg_having_2_types(int index, object expected)
         {
            var value = index switch
            {
               1 => new TestUnion_class_string_int("text"),
               2 => new TestUnion_class_string_int(42),
               _ => throw new Exception()
            };

            var calledActionOn = value.SwitchPartially(@default: v => (object)$"default:{v}",
                                                       int32: v => v);

            calledActionOn.Should().Be(expected);
         }

         [Theory]
         [InlineData(1, "default:text")]
         [InlineData(2, 42)]
         [InlineData(3, true)]
         public void Should_use_default_arg_having_3_types(int index, object expected)
         {
            var value = index switch
            {
               1 => new TestUnion_class_string_int_bool("text"),
               2 => new TestUnion_class_string_int_bool(42),
               3 => new TestUnion_class_string_int_bool(true),
               _ => throw new Exception()
            };

            var calledActionOn = value.SwitchPartially(@default: v => (object)$"default:{v}",
                                                       int32: v => v,
                                                       boolean: v => v);

            calledActionOn.Should().Be(expected);
         }

         [Theory]
         [InlineData(1, "default:text")]
         [InlineData(2, 42)]
         [InlineData(3, true)]
         [InlineData(4, "5C392E64-2AEC-401F-98A9-35E5913B369A")]
         public void Should_use_default_arg_having_4_types(int index, object expected)
         {
            var value = index switch
            {
               1 => new TestUnion_class_string_int_bool_guid("text"),
               2 => new TestUnion_class_string_int_bool_guid(42),
               3 => new TestUnion_class_string_int_bool_guid(true),
               4 => new TestUnion_class_string_int_bool_guid(new Guid("5C392E64-2AEC-401F-98A9-35E5913B369A")),
               _ => throw new Exception()
            };

            var calledActionOn = value.SwitchPartially(@default: v => (object)$"default:{v}",
                                                       int32: v => v,
                                                       boolean: v => v,
                                                       guid: v => v);

            calledActionOn.Should().Be(index == 4 ? new Guid((string)expected) : expected);
         }

         [Theory]
         [InlineData(1, "default:text")]
         [InlineData(2, 42)]
         [InlineData(3, true)]
         [InlineData(4, "5C392E64-2AEC-401F-98A9-35E5913B369A")]
         [InlineData(5, 'A')]
         public void Should_use_default_arg_having_5_types(int index, object expected)
         {
            var value = index switch
            {
               1 => new TestUnion_class_string_int_bool_guid_char("text"),
               2 => new TestUnion_class_string_int_bool_guid_char(42),
               3 => new TestUnion_class_string_int_bool_guid_char(true),
               4 => new TestUnion_class_string_int_bool_guid_char(new Guid("5C392E64-2AEC-401F-98A9-35E5913B369A")),
               5 => new TestUnion_class_string_int_bool_guid_char('A'),
               _ => throw new Exception()
            };

            var calledActionOn = value.SwitchPartially(@default: v => (object)$"default:{v}",
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

            var calledActionOn = value.SwitchPartially(state,
                                                       @default: (ctx, v) =>
                                                                 {
                                                                    ctx.Should().Be(state);
                                                                    return (object)$"default:{v}";
                                                                 },
                                                       @string: (ctx, v) =>
                                                                {
                                                                   ctx.Should().Be(state);
                                                                   return v;
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

            var calledActionOn = value.SwitchPartially(state,
                                                       @default: (ctx, v) =>
                                                                 {
                                                                    ctx.Should().Be(state);
                                                                    return (object)$"default:{v}";
                                                                 },
                                                       @string: (ctx, v) =>
                                                                {
                                                                   ctx.Should().Be(state);
                                                                   return v;
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
         [InlineData(4, "10C287C8-4D64-45CC-859E-873024D53DE3")]
         public void Should_pass_context_having_4_types(int index, object expected)
         {
            var value = index switch
            {
               1 => new TestUnion_class_string_int_bool_guid("text"),
               2 => new TestUnion_class_string_int_bool_guid(42),
               3 => new TestUnion_class_string_int_bool_guid(true),
               4 => new TestUnion_class_string_int_bool_guid(new Guid("10C287C8-4D64-45CC-859E-873024D53DE3")),
               _ => throw new Exception()
            };

            var state = new object();

            var calledActionOn = value.SwitchPartially(state,
                                                       @default: (ctx, v) =>
                                                                 {
                                                                    ctx.Should().Be(state);
                                                                    return (object)$"default:{v}";
                                                                 },
                                                       @string: (ctx, v) =>
                                                                {
                                                                   ctx.Should().Be(state);
                                                                   return v;
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
         [InlineData(4, "10C287C8-4D64-45CC-859E-873024D53DE3")]
         [InlineData(5, 'A')]
         public void Should_pass_context_having_5_types(int index, object expected)
         {
            var value = index switch
            {
               1 => new TestUnion_class_string_int_bool_guid_char("text"),
               2 => new TestUnion_class_string_int_bool_guid_char(42),
               3 => new TestUnion_class_string_int_bool_guid_char(true),
               4 => new TestUnion_class_string_int_bool_guid_char(new Guid("10C287C8-4D64-45CC-859E-873024D53DE3")),
               5 => new TestUnion_class_string_int_bool_guid_char('A'),
               _ => throw new Exception()
            };

            var state = new object();

            var calledActionOn = value.SwitchPartially(state,
                                                       @default: (ctx, v) =>
                                                                 {
                                                                    ctx.Should().Be(state);
                                                                    return (object)$"default:{v}";
                                                                 },
                                                       @string: (ctx, v) =>
                                                                {
                                                                   ctx.Should().Be(state);
                                                                   return v;
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

         [Theory]
         [InlineData(1, "default:text")]
         [InlineData(2, 42)]
         public void Should_pass_context_to_default_having_2_types(int index, object expected)
         {
            var value = index switch
            {
               1 => new TestUnion_class_string_int("text"),
               2 => new TestUnion_class_string_int(42),
               _ => throw new Exception()
            };

            var state = new object();

            var calledActionOn = value.SwitchPartially(state,
                                                       @default: (ctx, v) =>
                                                                 {
                                                                    ctx.Should().Be(state);
                                                                    return (object)$"default:{v}";
                                                                 },
                                                       int32: (ctx, v) =>
                                                              {
                                                                 ctx.Should().Be(state);
                                                                 return v;
                                                              });

            calledActionOn.Should().Be(expected);
         }

         [Theory]
         [InlineData(1, "default:text")]
         [InlineData(2, 42)]
         [InlineData(3, true)]
         public void Should_pass_context_to_default_having_3_types(int index, object expected)
         {
            var value = index switch
            {
               1 => new TestUnion_class_string_int_bool("text"),
               2 => new TestUnion_class_string_int_bool(42),
               3 => new TestUnion_class_string_int_bool(true),
               _ => throw new Exception()
            };

            var state = new object();

            var calledActionOn = value.SwitchPartially(state,
                                                       @default: (ctx, v) =>
                                                                 {
                                                                    ctx.Should().Be(state);
                                                                    return (object)$"default:{v}";
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
         [InlineData(1, "default:text")]
         [InlineData(2, 42)]
         [InlineData(3, true)]
         [InlineData(4, "1F99EE8B-862C-4068-B5E1-015EA81AA470")]
         public void Should_pass_context_to_default_having_4_types(int index, object expected)
         {
            var value = index switch
            {
               1 => new TestUnion_class_string_int_bool_guid("text"),
               2 => new TestUnion_class_string_int_bool_guid(42),
               3 => new TestUnion_class_string_int_bool_guid(true),
               4 => new TestUnion_class_string_int_bool_guid(new Guid("1F99EE8B-862C-4068-B5E1-015EA81AA470")),
               _ => throw new Exception()
            };

            var state = new object();

            var calledActionOn = value.SwitchPartially(state,
                                                       @default: (ctx, v) =>
                                                                 {
                                                                    ctx.Should().Be(state);
                                                                    return (object)$"default:{v}";
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
         [InlineData(1, "default:text")]
         [InlineData(2, 42)]
         [InlineData(3, true)]
         [InlineData(4, "1F99EE8B-862C-4068-B5E1-015EA81AA470")]
         [InlineData(5, 'A')]
         public void Should_pass_context_to_default_having_5_types(int index, object expected)
         {
            var value = index switch
            {
               1 => new TestUnion_class_string_int_bool_guid_char("text"),
               2 => new TestUnion_class_string_int_bool_guid_char(42),
               3 => new TestUnion_class_string_int_bool_guid_char(true),
               4 => new TestUnion_class_string_int_bool_guid_char(new Guid("1F99EE8B-862C-4068-B5E1-015EA81AA470")),
               5 => new TestUnion_class_string_int_bool_guid_char('A'),
               _ => throw new Exception()
            };

            var state = new object();

            var calledActionOn = value.SwitchPartially(state,
                                                       @default: (ctx, v) =>
                                                                 {
                                                                    ctx.Should().Be(state);
                                                                    return (object)$"default:{v}";
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

   public class HavingStrcut
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

            value.SwitchPartially(@string: v =>
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
         [InlineData(1, "default:text")]
         [InlineData(2, 42)]
         public void Should_use_default_arg_having_2_types(int index, object expected)
         {
            var value = index switch
            {
               1 => new TestUnion_struct_string_int("text"),
               2 => new TestUnion_struct_string_int(42),
               _ => throw new Exception()
            };
            object calledActionOn = null;

            value.SwitchPartially(@default: v =>
                                            {
                                               calledActionOn = $"default:{v}";
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

            value.SwitchPartially(state,
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

         [Theory]
         [InlineData(1, "default:text")]
         [InlineData(2, 42)]
         public void Should_pass_context_to_default_having_2_types(int index, object expected)
         {
            var value = index switch
            {
               1 => new TestUnion_struct_string_int("text"),
               2 => new TestUnion_struct_string_int(42),
               _ => throw new Exception()
            };

            var state = new object();
            object calledActionOn = null;

            value.SwitchPartially(state,
                                  @default: (ctx, v) =>
                                            {
                                               ctx.Should().Be(state);
                                               calledActionOn = $"default:{v}";
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
         public void Should_use_correct_arg_having_2_types(int index, object expected)
         {
            var value = index switch
            {
               1 => new TestUnion_struct_string_int("text"),
               2 => new TestUnion_struct_string_int(42),
               _ => throw new Exception()
            };

            var calledActionOn = value.SwitchPartially(@default: v => $"default:{v}",
                                                       @string: v => (object)v,
                                                       int32: v => v);

            calledActionOn.Should().Be(expected);
         }

         [Theory]
         [InlineData(1, "default:text")]
         [InlineData(2, 42)]
         public void Should_use_default_arg_having_2_types(int index, object expected)
         {
            var value = index switch
            {
               1 => new TestUnion_struct_string_int("text"),
               2 => new TestUnion_struct_string_int(42),
               _ => throw new Exception()
            };

            var calledActionOn = value.SwitchPartially(@default: v => (object)$"default:{v}",
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

            var calledActionOn = value.SwitchPartially(state,
                                                       @default: (ctx, v) =>
                                                                 {
                                                                    ctx.Should().Be(state);
                                                                    return (object)$"default:{v}";
                                                                 },
                                                       @string: (ctx, v) =>
                                                                {
                                                                   ctx.Should().Be(state);
                                                                   return v;
                                                                },
                                                       int32: (ctx, v) =>
                                                              {
                                                                 ctx.Should().Be(state);
                                                                 return v;
                                                              });

            calledActionOn.Should().Be(expected);
         }

         [Theory]
         [InlineData(1, "default:text")]
         [InlineData(2, 42)]
         public void Should_pass_context_to_default_having_2_types(int index, object expected)
         {
            var value = index switch
            {
               1 => new TestUnion_struct_string_int("text"),
               2 => new TestUnion_struct_string_int(42),
               _ => throw new Exception()
            };

            var state = new object();

            var calledActionOn = value.SwitchPartially(state,
                                                       @default: (ctx, v) =>
                                                                 {
                                                                    ctx.Should().Be(state);
                                                                    return (object)$"default:{v}";
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
