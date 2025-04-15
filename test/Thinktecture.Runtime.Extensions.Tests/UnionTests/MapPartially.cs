using System;
using Thinktecture.Runtime.Tests.TestUnions;

namespace Thinktecture.Runtime.Tests.UnionTests;

// ReSharper disable once InconsistentNaming
public class MapPartially
{
   public class HavingClass
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

         var calledActionOn = value.MapPartially(@default: "default",
                                                 child1: "1",
                                                 child2: "2");

         calledActionOn.Should().Be(expected);
      }

#if NET9_0_OR_GREATER
      [Theory]
      [InlineData(1, "1")]
      [InlineData(2, "2")]
      public void Should_use_correct_arg_having_2_types_and_return_ref_struct(int index, string expected)
      {
         var value = index switch
         {
            1 => (TestUnion)new TestUnion.Child1("1"),
            2 => new TestUnion.Child2("2"),
            _ => throw new Exception()
         };

         var calledActionOn = value.MapPartially(@default: new TestRefStruct("default"),
                                                 child1: new TestRefStruct("1"),
                                                 child2: new TestRefStruct("2"));

         calledActionOn.Value.Should().Be(expected);
      }
#endif

      [Theory]
      [InlineData(1, "default")]
      [InlineData(2, "2")]
      public void Should_use_default_having_2_types(int index, string expected)
      {
         var value = index switch
         {
            1 => (TestUnion)new TestUnion.Child1("1"),
            2 => new TestUnion.Child2("2"),
            _ => throw new Exception()
         };

         var calledActionOn = value.MapPartially(@default: "default",
                                                 child2: "2");

         calledActionOn.Should().Be(expected);
      }
   }

   public class HavingRecord
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

         var calledActionOn = value.MapPartially(@default: "default",
                                                 child1: "1",
                                                 child2: "2");

         calledActionOn.Should().Be(expected);
      }

      [Theory]
      [InlineData(1, "default")]
      [InlineData(2, "2")]
      public void Should_use_default_having_2_types(int index, string expected)
      {
         var value = index switch
         {
            1 => (TestUnionRecord)new TestUnionRecord.Child1("1"),
            2 => new TestUnionRecord.Child2("2"),
            _ => throw new Exception()
         };

         var calledActionOn = value.MapPartially(@default: "default",
                                                 child2: "2");

         calledActionOn.Should().Be(expected);
      }
   }
}
