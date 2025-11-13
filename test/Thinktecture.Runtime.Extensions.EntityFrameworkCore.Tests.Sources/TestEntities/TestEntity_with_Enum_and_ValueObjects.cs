using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
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
      ValueConverterRegistration valueConverterRegistration)
   {
      var configureOnEntityTypeLevel = valueConverterRegistration
                                          is ValueConverterRegistration.EntityConfiguration
                                          or ValueConverterRegistration.ComplexTypeConfiguration;

      modelBuilder.Entity<TestEntity_with_Enum_and_ValueObjects>(builder =>
      {
         var primitiveCollectionBuilder = builder.PrimitiveCollection(e => e.CollectionOfIntBasedReferenceValueObject);

         if (valueConverterRegistration == ValueConverterRegistration.PropertyConfiguration)
         {
            builder.Property(e => e.SmartEnum_IntBased).HasThinktectureValueConverter();
            builder.Property(e => e.SmartEnum_StringBased).HasThinktectureValueConverter();
            builder.Property(e => e.TestSmartEnum_CustomError).HasThinktectureValueConverter();

            builder.Property(e => e.IntBasedReferenceValueObject).HasThinktectureValueConverter();
            builder.Property(e => e.IntBasedStructValueObject).HasThinktectureValueConverter();
            builder.Property(e => e.StringBasedReferenceValueObject).HasThinktectureValueConverter();
            builder.Property(e => e.StringBasedStructValueObject).HasThinktectureValueConverter();
            builder.Property(e => e.StringBasedReferenceValueObjectWithCustomError).HasThinktectureValueConverter();

            builder.Property(e => e.IntBasedReferenceValueObjectWitCustomFactoryName).HasThinktectureValueConverter();

            builder.Property(e => e.TestComplexValueObject_ObjectFactory).HasThinktectureValueConverter();
            builder.Property(e => e.TestComplexValueObject_ObjectFactory_and_Constructor).HasThinktectureValueConverter();
            builder.Property(e => e.CustomObject_ObjectFactory).HasThinktectureValueConverter();

            primitiveCollectionBuilder.HasThinktectureValueConverter();
         }

         builder.OwnsOne(e => e.Boundary, navigationBuilder =>
         {
            if (!configureOnEntityTypeLevel)
               navigationBuilder.AddThinktectureValueConverters();
         });
         builder.OwnsOne(e => e.BoundaryWithCustomError, navigationBuilder =>
         {
            if (!configureOnEntityTypeLevel)
               navigationBuilder.AddThinktectureValueConverters();
         });
         builder.OwnsOne(e => e.BoundaryWithCustomFactoryNames, navigationBuilder =>
         {
            if (!configureOnEntityTypeLevel)
               navigationBuilder.AddThinktectureValueConverters();
         });

         if (configureOnEntityTypeLevel)
            builder.AddThinktectureValueConverters();
      });
   }
}
