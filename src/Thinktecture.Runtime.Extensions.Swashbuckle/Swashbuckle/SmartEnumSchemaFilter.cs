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
      p => ActivatorUtilities.CreateInstance<DefaultSmartEnumSchemaFilter>(p));

   /// <summary>
   /// Schema filter using "oneOf".
   /// </summary>
   public static readonly SmartEnumSchemaFilter OneOf = new(
      nameof(OneOf),
      p => ActivatorUtilities.CreateInstance<OneOfSmartEnumSchemaFilter>(p));

   /// <summary>
   /// Schema filter using "anyOf".
   /// </summary>
   public static readonly SmartEnumSchemaFilter AnyOf = new(
      nameof(AnyOf),
      p => ActivatorUtilities.CreateInstance<AnyOfSmartEnumSchemaFilter>(p));

   /// <summary>
   /// Schema filter using "allOf".
   /// </summary>
   public static readonly SmartEnumSchemaFilter AllOf = new(
      nameof(AllOf),
      p => ActivatorUtilities.CreateInstance<AllOfSmartEnumSchemaFilter>(p));

   /// <summary>
   /// Schema filter is resolved via dependency injection.
   /// </summary>
   public static readonly SmartEnumSchemaFilter FromDependencyInjection = new(
      nameof(FromDependencyInjection),
      p => p.GetRequiredService<ISmartEnumSchemaFilter>());

   [UseDelegateFromConstructor]
   internal partial ISmartEnumSchemaFilter CreateSchemaFilter(
      IServiceProvider serviceProvider);
}
