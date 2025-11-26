using Thinktecture.EntityFrameworkCore;
using Thinktecture.EntityFrameworkCore.Internal;
using Thinktecture.Internal;

namespace Thinktecture;

internal static class MutableItemExtensions
{
   public static void ApplyMaxLengthFromStrategy(
      this MutableItem item,
      Configuration configuration)
   {
      var metadata = MetadataLookup.Find(item.Type);

      switch (metadata)
      {
         case Metadata.Keyed.SmartEnum smartEnumMetadata:
            ApplySmartEnumMaxLength(item, configuration.SmartEnums.MaxLengthStrategy, smartEnumMetadata);
            break;
         case Metadata.Keyed.ValueObject keyedValueObjectMetadata:
            ApplyKeyedValueObjectMaxLength(item, configuration.KeyedValueObjects.MaxLengthStrategy, keyedValueObjectMetadata);
            break;
      }
   }

   private static void ApplySmartEnumMaxLength(
      MutableItem item,
      ISmartEnumMaxLengthStrategy strategy,
      Metadata.Keyed.SmartEnum smartEnumMetadata)
   {
      // Check if we should overwrite existing max length
      if (!strategy.OverwriteExistingMaxLength)
      {
         var existingMaxLength = item.GetMaxLength();

         if (existingMaxLength.HasValue)
            return;
      }

      var items = smartEnumMetadata.Items.Value;

      if (items.Count == 0)
         return;

      // Pass items directly to cache - conversion happens lazily only on cache miss
      var maxLengthChange = MaxLengthCache.GetOrComputeSmartEnumMaxLength(
         strategy,
         smartEnumMetadata.Type,
         smartEnumMetadata.KeyType,
         items);

      if (maxLengthChange.IsSet)
         item.SetMaxLength(maxLengthChange.Value);
   }

   private static void ApplyKeyedValueObjectMaxLength(
      MutableItem item,
      IKeyedValueObjectMaxLengthStrategy strategy,
      Metadata.Keyed.ValueObject keyedValueObjectMetadata)
   {
      // Check if we should overwrite existing max length
      if (!strategy.OverwriteExistingMaxLength)
      {
         var existingMaxLength = item.GetMaxLength();

         if (existingMaxLength.HasValue)
            return;
      }

      var maxLengthChange = MaxLengthCache.GetOrComputeKeyedValueObjectMaxLength(
         strategy,
         keyedValueObjectMetadata.Type,
         keyedValueObjectMetadata.KeyType);

      if (maxLengthChange.IsSet)
         item.SetMaxLength(maxLengthChange.Value);
   }
}
