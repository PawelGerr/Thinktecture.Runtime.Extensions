using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Thinktecture.EntityFrameworkCore.Storage.ValueConversion;
using Thinktecture.Internal;

namespace Thinktecture;

/// <summary>
/// Extensions for <see cref="ModelBuilder"/>.
/// </summary>
public static class ModelBuilderExtensions
{
   /// <summary>
   /// Adds value converter to all properties implementing <see cref="IEnum{TKey}"/>/<see cref="IValidatableEnum{TKey}"/>
   /// and having the <see cref="ValueObjectAttribute"/>.
   /// Properties with a value provider are skipped.
   /// </summary>
   /// <param name="modelBuilder">EF model builder.</param>
   /// <param name="validateOnWrite">In case of an <see cref="IValidatableEnum{TKey}"/>, ensures that the item is valid before writing it to database.</param>
   /// <param name="configureEnumsAndKeyedValueObjects">Action for further configuration of the property.</param>
   /// <exception cref="ArgumentNullException">If <paramref name="modelBuilder"/> is <c>null</c>.</exception>
   public static void AddEnumAndValueObjectConverters(
      this ModelBuilder modelBuilder,
      bool validateOnWrite,
      Action<IMutableProperty>? configureEnumsAndKeyedValueObjects = null)
   {
      if (modelBuilder is null)
         throw new ArgumentNullException(nameof(modelBuilder));

      configureEnumsAndKeyedValueObjects ??= Empty.Action;
      var converterLookup = new Dictionary<Type, ValueConverter>();

      foreach (var entity in modelBuilder.Model.GetEntityTypes())
      {
         AddSmartEnumAndKeyedValueObjects(validateOnWrite, converterLookup, configureEnumsAndKeyedValueObjects, entity);
         AddNonKeyedValueObjectMembers(entity);

         AddConverterForScalarProperties(entity, validateOnWrite, converterLookup, configureEnumsAndKeyedValueObjects);
         AddConvertersForNavigations(entity, modelBuilder, validateOnWrite, converterLookup, configureEnumsAndKeyedValueObjects);
      }
   }

   private static void AddSmartEnumAndKeyedValueObjects(
      bool validateOnWrite,
      Dictionary<Type, ValueConverter> converterLookup,
      Action<IMutableProperty> configure,
      IMutableEntityType entity)
   {
      foreach (var propertyInfo in entity.ClrType.GetRuntimeProperties())
      {
         // will be handled by "AddConvertersForNavigations"
         if (entity.FindNavigation(propertyInfo) is not null)
            continue;

         // wil be handled by AddConverterForScalarProperties
         if (entity.FindProperty(propertyInfo) is not null)
            continue;

         if (!propertyInfo.IsCandidateProperty())
            continue;

         var propertyType = propertyInfo.PropertyType;
         var metadata = ValueObjectMetadataLookup.Find(propertyType);

         if (metadata is null)
            continue;

         var property = entity.AddProperty(propertyInfo);

         SetConverterAndExecuteCallback(validateOnWrite, converterLookup, configure, property);
      }
   }

   private static void AddNonKeyedValueObjectMembers(IMutableEntityType entity)
   {
      if (entity.ClrType.GetCustomAttribute<KeyedValueObjectAttribute>() is not null)
         return;

      var ctorAttr = entity.ClrType.GetCustomAttribute<ValueObjectConstructorAttribute>();

      if (ctorAttr is null || ctorAttr.Members.Length == 0)
         return;

      foreach (var memberName in ctorAttr.Members)
      {
         var property = entity.FindProperty(memberName);

         if (property is null)
            entity.AddProperty(memberName);
      }
   }

   private static void AddConvertersForNavigations(
      IMutableEntityType entity,
      ModelBuilder modelBuilder,
      bool validateOnWrite,
      Dictionary<Type, ValueConverter> converterLookup,
      Action<IMutableProperty> configure)
   {
      List<IMutableNavigation>? navigationsToConvert = null;

      foreach (var navigation in entity.GetNavigations())
      {
         if (ValueObjectMetadataLookup.Find(navigation.ClrType) is not null)
            (navigationsToConvert ??= new List<IMutableNavigation>()).Add(navigation);
      }

      if (navigationsToConvert is null)
         return;

      var builders = modelBuilder.FindEntityBuilder(entity);

      foreach (var navigation in navigationsToConvert)
      {
         var property = FindPropertyBuilder(builders, entity, navigation.Name);

         SetConverterAndExecuteCallback(validateOnWrite, converterLookup, configure, property.Metadata);
      }
   }

   private static void AddConverterForScalarProperties(
      IMutableEntityType entity,
      bool validateOnWrite,
      Dictionary<Type, ValueConverter> converterLookup,
      Action<IMutableProperty> configure)
   {
      foreach (var property in entity.GetProperties())
      {
         var valueConverter = property.GetValueConverter();

         if (valueConverter is not null)
            continue;

         var propertyType = property.ClrType;

         if (ValueObjectMetadataLookup.Find(propertyType) is null)
            continue;

         SetConverterAndExecuteCallback(validateOnWrite, converterLookup, configure, property);
      }
   }

   private static PropertyBuilder FindPropertyBuilder((EntityTypeBuilder?, OwnedNavigationBuilder?) builders, IMutableEntityType entityType, string propertyName)
   {
      return builders.Item1?.Property(propertyName)
             ?? builders.Item2?.Property(propertyName)
             ?? throw new Exception($"Property '{propertyName}' not found in the entity '{entityType.Name}'.");
   }

   private static (EntityTypeBuilder?, OwnedNavigationBuilder?) FindEntityBuilder(this ModelBuilder modelBuilder, IMutableEntityType entityType)
   {
      if (!entityType.IsOwned())
         return (modelBuilder.Entity(entityType.Name), null);

      var ownership = entityType.FindOwnership() ?? throw new Exception($"Ownership of the owned entity '{entityType.Name}' not found.");
      var navigation = ownership.GetNavigation(false)?.Name ?? throw new Exception($"Navigation of the owned entity '{entityType.Name}' not found.");
      var (entityTypeBuilder, ownedNavigationBuilder) = modelBuilder.FindEntityBuilder(ownership.PrincipalEntityType);

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
      Dictionary<Type, ValueConverter> converterLookup,
      Action<IMutableProperty> configure,
      IMutableProperty property)
   {
      var valueConverter = GetValueConverter(validateOnWrite, converterLookup, property.ClrType);

      property.SetValueConverter(valueConverter);
      configure(property);
   }

   private static ValueConverter GetValueConverter(bool validateOnWrite, Dictionary<Type, ValueConverter> converterLookup, Type naviType)
   {
      if (!converterLookup.TryGetValue(naviType, out var valueConverter))
      {
         valueConverter = ValueObjectValueConverterFactory.Create(naviType, validateOnWrite);
         converterLookup.Add(naviType, valueConverter);
      }

      return valueConverter;
   }
}
