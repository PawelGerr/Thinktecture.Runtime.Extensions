using Microsoft.EntityFrameworkCore;

namespace Thinktecture;

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

      // Alternative way to apply ValueConverters to Smart Enums and Value Objects
      // modelBuilder.AddEnumAndValueObjectConverters(true, property =>
      //                                                    {
      //
      //                                                    });
   }
}
