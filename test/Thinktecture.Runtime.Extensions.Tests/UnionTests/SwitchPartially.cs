using System;
using Thinktecture.Runtime.Tests.TestUnions;

namespace Thinktecture.Runtime.Tests.UnionTests;

// ReSharper disable once InconsistentNaming
public class SwitchPartially
{
   public class HavingClass
   {
      public class WithAction
      {
         [Theory]
         [InlineData(1, "1")]
         [InlineData(2, "2")]
         public void Should_use_correct_arg_having_2_values(int index, string expected)
         {
            var value = index switch
            {
               1 => (TestUnion)new TestUnion.Child1("1"),
               2 => new TestUnion.Child2("2"),
               _ => throw new Exception()
            };
            string calledActionOn = null;

            value.SwitchPartially(child1: v =>
                                          {
                                             calledActionOn = v.Name;
                                          },
                                  child2: v =>
                                          {
                                             calledActionOn = v.Name;
                                          });

            calledActionOn.Should().Be(expected);
         }

         [Theory]
         [InlineData(1, "default:1")]
         [InlineData(2, "2")]
         public void Should_use_default_arg_having_2_types(int index, string expected)
         {
            var value = index switch
            {
               1 => (TestUnion)new TestUnion.Child1("1"),
               2 => new TestUnion.Child2("2"),
               _ => throw new Exception()
            };
            string calledActionOn = null;

            value.SwitchPartially(@default: v =>
                                            {
                                               calledActionOn = $"default:{v.Name}";
                                            },
                                  child2: v =>
                                          {
                                             calledActionOn = v.Name;
                                          });

            calledActionOn.Should().Be(expected);
         }
      }

      public class WithActionAndState
      {
         [Theory]
         [InlineData(1, "1")]
         [InlineData(2, "2")]
         public void Should_pass_state_having_2_types(int index, string expected)
         {
            var value = index switch
            {
               1 => (TestUnion)new TestUnion.Child1("1"),
               2 => new TestUnion.Child2("2"),
               _ => throw new Exception()
            };

            var state = new object();
            string calledActionOn = null;

            value.SwitchPartially(state,
                                  child1: (s, v) =>
                                          {
                                             s.Should().Be(state);
                                             calledActionOn = v.Name;
                                          },
                                  child2: (s, v) =>
                                          {
                                             s.Should().Be(state);
                                             calledActionOn = v.Name;
                                          });

            calledActionOn.Should().Be(expected);
         }

         [Theory]
         [InlineData(1, "default:1")]
         [InlineData(2, "2")]
         public void Should_pass_state_to_default_having_2_types(int index, string expected)
         {
            var value = index switch
            {
               1 => (TestUnion)new TestUnion.Child1("1"),
               2 => new TestUnion.Child2("2"),
               _ => throw new Exception()
            };

            var state = new object();
            string calledActionOn = null;

            value.SwitchPartially(state,
                                  @default: (ctx, v) =>
                                            {
                                               ctx.Should().Be(state);
                                               calledActionOn = $"default:{v.Name}";
                                            },
                                  child2: (ctx, v) =>
                                          {
                                             ctx.Should().Be(state);
                                             calledActionOn = v.Name;
                                          });

            calledActionOn.Should().Be(expected);
         }
      }

      public class WithFunc
      {
         [Theory]
         [InlineData(1, "1")]
         [InlineData(2, "2")]
         public void Should_use_correct_arg_having_2_types(int index, string expected)
         {
            var value = index switch
            {
               1 => (TestUnion)new TestUnion.Child1("1"),
               2 => new TestUnion.Child2("2"),
               _ => throw new Exception()
            };

            var calledActionOn = value.SwitchPartially(@default: v => $"default:{v.Name}",
                                                       child1: v => v.Name,
                                                       child2: v => v.Name);

            calledActionOn.Should().Be(expected);
         }

         [Theory]
         [InlineData(1, "default:1")]
         [InlineData(2, "2")]
         public void Should_use_default_arg_having_2_types(int index, string expected)
         {
            var value = index switch
            {
               1 => (TestUnion)new TestUnion.Child1("1"),
               2 => new TestUnion.Child2("2"),
               _ => throw new Exception()
            };

            var calledActionOn = value.SwitchPartially(@default: v => $"default:{v.Name}",
                                                       child2: v => v.Name);

            calledActionOn.Should().Be(expected);
         }
      }

      public class WithFuncAndContext
      {
         [Theory]
         [InlineData(1, "1")]
         [InlineData(2, "2")]
         public void Should_pass_state_having_2_types(int index, string expected)
         {
            var value = index switch
            {
               1 => (TestUnion)new TestUnion.Child1("1"),
               2 => new TestUnion.Child2("2"),
               _ => throw new Exception()
            };

            var state = new object();

            var calledActionOn = value.SwitchPartially(state,
                                                       @default: (ctx, v) =>
                                                                 {
                                                                    ctx.Should().Be(state);
                                                                    return $"default:{v.Name}";
                                                                 },
                                                       child1: (ctx, v) =>
                                                               {
                                                                  ctx.Should().Be(state);
                                                                  return v.Name;
                                                               },
                                                       child2: (ctx, v) =>
                                                               {
                                                                  ctx.Should().Be(state);
                                                                  return v.Name;
                                                               });

            calledActionOn.Should().Be(expected);
         }

         [Theory]
         [InlineData(1, "default:1")]
         [InlineData(2, "2")]
         public void Should_pass_state_to_default_having_2_types(int index, string expected)
         {
            var value = index switch
            {
               1 => (TestUnion)new TestUnion.Child1("1"),
               2 => new TestUnion.Child2("2"),
               _ => throw new Exception()
            };

            var state = new object();

            var calledActionOn = value.SwitchPartially(state,
                                                       @default: (ctx, v) =>
                                                                 {
                                                                    ctx.Should().Be(state);
                                                                    return $"default:{v.Name}";
                                                                 },
                                                       child2: (ctx, v) =>
                                                               {
                                                                  ctx.Should().Be(state);
                                                                  return v.Name;
                                                               });

            calledActionOn.Should().Be(expected);
         }
      }
   }

   public class HavingRecord
   {
      public class WithAction
      {
         [Theory]
         [InlineData(1, "1")]
         [InlineData(2, "2")]
         public void Should_use_correct_arg_having_2_values(int index, string expected)
         {
            var value = index switch
            {
               1 => (TestUnionRecord)new TestUnionRecord.Child1("1"),
               2 => new TestUnionRecord.Child2("2"),
               _ => throw new Exception()
            };
            string calledActionOn = null;

            value.SwitchPartially(child1: v =>
                                          {
                                             calledActionOn = v.Name;
                                          },
                                  child2: v =>
                                          {
                                             calledActionOn = v.Name;
                                          });

            calledActionOn.Should().Be(expected);
         }

         [Theory]
         [InlineData(1, "default:1")]
         [InlineData(2, "2")]
         public void Should_use_default_arg_having_2_types(int index, string expected)
         {
            var value = index switch
            {
               1 => (TestUnionRecord)new TestUnionRecord.Child1("1"),
               2 => new TestUnionRecord.Child2("2"),
               _ => throw new Exception()
            };
            string calledActionOn = null;

            value.SwitchPartially(@default: v =>
                                            {
                                               calledActionOn = $"default:{v.Name}";
                                            },
                                  child2: v =>
                                          {
                                             calledActionOn = v.Name;
                                          });

            calledActionOn.Should().Be(expected);
         }
      }

      public class WithActionAndState
      {
         [Theory]
         [InlineData(1, "1")]
         [InlineData(2, "2")]
         public void Should_pass_state_having_2_types(int index, string expected)
         {
            var value = index switch
            {
               1 => (TestUnionRecord)new TestUnionRecord.Child1("1"),
               2 => new TestUnionRecord.Child2("2"),
               _ => throw new Exception()
            };

            var state = new object();
            string calledActionOn = null;

            value.SwitchPartially(state,
                                  child1: (s, v) =>
                                          {
                                             s.Should().Be(state);
                                             calledActionOn = v.Name;
                                          },
                                  child2: (s, v) =>
                                          {
                                             s.Should().Be(state);
                                             calledActionOn = v.Name;
                                          });

            calledActionOn.Should().Be(expected);
         }

         [Theory]
         [InlineData(1, "default:1")]
         [InlineData(2, "2")]
         public void Should_pass_state_to_default_having_2_types(int index, string expected)
         {
            var value = index switch
            {
               1 => (TestUnionRecord)new TestUnionRecord.Child1("1"),
               2 => new TestUnionRecord.Child2("2"),
               _ => throw new Exception()
            };

            var state = new object();
            string calledActionOn = null;

            value.SwitchPartially(state,
                                  @default: (ctx, v) =>
                                            {
                                               ctx.Should().Be(state);
                                               calledActionOn = $"default:{v.Name}";
                                            },
                                  child2: (ctx, v) =>
                                          {
                                             ctx.Should().Be(state);
                                             calledActionOn = v.Name;
                                          });

            calledActionOn.Should().Be(expected);
         }
      }

      public class WithFunc
      {
         [Theory]
         [InlineData(1, "1")]
         [InlineData(2, "2")]
         public void Should_use_correct_arg_having_2_types(int index, string expected)
         {
            var value = index switch
            {
               1 => (TestUnionRecord)new TestUnionRecord.Child1("1"),
               2 => new TestUnionRecord.Child2("2"),
               _ => throw new Exception()
            };

            var calledActionOn = value.SwitchPartially(@default: v => $"default:{v.Name}",
                                                       child1: v => v.Name,
                                                       child2: v => v.Name);

            calledActionOn.Should().Be(expected);
         }

         [Theory]
         [InlineData(1, "default:1")]
         [InlineData(2, "2")]
         public void Should_use_default_arg_having_2_types(int index, string expected)
         {
            var value = index switch
            {
               1 => (TestUnionRecord)new TestUnionRecord.Child1("1"),
               2 => new TestUnionRecord.Child2("2"),
               _ => throw new Exception()
            };

            var calledActionOn = value.SwitchPartially(@default: v => $"default:{v.Name}",
                                                       child2: v => v.Name);

            calledActionOn.Should().Be(expected);
         }
      }

      public class WithFuncAndContext
      {
         [Theory]
         [InlineData(1, "1")]
         [InlineData(2, "2")]
         public void Should_pass_state_having_2_types(int index, string expected)
         {
            var value = index switch
            {
               1 => (TestUnionRecord)new TestUnionRecord.Child1("1"),
               2 => new TestUnionRecord.Child2("2"),
               _ => throw new Exception()
            };

            var state = new object();

            var calledActionOn = value.SwitchPartially(state,
                                                       @default: (ctx, v) =>
                                                                 {
                                                                    ctx.Should().Be(state);
                                                                    return $"default:{v.Name}";
                                                                 },
                                                       child1: (ctx, v) =>
                                                               {
                                                                  ctx.Should().Be(state);
                                                                  return v.Name;
                                                               },
                                                       child2: (ctx, v) =>
                                                               {
                                                                  ctx.Should().Be(state);
                                                                  return v.Name;
                                                               });

            calledActionOn.Should().Be(expected);
         }

         [Theory]
         [InlineData(1, "default:1")]
         [InlineData(2, "2")]
         public void Should_pass_state_to_default_having_2_types(int index, string expected)
         {
            var value = index switch
            {
               1 => (TestUnionRecord)new TestUnionRecord.Child1("1"),
               2 => new TestUnionRecord.Child2("2"),
               _ => throw new Exception()
            };

            var state = new object();

            var calledActionOn = value.SwitchPartially(state,
                                                       @default: (ctx, v) =>
                                                                 {
                                                                    ctx.Should().Be(state);
                                                                    return $"default:{v.Name}";
                                                                 },
                                                       child2: (ctx, v) =>
                                                               {
                                                                  ctx.Should().Be(state);
                                                                  return v.Name;
                                                               });

            calledActionOn.Should().Be(expected);
         }
      }
   }
}
