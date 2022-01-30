using System.Reflection;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Thinktecture.EntityFrameworkCore.Storage.ValueConversion;
using Thinktecture.Internal;

#if !NET6_0
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
      AddNonKeyedValueObjectMembers(entityTypeBuilder);
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
      var naviType = navigation.ClrType;

      if (ValueObjectMetadataLookup.Find(naviType) is null)
         return;

      if (!_converterLookup.TryGetValue(naviType, out var valueConverter))
      {
         valueConverter = ValueObjectValueConverterFactory.Create(naviType, _validateOnWrite);
         _converterLookup.Add(naviType, valueConverter);
      }

      var property = navigation.DeclaringEntityType.Builder
                               .Property(naviType, navigation.Name)?
                               .Metadata;

      if (property is null)
         return;

      property.SetValueConverter(valueConverter);

      _configureEnumsAndKeyedValueObjects(property);
   }

   public void ProcessPropertyAdded(IConventionPropertyBuilder propertyBuilder, IConventionContext<IConventionPropertyBuilder> context)
   {
      var property = propertyBuilder.Metadata;
      var valueConverter = property.GetValueConverter();

      if (valueConverter is not null)
         return;

      var propertyType = property.ClrType;

      if (ValueObjectMetadataLookup.Find(propertyType) is null)
         return;

      if (!_converterLookup.TryGetValue(propertyType, out valueConverter))
      {
         valueConverter = ValueObjectValueConverterFactory.Create(propertyType, _validateOnWrite);
         _converterLookup.Add(propertyType, valueConverter);
      }

      propertyBuilder.Metadata.SetValueConverter(valueConverter);
      _configureEnumsAndKeyedValueObjects(property);
   }
}
