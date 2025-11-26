using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Thinktecture.EntityFrameworkCore.Conventions;

namespace Thinktecture.EntityFrameworkCore.Infrastructure;

internal sealed class ThinktectureDbContextOptionsExtension : IDbContextOptionsExtension
{
   public DbContextOptionsExtensionInfo Info => field ??= new ThinktectureDbContextOptionsExtensionInfo(this);
   public ThinktectureValueConverterSettings Settings { get; private set; } = new(true, Configuration.Default, null);

   public ThinktectureDbContextOptionsExtension UseThinktectureValueConverters(
      Configuration configuration)
   {
      Settings = new ThinktectureValueConverterSettings(true, configuration, null);

      return this;
   }

   [Obsolete("Use UseThinktectureValueConverters(Configuration) instead.")]
   public ThinktectureDbContextOptionsExtension UseThinktectureValueConverters(
      bool useThinktectureConverters,
      bool useConstructorForRead = true,
      Action<IConventionProperty>? configureEnumsAndKeyedValueObjects = null)
   {
      var configuration = new Configuration
                          {
                             UseConstructorForRead = useConstructorForRead
                          };
      Settings = new ThinktectureValueConverterSettings(
         useThinktectureConverters,
         configuration,
         configureEnumsAndKeyedValueObjects);

      return this;
   }

   public void ApplyServices(IServiceCollection services)
   {
      services.TryAddSingleton<ThinktectureDbContextOptionsExtensionOptions>();
      services.AddSingleton<ISingletonOptions>(provider => provider.GetRequiredService<ThinktectureDbContextOptionsExtensionOptions>());

      services.Add(ServiceDescriptor.Describe(typeof(IConventionSetPlugin), typeof(ThinktectureConventionSetPlugin), GetLifetime<IConventionSetPlugin>()));
   }

   public void Validate(IDbContextOptions options)
   {
   }

   [SuppressMessage("Usage", "EF1001", MessageId = "Internal EF Core API usage.")]
   private static ServiceLifetime GetLifetime<TService>()
   {
      var serviceType = typeof(TService);

      if (EntityFrameworkRelationalServicesBuilder.RelationalServices.TryGetValue(serviceType, out var serviceCharacteristics) ||
          EntityFrameworkServicesBuilder.CoreServices.TryGetValue(serviceType, out serviceCharacteristics))
         return serviceCharacteristics.Lifetime;

      throw new InvalidOperationException($"No service characteristics for service '{serviceType.Name}' found.");
   }

   private sealed class ThinktectureDbContextOptionsExtensionInfo : DbContextOptionsExtensionInfo
   {
      private readonly ThinktectureDbContextOptionsExtension _extension;
      public override bool IsDatabaseProvider => false;

      public override string LogFragment => field ??= CreateLogFragment();

      private string CreateLogFragment()
      {
         if (!_extension.Settings.IsEnabled)
            return String.Empty;

         return $"ThinktectureValueConverters(IsEnabled={_extension.Settings.IsEnabled}, UseConstructorForRead={_extension.Settings.Configuration.UseConstructorForRead}) ";
      }

      public ThinktectureDbContextOptionsExtensionInfo(ThinktectureDbContextOptionsExtension extension)
         : base(extension)
      {
         _extension = extension;
      }

      public override bool ShouldUseSameServiceProvider(DbContextOptionsExtensionInfo other)
      {
         return other is ThinktectureDbContextOptionsExtensionInfo otherInfo
                && _extension.Settings == otherInfo._extension.Settings;
      }

      public override int GetServiceProviderHashCode()
      {
         return HashCode.Combine(_extension.Settings);
      }

      public override void PopulateDebugInfo(IDictionary<string, string> debugInfo)
      {
         debugInfo["Thinktecture:ValueConverters"] = $"ThinktectureValueConverters(Enabled={_extension.Settings.IsEnabled})";
      }
   }
}
