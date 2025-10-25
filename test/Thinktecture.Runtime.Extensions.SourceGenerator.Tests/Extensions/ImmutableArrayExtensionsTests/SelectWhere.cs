using System.Collections.Immutable;

namespace Thinktecture.Runtime.Tests.ImmutableArrayExtensionsTests;

public class SelectWhere
{
   [Fact]
   public void Should_select_and_transform_with_arg()
   {
      var array = ImmutableArray.CreateRange([1, 2, 3, 4]);

      var result = array.SelectWhere(static (int i, int threshold, out int r) =>
      {
         if (i > threshold)
         {
            r = i * 10;
            return true;
         }

         r = 0;
         return false;
      }, 2);

      result.Should().BeEquivalentTo([30, 40]);
   }

   [Fact]
   public void Should_select_and_transform_without_arg()
   {
      var array = ImmutableArray.CreateRange([1, 2, 3, 4]);

      var result = array.SelectWhere(static (int i, out int r) =>
      {
         if ((i & 1) == 0)
         {
            r = i * 10;
            return true;
         }

         r = 0;
         return false;
      });

      result.Should().BeEquivalentTo([20, 40]);
   }

   [Fact]
   public void Should_return_empty_for_empty_array_without_arg()
   {
      var array = ImmutableArray<int>.Empty;

      var result = array.SelectWhere(static (int _, out int r) =>
      {
         r = 0;
         return false;
      });

      result.IsEmpty.Should().BeTrue();
   }

   [Fact]
   public void Should_return_empty_for_default_array_with_arg()
   {
      ImmutableArray<int> array = default;

      var result = array.SelectWhere(static (int i, int threshold, out int r) =>
      {
         if (i > threshold)
         {
            r = i * 10;
            return true;
         }

         r = 0;
         return false;
      }, 2);

      result.IsEmpty.Should().BeTrue();
   }

   [Fact]
   public void Should_return_empty_if_no_items_selected_with_arg()
   {
      var array = ImmutableArray.CreateRange([1, 2, 3]);

      var result = array.SelectWhere(static (int i, int threshold, out int r) =>
      {
         if (i > threshold)
         {
            r = i;
            return true;
         }

         r = 0;
         return false;
      }, 100);

      result.IsEmpty.Should().BeTrue();
   }
}
