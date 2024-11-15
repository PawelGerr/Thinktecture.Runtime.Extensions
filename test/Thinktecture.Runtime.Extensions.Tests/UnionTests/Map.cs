using System;
using Thinktecture.Runtime.Tests.TestAdHocUnions;
using Thinktecture.Runtime.Tests.TestUnions;

namespace Thinktecture.Runtime.Tests.UnionTests;

// ReSharper disable once InconsistentNaming
public class Map
{
   public class HavingClass
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

         var calledActionOn = value.Map(child1: "1",
                                        child2: "2");

         calledActionOn.Should().Be(expected);
      }
   }

   public class HavingRecord
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

         var calledActionOn = value.Map(child1: "1",
                                        child2: "2");

         calledActionOn.Should().Be(expected);
      }
   }
}
