using System;
using System.Collections.Generic;
using System.Reflection;
using FluentAssertions;
using Thinktecture.TestEnums;
using Xunit;

namespace Thinktecture.EnumTests
{
   public class GetAll
   {
      [Fact]
      public void Should_return_empty_collection_if_enum_has_no_items()
      {
         EmptyEnum.GetAll().Should().BeEmpty();
      }

      [Fact]
      public void Should_return_public_fields_only()
      {
         var enums = TestEnum.GetAll();
         enums.Should().HaveCount(2);
         enums.Should().Contain(TestEnum.Item1);
         enums.Should().Contain(TestEnum.Item2);
      }

      [Fact]
      public void Should_return_fields_of_an_struct()
      {
         var enums = StructIntegerEnum.GetAll();
         enums.Should().HaveCount(2);
         enums.Should().Contain(StructIntegerEnum.Item1);
         enums.Should().Contain(StructIntegerEnum.Item2);
      }

      [Fact]
      public void Should_return_public_fields_only_via_reflection()
      {
         var enums = (IReadOnlyList<IEnum<string>>)typeof(TestEnum).GetMethod("GetAll", BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy)
                                                                   ?.Invoke(null, new object[0]);

         enums.Should().HaveCount(2);
         enums.Should().Contain(TestEnum.Item1);
         enums.Should().Contain(TestEnum.Item2);
      }
   }
}
