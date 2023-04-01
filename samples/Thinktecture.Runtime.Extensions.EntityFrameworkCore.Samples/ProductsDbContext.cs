using Microsoft.EntityFrameworkCore;

namespace Thinktecture;

public class ProductsDbContext : DbContext
{
   public DbSet<Product> Products => Set<Product>();

   public ProductsDbContext(DbContextOptions<ProductsDbContext> options)
      : base(options)
   {
   }

   protected override void OnModelCreating(ModelBuilder modelBuilder)
   {
      base.OnModelCreating(modelBuilder);

      modelBuilder.Entity<Product>(builder =>
                                   {
                                      builder.HasKey(p => p.Id);

                                      builder.OwnsOne(p => p.Boundary,
                                                      boundaryBuilder =>
                                                      {
                                                         boundaryBuilder.Property(b => b.Lower).HasColumnName("Lower").HasPrecision(18, 2);
                                                         boundaryBuilder.Property(b => b.Upper).HasColumnName("Upper").HasPrecision(18, 2);
                                                      });
                                   });

      // Alternative way to apply ValueConverters to Smart Enums and Value Objects
      // modelBuilder.AddEnumAndValueObjectConverters(validateOnWrite: true,
      //                                              configureEnumsAndKeyedValueObjects: property =>
      //                                                                                  {
      //                                                                                     if (property.ClrType == typeof(ProductType))
      //                                                                                        property.SetMaxLength(20);
      //                                                                                  });
   }
}
