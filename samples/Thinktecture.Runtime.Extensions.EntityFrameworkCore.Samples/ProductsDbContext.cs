using Microsoft.EntityFrameworkCore;

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

         modelBuilder.AddEnumAndValueTypeConverters(true);
      }
   }
}
