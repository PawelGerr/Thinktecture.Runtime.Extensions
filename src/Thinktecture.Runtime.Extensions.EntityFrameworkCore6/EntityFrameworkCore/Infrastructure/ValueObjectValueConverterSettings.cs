using Microsoft.EntityFrameworkCore.Metadata;

namespace Thinktecture.EntityFrameworkCore.Infrastructure;

internal record ValueObjectValueConverterSettings(bool IsEnabled, bool ValidateOnWrite, bool UseConstructorForRead, Action<IConventionProperty>? ConfigureEnumsAndKeyedValueObjects);
