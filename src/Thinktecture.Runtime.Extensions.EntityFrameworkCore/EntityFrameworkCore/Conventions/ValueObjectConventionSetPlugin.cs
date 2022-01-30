using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using Thinktecture.EntityFrameworkCore.Infrastructure;

namespace Thinktecture.EntityFrameworkCore.Conventions;

internal class ValueObjectConventionSetPlugin : IConventionSetPlugin
{
   private readonly ValueObjectValueConverterSettings? _settings;

   public ValueObjectConventionSetPlugin(ValueObjectDbContextOptionsExtensionOptions options)
   {
      _settings = options.ValueObjectValueConverterSettings;
   }

   public ConventionSet ModifyConventions(ConventionSet conventionSet)
   {
      if (_settings?.IsEnabled == true)
      {
         var convention = new ValueObjectConventionPlugin(_settings.ValidateOnWrite, _settings.ConfigureEnumsAndKeyedValueObjects);

         conventionSet.NavigationAddedConventions.Add(convention);
         conventionSet.PropertyAddedConventions.Add(convention);
         conventionSet.EntityTypeAddedConventions.Add(convention);
      }

      return conventionSet;
   }
}
