using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
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
   /// <param name="validateOnWrite">In case of a validatable Smart Enum, ensures that the item is valid before writing it to database.</param>
   /// <param name="useConstructorForRead">For keyed value objects only. Use the constructor instead of the factory method when reading the data from database.</param>
   /// <param name="addConvertersForOwnedTypes">Indication whether to search for owned entities recursively and add value converters to their properties.</param>
   /// <param name="configureEnumsAndKeyedValueObjects">Action for further configuration of the property.</param>
   /// <typeparam name="T">The entity type.</typeparam>
   /// <returns>The entity type builder for method chaining.</returns>
   public static EntityTypeBuilder<T> AddValueObjectConverters<T>(
      this EntityTypeBuilder<T> entityTypeBuilder,
      bool validateOnWrite,
      bool useConstructorForRead = true,
      bool addConvertersForOwnedTypes = true,
      Action<IMutableProperty>? configureEnumsAndKeyedValueObjects = null)
      where T : class
   {
      ((EntityTypeBuilder)entityTypeBuilder).AddValueObjectConverters(
         validateOnWrite,
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
   /// <param name="validateOnWrite">In case of a validatable Smart Enum, ensures that the item is valid before writing it to database.</param>
   /// <param name="useConstructorForRead">For keyed value objects only. Use the constructor instead of the factory method when reading the data from database.</param>
   /// <param name="addConvertersForOwnedTypes">Indication whether to search for owned entities recursively and add value converters to their properties.</param>
   /// <param name="configureEnumsAndKeyedValueObjects">Action for further configuration of the property.</param>
   /// <returns>The entity type builder for method chaining.</returns>
   public static EntityTypeBuilder AddValueObjectConverters(
      this EntityTypeBuilder entityTypeBuilder,
      bool validateOnWrite,
      bool useConstructorForRead = true,
      bool addConvertersForOwnedTypes = true,
      Action<IMutableProperty>? configureEnumsAndKeyedValueObjects = null)
   {
      configureEnumsAndKeyedValueObjects ??= Empty.Action;

      AddConvertersToEntity(
         entityTypeBuilder.Metadata,
         validateOnWrite,
         useConstructorForRead,
         addConvertersForOwnedTypes,
         null, // Cache for a single entity will be more of a drawback
         configureEnumsAndKeyedValueObjects);

      return entityTypeBuilder;
   }

   /// <summary>
   /// Adds value converter to all properties that are Smart Enums or keyed Value Objects.
   /// Properties with a value provider are skipped.
   /// </summary>
   /// <param name="entityTypeBuilder">The entity type builder.</param>
   /// <param name="validateOnWrite">In case of a validatable Smart Enum, ensures that the item is valid before writing it to database.</param>
   /// <param name="useConstructorForRead">For keyed value objects only. Use the constructor instead of the factory method when reading the data from database.</param>
   /// <param name="addConvertersForOwnedTypes">Indication whether to search for owned entities recursively and add value converters to their properties.</param>
   /// <param name="configureEnumsAndKeyedValueObjects">Action for further configuration of the property.</param>
   /// <returns>The entity type builder for method chaining.</returns>
   public static OwnedNavigationBuilder<TEntity, TRelatedEntity> AddValueObjectConverters<TEntity, TRelatedEntity>(
      this OwnedNavigationBuilder<TEntity, TRelatedEntity> entityTypeBuilder,
      bool validateOnWrite,
      bool useConstructorForRead = true,
      bool addConvertersForOwnedTypes = true,
      Action<IMutableProperty>? configureEnumsAndKeyedValueObjects = null)
      where TEntity : class
      where TRelatedEntity : class
   {
      ((OwnedNavigationBuilder)entityTypeBuilder).AddValueObjectConverters(
         validateOnWrite,
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
   /// <param name="validateOnWrite">In case of a validatable Smart Enum, ensures that the item is valid before writing it to database.</param>
   /// <param name="useConstructorForRead">For keyed value objects only. Use the constructor instead of the factory method when reading the data from database.</param>
   /// <param name="addConvertersForOwnedTypes">Indication whether to search for owned entities recursively and add value converters to their properties.</param>
   /// <param name="configureEnumsAndKeyedValueObjects">Action for further configuration of the property.</param>
   /// <returns>The entity type builder for method chaining.</returns>
   public static OwnedNavigationBuilder AddValueObjectConverters(
      this OwnedNavigationBuilder entityTypeBuilder,
      bool validateOnWrite,
      bool useConstructorForRead = true,
      bool addConvertersForOwnedTypes = true,
      Action<IMutableProperty>? configureEnumsAndKeyedValueObjects = null)
   {
      configureEnumsAndKeyedValueObjects ??= Empty.Action;

      AddConvertersToEntity(
         entityTypeBuilder.OwnedEntityType,
         validateOnWrite,
         useConstructorForRead,
         addConvertersForOwnedTypes,
         null, // Cache for a single entity will be more of a drawback
         configureEnumsAndKeyedValueObjects);

      return entityTypeBuilder;
   }

#if COMPLEX_TYPES

   /// <summary>
   /// Adds value converter to all properties that are Smart Enums or keyed Value Objects.
   /// Properties with a value provider are skipped.
   /// </summary>
   /// <param name="complexPropertyBuilder">The complex property builder.</param>
   /// <param name="validateOnWrite">In case of a validatable Smart Enum, ensures that the item is valid before writing it to database.</param>
   /// <param name="useConstructorForRead">For keyed value objects only. Use the constructor instead of the factory method when reading the data from database.</param>
   /// <param name="configureEnumsAndKeyedValueObjects">Action for further configuration of the property.</param>
   /// <returns>The entity type builder for method chaining.</returns>
   public static ComplexPropertyBuilder<TComplex> AddValueObjectConverters<TComplex>(
      this ComplexPropertyBuilder<TComplex> complexPropertyBuilder,
      bool validateOnWrite,
      bool useConstructorForRead = true,
      Action<IMutableProperty>? configureEnumsAndKeyedValueObjects = null)
      where TComplex : class
   {
      ((ComplexPropertyBuilder)complexPropertyBuilder).AddValueObjectConverters(
         validateOnWrite,
         useConstructorForRead,
         configureEnumsAndKeyedValueObjects);

      return complexPropertyBuilder;
   }

   /// <summary>
   /// Adds value converter to all properties that are Smart Enums or keyed Value Objects.
   /// Properties with a value provider are skipped.
   /// </summary>
   /// <param name="complexPropertyBuilder">The complex property builder.</param>
   /// <param name="validateOnWrite">In case of a validatable Smart Enum, ensures that the item is valid before writing it to database.</param>
   /// <param name="useConstructorForRead">For keyed value objects only. Use the constructor instead of the factory method when reading the data from database.</param>
   /// <param name="configureEnumsAndKeyedValueObjects">Action for further configuration of the property.</param>
   /// <returns>The entity type builder for method chaining.</returns>
   public static ComplexPropertyBuilder AddValueObjectConverters(
      this ComplexPropertyBuilder complexPropertyBuilder,
      bool validateOnWrite,
      bool useConstructorForRead = true,
      Action<IMutableProperty>? configureEnumsAndKeyedValueObjects = null)
   {
      configureEnumsAndKeyedValueObjects ??= Empty.Action;

      AddConvertersForComplexProperty(
         complexPropertyBuilder.Metadata,
         validateOnWrite,
         useConstructorForRead,
         null, // Cache for a single entity will be more of a drawback
         configureEnumsAndKeyedValueObjects);

      return complexPropertyBuilder;
   }
#endif

   internal static void AddConvertersToEntity(
      this IMutableEntityType entity,
      bool validateOnWrite,
      bool useConstructorForRead,
      bool addConvertersForOwnedTypes,
      Dictionary<Type, ValueConverter>? converterLookup,
      Action<IMutableProperty> configureEnumsAndKeyedValueObjects)
   {
      AddSmartEnumAndKeyedValueObjects(entity, validateOnWrite, useConstructorForRead, converterLookup, configureEnumsAndKeyedValueObjects);
      AddNonKeyedValueObjectMembers(entity);

      AddConverterForScalarProperties(entity, validateOnWrite, useConstructorForRead, converterLookup, configureEnumsAndKeyedValueObjects);
#if COMPLEX_TYPES
      AddConverterForComplexProperties(entity, validateOnWrite, useConstructorForRead, converterLookup, configureEnumsAndKeyedValueObjects);
#endif
      AddConvertersForNavigations(entity, validateOnWrite, useConstructorForRead, converterLookup, configureEnumsAndKeyedValueObjects);

      if (addConvertersForOwnedTypes)
         AddConvertersForOwnedTypes(entity, validateOnWrite, useConstructorForRead, converterLookup, configureEnumsAndKeyedValueObjects);
   }

   private static void AddSmartEnumAndKeyedValueObjects(
      IMutableEntityType entity,
      bool validateOnWrite,
      bool useConstructorForRead,
      Dictionary<Type, ValueConverter>? converterLookup,
      Action<IMutableProperty> configure)
   {
      foreach (var propertyInfo in entity.ClrType.GetRuntimeProperties())
      {
         // will be handled by "AddConvertersForNavigations"
         if (entity.FindNavigation(propertyInfo) is not null)
            continue;

         AddConverterToNonNavigation(entity, propertyInfo, validateOnWrite, useConstructorForRead, converterLookup, configure);
      }
   }

#if COMPLEX_TYPES
   private static void AddSmartEnumAndKeyedValueObjects(
      IMutableTypeBase entity,
      bool validateOnWrite,
      bool useConstructorForRead,
      Dictionary<Type, ValueConverter>? converterLookup,
      Action<IMutableProperty> configure)
   {
      foreach (var propertyInfo in entity.ClrType.GetRuntimeProperties())
      {
         AddConverterToNonNavigation(entity, propertyInfo, validateOnWrite, useConstructorForRead, converterLookup, configure);
      }
   }
#endif

   private static void AddConverterToNonNavigation(
#if COMPLEX_TYPES
      IMutableTypeBase
#else
      IMutableEntityType
#endif
         entity,
      PropertyInfo propertyInfo,
      bool validateOnWrite,
      bool useConstructorForRead,
      Dictionary<Type, ValueConverter>? converterLookup,
      Action<IMutableProperty> configure)
   {
      if (entity.IsIgnored(propertyInfo.Name))
         return;

      // wil be handled by AddConverterForScalarProperties
      if (entity.FindProperty(propertyInfo) is not null)
         return;

#if COMPLEX_TYPES
#if USE_FIND_COMPLEX_PROPERTY_FIX
      var complexProperty = entity.FindComplexPropertyFix(propertyInfo);
#else
      var complexProperty = entity.FindComplexProperty(propertyInfo);
#endif
      // wil be handled by AddConverterForScalarProperties
      if (complexProperty is not null)
         return;
#endif

      if (!propertyInfo.IsCandidateProperty())
         return;

      var metadata = KeyedValueObjectMetadataLookup.Find(propertyInfo.PropertyType);

      if (metadata is null)
         return;

      var property = entity.AddProperty(propertyInfo);

      SetConverterAndExecuteCallback(validateOnWrite, useConstructorForRead, converterLookup, configure, property, metadata);
   }

   private static void AddNonKeyedValueObjectMembers(
#if COMPLEX_TYPES
      IMutableTypeBase
#else
      IMutableEntityType
#endif
         entity)
   {
      if (!entity.ClrType.TryGetAssignableMembers(out var members))
         return;

      foreach (var member in members)
      {
         if (entity.IsIgnored(member.Name))
            continue;

#if COMPLEX_TYPES
#if USE_FIND_COMPLEX_PROPERTY_FIX
         var complexProperty = entity.FindComplexPropertyFix(member);
#else
         var complexProperty = entity.FindComplexProperty(memberName);
#endif
         if (complexProperty is not null)
            continue;
#endif

         var property = entity.FindProperty(member);

         if (property is null)
            entity.AddProperty(member);
      }
   }

   private static void AddConvertersForNavigations(
      IMutableEntityType entity,
      bool validateOnWrite,
      bool useConstructorForRead,
      Dictionary<Type, ValueConverter>? converterLookup,
      Action<IMutableProperty> configure)
   {
      List<(IMutableNavigation, KeyedValueObjectMetadata)>? navigationsToConvert = null;

      foreach (var navigation in entity.GetNavigations())
      {
         var metadata = KeyedValueObjectMetadataLookup.Find(navigation.ClrType);

         if (metadata is null)
            continue;

         (navigationsToConvert ??= new List<(IMutableNavigation, KeyedValueObjectMetadata)>()).Add((navigation, metadata));
      }

      if (navigationsToConvert is null)
         return;

      var builders = entity.FindEntityBuilder();

      foreach (var navigation in navigationsToConvert)
      {
         var property = FindPropertyBuilder(builders, entity, navigation.Item1.Name);

         SetConverterAndExecuteCallback(validateOnWrite, useConstructorForRead, converterLookup, configure, property.Metadata, navigation.Item2);
      }
   }

   private static void AddConvertersForOwnedTypes(
      IMutableEntityType entity,
      bool validateOnWrite,
      bool useConstructorForRead,
      Dictionary<Type, ValueConverter>? converterLookup,
      Action<IMutableProperty> configure)
   {
      foreach (var navigation in entity.GetNavigations())
      {
         if (!navigation.ForeignKey.IsOwnership || entity != navigation.ForeignKey.PrincipalEntityType)
            continue;

         AddConvertersToEntity(
            navigation.TargetEntityType,
            validateOnWrite,
            useConstructorForRead,
            true,
            converterLookup,
            configure);
      }
   }

   private static void AddConverterForScalarProperties(
#if COMPLEX_TYPES
      IMutableTypeBase
#else
      IMutableEntityType
#endif
         entity,
      bool validateOnWrite,
      bool useConstructorForRead,
      Dictionary<Type, ValueConverter>? converterLookup,
      Action<IMutableProperty> configure)
   {
      foreach (var property in entity.GetProperties())
      {
         var valueConverter = property.GetValueConverter();

         if (valueConverter is not null)
            continue;

         var metadata = KeyedValueObjectMetadataLookup.Find(property.ClrType);

         if (metadata is null)
            continue;

         SetConverterAndExecuteCallback(validateOnWrite, useConstructorForRead, converterLookup, configure, property, metadata);
      }
   }

#if COMPLEX_TYPES
   private static void AddConverterForComplexProperties(
      IMutableTypeBase entity,
      bool validateOnWrite,
      bool useConstructorForRead,
      Dictionary<Type, ValueConverter>? converterLookup,
      Action<IMutableProperty> configureEnumsAndKeyedValueObjects)
   {
      foreach (var complexProperty in entity.GetComplexProperties())
      {
         AddConvertersForComplexProperty(complexProperty, validateOnWrite, useConstructorForRead, converterLookup, configureEnumsAndKeyedValueObjects);
      }
   }

   private static void AddConvertersForComplexProperty(
      IMutableComplexProperty complexProperty,
      bool validateOnWrite,
      bool useConstructorForRead,
      Dictionary<Type, ValueConverter>? converterLookup,
      Action<IMutableProperty> configureEnumsAndKeyedValueObjects)
   {
      AddSmartEnumAndKeyedValueObjects(complexProperty.ComplexType, validateOnWrite, useConstructorForRead, converterLookup, configureEnumsAndKeyedValueObjects);
      AddNonKeyedValueObjectMembers(complexProperty.ComplexType);

      AddConverterForScalarProperties(complexProperty.ComplexType, validateOnWrite, useConstructorForRead, converterLookup, configureEnumsAndKeyedValueObjects);
      AddConverterForComplexProperties(complexProperty.ComplexType, validateOnWrite, useConstructorForRead, converterLookup, configureEnumsAndKeyedValueObjects);
   }
#endif

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
      bool validateOnWrite,
      bool useConstructorForRead,
      Dictionary<Type, ValueConverter>? converterLookup,
      Action<IMutableProperty> configure,
      IMutableProperty property,
      KeyedValueObjectMetadata metadata)
   {
      var valueConverter = GetValueConverter(validateOnWrite, useConstructorForRead, converterLookup, metadata);

      property.SetValueConverter(valueConverter);
      configure(property);
   }

   private static ValueConverter GetValueConverter(
      bool validateOnWrite,
      bool useConstructorForRead,
      Dictionary<Type, ValueConverter>? converterLookup,
      KeyedValueObjectMetadata metadata)
   {
      if (converterLookup is null || !converterLookup.TryGetValue(metadata.Type, out var valueConverter))
      {
         valueConverter = ValueObjectValueConverterFactory.Create(metadata, validateOnWrite, useConstructorForRead);
         converterLookup?.Add(metadata.Type, valueConverter);
      }

      return valueConverter;
   }
}
