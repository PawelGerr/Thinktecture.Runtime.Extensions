using System;
using System.Linq.Expressions;
using JetBrains.Annotations;

namespace Thinktecture
{
   /// <summary>
   /// Non-generic interface implemented by <see cref="Enum{TEnum,TKey}"/>.
   /// </summary>
   public interface IEnum
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
      /// The key of the enumeration item.
      /// </summary>
      [NotNull]
      object Key { get; }
   }
}
