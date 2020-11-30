using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Thinktecture
{
   /// <summary>
   /// Base class for enum-like classes.
   /// </summary>
   /// <remarks>
   /// Derived classes must have a default constructor for creation of "invalid" enumeration items.
   /// The default constructor should not be public.
   /// </remarks>
   /// <typeparam name="TKey">Type of the key.</typeparam>
#pragma warning disable CA1716, CA1000
   public interface IEnum<TKey> : IEnum
#pragma warning restore CA1716
      where TKey : notnull
   {
      /// <summary>
      /// The key of the enumeration item.
      /// </summary>
      [NotNull]
      new TKey Key { get; }
   }
}
