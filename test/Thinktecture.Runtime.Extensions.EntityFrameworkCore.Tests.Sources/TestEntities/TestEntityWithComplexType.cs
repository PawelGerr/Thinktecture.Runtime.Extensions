using System;
using Microsoft.EntityFrameworkCore;
using Thinktecture.EntityFrameworkCore;

namespace Thinktecture.Runtime.Tests.TestEntities;

public class TestEntityWithComplexType
{
   public Guid Id { get; set; }
   public TestComplexType TestComplexType { get; set; }

   public static void Configure(
      ModelBuilder modelBuilder,
      ValueConverterRegistration valueConverterRegistration,
      Configuration configuration)
   {
      modelBuilder.Entity<TestEntityWithComplexType>(builder =>
      {
         builder.ComplexProperty(e => e.TestComplexType,
                                 b =>
                                 {
                                    b.IsRequired();
                                    var propertyBuilder = b.Property(t => t.TestEnum);

                                    if (valueConverterRegistration == ValueConverterRegistration.PropertyConfiguration)
                                       propertyBuilder.HasThinktectureValueConverter(configuration);

                                    if (valueConverterRegistration == ValueConverterRegistration.ComplexTypeConfiguration)
                                       b.AddThinktectureValueConverters(configuration);
                                 });

         if (valueConverterRegistration == ValueConverterRegistration.EntityConfiguration)
            builder.AddThinktectureValueConverters(configuration);
      });
   }
}
