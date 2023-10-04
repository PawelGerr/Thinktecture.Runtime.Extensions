using Microsoft.EntityFrameworkCore.Metadata;

namespace Thinktecture.EntityFrameworkCore.Infrastructure;

internal sealed record ValueObjectValueConverterSettings(bool IsEnabled, bool ValidateOnWrite, bool UseConstructorForRead, Action<IConventionProperty>? ConfigureEnumsAndKeyedValueObjects);
