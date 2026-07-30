using Microsoft.EntityFrameworkCore.Metadata;
using Thinktecture.Internal;

namespace Thinktecture.EntityFrameworkCore.Internal;

/// <summary>
/// Abstracts the model item that carries the max length. For a scalar property this is the property itself;
/// for a primitive collection it is the element, because the collection is persisted as a single JSON column
/// and a max length on the property would constrain the whole array to the length of one element.
/// </summary>
internal readonly struct MaxLengthTarget
{
   private readonly Func<int?> _getMaxLength;
   private readonly Action<int?> _setMaxLength;

   public MaxLengthTarget(IMutableProperty property)
   {
      _getMaxLength = () => property.GetMaxLength();
      _setMaxLength = maxLength => property.SetMaxLength(maxLength);
   }

   public MaxLengthTarget(IMutableElementType elementType)
   {
      _getMaxLength = () => elementType.GetMaxLength();
      _setMaxLength = maxLength => elementType.SetMaxLength(maxLength);
   }

   public MaxLengthTarget(IConventionProperty property)
   {
      _getMaxLength = () => property.GetMaxLength();
      _setMaxLength = maxLength => property.Builder.HasMaxLength(maxLength);
   }

   public MaxLengthTarget(IConventionElementType elementType)
   {
      _getMaxLength = () => elementType.GetMaxLength();
      _setMaxLength = maxLength => elementType.Builder.HasMaxLength(maxLength);
   }

   public void ApplyMaxLengthFromStrategy(Configuration configuration, ConversionMetadata conversionMetadata)
   {
      var metadata = MetadataLookup.Find(conversionMetadata.Type);

      // An Entity-Framework-flagged object factory controls the persisted value. Even when its ValueType
      // equals the key type, the persisted values may differ from the item keys, so the key-based
      // max-length strategy could silently truncate them. ReadOnlySpan<char> factories are excluded to
      // mirror TypeExtensions.FindMetadataForValueConverter, where the conversion falls back to the key.
      if (metadata is null || HasObjectFactoryForEntityFramework(metadata))
         return;

      switch (metadata)
      {
         case Metadata.Keyed.SmartEnum smartEnumMetadata:
            ApplySmartEnumMaxLength(configuration.SmartEnums.MaxLengthStrategy, smartEnumMetadata);
            break;
         case Metadata.Keyed.ValueObject keyedValueObjectMetadata:
            ApplyKeyedValueObjectMaxLength(configuration.KeyedValueObjects.MaxLengthStrategy, keyedValueObjectMetadata);
            break;
      }
   }

   private static bool HasObjectFactoryForEntityFramework(Metadata metadata)
   {
      return metadata.ObjectFactories.Any(f => f.UseWithEntityFramework && f.ValueType != typeof(ReadOnlySpan<char>));
   }

   private void ApplySmartEnumMaxLength(
      ISmartEnumMaxLengthStrategy strategy,
      Metadata.Keyed.SmartEnum smartEnumMetadata)
   {
      // Check if we should overwrite existing max length
      if (!strategy.OverwriteExistingMaxLength && _getMaxLength().HasValue)
         return;

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
         _setMaxLength(maxLengthChange.Value);
   }

   private void ApplyKeyedValueObjectMaxLength(
      IKeyedValueObjectMaxLengthStrategy strategy,
      Metadata.Keyed.ValueObject keyedValueObjectMetadata)
   {
      // Check if we should overwrite existing max length
      if (!strategy.OverwriteExistingMaxLength && _getMaxLength().HasValue)
         return;

      var maxLengthChange = MaxLengthCache.GetOrComputeKeyedValueObjectMaxLength(
         strategy,
         keyedValueObjectMetadata.Type,
         keyedValueObjectMetadata.KeyType);

      if (maxLengthChange.IsSet)
         _setMaxLength(maxLengthChange.Value);
   }
}
