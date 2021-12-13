using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace Thinktecture.Runtime.Tests.EmptyCollectionTests;

// ReSharper disable once InconsistentNaming
public class Empty_Lookup
{
   private ILookup<object, object> SUT => Empty.Lookup<object, object>();

   [Fact]
   public void Should_not_be_null()
   {
      SUT.Should().NotBeNull();
   }

   [Fact]
   public void Should_be_empty()
   {
      SUT.Should().BeEmpty();
   }

   [Fact]
   public void Should_have_count_of_0()
   {
      SUT.Count.Should().Be(0);
   }

   [Fact]
   public void Should_return_empty_collection_if_using_indexer()
   {
      SUT[new object()].Should().BeEmpty();
   }
}