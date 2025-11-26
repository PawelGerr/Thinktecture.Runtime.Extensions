using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Thinktecture.EntityFrameworkCore;
using Thinktecture.Runtime.Tests.TestEnums;
using Thinktecture.Runtime.Tests.TestValueObjects;

#nullable enable

namespace Thinktecture.Runtime.Tests.TestEntities;

// ReSharper disable InconsistentNaming
public class TestEntity_with_Enum_and_ValueObjects
{
   public Guid Id { get; set; }

   public SmartEnum_IntBased? SmartEnum_IntBased { get; set; }
   public SmartEnum_StringBased? SmartEnum_StringBased { get; set; }
   public TestSmartEnum_CustomError? TestSmartEnum_CustomError { get; set; }

   public IntBasedReferenceValueObject? IntBasedReferenceValueObject { get; set; }
   public IntBasedStructValueObject IntBasedStructValueObject { get; set; }
   public StringBasedReferenceValueObject? StringBasedReferenceValueObject { get; set; }
   public required StringBasedStructValueObject StringBasedStructValueObject { get; set; }
   public StringBasedReferenceValueObjectWithCustomError? StringBasedReferenceValueObjectWithCustomError { get; set; }
   public List<IntBasedReferenceValueObject> CollectionOfIntBasedReferenceValueObject { get; set; } = [];
   public Boundary? Boundary { get; set; }
   public BoundaryWithCustomError? BoundaryWithCustomError { get; set; }
   public BoundaryWithCustomFactoryNames? BoundaryWithCustomFactoryNames { get; set; }
   public IntBasedReferenceValueObjectWithCustomFactoryNames? IntBasedReferenceValueObjectWitCustomFactoryName { get; set; }

   public TestComplexValueObject_ObjectFactory? TestComplexValueObject_ObjectFactory { get; set; }
   public required TestComplexValueObject_ObjectFactory_and_Constructor TestComplexValueObject_ObjectFactory_and_Constructor { get; set; }
   public CustomObject_ObjectFactory? CustomObject_ObjectFactory { get; set; }

   public static void Configure(
      ModelBuilder modelBuilder,
      ValueConverterRegistration valueConverterRegistration,
      Configuration configuration)
   {
      var configureOnEntityTypeLevel = valueConverterRegistration
                                          is ValueConverterRegistration.EntityConfiguration
                                          or ValueConverterRegistration.ComplexTypeConfiguration;

      modelBuilder.Entity<TestEntity_with_Enum_and_ValueObjects>(builder =>
      {
         var primitiveCollectionBuilder = builder.PrimitiveCollection(e => e.CollectionOfIntBasedReferenceValueObject);

         if (valueConverterRegistration == ValueConverterRegistration.PropertyConfiguration)
         {
            builder.Property(e => e.SmartEnum_IntBased).HasThinktectureValueConverter(configuration);
            builder.Property(e => e.SmartEnum_StringBased).HasThinktectureValueConverter(configuration);
            builder.Property(e => e.TestSmartEnum_CustomError).HasThinktectureValueConverter(configuration);

            builder.Property(e => e.IntBasedReferenceValueObject).HasThinktectureValueConverter(configuration);
            builder.Property(e => e.IntBasedStructValueObject).HasThinktectureValueConverter(configuration);
            builder.Property(e => e.StringBasedReferenceValueObject).HasThinktectureValueConverter(configuration);
            builder.Property(e => e.StringBasedStructValueObject).HasThinktectureValueConverter(configuration);
            builder.Property(e => e.StringBasedReferenceValueObjectWithCustomError).HasThinktectureValueConverter(configuration);

            builder.Property(e => e.IntBasedReferenceValueObjectWitCustomFactoryName).HasThinktectureValueConverter(configuration);

            builder.Property(e => e.TestComplexValueObject_ObjectFactory).HasThinktectureValueConverter(configuration);
            builder.Property(e => e.TestComplexValueObject_ObjectFactory_and_Constructor).HasThinktectureValueConverter(configuration);
            builder.Property(e => e.CustomObject_ObjectFactory).HasThinktectureValueConverter(configuration);

            primitiveCollectionBuilder.HasThinktectureValueConverter(configuration);
         }

         builder.OwnsOne(e => e.Boundary, navigationBuilder =>
         {
            if (!configureOnEntityTypeLevel)
               navigationBuilder.AddThinktectureValueConverters(configuration);
         });
         builder.OwnsOne(e => e.BoundaryWithCustomError, navigationBuilder =>
         {
            if (!configureOnEntityTypeLevel)
               navigationBuilder.AddThinktectureValueConverters(configuration);
         });
         builder.OwnsOne(e => e.BoundaryWithCustomFactoryNames, navigationBuilder =>
         {
            if (!configureOnEntityTypeLevel)
               navigationBuilder.AddThinktectureValueConverters(configuration);
         });

         if (configureOnEntityTypeLevel)
            builder.AddThinktectureValueConverters(configuration);
      });
   }
}
