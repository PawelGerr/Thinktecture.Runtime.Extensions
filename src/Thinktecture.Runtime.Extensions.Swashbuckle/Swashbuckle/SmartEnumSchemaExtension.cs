using Microsoft.Extensions.DependencyInjection;
using Thinktecture.Swashbuckle.Internal.SmartEnums;

namespace Thinktecture.Swashbuckle;

/// <summary>
/// Represents a filter for Smart Enums.
/// </summary>
[SmartEnum<string>]
public partial class SmartEnumSchemaExtension
{
   /// <summary>
   /// No further changes.
   /// </summary>
   public static readonly SmartEnumSchemaExtension None = new(
      nameof(None),
      p => ActivatorUtilities.CreateInstance<NoSmartEnumSchemaExtension>(p));

   /// <summary>
   /// Extends the schema with "x-enum-varnames" using the string representation of the items.
   /// </summary>
   public static readonly SmartEnumSchemaExtension VarNamesFromStringRepresentation = new(
      nameof(VarNamesFromStringRepresentation),
      p => ActivatorUtilities.CreateInstance<VarNamesFromStringRepresentationSchemaExtension>(p));

   /// <summary>
   /// Schema extension is resolved via dependency injection.
   /// </summary>
   public static readonly SmartEnumSchemaExtension FromDependencyInjection = new(
      nameof(FromDependencyInjection),
      p => p.GetRequiredService<ISmartEnumSchemaExtension>());

   [UseDelegateFromConstructor]
   internal partial ISmartEnumSchemaExtension CreateSchemaExtension(IServiceProvider serviceProvider);
}
