﻿using System;
using System.Collections.Generic;

namespace Thinktecture.Runtime.Tests.EmptyCollectionTests;

// ReSharper disable once InconsistentNaming
public class Empty_Dictionary
{
   // ReSharper disable once MemberCanBeMadeStatic.Local
   // ReSharper disable once InconsistentNaming
   private IReadOnlyDictionary<object, object> SUT => Empty.Dictionary<object, object>();

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
   public void Should_throw_if_using_indexer()
   {
      // ReSharper disable once AssignmentIsFullyDiscarded
      Action action = () => _ = SUT[new object()];
      action.Should().Throw<KeyNotFoundException>();
   }
}
