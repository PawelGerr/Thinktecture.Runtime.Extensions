using System.Reflection;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Thinktecture.EntityFrameworkCore.Storage.ValueConversion;

#if EFCORE5
using Microsoft.EntityFrameworkCore;
#endif

namespace Thinktecture.EntityFrameworkCore.Conventions;

internal sealed class ValueObjectConventionPlugin : INavigationAddedConvention, IPropertyAddedConvention, IEntityTypeAddedConvention
{
   private readonly bool _validateOnWrite;
   private readonly bool _useConstructorForRead;
   private readonly Action<IConventionProperty> _configureEnumsAndKeyedValueObjects;
   private readonly Dictionary<Type, ValueConverter> _converterLookup;

   public ValueObjectConventionPlugin(bool validateOnWrite, bool useConstructorForRead, Action<IConventionProperty>? configureEnumsAndKeyedValueObjects)
   {
      _validateOnWrite = validateOnWrite;
      _useConstructorForRead = useConstructorForRead;
      _configureEnumsAndKeyedValueObjects = configureEnumsAndKeyedValueObjects ?? (static _ =>
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

         if (!typeof(IKeyedValueObject).IsAssignableFrom(propertyInfo.PropertyType))
            continue;

         property = entity.AddProperty(propertyInfo);

         if (property is not null)
            SetConverterAndExecuteCallback(property);
      }
   }

   private static void AddNonKeyedValueObjectMembers(IConventionEntityTypeBuilder entityTypeBuilder)
   {
      var entity = entityTypeBuilder.Metadata;

      if (!entity.ClrType.TryGetAssignableMembers(out var members))
         return;

      foreach (var memberName in members)
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

      if (!typeof(IKeyedValueObject).IsAssignableFrom(naviType))
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

      if (!typeof(IKeyedValueObject).IsAssignableFrom(property.ClrType))
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

      valueConverter = ValueObjectValueConverterFactory.Create(type, _validateOnWrite, _useConstructorForRead);
      _converterLookup.Add(type, valueConverter);

      return valueConverter;
   }
}
