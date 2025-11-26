using Microsoft.EntityFrameworkCore.Metadata;

namespace Thinktecture.EntityFrameworkCore.Infrastructure;

internal sealed record ThinktectureValueConverterSettings(
   bool IsEnabled,
   Configuration Configuration,
   Action<IConventionProperty>? ConfigureEnumsAndKeyedValueObjects);
