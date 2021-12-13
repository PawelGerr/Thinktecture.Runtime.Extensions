using System.Linq;
using Microsoft.EntityFrameworkCore;
using Thinktecture.SmartEnums;

namespace Thinktecture
{
   public class ProductsDbContext : DbContext
   {
#nullable disable
      public DbSet<Product> Products { get; set; }
#nullable enable

      public ProductsDbContext(DbContextOptions<ProductsDbContext> options)
         : base(options)
      {
      }

      protected override void OnModelCreating(ModelBuilder modelBuilder)
      {
         base.OnModelCreating(modelBuilder);

         modelBuilder.Entity<Product>(builder =>
                                      {
                                         builder.OwnsOne(p => p.Boundary,
                                                         boundaryBuilder =>
                                                         {
                                                            boundaryBuilder.Property(b => b.Lower).HasColumnName("Lower").HasPrecision(18, 2);
                                                            boundaryBuilder.Property(b => b.Upper).HasColumnName("Upper").HasPrecision(18, 2);
                                                         });
                                      });

         modelBuilder.AddEnumAndValueObjectConverters(true, property =>
                                                            {
                                                               if (property.ClrType == typeof(SpecialProductType))
                                                               {
                                                                  var maxLength = SpecialProductType.Items.Max(i => i.Key.Length);
                                                                  property.SetMaxLength(RoundUp(maxLength));
                                                               }
                                                            });
      }

      private static int RoundUp(int value)
      {
         return value + (10 - value % 10);
      }
   }
}
