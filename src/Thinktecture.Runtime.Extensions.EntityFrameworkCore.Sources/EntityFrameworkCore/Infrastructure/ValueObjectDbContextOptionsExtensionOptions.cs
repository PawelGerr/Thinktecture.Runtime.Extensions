using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Thinktecture.EntityFrameworkCore.Infrastructure;

internal sealed class ValueObjectDbContextOptionsExtensionOptions : ISingletonOptions
{
   public ValueObjectValueConverterSettings? ValueObjectValueConverterSettings { get; private set; }

   public void Initialize(IDbContextOptions options)
   {
      var extension = GetExtension(options);

      ValueObjectValueConverterSettings = extension.ValueObjectValueConverterSettings;
   }

   public void Validate(IDbContextOptions options)
   {
      var extension = GetExtension(options);

      if (extension.ValueObjectValueConverterSettings != ValueObjectValueConverterSettings)
         throw new InvalidOperationException($"The setting '{nameof(ValueObjectDbContextOptionsExtension.ValueObjectValueConverterSettings)}' has been changed.");
   }

   private static ValueObjectDbContextOptionsExtension GetExtension(IDbContextOptions options)
   {
      return options.FindExtension<ValueObjectDbContextOptionsExtension>()
             ?? throw new InvalidOperationException($"{nameof(ValueObjectDbContextOptionsExtension)} not found in current '{nameof(IDbContextOptions)}'.");
   }
}
