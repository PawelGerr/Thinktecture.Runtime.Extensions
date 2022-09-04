using System.Reflection;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Thinktecture.EntityFrameworkCore.Storage.ValueConversion;
using Thinktecture.Internal;

#if EFCORE5
using Microsoft.EntityFrameworkCore;
#endif

namespace Thinktecture.EntityFrameworkCore.Conventions;

internal class ValueObjectConventionPlugin : INavigationAddedConvention, IPropertyAddedConvention, IEntityTypeAddedConvention
{
   private readonly bool _validateOnWrite;
   private readonly Action<IConventionProperty> _configureEnumsAndKeyedValueObjects;
   private readonly Dictionary<Type, ValueConverter> _converterLookup;

   public ValueObjectConventionPlugin(bool validateOnWrite, Action<IConventionProperty>? configureEnumsAndKeyedValueObjects)
   {
      _validateOnWrite = validateOnWrite;
      _configureEnumsAndKeyedValueObjects = configureEnumsAndKeyedValueObjects ?? (_ =>
                                                                                   {
                                                                                   });
      _converterLookup = new Dictionary<Type, ValueConverter>();
   }

   public void ProcessEntityTypeAdded(IConventionEntityTypeBuilder entityTypeBuilder, IConventionContext<IConventionEntityTypeBuilder> context)
   {
      if (entityTypeBuilder.Metadata.ClrType is null)
         return;

      AddSmartEnumAndKeyedValueObjects(entityTypeBuilder);
      AddNonKeyedValueObjectMembers(entityTypeBuilder);
   }

   private void AddSmartEnumAndKeyedValueObjects(IConventionEntityTypeBuilder entityTypeBuilder)
   {
      var entity = entityTypeBuilder.Metadata;

      foreach (var propertyInfo in entity.ClrType.GetRuntimeProperties())
      {
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

         var propertyType = propertyInfo.PropertyType;
         var metadata = ValueObjectMetadataLookup.Find(propertyType);

         if (metadata is null)
            continue;

         property = entity.AddProperty(propertyInfo);

         if (property is not null)
            SetConverterAndExecuteCallback(property);
      }
   }

   private static void AddNonKeyedValueObjectMembers(IConventionEntityTypeBuilder entityTypeBuilder)
   {
      var entity = entityTypeBuilder.Metadata;

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

   public void ProcessNavigationAdded(IConventionNavigationBuilder navigationBuilder, IConventionContext<IConventionNavigationBuilder> context)
   {
      var navigation = navigationBuilder.Metadata;
      ProcessNavigation(navigation);
   }

   public void ProcessPropertyAdded(IConventionPropertyBuilder propertyBuilder, IConventionContext<IConventionPropertyBuilder> context)
   {
      var property = propertyBuilder.Metadata;
      ProcessProperty(property);
   }

   private void ProcessNavigation(IConventionNavigation navigation)
   {
      var naviType = navigation.ClrType;

      if (ValueObjectMetadataLookup.Find(naviType) is null)
         return;

      var property = navigation.DeclaringEntityType.Builder
                               .Property(naviType, navigation.Name)?
                               .Metadata;

      if (property is null)
         return;

      SetConverterAndExecuteCallback(property);
   }

   private void ProcessProperty(IConventionProperty property)
   {
      var valueConverter = property.GetValueConverter();

      if (valueConverter is not null)
         return;

      if (ValueObjectMetadataLookup.Find(property.ClrType) is null)
         return;

      SetConverterAndExecuteCallback(property);
   }

   private void SetConverterAndExecuteCallback(IConventionProperty property)
   {
      property.SetValueConverter(GetValueConverter(property.ClrType));
      _configureEnumsAndKeyedValueObjects(property);
   }

   private ValueConverter GetValueConverter(Type type)
   {
      if (_converterLookup.TryGetValue(type, out var valueConverter))
         return valueConverter;

      valueConverter = ValueObjectValueConverterFactory.Create(type, _validateOnWrite);
      _converterLookup.Add(type, valueConverter);

      return valueConverter;
   }
}
