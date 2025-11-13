using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thinktecture.EntityFrameworkCore.Storage.ValueConversion;
using Thinktecture.Internal;

namespace Thinktecture;

/// <summary>
/// Extension methods for <see cref="EntityTypeBuilder"/> to add value object converters.
/// </summary>
public static class EntityTypeBuilderExtensions
{
   /// <summary>
   /// Adds value converter to all properties that are Smart Enums or keyed Value Objects.
   /// Properties with a value provider are skipped.
   /// </summary>
   /// <param name="entityTypeBuilder">The entity type builder.</param>
   /// <param name="useConstructorForRead">For keyed value objects only. Use the constructor instead of the factory method when reading the data from the database.</param>
   /// <param name="addConvertersForOwnedTypes">Indication whether to search for owned entities recursively and add value converters to their properties.</param>
   /// <param name="configureEnumsAndKeyedValueObjects">Action for further configuration of the property.</param>
   /// <typeparam name="T">The entity type.</typeparam>
   /// <returns>The entity type builder for method chaining.</returns>
   [Obsolete("Use 'AddThinktectureValueConverters' instead.")]
   public static EntityTypeBuilder<T> AddValueObjectConverters<T>(
      this EntityTypeBuilder<T> entityTypeBuilder,
      bool useConstructorForRead = true,
      bool addConvertersForOwnedTypes = true,
      Action<IMutableProperty>? configureEnumsAndKeyedValueObjects = null)
      where T : class
   {
      return entityTypeBuilder.AddThinktectureValueConverters(useConstructorForRead, addConvertersForOwnedTypes, configureEnumsAndKeyedValueObjects);
   }

   /// <summary>
   /// Adds value converter to all properties that are Smart Enums or keyed Value Objects.
   /// Properties with a value provider are skipped.
   /// </summary>
   /// <param name="entityTypeBuilder">The entity type builder.</param>
   /// <param name="useConstructorForRead">For keyed value objects only. Use the constructor instead of the factory method when reading the data from the database.</param>
   /// <param name="addConvertersForOwnedTypes">Indication whether to search for owned entities recursively and add value converters to their properties.</param>
   /// <param name="configureEnumsAndKeyedValueObjects">Action for further configuration of the property.</param>
   /// <returns>The entity type builder for method chaining.</returns>
   [Obsolete("Use 'AddThinktectureValueConverters' instead.")]
   public static EntityTypeBuilder AddValueObjectConverters(
      this EntityTypeBuilder entityTypeBuilder,
      bool useConstructorForRead = true,
      bool addConvertersForOwnedTypes = true,
      Action<IMutableProperty>? configureEnumsAndKeyedValueObjects = null)
   {
      return entityTypeBuilder.AddThinktectureValueConverters(useConstructorForRead, addConvertersForOwnedTypes, configureEnumsAndKeyedValueObjects);
   }

   /// <summary>
   /// Adds value converter to all properties that are Smart Enums or keyed Value Objects.
   /// Properties with a value provider are skipped.
   /// </summary>
   /// <param name="entityTypeBuilder">The entity type builder.</param>
   /// <param name="useConstructorForRead">For keyed value objects only. Use the constructor instead of the factory method when reading the data from the database.</param>
   /// <param name="addConvertersForOwnedTypes">Indication whether to search for owned entities recursively and add value converters to their properties.</param>
   /// <param name="configureEnumsAndKeyedValueObjects">Action for further configuration of the property.</param>
   /// <returns>The entity type builder for method chaining.</returns>
   [Obsolete("Use 'AddThinktectureValueConverters' instead.")]
   public static OwnedNavigationBuilder<TEntity, TRelatedEntity> AddValueObjectConverters<TEntity, TRelatedEntity>(
      this OwnedNavigationBuilder<TEntity, TRelatedEntity> entityTypeBuilder,
      bool useConstructorForRead = true,
      bool addConvertersForOwnedTypes = true,
      Action<IMutableProperty>? configureEnumsAndKeyedValueObjects = null)
      where TEntity : class
      where TRelatedEntity : class
   {
      return entityTypeBuilder.AddThinktectureValueConverters(useConstructorForRead, addConvertersForOwnedTypes, configureEnumsAndKeyedValueObjects);
   }

   /// <summary>
   /// Adds value converter to all properties that are Smart Enums or keyed Value Objects.
   /// Properties with a value provider are skipped.
   /// </summary>
   /// <param name="entityTypeBuilder">The entity type builder.</param>
   /// <param name="useConstructorForRead">For keyed value objects only. Use the constructor instead of the factory method when reading the data from the database.</param>
   /// <param name="addConvertersForOwnedTypes">Indication whether to search for owned entities recursively and add value converters to their properties.</param>
   /// <param name="configureEnumsAndKeyedValueObjects">Action for further configuration of the property.</param>
   /// <returns>The entity type builder for method chaining.</returns>
   [Obsolete("Use 'AddThinktectureValueConverters' instead.")]
   public static OwnedNavigationBuilder AddValueObjectConverters(
      this OwnedNavigationBuilder entityTypeBuilder,
      bool useConstructorForRead = true,
      bool addConvertersForOwnedTypes = true,
      Action<IMutableProperty>? configureEnumsAndKeyedValueObjects = null)
   {
      return entityTypeBuilder.AddThinktectureValueConverters(useConstructorForRead, addConvertersForOwnedTypes, configureEnumsAndKeyedValueObjects);
   }

   /// <summary>
   /// Adds value converter to all properties that are Smart Enums or keyed Value Objects.
   /// Properties with a value provider are skipped.
   /// </summary>
   /// <param name="entityTypeBuilder">The entity type builder.</param>
   /// <param name="useConstructorForRead">For keyed value objects only. Use the constructor instead of the factory method when reading the data from the database.</param>
   /// <param name="addConvertersForOwnedTypes">Indication whether to search for owned entities recursively and add value converters to their properties.</param>
   /// <param name="configureEnumsAndKeyedValueObjects">Action for further configuration of the property.</param>
   /// <typeparam name="T">The entity type.</typeparam>
   /// <returns>The entity type builder for method chaining.</returns>
   public static EntityTypeBuilder<T> AddThinktectureValueConverters<T>(
      this EntityTypeBuilder<T> entityTypeBuilder,
      bool useConstructorForRead = true,
      bool addConvertersForOwnedTypes = true,
      Action<IMutableProperty>? configureEnumsAndKeyedValueObjects = null)
      where T : class
   {
      ((EntityTypeBuilder)entityTypeBuilder).AddThinktectureValueConverters(
         useConstructorForRead,
         addConvertersForOwnedTypes,
         configureEnumsAndKeyedValueObjects);

      return entityTypeBuilder;
   }

   /// <summary>
   /// Adds value converter to all properties that are Smart Enums or keyed Value Objects.
   /// Properties with a value provider are skipped.
   /// </summary>
   /// <param name="entityTypeBuilder">The entity type builder.</param>
   /// <param name="useConstructorForRead">For keyed value objects only. Use the constructor instead of the factory method when reading the data from the database.</param>
   /// <param name="addConvertersForOwnedTypes">Indication whether to search for owned entities recursively and add value converters to their properties.</param>
   /// <param name="configureEnumsAndKeyedValueObjects">Action for further configuration of the property.</param>
   /// <returns>The entity type builder for method chaining.</returns>
   public static EntityTypeBuilder AddThinktectureValueConverters(
      this EntityTypeBuilder entityTypeBuilder,
      bool useConstructorForRead = true,
      bool addConvertersForOwnedTypes = true,
      Action<IMutableProperty>? configureEnumsAndKeyedValueObjects = null)
   {
      configureEnumsAndKeyedValueObjects ??= Empty.Action;

      AddConvertersToEntity(
         entityTypeBuilder.Metadata,
         useConstructorForRead,
         addConvertersForOwnedTypes,
         configureEnumsAndKeyedValueObjects);

      return entityTypeBuilder;
   }

   /// <summary>
   /// Adds value converter to all properties that are Smart Enums or keyed Value Objects.
   /// Properties with a value provider are skipped.
   /// </summary>
   /// <param name="entityTypeBuilder">The entity type builder.</param>
   /// <param name="useConstructorForRead">For keyed value objects only. Use the constructor instead of the factory method when reading the data from the database.</param>
   /// <param name="addConvertersForOwnedTypes">Indication whether to search for owned entities recursively and add value converters to their properties.</param>
   /// <param name="configureEnumsAndKeyedValueObjects">Action for further configuration of the property.</param>
   /// <returns>The entity type builder for method chaining.</returns>
   public static OwnedNavigationBuilder<TEntity, TRelatedEntity> AddThinktectureValueConverters<TEntity, TRelatedEntity>(
      this OwnedNavigationBuilder<TEntity, TRelatedEntity> entityTypeBuilder,
      bool useConstructorForRead = true,
      bool addConvertersForOwnedTypes = true,
      Action<IMutableProperty>? configureEnumsAndKeyedValueObjects = null)
      where TEntity : class
      where TRelatedEntity : class
   {
      ((OwnedNavigationBuilder)entityTypeBuilder).AddThinktectureValueConverters(
         useConstructorForRead,
         addConvertersForOwnedTypes,
         configureEnumsAndKeyedValueObjects);

      return entityTypeBuilder;
   }

   /// <summary>
   /// Adds value converter to all properties that are Smart Enums or keyed Value Objects.
   /// Properties with a value provider are skipped.
   /// </summary>
   /// <param name="entityTypeBuilder">The entity type builder.</param>
   /// <param name="useConstructorForRead">For keyed value objects only. Use the constructor instead of the factory method when reading the data from the database.</param>
   /// <param name="addConvertersForOwnedTypes">Indication whether to search for owned entities recursively and add value converters to their properties.</param>
   /// <param name="configureEnumsAndKeyedValueObjects">Action for further configuration of the property.</param>
   /// <returns>The entity type builder for method chaining.</returns>
   public static OwnedNavigationBuilder AddThinktectureValueConverters(
      this OwnedNavigationBuilder entityTypeBuilder,
      bool useConstructorForRead = true,
      bool addConvertersForOwnedTypes = true,
      Action<IMutableProperty>? configureEnumsAndKeyedValueObjects = null)
   {
      configureEnumsAndKeyedValueObjects ??= Empty.Action;

      AddConvertersToEntity(
         entityTypeBuilder.OwnedEntityType,
         useConstructorForRead,
         addConvertersForOwnedTypes,
         configureEnumsAndKeyedValueObjects);

      return entityTypeBuilder;
   }

   /// <summary>
   /// Adds value converter to all properties that are Smart Enums or keyed Value Objects.
   /// Properties with a value provider are skipped.
   /// </summary>
   /// <param name="complexPropertyBuilder">The complex property builder.</param>
   /// <param name="useConstructorForRead">For keyed value objects only. Use the constructor instead of the factory method when reading the data from the database.</param>
   /// <param name="configureEnumsAndKeyedValueObjects">Action for further configuration of the property.</param>
   /// <returns>The entity type builder for method chaining.</returns>
   [Obsolete("Use 'AddThinktectureValueConverters' instead.")]
   public static ComplexPropertyBuilder<TComplex> AddValueObjectConverters<TComplex>(
      this ComplexPropertyBuilder<TComplex> complexPropertyBuilder,
      bool useConstructorForRead = true,
      Action<IMutableProperty>? configureEnumsAndKeyedValueObjects = null)
      where TComplex : class
   {
      return complexPropertyBuilder.AddThinktectureValueConverters(useConstructorForRead, configureEnumsAndKeyedValueObjects);
   }

   /// <summary>
   /// Adds value converter to all properties that are Smart Enums or keyed Value Objects.
   /// Properties with a value provider are skipped.
   /// </summary>
   /// <param name="complexPropertyBuilder">The complex property builder.</param>
   /// <param name="useConstructorForRead">For keyed value objects only. Use the constructor instead of the factory method when reading the data from the database.</param>
   /// <param name="configureEnumsAndKeyedValueObjects">Action for further configuration of the property.</param>
   /// <returns>The entity type builder for method chaining.</returns>
   [Obsolete("Use 'AddThinktectureValueConverters' instead.")]
   public static ComplexPropertyBuilder AddValueObjectConverters(
      this ComplexPropertyBuilder complexPropertyBuilder,
      bool useConstructorForRead = true,
      Action<IMutableProperty>? configureEnumsAndKeyedValueObjects = null)
   {
      return complexPropertyBuilder.AddThinktectureValueConverters(useConstructorForRead, configureEnumsAndKeyedValueObjects);
   }

   /// <summary>
   /// Adds value converter to all properties that are Smart Enums or keyed Value Objects.
   /// Properties with a value provider are skipped.
   /// </summary>
   /// <param name="complexPropertyBuilder">The complex property builder.</param>
   /// <param name="useConstructorForRead">For keyed value objects only. Use the constructor instead of the factory method when reading the data from the database.</param>
   /// <param name="configureEnumsAndKeyedValueObjects">Action for further configuration of the property.</param>
   /// <returns>The entity type builder for method chaining.</returns>
   public static ComplexPropertyBuilder<TComplex> AddThinktectureValueConverters<TComplex>(
      this ComplexPropertyBuilder<TComplex> complexPropertyBuilder,
      bool useConstructorForRead = true,
      Action<IMutableProperty>? configureEnumsAndKeyedValueObjects = null)
      where TComplex : class
   {
      ((ComplexPropertyBuilder)complexPropertyBuilder).AddThinktectureValueConverters(
         useConstructorForRead,
         configureEnumsAndKeyedValueObjects);

      return complexPropertyBuilder;
   }

   /// <summary>
   /// Adds value converter to all properties that are Smart Enums or keyed Value Objects.
   /// Properties with a value provider are skipped.
   /// </summary>
   /// <param name="complexPropertyBuilder">The complex property builder.</param>
   /// <param name="useConstructorForRead">For keyed value objects only. Use the constructor instead of the factory method when reading the data from the database.</param>
   /// <param name="configureEnumsAndKeyedValueObjects">Action for further configuration of the property.</param>
   /// <returns>The entity type builder for method chaining.</returns>
   public static ComplexPropertyBuilder AddThinktectureValueConverters(
      this ComplexPropertyBuilder complexPropertyBuilder,
      bool useConstructorForRead = true,
      Action<IMutableProperty>? configureEnumsAndKeyedValueObjects = null)
   {
      configureEnumsAndKeyedValueObjects ??= Empty.Action;

      AddConvertersForComplexProperty(
         complexPropertyBuilder.Metadata,
         useConstructorForRead,
         configureEnumsAndKeyedValueObjects);

      return complexPropertyBuilder;
   }

   internal static void AddConvertersToEntity(
      this IMutableEntityType entity,
      bool useConstructorForRead,
      bool addConvertersForOwnedTypes,
      Action<IMutableProperty> configureEnumsAndKeyedValueObjects)
   {
      AddSmartEnumAndKeyedValueObjects(entity, useConstructorForRead, configureEnumsAndKeyedValueObjects);
      AddNonKeyedValueObjectMembers(entity);

      AddConverterForScalarProperties(entity, useConstructorForRead, configureEnumsAndKeyedValueObjects);
      AddConverterForComplexProperties(entity, useConstructorForRead, configureEnumsAndKeyedValueObjects);
      AddConvertersForNavigations(entity, useConstructorForRead, configureEnumsAndKeyedValueObjects);

      if (addConvertersForOwnedTypes)
         AddConvertersForOwnedTypes(entity, useConstructorForRead, configureEnumsAndKeyedValueObjects);
   }

   private static void AddSmartEnumAndKeyedValueObjects(
      IMutableEntityType entity,
      bool useConstructorForRead,
      Action<IMutableProperty> configure)
   {
      foreach (var propertyInfo in entity.ClrType.GetRuntimeProperties())
      {
         // will be handled by "AddConvertersForNavigations"
         if (entity.FindNavigation(propertyInfo) is not null)
            continue;

         AddConverterToNonNavigation(entity, propertyInfo, useConstructorForRead, configure);
      }
   }

   private static void AddSmartEnumAndKeyedValueObjects(
      IMutableTypeBase entity,
      bool useConstructorForRead,
      Action<IMutableProperty> configure)
   {
      foreach (var propertyInfo in entity.ClrType.GetRuntimeProperties())
      {
         AddConverterToNonNavigation(entity, propertyInfo, useConstructorForRead, configure);
      }
   }

   private static void AddConverterToNonNavigation(
      IMutableTypeBase entity,
      PropertyInfo propertyInfo,
      bool useConstructorForRead,
      Action<IMutableProperty> configure)
   {
      if (entity.IsIgnored(propertyInfo.Name))
         return;

      // will be handled by AddConverterForScalarProperties
      if (entity.FindProperty(propertyInfo) is not null)
         return;

#if USE_FIND_COMPLEX_PROPERTY_FIX
      var complexProperty = entity.FindComplexPropertyFix(propertyInfo);
#else
      var complexProperty = entity.FindComplexProperty(propertyInfo);
#endif
      // will be handled by AddConverterForComplexProperties
      if (complexProperty is not null)
         return;

      if (!propertyInfo.IsCandidateProperty())
         return;

      if (propertyInfo.PropertyType.FindMetadataForValueConverter() is not { } metadata)
         return;

      var property = entity.AddProperty(propertyInfo);

      SetConverterAndExecuteCallback(useConstructorForRead, configure, property, metadata);
   }

   private static void AddNonKeyedValueObjectMembers(
      IMutableTypeBase entity)
   {
      if (!entity.ClrType.TryGetAssignableMembers(out var members))
         return;

      foreach (var member in members)
      {
         if (entity.IsIgnored(member.Name))
            continue;

#if USE_FIND_COMPLEX_PROPERTY_FIX
         var complexProperty = entity.FindComplexPropertyFix(member);
#else
         var complexProperty = entity.FindComplexProperty(member);
#endif
         // will be handled by AddConverterForComplexProperties
         if (complexProperty is not null)
            continue;

         var property = entity.FindProperty(member);

         if (property is null)
            entity.AddProperty(member);
      }
   }

   private static void AddConvertersForNavigations(
      IMutableEntityType entity,
      bool useConstructorForRead,
      Action<IMutableProperty> configure)
   {
      List<(IMutableNavigation, ConversionMetadata)>? navigationsToConvert = null;

      foreach (var navigation in entity.GetNavigations())
      {
         if (navigation.ClrType.FindMetadataForValueConverter() is not { } metadata)
            continue;

         (navigationsToConvert ??= []).Add((navigation, metadata));
      }

      if (navigationsToConvert is null)
         return;

      var builders = entity.FindEntityBuilder();

      foreach (var navigation in navigationsToConvert)
      {
         var property = FindPropertyBuilder(builders, entity, navigation.Item1.Name);

         SetConverterAndExecuteCallback(useConstructorForRead, configure, property.Metadata, navigation.Item2);
      }
   }

   private static void AddConvertersForOwnedTypes(
      IMutableEntityType entity,
      bool useConstructorForRead,
      Action<IMutableProperty> configure)
   {
      foreach (var navigation in entity.GetNavigations())
      {
         if (!navigation.ForeignKey.IsOwnership || entity != navigation.ForeignKey.PrincipalEntityType)
            continue;

         AddConvertersToEntity(
            navigation.TargetEntityType,
            useConstructorForRead,
            true,
            configure);
      }
   }

   private static void AddConverterForScalarProperties(
      IMutableTypeBase entity,
      bool useConstructorForRead,
      Action<IMutableProperty> configure)
   {
      foreach (var property in entity.GetProperties())
      {
         var valueConverter = property.GetValueConverter();

         if (valueConverter is not null)
            continue;

         if (property.IsPrimitiveCollection)
         {
            AddConverterForPrimitiveCollections(property, useConstructorForRead, configure);
            continue;
         }

         if (property.ClrType.FindMetadataForValueConverter() is not { } metadata)
            continue;

         SetConverterAndExecuteCallback(useConstructorForRead, configure, property, metadata);
      }
   }

   private static void AddConverterForPrimitiveCollections(
      IMutableProperty property,
      bool useConstructorForRead,
      Action<IMutableProperty> configure)
   {
      var elementType = property.GetElementType();

      if (elementType is null)
         return;

      if (elementType.ClrType.FindMetadataForValueConverter() is not { } metadata)
         return;

      var valueConverter = ThinktectureValueConverterFactory.Create(metadata, useConstructorForRead);
      elementType.SetValueConverter(valueConverter);
      configure(property);
   }

   private static void AddConverterForComplexProperties(
      IMutableTypeBase entity,
      bool useConstructorForRead,
      Action<IMutableProperty> configureEnumsAndKeyedValueObjects)
   {
      foreach (var complexProperty in entity.GetComplexProperties())
      {
         AddConvertersForComplexProperty(complexProperty, useConstructorForRead, configureEnumsAndKeyedValueObjects);
      }
   }

   private static void AddConvertersForComplexProperty(
      IMutableComplexProperty complexProperty,
      bool useConstructorForRead,
      Action<IMutableProperty> configureEnumsAndKeyedValueObjects)
   {
      AddSmartEnumAndKeyedValueObjects(complexProperty.ComplexType, useConstructorForRead, configureEnumsAndKeyedValueObjects);
      AddNonKeyedValueObjectMembers(complexProperty.ComplexType);

      AddConverterForScalarProperties(complexProperty.ComplexType, useConstructorForRead, configureEnumsAndKeyedValueObjects);
      AddConverterForComplexProperties(complexProperty.ComplexType, useConstructorForRead, configureEnumsAndKeyedValueObjects);
   }

   private static PropertyBuilder FindPropertyBuilder((EntityTypeBuilder?, OwnedNavigationBuilder?) builders, IMutableEntityType entityType, string propertyName)
   {
      return builders.Item1?.Property(propertyName)
             ?? builders.Item2?.Property(propertyName)
             ?? throw new Exception($"Property '{propertyName}' not found in the entity '{entityType.Name}'.");
   }

   [SuppressMessage("Usage", "EF1001:Internal EF Core API usage.")]
   private static (EntityTypeBuilder?, OwnedNavigationBuilder?) FindEntityBuilder(this IMutableEntityType entityType)
   {
      if (!entityType.IsOwned())
         return (new EntityTypeBuilder(entityType), null);

      var ownership = entityType.FindOwnership() ?? throw new Exception($"Ownership of the owned entity '{entityType.Name}' not found.");
      var navigation = ownership.GetNavigation(false)?.Name ?? throw new Exception($"Navigation of the owned entity '{entityType.Name}' not found.");
      var (entityTypeBuilder, ownedNavigationBuilder) = ownership.PrincipalEntityType.FindEntityBuilder();

      if (entityTypeBuilder is not null)
      {
         ownedNavigationBuilder = ownership.IsUnique
                                     ? entityTypeBuilder.OwnsOne(entityType.ClrType, navigation)
                                     : entityTypeBuilder.OwnsMany(entityType.ClrType, navigation);
      }
      else if (ownedNavigationBuilder is not null)
      {
         ownedNavigationBuilder = ownership.IsUnique
                                     ? ownedNavigationBuilder.OwnsOne(entityType.ClrType, navigation)
                                     : ownedNavigationBuilder.OwnsMany(entityType.ClrType, navigation);
      }
      else
      {
         throw new Exception($"Entity Builder for Owned Type '{entityType.Name}' not found.");
      }

      return (null, ownedNavigationBuilder);
   }

   private static void SetConverterAndExecuteCallback(
      bool useConstructorForRead,
      Action<IMutableProperty> configure,
      IMutableProperty property,
      ConversionMetadata metadata)
   {
      var valueConverter = ThinktectureValueConverterFactory.Create(metadata, useConstructorForRead);
      property.SetValueConverter(valueConverter);
      configure(property);
   }
}
