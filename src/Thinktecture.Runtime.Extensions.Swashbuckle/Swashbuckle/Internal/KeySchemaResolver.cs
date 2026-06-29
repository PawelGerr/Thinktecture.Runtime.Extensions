using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Thinktecture.Swashbuckle.Internal;

internal static class KeySchemaResolver
{
   public static IOpenApiSchema Resolve(SchemaFilterContext context, Type keyType)
   {
      return Resolve(context, keyType, out _);
   }

   public static IOpenApiSchema Resolve(SchemaFilterContext context, Type keyType, out string? keyTypeComponentId)
   {
      keyTypeComponentId = null;

      // Generate the key schema into a throwaway repository so a key that gets its own component (e.g. an enum)
      // does not leak an orphaned, unreferenced schema into the document's "components". Primitive keys
      // (int, string, ...) are inlined by Swashbuckle and behave exactly as before.
      var keySchemaRepository = new SchemaRepository();
      var keySchema = context.SchemaGenerator.GenerateSchema(keyType, keySchemaRepository);

      // For keys with their own component the generated schema is a "$ref" whose own scalar facets are empty.
      // Resolve it to the referenced component so the real wire representation can be copied by the caller, and
      // expose the component id so the caller can clean up the equivalent component Swashbuckle may have already
      // registered in the main repository (see "RemoveOrphanedKeyTypeSchemasDocumentFilter").
      if (keySchema is OpenApiSchemaReference keySchemaReference
          && keySchemaReference.Reference.Id is { } keySchemaId)
      {
         keyTypeComponentId = keySchemaId;

         if (keySchemaRepository.Schemas.TryGetValue(keySchemaId, out var referencedKeySchema))
            keySchema = referencedKeySchema;
      }

      return keySchema;
   }
}
