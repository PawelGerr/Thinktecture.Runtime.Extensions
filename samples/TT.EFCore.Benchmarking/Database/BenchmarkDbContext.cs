using Microsoft.EntityFrameworkCore;

namespace Thinktecture.Database;

public class BenchmarkDbContext : DbContext
{
   public DbSet<Entity_Enum_StringConverter> Entity_Enum_StringConverter { get; set; } = null!;
   public DbSet<Entity_Enum_IntBased> Entity_Enum_IntBased { get; set; } = null!;
   public DbSet<Entity_SmartEnum_Class_StringBased> Entity_SmartEnum_Class_StringBased { get; set; } = null!;
   public DbSet<Entity_SmartEnum_Struct_StringBased> Entity_SmartEnum_Struct_StringBased { get; set; } = null!;
   public DbSet<Entity_SmartEnum_Class_IntBased> Entity_SmartEnum_Class_IntBased { get; set; } = null!;
   public DbSet<Entity_SmartEnum_Struct_IntBased> Entity_SmartEnum_Struct_IntBased { get; set; } = null!;

   public DbSet<Entity_with_ValueObjects> Entity_with_ValueObjects { get; set; } = null!;
   public DbSet<Entity_without_ValueObjects> Entity_without_ValueObjects { get; set; } = null!;

   public BenchmarkDbContext(DbContextOptions<BenchmarkDbContext> options)
      : base(options)
   {
   }

   protected override void OnModelCreating(ModelBuilder modelBuilder)
   {
      modelBuilder.Entity<Entity_Enum_StringConverter>(builder =>
                                                       {
                                                          builder.Property(e => e.Id).ValueGeneratedNever();
                                                          builder.Property(e => e.Enum).HasConversion<string>().HasMaxLength(20);
                                                       });

      modelBuilder.Entity<Entity_Enum_IntBased>(builder =>
                                                {
                                                   builder.Property(e => e.Id).ValueGeneratedNever();
                                                });

      modelBuilder.Entity<Entity_SmartEnum_Class_StringBased>(builder =>
                                                              {
                                                                 builder.Property(e => e.Id).ValueGeneratedNever();
                                                                 builder.Property(e => e.Enum).HasMaxLength(20);
                                                              });

      modelBuilder.Entity<Entity_SmartEnum_Struct_StringBased>(builder =>
                                                               {
                                                                  builder.Property(e => e.Id).ValueGeneratedNever();
                                                                  builder.Property(e => e.Enum).HasMaxLength(20);
                                                               });
      modelBuilder.Entity<Entity_SmartEnum_Class_IntBased>(builder =>
                                                           {
                                                              builder.Property(e => e.Id).ValueGeneratedNever();
                                                           });
      modelBuilder.Entity<Entity_SmartEnum_Struct_IntBased>(builder =>
                                                            {
                                                               builder.Property(e => e.Id).ValueGeneratedNever();
                                                            });

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
   }
}
