using System;
using Thinktecture.Runtime.Tests.TestRegularUnions;

namespace Thinktecture.Runtime.Tests.RegularUnionTests;

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

#if NET9_0_OR_GREATER
      [Theory]
      [InlineData(1, "1")]
      [InlineData(2, "2")]
      public void Should_use_correct_arg_having_2_values_and_return_ref_struct(int index, string expected)
      {
         var value = index switch
         {
            1 => (TestUnion)new TestUnion.Child1("1"),
            2 => new TestUnion.Child2("2"),
            _ => throw new Exception()
         };

         var calledActionOn = value.Map(child1: new TestRefStruct("1"),
                                        child2: new TestRefStruct("2"));

         calledActionOn.Value.Should().Be(expected);
      }
#endif
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
