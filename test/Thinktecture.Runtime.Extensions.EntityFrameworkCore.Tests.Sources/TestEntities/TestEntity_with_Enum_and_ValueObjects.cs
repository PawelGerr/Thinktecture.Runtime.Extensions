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
   public TestSmartEnum_Struct_IntBased TestSmartEnum_Struct_IntBased { get; set; }
   public TestSmartEnum_Struct_StringBased TestSmartEnum_Struct_StringBased { get; set; }
   public TestSmartEnum_Struct_StringBased? NullableTestSmartEnum_Struct_StringBased { get; set; }
   public TestEnumWithCustomError? TestEnumWithCustomError { get; set; }

   public IntBasedReferenceValueObject? IntBasedReferenceValueObject { get; set; }
   public IntBasedStructValueObject IntBasedStructValueObject { get; set; }
   public StringBasedReferenceValueObject? StringBasedReferenceValueObject { get; set; }
   public StringBasedStructValueObject StringBasedStructValueObject { get; set; }
   public StringBasedReferenceValueObjectWithCustomError? StringBasedReferenceValueObjectWithCustomError { get; set; }

   public Boundary? Boundary { get; set; }
   public BoundaryWithCustomError? BoundaryWithCustomError { get; set; }
   public BoundaryWithCustomFactoryNames? BoundaryWithCustomFactoryNames { get; set; }
   public IntBasedReferenceValueObjectWithCustomFactoryNames? IntBasedReferenceValueObjectWitCustomFactoryName { get; set; }

   public static void Configure(
      ModelBuilder modelBuilder,
      bool registerValueConverters)
   {
      modelBuilder.Entity<TestEntity_with_Enum_and_ValueObjects>(builder =>
      {
         builder.OwnsOne(e => e.Boundary, navigationBuilder => navigationBuilder.AddValueObjectConverters(true));
         builder.OwnsOne(e => e.BoundaryWithCustomError, navigationBuilder => navigationBuilder.AddValueObjectConverters(true));
         builder.OwnsOne(e => e.BoundaryWithCustomFactoryNames, navigationBuilder => navigationBuilder.AddValueObjectConverters(true));

         if (registerValueConverters)
            builder.AddValueObjectConverters(true);
      });
   }
}
