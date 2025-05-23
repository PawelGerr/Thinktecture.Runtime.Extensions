using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using Thinktecture.EntityFrameworkCore.Infrastructure;

namespace Thinktecture.EntityFrameworkCore.Conventions;

internal sealed class ThinktectureConventionSetPlugin : IConventionSetPlugin
{
   private readonly ThinktectureValueConverterSettings? _settings;

   public ThinktectureConventionSetPlugin(ThinktectureDbContextOptionsExtensionOptions options)
   {
      _settings = options.Settings;
   }

   public ConventionSet ModifyConventions(ConventionSet conventionSet)
   {
      if (_settings?.IsEnabled == true)
      {
         var convention = new ThinktectureConventionsPlugin(_settings.ValidateOnWrite, _settings.UseConstructorForRead, _settings.ConfigureEnumsAndKeyedValueObjects);

         conventionSet.NavigationAddedConventions.Add(convention);
         conventionSet.PropertyAddedConventions.Add(convention);
         conventionSet.EntityTypeAddedConventions.Add(convention);

#if PRIMITIVE_COLLECTIONS
         conventionSet.PropertyElementTypeChangedConventions.Add(convention);
#endif
      }

      return conventionSet;
   }
}
