#if !NET6_0
namespace Thinktecture.Runtime.Tests.Extensions.ReadOnlyListExtensions;

public class SplitInBuckets
{
   [Fact]
   public void Should_throw_if_collection_is_null()
   {
      0.Invoking(_ => ((IReadOnlyList<int>)null).SplitInBuckets(5).ToList())
       .Should().Throw<ArgumentNullException>()
       .WithMessage("Value cannot be null. (Parameter 'collection')");
   }

   [Fact]
   public void Should_throw_if_bucke_size_is_0()
   {
      Collection<int>().Invoking(c => c.SplitInBuckets(0).ToList())
                       .Should().Throw<ArgumentOutOfRangeException>()
                       .WithMessage("Bucket size cannot be less than 1. (Parameter 'bucketSize')");
   }

   [Fact]
   public void Should_return_empty_enumerable_if_collection_is_empty()
   {
      Collection<int>().SplitInBuckets(5)
                       .Should().BeEmpty();
   }

   [Fact]
   public void Should_return_enumerable_with_1_item_if_collection_size_is_smaller_than_bucket_size()
   {
      var buckets = Collection(1, 2, 3).SplitInBuckets(5).ToList();
      buckets.Should().HaveCount(1);
      buckets[0].Should().HaveCount(3).And.ContainInOrder(new[] { 1, 2, 3 });
   }

   [Fact]
   public void Should_return_enumerable_with_1_item_if_collection_size_equals_the_bucket_size()
   {
      var buckets = Collection(1, 2, 3, 4, 5).SplitInBuckets(5).ToList();
      buckets.Should().HaveCount(1);
      buckets[0].Should().HaveCount(5).And.ContainInOrder(new[] { 1, 2, 3, 4, 5 });
   }

   [Fact]
   public void Should_return_enumerable_with_2_items_if_collection_size_slightly_bigger_than_bucket_size()
   {
      var buckets = Collection(1, 2, 3, 4, 5, 6).SplitInBuckets(5).ToList();
      buckets.Should().HaveCount(2);
      buckets[0].Should().HaveCount(5).And.ContainInOrder(new[] { 1, 2, 3, 4, 5 });
      buckets[1].Should().HaveCount(1).And.ContainInOrder(new[] { 6 });
   }

   [Fact]
   public void Should_return_enumerable_with_2_items_if_collection_size_twice_as_big_as_bucket_size()
   {
      var buckets = Collection(1, 2, 3, 4, 5, 6, 7, 8, 9, 10).SplitInBuckets(5).ToList();
      buckets.Should().HaveCount(2);
      buckets[0].Should().HaveCount(5).And.ContainInOrder(new[] { 1, 2, 3, 4, 5 });
      buckets[1].Should().HaveCount(5).And.ContainInOrder(new[] { 6, 7, 8, 9, 10 });
   }

   [Fact]
   public void Should_return_enumerable_with_3_items_if_collection_size_more_than_twice_as_big_as_bucket_size()
   {
      var buckets = Collection(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12).SplitInBuckets(5).ToList();
      buckets.Should().HaveCount(3);
      buckets[0].Should().HaveCount(5).And.ContainInOrder(new[] { 1, 2, 3, 4, 5 });
      buckets[1].Should().HaveCount(5).And.ContainInOrder(new[] { 6, 7, 8, 9, 10 });
      buckets[2].Should().HaveCount(2).And.ContainInOrder(new[] { 12 });
   }

   private static IReadOnlyList<T> Collection<T>(params T[] values)
   {
      return values;
   }
}
#endif
