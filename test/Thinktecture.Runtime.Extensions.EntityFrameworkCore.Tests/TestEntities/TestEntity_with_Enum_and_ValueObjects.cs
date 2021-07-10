using System;
using Microsoft.EntityFrameworkCore;
using Thinktecture.Runtime.Tests.TestEnums;
using Thinktecture.Runtime.Tests.TestValueObjects;

namespace Thinktecture.Runtime.Tests.TestEntities
{
   public class TestEntity_with_Enum_and_ValueObjects
   {
      public Guid Id { get; set; }

      public TestEnum TestEnum { get; set; }
      public IntBasedReferenceValueObject IntBasedReferenceValueObject { get; set; }
      public IntBasedStructValueObject IntBasedStructValueObject { get; set; }
      public StringBasedReferenceValueObject StringBasedReferenceValueObject { get; set; }
      public StringBasedStructValueObject StringBasedStructValueObject { get; set; }
      public Boundary Boundary { get; set; }

      public static void Configure(ModelBuilder modelBuilder)
      {
         modelBuilder.Entity<TestEntity_with_Enum_and_ValueObjects>(builder =>
                                                                    {
                                                                       // struct are not added bey EF by default
                                                                       builder.Property(p => p.StringBasedStructValueObject);
                                                                       builder.Property(p => p.IntBasedStructValueObject);

                                                                       builder.OwnsOne(e => e.Boundary);
                                                                    });
      }
   }
}
