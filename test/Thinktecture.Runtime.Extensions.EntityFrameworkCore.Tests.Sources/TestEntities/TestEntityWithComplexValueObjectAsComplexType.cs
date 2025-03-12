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
                                    b.Property(t => t.TestEnum);

                                    if (valueConverterRegistration == ValueConverterRegistration.ComplexTypeConfiguration)
                                       b.AddValueObjectConverters(true);
                                 });

         if (valueConverterRegistration == ValueConverterRegistration.EntityConfiguration)
            builder.AddValueObjectConverters(true);
      });
   }
}
#endif
