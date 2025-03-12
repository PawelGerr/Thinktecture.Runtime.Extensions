#if COMPLEX_TYPES
using System;
using Microsoft.EntityFrameworkCore;

namespace Thinktecture.Runtime.Tests.TestEntities;

[ComplexValueObject]
public partial class ComplexValueObjectWithComplexType
{
   public Guid Id { get; }
   public TestComplexType TestComplexType { get; }

   private ComplexValueObjectWithComplexType(Guid id)
   {
      Id = id;
   }

   public static void Configure(
      ModelBuilder modelBuilder,
      ValueConverterRegistration valueConverterRegistration)
   {
      modelBuilder.Entity<ComplexValueObjectWithComplexType>(builder =>
      {
         builder.Property(p => p.Id);
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
