using Microsoft.EntityFrameworkCore.Metadata;

namespace Thinktecture.EntityFrameworkCore.Infrastructure;

internal sealed record ThinktectureValueConverterSettings(
   bool IsEnabled,
   bool UseConstructorForRead,
   Action<IConventionProperty>? ConfigureEnumsAndKeyedValueObjects);
