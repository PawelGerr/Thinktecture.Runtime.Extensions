using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Thinktecture.EntityFrameworkCore.Infrastructure;

internal sealed class ThinktectureDbContextOptionsExtensionOptions : ISingletonOptions
{
   public ThinktectureValueConverterSettings? Settings { get; private set; }

   public void Initialize(IDbContextOptions options)
   {
      var extension = GetExtension(options);

      Settings = extension.Settings;
   }

   public void Validate(IDbContextOptions options)
   {
      var extension = GetExtension(options);

      if (extension.Settings != Settings)
         throw new InvalidOperationException($"The setting '{nameof(ThinktectureDbContextOptionsExtension.Settings)}' has been changed.");
   }

   private static ThinktectureDbContextOptionsExtension GetExtension(IDbContextOptions options)
   {
      return options.FindExtension<ThinktectureDbContextOptionsExtension>()
             ?? throw new InvalidOperationException($"{nameof(ThinktectureDbContextOptionsExtension)} not found in current '{nameof(IDbContextOptions)}'.");
   }
}
