using System;

namespace Thinktecture
{
   /// <summary>
   /// Base class for enum-like classes.
   /// </summary>
   /// <typeparam name="TKey">Type of the key.</typeparam>
   public interface IValidatableEnum<out TKey> : IEnum<TKey>
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
   }
}
