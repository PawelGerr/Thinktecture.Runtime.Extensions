using System;
using Microsoft.EntityFrameworkCore;
using Thinktecture.Runtime.Tests.TestEnums;
using Thinktecture.Runtime.Tests.TestValueObjects;

#nullable enable

namespace Thinktecture.Runtime.Tests.TestEntities;

// ReSharper disable InconsistentNaming
public class TestEntity_with_Enum_and_ValueObjects
{
   public Guid Id { get; set; }

   public TestEnum? TestEnum { get; set; }

   public TestSmartEnum_Class_IntBased? TestSmartEnum_Class_IntBased { get; set; }
   public TestSmartEnum_Class_StringBased? TestSmartEnum_Class_StringBased { get; set; }
   public TestSmartEnum_Struct_IntBased_Validatable TestSmartEnum_Struct_IntBased { get; set; }
   public required TestSmartEnum_Struct_StringBased_Validatable TestSmartEnum_Struct_StringBased { get; set; }
   public TestSmartEnum_Struct_StringBased_Validatable? NullableTestSmartEnum_Struct_StringBased { get; set; }
   public TestEnumWithCustomError? TestEnumWithCustomError { get; set; }

   public IntBasedReferenceValueObject? IntBasedReferenceValueObject { get; set; }
   public IntBasedStructValueObject IntBasedStructValueObject { get; set; }
   public StringBasedReferenceValueObject? StringBasedReferenceValueObject { get; set; }
   public required StringBasedStructValueObject StringBasedStructValueObject { get; set; }
   public StringBasedReferenceValueObjectWithCustomError? StringBasedReferenceValueObjectWithCustomError { get; set; }

   public Boundary? Boundary { get; set; }
   public BoundaryWithCustomError? BoundaryWithCustomError { get; set; }
   public BoundaryWithCustomFactoryNames? BoundaryWithCustomFactoryNames { get; set; }
   public IntBasedReferenceValueObjectWithCustomFactoryNames? IntBasedReferenceValueObjectWitCustomFactoryName { get; set; }

   public static void Configure(
      ModelBuilder modelBuilder,
      ValueConverterRegistration valueConverterRegistration)
   {
      var configureOnEntityTypeLevel = valueConverterRegistration
                                          is ValueConverterRegistration.EntityConfiguration
                                          or ValueConverterRegistration.ComplexTypeConfiguration;

      modelBuilder.Entity<TestEntity_with_Enum_and_ValueObjects>(builder =>
      {
         if (valueConverterRegistration == ValueConverterRegistration.PropertyConfiguration)
         {
            builder.Property(e => e.TestEnum).HasValueObjectConversion(true);

            builder.Property(e => e.TestSmartEnum_Class_IntBased).HasValueObjectConversion(true);
            builder.Property(e => e.TestSmartEnum_Class_StringBased).HasValueObjectConversion(true);
            builder.Property(e => e.TestSmartEnum_Struct_IntBased).HasValueObjectConversion(true);
            builder.Property(e => e.TestSmartEnum_Struct_StringBased).HasValueObjectConversion(true);
            builder.Property(e => e.NullableTestSmartEnum_Struct_StringBased).HasValueObjectConversion(true);
            builder.Property(e => e.TestEnumWithCustomError).HasValueObjectConversion(true);

            builder.Property(e => e.IntBasedReferenceValueObject).HasValueObjectConversion(true);
            builder.Property(e => e.IntBasedStructValueObject).HasValueObjectConversion(true);
            builder.Property(e => e.StringBasedReferenceValueObject).HasValueObjectConversion(true);
            builder.Property(e => e.StringBasedStructValueObject).HasValueObjectConversion(true);
            builder.Property(e => e.StringBasedReferenceValueObjectWithCustomError).HasValueObjectConversion(true);

            builder.Property(e => e.IntBasedReferenceValueObjectWitCustomFactoryName).HasValueObjectConversion(true);
         }

         builder.OwnsOne(e => e.Boundary, navigationBuilder =>
         {
            if (!configureOnEntityTypeLevel)
               navigationBuilder.AddValueObjectConverters(true);
         });
         builder.OwnsOne(e => e.BoundaryWithCustomError, navigationBuilder =>
         {
            if (!configureOnEntityTypeLevel)
               navigationBuilder.AddValueObjectConverters(true);
         });
         builder.OwnsOne(e => e.BoundaryWithCustomFactoryNames, navigationBuilder =>
         {
            if (!configureOnEntityTypeLevel)
               navigationBuilder.AddValueObjectConverters(true);
         });

         if (configureOnEntityTypeLevel)
            builder.AddValueObjectConverters(true);
      });
   }
}
