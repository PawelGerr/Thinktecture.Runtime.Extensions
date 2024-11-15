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

            value.Switch(child1: v =>
                                 {
                                    calledActionOn = v.Name;
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
         public void Should_pass_context_having_2_types(int index, string expected)
         {
            var value = index switch
            {
               1 => (TestUnion)new TestUnion.Child1("1"),
               2 => new TestUnion.Child1("2"),
               _ => throw new Exception()
            };

            var state = new object();

            string calledActionOn = null;

            value.Switch(state,
                         child1: (s, v) =>
                                 {
                                    s.Should().Be(s);
                                    calledActionOn = v.Name;
                                 },
                         child2: (s, v) =>
                                 {
                                    s.Should().Be(state);
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
         public void Should_call_correct_arg_having_2_types(int index, string expected)
         {
            var value = index switch
            {
               1 => (TestUnion)new TestUnion.Child1("1"),
               2 => new TestUnion.Child1("2"),
               _ => throw new Exception()
            };

            var result = value.Switch(child1: v => v.Name,
                                      child2: v => v.Name);

            result.Should().Be(expected);
         }
      }

      public class WithFuncAndContext
      {
         [Theory]
         [InlineData(1, "1")]
         [InlineData(2, "2")]
         public void Should_pass_context_having_2_types(int index, string expected)
         {
            var value = index switch
            {
               1 => (TestUnion)new TestUnion.Child1("1"),
               2 => new TestUnion.Child1("2"),
               _ => throw new Exception()
            };

            var state = new object();
            var calledActionOn = value.Switch(state,
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

            value.Switch(child1: v =>
                                 {
                                    calledActionOn = v.Name;
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
         public void Should_pass_context_having_2_types(int index, string expected)
         {
            var value = index switch
            {
               1 => (TestUnionRecord)new TestUnionRecord.Child1("1"),
               2 => new TestUnionRecord.Child1("2"),
               _ => throw new Exception()
            };

            var state = new object();

            string calledActionOn = null;

            value.Switch(state,
                         child1: (s, v) =>
                                 {
                                    s.Should().Be(s);
                                    calledActionOn = v.Name;
                                 },
                         child2: (s, v) =>
                                 {
                                    s.Should().Be(state);
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
         public void Should_call_correct_arg_having_2_types(int index, string expected)
         {
            var value = index switch
            {
               1 => (TestUnionRecord)new TestUnionRecord.Child1("1"),
               2 => new TestUnionRecord.Child1("2"),
               _ => throw new Exception()
            };

            var result = value.Switch(child1: v => v.Name,
                                      child2: v => v.Name);

            result.Should().Be(expected);
         }
      }

      public class WithFuncAndContext
      {
         [Theory]
         [InlineData(1, "1")]
         [InlineData(2, "2")]
         public void Should_pass_context_having_2_types(int index, string expected)
         {
            var value = index switch
            {
               1 => (TestUnionRecord)new TestUnionRecord.Child1("1"),
               2 => new TestUnionRecord.Child1("2"),
               _ => throw new Exception()
            };

            var state = new object();
            var calledActionOn = value.Switch(state,
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
      }
   }
}
