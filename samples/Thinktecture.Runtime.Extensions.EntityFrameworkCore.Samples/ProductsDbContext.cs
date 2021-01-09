using Microsoft.EntityFrameworkCore;
using Thinktecture.EntityFrameworkCore.Storage.ValueConversion;
using Thinktecture.EnumLikeClasses;
using Thinktecture.ValueTypes;

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
                                         builder.Property(p => p.Category)
                                                .HasConversion(ValueTypeValueConverterFactory.Create<ProductCategory, string>(true));

                                         builder.Property(p => p.Name)
                                                .HasConversion(ValueTypeValueConverterFactory.Create<ProductName, string>());
                                      });
      }
   }
}
