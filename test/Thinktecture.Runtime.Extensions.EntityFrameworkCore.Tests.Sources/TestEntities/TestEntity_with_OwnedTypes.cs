using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Thinktecture.Runtime.Tests.TestEnums;
using Thinktecture.Runtime.Tests.TestValueObjects;

// ReSharper disable InconsistentNaming
namespace Thinktecture.Runtime.Tests.TestEntities;

public class TestEntity_with_OwnedTypes
{
   public Guid Id { get; set; }

   public TestEnum TestEnum { get; set; }
   public Boundary Boundary { get; set; }

   public OwnedEntity_Owns_Inline Inline_Inline { get; set; }
   public OwnedEntity_Owns_SeparateOne Inline_SeparateOne { get; set; }
   public OwnedEntity_Owns_SeparateMany Inline_SeparateMany { get; set; }

   public List<OwnedEntity_Owns_Inline> SeparateMany_Inline { get; set; }
   public List<OwnedEntity_Owns_SeparateOne> SeparateMany_SeparateOne { get; set; }
   public List<OwnedEntity_Owns_SeparateMany> SeparateMany_SeparateMany { get; set; }

   public OwnedEntity_Owns_Inline SeparateOne_Inline { get; set; }
   public OwnedEntity_Owns_SeparateOne SeparateOne_SeparateOne { get; set; }
   public OwnedEntity_Owns_SeparateMany SeparateOne_SeparateMany { get; set; }

   public static void Configure(
      ModelBuilder modelBuilder,
      ValueConverterRegistration valueConverterRegistration)
   {
      var configureOnEntityTypeLevel = valueConverterRegistration
                                          is ValueConverterRegistration.EntityConfiguration
                                          or ValueConverterRegistration.ComplexTypeConfiguration;

      modelBuilder.Entity<TestEntity_with_OwnedTypes>(builder =>
      {
         builder.OwnsOne(e => e.Boundary, boundaryBuilder =>
         {
            boundaryBuilder.Property(b => b.Lower);

            if (valueConverterRegistration == ValueConverterRegistration.PropertyConfiguration)
               boundaryBuilder.Property(b => b.Upper);
         });

         if (valueConverterRegistration == ValueConverterRegistration.PropertyConfiguration)
            builder.Property(e => e.TestEnum).HasValueObjectConversion(true);

         builder.OwnsOne(e => e.Inline_Inline,
                         navigationBuilder =>
                         {
                            if (valueConverterRegistration == ValueConverterRegistration.PropertyConfiguration)
                               navigationBuilder.Property(e => e.TestEnum).HasValueObjectConversion(true);

                            navigationBuilder.OwnsOne(
                               e => e.InlineEntity,
                               innerBuilder =>
                               {
                                  if (valueConverterRegistration == ValueConverterRegistration.PropertyConfiguration)
                                     innerBuilder.Property(e => e.TestEnum).HasValueObjectConversion(true);
                               });
                         });

         builder.OwnsOne(e => e.Inline_SeparateOne,
                         navigationBuilder =>
                         {
                            if (valueConverterRegistration == ValueConverterRegistration.PropertyConfiguration)
                               navigationBuilder.Property(e => e.TestEnum).HasValueObjectConversion(true);

                            navigationBuilder.OwnsOne(
                               e => e.SeparateEntity,
                               innerBuilder =>
                               {
                                  innerBuilder.ToTable("InlineEntities_SeparateOne");

                                  if (valueConverterRegistration == ValueConverterRegistration.PropertyConfiguration)
                                     innerBuilder.Property(e => e.TestEnum).HasValueObjectConversion(true);
                               });
                         });

         builder.OwnsOne(e => e.Inline_SeparateMany,
                         navigationBuilder =>
                         {
                            if (valueConverterRegistration == ValueConverterRegistration.PropertyConfiguration)
                               navigationBuilder.Property(e => e.TestEnum).HasValueObjectConversion(true);

                            navigationBuilder.OwnsMany(e => e.SeparateEntities,
                                                       innerBuilder =>
                                                       {
                                                          innerBuilder.ToTable("InlineEntities_SeparateMany");

                                                          if (valueConverterRegistration == ValueConverterRegistration.PropertyConfiguration)
                                                             innerBuilder.Property(e => e.TestEnum).HasValueObjectConversion(true);
                                                       });
                         });

         builder.OwnsMany(e => e.SeparateMany_Inline,
                          navigationBuilder =>
                          {
                             navigationBuilder.ToTable("SeparateEntitiesMany_Inline");

                             if (valueConverterRegistration == ValueConverterRegistration.PropertyConfiguration)
                                navigationBuilder.Property(e => e.TestEnum).HasValueObjectConversion(true);

                             navigationBuilder.OwnsOne(
                                e => e.InlineEntity,
                                innerBuilder =>
                                {
                                   if (valueConverterRegistration == ValueConverterRegistration.PropertyConfiguration)
                                      innerBuilder.Property(e => e.TestEnum).HasValueObjectConversion(true);
                                });
                          });

         builder.OwnsMany(e => e.SeparateMany_SeparateOne,
                          navigationBuilder =>
                          {
                             navigationBuilder.ToTable("SeparateEntitiesMany_SeparateEntitiesOne");

                             if (valueConverterRegistration == ValueConverterRegistration.PropertyConfiguration)
                                navigationBuilder.Property(e => e.TestEnum).HasValueObjectConversion(true);

                             navigationBuilder.OwnsOne(
                                e => e.SeparateEntity,
                                innerBuilder =>
                                {
                                   innerBuilder.ToTable("SeparateEntitiesMany_SeparateEntitiesOne_Inner");

                                   if (valueConverterRegistration == ValueConverterRegistration.PropertyConfiguration)
                                      innerBuilder.Property(e => e.TestEnum).HasValueObjectConversion(true);
                                });
                          });

         builder.OwnsMany(e => e.SeparateMany_SeparateMany,
                          navigationBuilder =>
                          {
                             navigationBuilder.ToTable("SeparateEntitiesMany_SeparateEntitiesMany");

                             if (valueConverterRegistration == ValueConverterRegistration.PropertyConfiguration)
                                navigationBuilder.Property(e => e.TestEnum).HasValueObjectConversion(true);

                             navigationBuilder.OwnsMany(
                                e => e.SeparateEntities,
                                innerBuilder =>
                                {
                                   innerBuilder.ToTable("SeparateEntitiesMany_SeparateEntitiesMany_Inner");

                                   if (valueConverterRegistration == ValueConverterRegistration.PropertyConfiguration)
                                      innerBuilder.Property(e => e.TestEnum).HasValueObjectConversion(true);
                                });
                          });

         builder.OwnsOne(e => e.SeparateOne_Inline,
                         navigationBuilder =>
                         {
                            navigationBuilder.ToTable("SeparateEntitiesOne_Inline");

                            if (valueConverterRegistration == ValueConverterRegistration.PropertyConfiguration)
                               navigationBuilder.Property(e => e.TestEnum).HasValueObjectConversion(true);

                            navigationBuilder.OwnsOne(
                               e => e.InlineEntity,
                               innerBuilder =>
                               {
                                  if (valueConverterRegistration == ValueConverterRegistration.PropertyConfiguration)
                                     innerBuilder.Property(e => e.TestEnum).HasValueObjectConversion(true);
                               });
                         });

         builder.OwnsOne(e => e.SeparateOne_SeparateOne,
                         navigationBuilder =>
                         {
                            navigationBuilder.ToTable("SeparateEntitiesOne_SeparateOne");

                            if (valueConverterRegistration == ValueConverterRegistration.PropertyConfiguration)
                               navigationBuilder.Property(e => e.TestEnum).HasValueObjectConversion(true);

                            navigationBuilder.OwnsOne(
                               e => e.SeparateEntity,
                               innerBuilder =>
                               {
                                  innerBuilder.ToTable("SeparateEntitiesOne_SeparateOne_Inner");

                                  if (valueConverterRegistration == ValueConverterRegistration.PropertyConfiguration)
                                     innerBuilder.Property(e => e.TestEnum).HasValueObjectConversion(true);
                               });
                         });

         builder.OwnsOne(e => e.SeparateOne_SeparateMany,
                         navigationBuilder =>
                         {
                            navigationBuilder.ToTable("SeparateEntitiesOne_SeparateMany");

                            if (valueConverterRegistration == ValueConverterRegistration.PropertyConfiguration)
                               navigationBuilder.Property(e => e.TestEnum).HasValueObjectConversion(true);

                            navigationBuilder.OwnsMany(
                               e => e.SeparateEntities,
                               innerBuilder =>
                               {
                                  innerBuilder.ToTable("SeparateEntitiesOne_SeparateMany_Inner");

                                  if (valueConverterRegistration == ValueConverterRegistration.PropertyConfiguration)
                                     innerBuilder.Property(e => e.TestEnum).HasValueObjectConversion(true);
                               });
                         });

         if (configureOnEntityTypeLevel)
            builder.AddValueObjectConverters(true);
      });
   }
}
