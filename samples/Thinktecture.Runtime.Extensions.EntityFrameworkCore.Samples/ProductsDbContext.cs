using Microsoft.EntityFrameworkCore;
using Thinktecture.EntityFrameworkCore.Storage.ValueConversion;
using Thinktecture.EnumLikeClass;

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

         modelBuilder.Entity<Product>()
                     .Property(p => p.Category)
                     .HasConversion(new EnumValueConverter<ProductCategory, string>());
      }
   }
}
