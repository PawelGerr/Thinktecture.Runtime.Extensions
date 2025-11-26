using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Thinktecture.EntityFrameworkCore;

namespace Thinktecture.Runtime.Tests.TestEntities;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public class TestEntity_with_Types_having_ObjectFactories
{
   public Guid Id { get; set; }
   public TestComplexValueObject_ObjectFactory TestComplexValueObject_ObjectFactory { get; set; }
   public TestComplexValueObject_ObjectFactory_and_Constructor TestComplexValueObject_ObjectFactory_and_Constructor { get; set; }
   public CustomObject_ObjectFactory CustomObject_ObjectFactory { get; set; }

   public static void Configure(
      ModelBuilder modelBuilder,
      ValueConverterRegistration valueConverterRegistration,
      Configuration configuration)
   {
      var configureOnEntityTypeLevel = valueConverterRegistration
                                          is ValueConverterRegistration.EntityConfiguration
                                          or ValueConverterRegistration.ComplexTypeConfiguration;

      modelBuilder.Entity<TestEntity_with_Types_having_ObjectFactories>(builder =>
      {
         if (valueConverterRegistration == ValueConverterRegistration.PropertyConfiguration)
         {
            builder.Property(e => e.TestComplexValueObject_ObjectFactory).HasThinktectureValueConverter(configuration);
            builder.Property(e => e.TestComplexValueObject_ObjectFactory_and_Constructor).HasThinktectureValueConverter(configuration);
            builder.Property(e => e.CustomObject_ObjectFactory).HasThinktectureValueConverter(configuration);
         }

         if (configureOnEntityTypeLevel)
            builder.AddThinktectureValueConverters(configuration);
      });
   }
}
