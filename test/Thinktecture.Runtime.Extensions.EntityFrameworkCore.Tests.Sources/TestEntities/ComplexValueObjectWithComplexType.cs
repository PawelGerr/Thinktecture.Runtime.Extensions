using System;
using Microsoft.EntityFrameworkCore;
using Thinktecture.EntityFrameworkCore;

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
      ValueConverterRegistration valueConverterRegistration,
      Configuration configuration)
   {
      modelBuilder.Entity<ComplexValueObjectWithComplexType>(builder =>
      {
         builder.Property(p => p.Id);
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
