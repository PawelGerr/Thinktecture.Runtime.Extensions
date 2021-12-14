using Microsoft.EntityFrameworkCore;
using Thinktecture.Runtime.Tests.TestEnums;
using Thinktecture.Runtime.Tests.TestValueObjects;

// ReSharper disable InconsistentNaming
#pragma warning disable 8618
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

   public static void Configure(ModelBuilder modelBuilder)
   {
      modelBuilder.Entity<TestEntity_with_OwnedTypes>(builder =>
                                                      {
                                                         builder.OwnsOne(e => e.Boundary, boundaryBuilder => boundaryBuilder.Property(b => b.Lower));

                                                         builder.OwnsOne(e => e.Inline_Inline,
                                                                         navigationBuilder => navigationBuilder.OwnsOne(e => e.InlineEntity));

                                                         builder.OwnsOne(e => e.Inline_SeparateOne,
                                                                         navigationBuilder => navigationBuilder.OwnsOne(e => e.SeparateEntity,
                                                                                                                        innerBuilder => innerBuilder.ToTable("InlineEntities_SeparateOne")));

                                                         builder.OwnsOne(e => e.Inline_SeparateMany,
                                                                         navigationBuilder => navigationBuilder.OwnsMany(e => e.SeparateEntities,
                                                                                                                         innerBuilder => innerBuilder.ToTable("InlineEntities_SeparateMany")));

                                                         builder.OwnsMany(e => e.SeparateMany_Inline,
                                                                          navigationBuilder =>
                                                                          {
                                                                             navigationBuilder.ToTable("SeparateEntitiesMany_Inline");
                                                                             navigationBuilder.OwnsOne(e => e.InlineEntity);
                                                                          });

                                                         builder.OwnsMany(e => e.SeparateMany_SeparateOne,
                                                                          navigationBuilder =>
                                                                          {
                                                                             navigationBuilder.ToTable("SeparateEntitiesMany_SeparateEntitiesOne");
                                                                             navigationBuilder.OwnsOne(e => e.SeparateEntity,
                                                                                                       innerBuilder => innerBuilder.ToTable("SeparateEntitiesMany_SeparateEntitiesOne_Inner"));
                                                                          });

                                                         builder.OwnsMany(e => e.SeparateMany_SeparateMany,
                                                                          navigationBuilder =>
                                                                          {
                                                                             navigationBuilder.ToTable("SeparateEntitiesMany_SeparateEntitiesMany");
                                                                             navigationBuilder.OwnsMany(e => e.SeparateEntities,
                                                                                                        innerBuilder => innerBuilder.ToTable("SeparateEntitiesMany_SeparateEntitiesMany_Inner"));
                                                                          });

                                                         builder.OwnsOne(e => e.SeparateOne_Inline,
                                                                         navigationBuilder =>
                                                                         {
                                                                            navigationBuilder.ToTable("SeparateEntitiesOne_Inline");
                                                                            navigationBuilder.OwnsOne(e => e.InlineEntity);
                                                                         });

                                                         builder.OwnsOne(e => e.SeparateOne_SeparateOne,
                                                                         navigationBuilder =>
                                                                         {
                                                                            navigationBuilder.ToTable("SeparateEntitiesOne_SeparateOne");
                                                                            navigationBuilder.OwnsOne(e => e.SeparateEntity,
                                                                                                      innerBuilder => innerBuilder.ToTable("SeparateEntitiesOne_SeparateOne_Inner"));
                                                                         });

                                                         builder.OwnsOne(e => e.SeparateOne_SeparateMany,
                                                                         navigationBuilder =>
                                                                         {
                                                                            navigationBuilder.ToTable("SeparateEntitiesOne_SeparateMany");
                                                                            navigationBuilder.OwnsMany(e => e.SeparateEntities,
                                                                                                       innerBuilder => innerBuilder.ToTable("SeparateEntitiesOne_SeparateMany_Inner"));
                                                                         });
                                                      });
   }
}
