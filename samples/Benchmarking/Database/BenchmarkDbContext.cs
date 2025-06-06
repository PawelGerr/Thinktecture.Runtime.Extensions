using Microsoft.EntityFrameworkCore;

namespace Thinktecture.Database;

// ReSharper disable InconsistentNaming
public class BenchmarkDbContext : DbContext
{
   public DbSet<Entity_RegularEnum_StringConverter> Entity_RegularEnum_StringConverter { get; set; } = null!;
   public DbSet<Entity_RegularEnum_IntBased> Entity_RegularEnum_IntBased { get; set; } = null!;
   public DbSet<Entity_SmartEnum_StringBased> Entity_SmartEnum_StringBased { get; set; } = null!;
   public DbSet<Entity_SmartEnum_IntBased> Entity_SmartEnum_IntBased { get; set; } = null!;

   public DbSet<Entity_with_ValueObjects> Entity_with_ValueObjects { get; set; } = null!;
   public DbSet<Entity_without_ValueObjects> Entity_without_ValueObjects { get; set; } = null!;
   public DbSet<Entity_with_StructValueObjects> Entity_with_StructValueObjects { get; set; } = null!;

   public BenchmarkDbContext(DbContextOptions<BenchmarkDbContext> options)
      : base(options)
   {
   }

   protected override void OnModelCreating(ModelBuilder modelBuilder)
   {
      modelBuilder.Entity<Entity_RegularEnum_StringConverter>(builder =>
      {
         builder.Property(e => e.Id).ValueGeneratedNever();
         builder.Property(e => e.Enum).HasConversion<string>().HasMaxLength(20);
      });

      modelBuilder.Entity<Entity_RegularEnum_IntBased>(builder => builder.Property(e => e.Id).ValueGeneratedNever());

      modelBuilder.Entity<Entity_SmartEnum_StringBased>(builder =>
      {
         builder.Property(e => e.Id).ValueGeneratedNever();
         builder.Property(e => e.Enum).HasMaxLength(20);
      });

      modelBuilder.Entity<Entity_SmartEnum_IntBased>(builder => builder.Property(e => e.Id).ValueGeneratedNever());

      modelBuilder.Entity<Entity_with_ValueObjects>(builder =>
      {
         builder.Property(e => e.Id).ValueGeneratedNever();
         builder.Property(e => e.Name).HasMaxLength(100);
         builder.Property(e => e.Description).HasMaxLength(200);
      });

      modelBuilder.Entity<Entity_without_ValueObjects>(builder =>
      {
         builder.Property(e => e.Id).ValueGeneratedNever();
         builder.Property(e => e.Name).HasMaxLength(100);
         builder.Property(e => e.Description).HasMaxLength(200);
      });

      modelBuilder.Entity<Entity_with_StructValueObjects>(builder =>
      {
         builder.Property(e => e.Id).ValueGeneratedNever();
         builder.Property(e => e.Name).HasMaxLength(100);
         builder.Property(e => e.Description).HasMaxLength(200);
      });
   }
}
