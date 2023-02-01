using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Thinktecture.EntityFrameworkCore.Conventions;

namespace Thinktecture.EntityFrameworkCore.Infrastructure;

internal sealed class ValueObjectDbContextOptionsExtension : IDbContextOptionsExtension
{
   private DbContextOptionsExtensionInfo? _info;
   public DbContextOptionsExtensionInfo Info => _info ??= new ValueObjectDbContextOptionsExtensionInfo(this);

   public ValueObjectValueConverterSettings ValueObjectValueConverterSettings { get; private set; }

   public ValueObjectDbContextOptionsExtension()
   {
      ValueObjectValueConverterSettings = new ValueObjectValueConverterSettings(true, true, true, null);
   }

   public ValueObjectDbContextOptionsExtension UseValueObjectValueConverter(
      bool useValueObjectConventions,
      bool validateOnWrite = true,
      bool useConstructorForRead = true,
      Action<IConventionProperty>? configureEnumsAndKeyedValueObjects = null)
   {
      ValueObjectValueConverterSettings = new ValueObjectValueConverterSettings(useValueObjectConventions, validateOnWrite, useConstructorForRead, configureEnumsAndKeyedValueObjects);

      return this;
   }

   public void ApplyServices(IServiceCollection services)
   {
      services.TryAddSingleton<ValueObjectDbContextOptionsExtensionOptions>();
      services.AddSingleton<ISingletonOptions>(provider => provider.GetRequiredService<ValueObjectDbContextOptionsExtensionOptions>());

      services.Add(ServiceDescriptor.Describe(typeof(IConventionSetPlugin), typeof(ValueObjectConventionSetPlugin), GetLifetime<IConventionSetPlugin>()));
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

   private sealed class ValueObjectDbContextOptionsExtensionInfo : DbContextOptionsExtensionInfo
   {
      private readonly ValueObjectDbContextOptionsExtension _extension;
      public override bool IsDatabaseProvider => false;

      private string? _logFragment;
      public override string LogFragment => _logFragment ??= CreateLogFragment();

      private string CreateLogFragment()
      {
         if (!_extension.ValueObjectValueConverterSettings.IsEnabled)
            return String.Empty;

         return $"ValueObjectValueConverter(validateOnWrite={_extension.ValueObjectValueConverterSettings.ValidateOnWrite}) ";
      }

      public ValueObjectDbContextOptionsExtensionInfo(ValueObjectDbContextOptionsExtension extension)
         : base(extension)
      {
         _extension = extension;
      }

#if EFCORE5
      public override long GetServiceProviderHashCode()
      {
         return HashCode.Combine(_extension.ValueObjectValueConverterSettings);
      }
#else
      public override bool ShouldUseSameServiceProvider(DbContextOptionsExtensionInfo other)
      {
         return other is ValueObjectDbContextOptionsExtensionInfo otherInfo
                && _extension.ValueObjectValueConverterSettings == otherInfo._extension.ValueObjectValueConverterSettings;
      }

      public override int GetServiceProviderHashCode()
      {
         return HashCode.Combine(_extension.ValueObjectValueConverterSettings);
      }

#endif

      public override void PopulateDebugInfo(IDictionary<string, string> debugInfo)
      {
         debugInfo["Thinktecture:ValueObjectValueConverter"] = $"ValueObjectValueConverter(isEnabled={_extension.ValueObjectValueConverterSettings.IsEnabled}, validateOnWrite={_extension.ValueObjectValueConverterSettings.ValidateOnWrite})";
      }
   }
}
