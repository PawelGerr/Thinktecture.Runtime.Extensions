using System;
using System.Diagnostics.CodeAnalysis;

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
   // ReSharper disable once UnusedTypeParameter
   public interface IEnum<out TKey>
#pragma warning restore CA1716
      where TKey : notnull
   {
      /// <summary>
      /// Indication whether the current enumeration item is valid or not.
      /// </summary>
      bool IsValid { get; }

      /// <summary>
      /// Checks whether current enumeration item is valid.
      /// </summary>
      /// <exception cref="InvalidOperationException">The enumeration item is not valid.</exception>
      void EnsureValid();

      /// <summary>
      /// Gets the key of the item.
      /// </summary>
      [return: NotNull]
      TKey GetKey();
   }
}
