using Microsoft.Extensions.DependencyInjection;
using Thinktecture.Swashbuckle.Internal.SmartEnums;

namespace Thinktecture.Swashbuckle;

/// <summary>
/// Represents a filter for Smart Enums.
/// </summary>
[SmartEnum<string>]
public partial class SmartEnumSchemaFilter
{
   /// <summary>
   /// The default schema filter.
   /// </summary>
   public static readonly SmartEnumSchemaFilter Default = new(
      nameof(Default),
      (p, e) => ActivatorUtilities.CreateInstance<DefaultSmartEnumSchemaFilter>(p, e.CreateSchemaExtension(p)));

   /// <summary>
   /// Schema filter using "oneOf".
   /// </summary>
   public static readonly SmartEnumSchemaFilter OneOf = new(
      nameof(OneOf),
      (p, e) => ActivatorUtilities.CreateInstance<OneOfSmartEnumSchemaFilter>(p, e.CreateSchemaExtension(p)));

   /// <summary>
   /// Schema filter using "anyOf".
   /// </summary>
   public static readonly SmartEnumSchemaFilter AnyOf = new(
      nameof(AnyOf),
      (p, e) => ActivatorUtilities.CreateInstance<AnyOfSmartEnumSchemaFilter>(p, e.CreateSchemaExtension(p)));

   /// <summary>
   /// Schema filter using "allOf".
   /// </summary>
   public static readonly SmartEnumSchemaFilter AllOf = new(
      nameof(AllOf),
      (p, e) => ActivatorUtilities.CreateInstance<AllOfSmartEnumSchemaFilter>(p, e.CreateSchemaExtension(p)));

   /// <summary>
   /// Schema filter is resolved via dependency injection.
   /// </summary>
   public static readonly SmartEnumSchemaFilter FromDependencyInjection = new(
      nameof(FromDependencyInjection),
      (p, _) => p.GetRequiredService<ISmartEnumSchemaFilter>());

   [UseDelegateFromConstructor]
   internal partial ISmartEnumSchemaFilter CreateSchemaFilter(
      IServiceProvider serviceProvider,
      SmartEnumSchemaExtension schemaExtension);
}
