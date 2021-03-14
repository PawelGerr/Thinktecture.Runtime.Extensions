using System;
using Microsoft.EntityFrameworkCore;
using Thinktecture.Runtime.Tests.TestEnums;
using Thinktecture.Runtime.Tests.TestValueTypes;

namespace Thinktecture.Runtime.Tests.TestEntities
{
   public class TestEntity_with_Enum_and_ValueTypes
   {
      public Guid Id { get; set; }

      public TestEnum TestEnum { get; set; }
      public IntBasedReferenceValueType IntBasedReferenceValueType { get; set; }
      public IntBasedStructValueType IntBasedStructValueType { get; set; }
      public StringBasedReferenceValueType StringBasedReferenceValueType { get; set; }
      public StringBasedStructValueType StringBasedStructValueType { get; set; }
      public Boundary Boundary { get; set; }

      public static void Configure(ModelBuilder modelBuilder)
      {
         modelBuilder.Entity<TestEntity_with_Enum_and_ValueTypes>(builder =>
                                                                  {
                                                                     // struct are not added bey EF by default
                                                                     builder.Property(p => p.StringBasedStructValueType);
                                                                     builder.Property(p => p.IntBasedStructValueType);

                                                                     builder.OwnsOne(e => e.Boundary);
                                                                  });
      }
   }
}
