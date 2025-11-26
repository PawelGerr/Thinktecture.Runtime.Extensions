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
      AddSmartEnumAndKeyedValueObjects(entityTypeBuilder);
      AddNonKeyedValueObjectMembers(entityTypeBuilder);
   }

   private void AddSmartEnumAndKeyedValueObjects(IConventionEntityTypeBuilder entityTypeBuilder)
   {
      var entity = entityTypeBuilder.Metadata;

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

   private static void AddNonKeyedValueObjectMembers(IConventionEntityTypeBuilder entityTypeBuilder)
   {
      var entity = entityTypeBuilder.Metadata;

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
      ApplyConfigurationToProperty(propertyBuilder.Metadata, metadata);
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
      ApplyConfigurationToProperty(property, metadata);
   }

   private void ApplyConfigurationToProperty(IConventionProperty property, ConversionMetadata conversionMetadata)
   {
      var metadata = MetadataLookup.Find(conversionMetadata.Type);

      switch (metadata)
      {
         case Metadata.Keyed.SmartEnum smartEnumMetadata:
         {
            ApplyToSmartEnumProperty(
               property,
               configuration.SmartEnums.MaxLengthStrategy,
               smartEnumMetadata);

            break;
         }
         case Metadata.Keyed.ValueObject keyedValueObjectMetadata:
         {
            ApplyToKeyedValueObjectProperty(
               property,
               configuration.KeyedValueObjects.MaxLengthStrategy,
               keyedValueObjectMetadata);
            break;
         }
      }

      // Apply legacy callback if present
      configureSmartEnumsAndKeyedValueObjects?.Invoke(property);
   }

   private ValueConverter GetValueConverter(ConversionMetadata metadata)
   {
      return ThinktectureValueConverterFactory.Create(metadata, useConstructorForRead: configuration.UseConstructorForRead);
   }

   private void ApplyToSmartEnumProperty(
      IConventionProperty property,
      ISmartEnumMaxLengthStrategy strategy,
      Metadata.Keyed.SmartEnum smartEnumMetadata)
   {
      var items = smartEnumMetadata.Items.Value;

      if (items.Count == 0)
         return;

      // Check if we should overwrite existing max length
      if (!strategy.OverwriteExistingMaxLength)
      {
         var existingMaxLength = property.GetMaxLength();

         if (existingMaxLength.HasValue)
            return;
      }

      var maxLengthChange = MaxLengthCache.GetOrComputeSmartEnumMaxLength(
         strategy,
         smartEnumMetadata.Type,
         smartEnumMetadata.KeyType,
         items);

      if (maxLengthChange.IsSet)
         property.Builder.HasMaxLength(maxLengthChange.Value);
   }

   private void ApplyToKeyedValueObjectProperty(
      IConventionProperty property,
      IKeyedValueObjectMaxLengthStrategy strategy,
      Metadata.Keyed.ValueObject keyedValueObjectMetadata)
   {
      // Check if we should overwrite existing max length
      if (!strategy.OverwriteExistingMaxLength)
      {
         var existingMaxLength = property.GetMaxLength();

         if (existingMaxLength.HasValue)
            return;
      }

      var maxLengthChange = MaxLengthCache.GetOrComputeKeyedValueObjectMaxLength(
         strategy,
         keyedValueObjectMetadata.Type,
         keyedValueObjectMetadata.KeyType);

      if (maxLengthChange.IsSet)
         property.Builder.HasMaxLength(maxLengthChange.Value);
   }
}
