using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Swashbuckle.AspNetCore.SwaggerGen;
using Thinktecture.Swashbuckle.Internal;
using Thinktecture.Swashbuckle.Internal.AdHocUnions;
using Thinktecture.Swashbuckle.Internal.ComplexValueObjects;
using Thinktecture.Swashbuckle.Internal.KeyedValueObjects;
using Thinktecture.Swashbuckle.Internal.SmartEnums;

namespace Thinktecture.Swashbuckle;

/// <summary>
/// Extension methods for registering schema filters in the service collection.
/// </summary>
public static class ServiceCollectionExtensions
{
   /// <summary>
   /// Adds Thinktecture schema filters to the service collection.
   /// </summary>
   /// <param name="services">The service collection to add the schema filters to.</param>
   /// <param name="configureOptions">An optional action to configure the schema filter options.</param>
   /// <returns>The service collection for chaining.</returns>
   public static IServiceCollection AddThinktectureOpenApiFilters(
      this IServiceCollection services,
      Action<ThinktectureSchemaFilterOptions>? configureOptions = null)
   {
      services.TryAddSingleton<IOpenApiValueFactoryProvider, JsonSerializerOpenApiValueFactoryProvider>();
      services.TryAddSingleton<IKeyedValueObjectSchemaFilter, KeyedValueObjectSchemaFilter>();
      services.TryAddSingleton<IComplexValueObjectSchemaFilter, ComplexValueObjectSchemaFilter>();
      services.TryAddSingleton<IRequiredMemberEvaluator, DefaultRequiredMemberEvaluator>();
      services.TryAddSingleton<IAdHocUnionSchemaFilter, AdHocUnionSchemaFilter>();
      services.TryAddSingleton<ISmartEnumSchemaFilter, DefaultSmartEnumSchemaFilter>();
      services.TryAddSingleton<ISmartEnumSchemaExtension, NoSmartEnumSchemaExtension>();

      services
         .AddSingleton<JsonSerializerOptionsResolver>()
         .Configure<ThinktectureSchemaFilterOptions>(options => configureOptions?.Invoke(options))
         .AddOptions<SwaggerGenOptions>()
         .Configure((SwaggerGenOptions options,
                     IServiceProvider serviceProvider) =>
         {
            var schemaFilter = ActivatorUtilities.CreateInstance<ThinktectureSchemaFilter>(serviceProvider, options);
            var parameterFilter = ActivatorUtilities.CreateInstance<ThinktectureParameterFilter>(serviceProvider);
            var requestBodyFilter = ActivatorUtilities.CreateInstance<ThinktectureRequestBodyFilter>(serviceProvider, options);

            options.AddSchemaFilterInstance(schemaFilter);
            options.AddParameterFilterInstance(parameterFilter);
            options.AddRequestBodyFilterInstance(requestBodyFilter);
         });

      return services;
   }
}
