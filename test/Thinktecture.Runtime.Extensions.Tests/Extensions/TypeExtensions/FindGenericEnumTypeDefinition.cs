using System;
using System.Collections.Generic;
using FluentAssertions;
using Xunit;

namespace Thinktecture.Extensions.TypeExtensions
{
   public class FindGenericEnumTypeDefinition
   {
      [Fact]
      public void Should_return_null_if_type_is_object()
      {
         typeof(object).FindGenericEnumTypeDefinition()
                       .Should().BeNull();
      }

      [Fact]
      public void Should_return_null_if_type_is_struct()
      {
         typeof(int).FindGenericEnumTypeDefinition()
                    .Should().BeNull();
      }

      [Fact]
      public void Should_throw_if_type_is_null()
      {
         ((Type)null).Invoking(t => t.FindGenericEnumTypeDefinition())
                     .Should().Throw<ArgumentNullException>();
      }

      [Fact]
      public void Should_return_null_for_interface()
      {
         typeof(IEnumerable<string>).FindGenericEnumTypeDefinition()
                                    .Should().BeNull();
      }
   }
}
