using System.Reflection;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Thinktecture.EntityFrameworkCore.Storage.ValueConversion;
using Thinktecture.Internal;

namespace Thinktecture.EntityFrameworkCore.Conventions;

internal sealed class ThinktectureConventionsPlugin : INavigationAddedConvention, IPropertyAddedConvention, IEntityTypeAddedConvention
{
   private readonly bool _useConstructorForRead;
   private readonly Action<IConventionProperty> _configureEnumsAndKeyedValueObjects;

   public ThinktectureConventionsPlugin(bool useConstructorForRead, Action<IConventionProperty>? configureEnumsAndKeyedValueObjects)
   {
      _useConstructorForRead = useConstructorForRead;
      _configureEnumsAndKeyedValueObjects = configureEnumsAndKeyedValueObjects ?? Empty.Action;
   }

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

         if (MetadataLookup.Find(propertyInfo.PropertyType) is not Metadata.Keyed metadata)
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

#if COMPLEX_TYPES
         var complexProperty = entity.FindComplexProperty(member);

         if (complexProperty is not null)
            continue;
#endif

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

   private void ProcessNavigation(IConventionNavigation navigation)
   {
      var naviType = navigation.ClrType;

      if (MetadataLookup.Find(naviType) is not Metadata.Keyed metadata)
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

      if (MetadataLookup.Find(property.ClrType) is not Metadata.Keyed metadata)
         return;

      SetConverterAndExecuteCallback(property, metadata);
   }

   private void SetConverterAndExecuteCallback(IConventionProperty property, Metadata.Keyed metadata)
   {
      property.SetValueConverter(GetValueConverter(metadata));
      _configureEnumsAndKeyedValueObjects(property);
   }

   private ValueConverter GetValueConverter(Metadata.Keyed metadata)
   {
      return ThinktectureValueConverterFactory.Create(metadata, _useConstructorForRead);
   }
}
