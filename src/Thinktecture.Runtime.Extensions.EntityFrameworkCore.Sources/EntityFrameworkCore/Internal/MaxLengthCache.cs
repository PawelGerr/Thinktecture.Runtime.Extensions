using System.Collections.Concurrent;

namespace Thinktecture.EntityFrameworkCore.Internal;

/// <summary>
/// Cache for computed max length values to avoid redundant calculations.
/// </summary>
public static class MaxLengthCache
{
   private static readonly ConcurrentDictionary<(ISmartEnumMaxLengthStrategy Strategy, Type Type), MaxLengthChange> _smartEnumMaxLengthCache = new();
   private static readonly ConcurrentDictionary<(IKeyedValueObjectMaxLengthStrategy Strategy, Type Type), MaxLengthChange> _keyedValueObjectMaxLengthCache = new();

   /// <summary>
   /// Clears the cache.
   /// </summary>
   public static void Clear()
   {
      _smartEnumMaxLengthCache.Clear();
      _keyedValueObjectMaxLengthCache.Clear();
   }

   /// <summary>
   /// Gets or computes the max length change for a smart enum type using the provided strategy.
   /// Results are cached to avoid redundant computation.
   /// This method accepts the raw items collection and handles lazy conversion to ISmartEnumItem only when needed (cache miss).
   /// </summary>
   /// <param name="strategy">The max length strategy to use.</param>
   /// <param name="type">The smart enum type.</param>
   /// <param name="keyType">The key type of the smart enum.</param>
   /// <param name="items">The raw smart enum items collection from metadata.</param>
   /// <returns>A <see cref="MaxLengthChange"/> indicating whether to change the max length and what value to set.</returns>
   public static MaxLengthChange GetOrComputeSmartEnumMaxLength(
      ISmartEnumMaxLengthStrategy strategy,
      Type type,
      Type keyType,
      IReadOnlyList<Thinktecture.Internal.SmartEnumItemMetadata> items)
   {
      var key = (strategy, type);

      if (_smartEnumMaxLengthCache.TryGetValue(key, out var maxLengthChange))
         return maxLengthChange;

      return _smartEnumMaxLengthCache.GetOrAdd(
         key,
         static (k, state) =>
         {
            var itemInfos = state.items.Select(i => new SmartEnumItem(i.Key, i.Item, i.Identifier)).ToList();
            return k.Strategy.GetMaxLength(state.type, state.keyType, itemInfos);
         },
         (type, keyType, items));
   }

   /// <summary>
   /// Gets or computes the max length change for a keyed value object type using the provided strategy.
   /// Results are cached to avoid redundant computation.
   /// </summary>
   /// <param name="strategy">The max length strategy to use.</param>
   /// <param name="type">The keyed value object type.</param>
   /// <param name="keyType">The key type of the keyed value object.</param>
   /// <returns>A <see cref="MaxLengthChange"/> indicating whether to change the max length and what value to set.</returns>
   public static MaxLengthChange GetOrComputeKeyedValueObjectMaxLength(
      IKeyedValueObjectMaxLengthStrategy strategy,
      Type type,
      Type keyType)
   {
      return _keyedValueObjectMaxLengthCache.GetOrAdd(
         (strategy, type),
         static (key, state) => key.Strategy.GetMaxLength(state.type, state.keyType),
         (type, keyType));
   }
}
