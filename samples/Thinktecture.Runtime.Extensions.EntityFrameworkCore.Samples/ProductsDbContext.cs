using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Thinktecture.Runtime.Extensions.Samples.EnumLikeClass;

namespace Thinktecture
{
	public class ProductsDbContext : DbContext
	{
		public DbSet<Product> Products { get; set; }

		public ProductsDbContext([NotNull] DbContextOptions<ProductsDbContext> options)
			: base(options)
		{
		}

		protected override void OnModelCreating([NotNull] ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<Product>()
			            .Property(p => p.Category)
			            .HasConversion(new EnumValueConverter<ProductCategory, string>());
		}
	}
}
