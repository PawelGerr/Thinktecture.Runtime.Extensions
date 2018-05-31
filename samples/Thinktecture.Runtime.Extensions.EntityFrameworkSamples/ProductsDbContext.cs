using Microsoft.EntityFrameworkCore;
using Thinktecture.Runtime.Extensions.Samples.EnumLikeClass;

namespace Thinktecture.Runtime.Extensions.EntityFrameworkSamples
{
	public class ProductsDbContext : DbContext
	{
		public DbSet<Product> Products { get; set; }

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
