using System.Reflection;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Thinktecture.EntityFrameworkCore.Internal;
using Thinktecture.EntityFrameworkCore.Storage.ValueConversion;
using Thinktecture.Internal;

namespace Thinktecture.EntityFrameworkCore.Conventions;

internal sealed class ThinktectureConventionsPlugin(
   Configuration configuration,
   Action<IConventionProperty>? configureSmartEnumsAndKeyedValueObjects)
   : INavigationAddedConvention,
     IPropertyAddedConvention,
     IEntityTypeAddedConvention,
     IPropertyElementTypeChangedConvention
{
   public void ProcessEntityTypeAdded(IConventionEntityTypeBuilder entityTypeBuilder, IConventionContext<IConventionEntityTypeBuilder> context)
   {
      AddSmartEnumAndKeyedValueObjects(entityTypeBuilder.Metadata);
      AddNonKeyedValueObjectMembers(entityTypeBuilder.Metadata);
   }

   private void AddSmartEnumAndKeyedValueObjects(IConventionEntityType entity)
   {
      foreach (var propertyInfo in entity.ClrType.GetRuntimeProperties())
      {
         if (entity.IsIgnored(propertyInfo.Name))
            continue;

         var navigation = entity.FindNavigation(propertyInfo);

         if (navigation is not null)
         {
            ProcessNavigation(navigation);
            continue;
         }

         var property = entity.FindProperty(propertyInfo);

         if (property is not null)
         {
            ProcessProperty(property);
            continue;
         }

         if (!propertyInfo.IsCandidateProperty())
            continue;

         if (propertyInfo.PropertyType.FindMetadataForValueConverter() is not { } metadata)
            continue;

         property = entity.AddProperty(propertyInfo);

         if (property is not null)
            SetConverterAndExecuteCallback(property, metadata);
      }
   }

   private static void AddNonKeyedValueObjectMembers(IConventionEntityType entity)
   {
      if (!entity.ClrType.TryGetAssignableMembers(out var members) || members.Count == 0)
         return;

      foreach (var member in members)
      {
         if (entity.IsIgnored(member.Name))
            continue;

         var complexProperty = entity.FindComplexProperty(member);

         // Ignore complex properties, even if it has an ObjectFactory
         if (complexProperty is not null)
            continue;

         var property = entity.FindProperty(member);

         if (property is null)
            entity.AddProperty(member);
      }
   }

   public void ProcessNavigationAdded(IConventionNavigationBuilder navigationBuilder, IConventionContext<IConventionNavigationBuilder> context)
   {
      ProcessNavigation(navigationBuilder.Metadata);
   }

   public void ProcessPropertyAdded(IConventionPropertyBuilder propertyBuilder, IConventionContext<IConventionPropertyBuilder> context)
   {
      ProcessProperty(propertyBuilder.Metadata);
   }

   public void ProcessPropertyElementTypeChanged(
      IConventionPropertyBuilder propertyBuilder,
      IElementType? newElementType,
      IElementType? oldElementType,
      IConventionContext<IElementType> context)
   {
      var elementType = propertyBuilder.Metadata.GetElementType();

      if (elementType is null)
         return;

      var valueConverter = elementType.GetValueConverter();

      if (valueConverter is not null)
         return;

      if (elementType.ClrType.FindMetadataForValueConverter() is not { } metadata)
         return;

      elementType.SetValueConverter(GetValueConverter(metadata));

      // The max-length strategy must be applied to the element, not to the collection property. The collection is
      // stored as a single JSON column, so a max length on the property would constrain the whole array to the
      // length of one element and truncate the persisted JSON.
      ApplyMaxLengthStrategy(new MaxLengthTarget(elementType), metadata);

      // Apply legacy callback if present. It operates on the collection property, matching the previous behavior.
      configureSmartEnumsAndKeyedValueObjects?.Invoke(propertyBuilder.Metadata);
   }

   private void ProcessNavigation(IConventionNavigation navigation)
   {
      var naviType = navigation.ClrType;

      if (naviType.FindMetadataForValueConverter() is not { } metadata)
         return;

      var property = navigation.DeclaringEntityType.Builder
                               .Property(naviType, navigation.Name)?
                               .Metadata;

      if (property is null)
         return;

      SetConverterAndExecuteCallback(property, metadata);
   }

   private void ProcessProperty(IConventionProperty property)
   {
      var valueConverter = property.GetValueConverter();

      if (valueConverter is not null)
         return;

      if (property.ClrType.FindMetadataForValueConverter() is not { } metadata)
         return;

      SetConverterAndExecuteCallback(property, metadata);
   }

   private void SetConverterAndExecuteCallback(IConventionProperty property, ConversionMetadata metadata)
   {
      property.SetValueConverter(GetValueConverter(metadata));
      ApplyMaxLengthStrategy(new MaxLengthTarget(property), metadata);

      // Apply legacy callback if present
      configureSmartEnumsAndKeyedValueObjects?.Invoke(property);
   }

   private void ApplyMaxLengthStrategy(MaxLengthTarget target, ConversionMetadata conversionMetadata)
   {
      var metadata = MetadataLookup.Find(conversionMetadata.Type);

      // The value converter stores the type reported by the conversion metadata, which may differ from the
      // key type of the Smart Enum or keyed Value Object when an Entity-Framework-flagged object factory is used.
      // The key-based max-length strategy only applies when the converter actually stores the key type.
      switch (metadata)
      {
         case Metadata.Keyed.SmartEnum smartEnumMetadata when conversionMetadata.KeyType == smartEnumMetadata.KeyType:
         {
            ApplyToSmartEnum(
               target,
               configuration.SmartEnums.MaxLengthStrategy,
               smartEnumMetadata);

            break;
         }
         case Metadata.Keyed.ValueObject keyedValueObjectMetadata when conversionMetadata.KeyType == keyedValueObjectMetadata.KeyType:
         {
            ApplyToKeyedValueObject(
               target,
               configuration.KeyedValueObjects.MaxLengthStrategy,
               keyedValueObjectMetadata);
            break;
         }
      }
   }

   private ValueConverter GetValueConverter(ConversionMetadata metadata)
   {
      return ThinktectureValueConverterFactory.Create(metadata, useConstructorForRead: configuration.UseConstructorForRead);
   }

   private static void ApplyToSmartEnum(
      MaxLengthTarget target,
      ISmartEnumMaxLengthStrategy strategy,
      Metadata.Keyed.SmartEnum smartEnumMetadata)
   {
      var items = smartEnumMetadata.Items.Value;

      if (items.Count == 0)
         return;

      // Check if we should overwrite existing max length
      if (!strategy.OverwriteExistingMaxLength)
      {
         var existingMaxLength = target.GetMaxLength();

         if (existingMaxLength.HasValue)
            return;
      }

      var maxLengthChange = MaxLengthCache.GetOrComputeSmartEnumMaxLength(
         strategy,
         smartEnumMetadata.Type,
         smartEnumMetadata.KeyType,
         items);

      if (maxLengthChange.IsSet)
         target.SetMaxLength(maxLengthChange.Value);
   }

   private static void ApplyToKeyedValueObject(
      MaxLengthTarget target,
      IKeyedValueObjectMaxLengthStrategy strategy,
      Metadata.Keyed.ValueObject keyedValueObjectMetadata)
   {
      // Check if we should overwrite existing max length
      if (!strategy.OverwriteExistingMaxLength)
      {
         var existingMaxLength = target.GetMaxLength();

         if (existingMaxLength.HasValue)
            return;
      }

      var maxLengthChange = MaxLengthCache.GetOrComputeKeyedValueObjectMaxLength(
         strategy,
         keyedValueObjectMetadata.Type,
         keyedValueObjectMetadata.KeyType);

      if (maxLengthChange.IsSet)
         target.SetMaxLength(maxLengthChange.Value);
   }

   /// <summary>
   /// Abstracts the metadata object that carries the max length. For a scalar property this is the property itself;
   /// for a primitive collection it is the element, because the collection is persisted as a single JSON column.
   /// </summary>
   private readonly struct MaxLengthTarget
   {
      private readonly IConventionProperty? _property;
      private readonly IConventionElementType? _elementType;

      public MaxLengthTarget(IConventionProperty property)
      {
         _property = property;
         _elementType = null;
      }

      public MaxLengthTarget(IConventionElementType elementType)
      {
         _property = null;
         _elementType = elementType;
      }

      public int? GetMaxLength()
      {
         return _property?.GetMaxLength() ?? _elementType?.GetMaxLength();
      }

      public void SetMaxLength(int? maxLength)
      {
         _property?.Builder.HasMaxLength(maxLength);
         _elementType?.Builder.HasMaxLength(maxLength);
      }
   }
}
