using System;

namespace Thinktecture
{
   /// <summary>
   /// Base class for enum-like classes.
   /// </summary>
   /// <typeparam name="TKey">Type of the key.</typeparam>
#pragma warning disable CA1716, CA1000
   // ReSharper disable once UnusedTypeParameter
   public interface IEnum<out TKey>
#pragma warning restore CA1716
      where TKey : notnull
   {
      /// <summary>
      /// Gets the key of the item.
      /// </summary>
      TKey GetKey()
      {
         throw new NotImplementedException("This method will be implemented by the source generator.");
      }
   }
}
