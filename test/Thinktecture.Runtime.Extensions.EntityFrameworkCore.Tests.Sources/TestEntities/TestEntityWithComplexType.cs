#if COMPLEX_TYPES
using System;
using Microsoft.EntityFrameworkCore;

namespace Thinktecture.Runtime.Tests.TestEntities;

public class TestEntityWithComplexType
{
   public Guid Id { get; set; }
   public TestComplexType TestComplexType { get; set; }

   public static void Configure(
      ModelBuilder modelBuilder,
      bool registerValueConverters)
   {
      modelBuilder.Entity<TestEntityWithComplexType>(builder =>
      {
         builder.ComplexProperty(e => e.TestComplexType,
                                 b =>
                                 {
                                    b.IsRequired();
                                    b.Property(t => t.TestEnum);
                                 });

         if (registerValueConverters)
            builder.AddValueObjectConverters(true);
      });
   }
}

#endif
