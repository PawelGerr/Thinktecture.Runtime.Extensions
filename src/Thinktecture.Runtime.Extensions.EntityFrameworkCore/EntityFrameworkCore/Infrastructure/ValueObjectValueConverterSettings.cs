using Microsoft.EntityFrameworkCore.Metadata;

namespace Thinktecture.EntityFrameworkCore.Infrastructure;

internal record ValueObjectValueConverterSettings(bool IsEnabled, bool ValidateOnWrite, Action<IConventionProperty>? ConfigureEnumsAndKeyedValueObjects);
