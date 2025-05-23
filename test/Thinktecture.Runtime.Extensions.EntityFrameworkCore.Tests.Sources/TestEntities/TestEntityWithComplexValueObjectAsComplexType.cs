#if COMPLEX_TYPES
using System;
using Microsoft.EntityFrameworkCore;

namespace Thinktecture.Runtime.Tests.TestEntities;

public class TestEntityWithComplexValueObjectAsComplexType
{
   public Guid Id { get; set; }
   public TestComplexValueObject TestComplexType { get; set; }

   public static void Configure(
      ModelBuilder modelBuilder,
      ValueConverterRegistration valueConverterRegistration)
   {
      modelBuilder.Entity<TestEntityWithComplexValueObjectAsComplexType>(builder =>
      {
         builder.ComplexProperty(e => e.TestComplexType,
                                 b =>
                                 {
                                    b.IsRequired();
                                    var propertyBuilder = b.Property(t => t.TestEnum);

                                    if (valueConverterRegistration == ValueConverterRegistration.PropertyConfiguration)
                                       propertyBuilder.HasThinktectureValueConverter(true);

                                    if (valueConverterRegistration == ValueConverterRegistration.ComplexTypeConfiguration)
                                       b.AddThinktectureValueConverters(true);
                                 });

         if (valueConverterRegistration == ValueConverterRegistration.EntityConfiguration)
            builder.AddThinktectureValueConverters(true);
      });
   }
}
#endif
