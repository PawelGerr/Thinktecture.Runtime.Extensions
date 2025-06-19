using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;
using Thinktecture.Swashbuckle.Internal;
using Thinktecture.Swashbuckle.Internal.AdHocUnions;
using Thinktecture.Swashbuckle.Internal.ComplexValueObjects;
using Thinktecture.Swashbuckle.Internal.KeyedValueObjects;
using Thinktecture.Swashbuckle.Internal.KeylessSmartEnums;
using Thinktecture.Swashbuckle.Internal.RegularUnions;
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
      services.TryAddSingleton<IKeylessSmartEnumSchemaFilter, KeylessSmartEnumSchemaFilter>();
      services.TryAddSingleton<IKeyedValueObjectSchemaFilter, KeyedValueObjectSchemaFilter>();
      services.TryAddSingleton<IComplexValueObjectSchemaFilter, ComplexValueObjectSchemaFilter>();
      services.TryAddSingleton<IRequiredMemberEvaluator, DefaultRequiredMemberEvaluator>();
      services.TryAddSingleton<IAdHocUnionSchemaFilter, AdHocUnionSchemaFilter>();
      services.TryAddSingleton<IRegularUnionSchemaFilter, RegularUnionSchemaFilter>();
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

            var ttOptions = serviceProvider.GetRequiredService<IOptions<ThinktectureSchemaFilterOptions>>().Value;

            if (ttOptions.ExtendSchemaIdSelector)
               ExtendSchemaIdSelector(options);

            options.AddSchemaFilterInstance(schemaFilter);
            options.AddParameterFilterInstance(parameterFilter);
            options.AddRequestBodyFilterInstance(requestBodyFilter);
         });

      return services;
   }

   private static void ExtendSchemaIdSelector(SwaggerGenOptions options)
   {
      var originalSchemaIdSelector = options.SchemaGeneratorOptions.SchemaIdSelector;

      options.SchemaGeneratorOptions.SchemaIdSelector = type =>
      {
         if (type.IsGenericType)
         {
            var typeDefinition = type;

            if (!typeDefinition.IsGenericTypeDefinition)
               typeDefinition = typeDefinition.GetGenericTypeDefinition();

            if (typeDefinition == typeof(BoundParameter<,>))
            {
               var originalType = type.GetGenericArguments()[0];
               var boundType = type.GetGenericArguments()[1];

               return $"{originalSchemaIdSelector(originalType)}As{originalSchemaIdSelector(boundType)}";
            }
         }

         return originalSchemaIdSelector(type);
      };
   }
}
