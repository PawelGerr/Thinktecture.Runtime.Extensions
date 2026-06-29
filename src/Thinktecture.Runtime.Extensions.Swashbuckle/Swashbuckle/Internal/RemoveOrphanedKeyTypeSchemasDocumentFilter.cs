using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Thinktecture.Swashbuckle.Internal;

/// <summary>
/// Removes orphaned key-type component schemas that Swashbuckle registers while generating a Smart Enum.
/// </summary>
/// <remarks>
/// While generating a Smart Enum whose key is itself componentized (e.g. an <c>enum</c>), Swashbuckle walks the
/// Smart Enum's public <c>Key</c> property and registers the key type as a named component before the schema
/// filters run. The Smart Enum schema filter then replaces the Smart Enum's own schema, which leaves that
/// key-type component behind, referenced by nothing. This filter removes such components, but only the ones the
/// Smart Enum filter recorded as candidates (see <see cref="OrphanKeyTypeSchemaRegistry"/>) and only when they
/// are not referenced anywhere in the finished document, so a key type that is also used directly elsewhere is
/// never removed.
/// </remarks>
internal sealed class RemoveOrphanedKeyTypeSchemasDocumentFilter : IDocumentFilter
{
   private readonly OrphanKeyTypeSchemaRegistry _registry;

   public RemoveOrphanedKeyTypeSchemasDocumentFilter(OrphanKeyTypeSchemaRegistry registry)
   {
      _registry = registry;
   }

   /// <inheritdoc />
   public void Apply(OpenApiDocument document, DocumentFilterContext context)
   {
      var schemas = document.Components?.Schemas;

      if (schemas is null || schemas.Count == 0)
         return;

      var candidates = _registry.GetCandidates(context.SchemaRepository);

      if (candidates.Count == 0)
         return;

      var removableCandidates = candidates.Where(schemas.ContainsKey).ToList();

      if (removableCandidates.Count == 0)
         return;

      var referencedSchemaIds = CollectReferencedSchemaIds(document);

      foreach (var candidate in removableCandidates)
      {
         if (!referencedSchemaIds.Contains(candidate))
            schemas.Remove(candidate);
      }
   }

   private static HashSet<string> CollectReferencedSchemaIds(OpenApiDocument document)
   {
      var visitor = new SchemaReferenceVisitor();
      new OpenApiWalker(visitor).Walk(document);

      return visitor.ReferencedSchemaIds;
   }

   private sealed class SchemaReferenceVisitor : OpenApiVisitorBase
   {
      public HashSet<string> ReferencedSchemaIds { get; } = new(StringComparer.Ordinal);

      public override void Visit(IOpenApiReferenceHolder referenceHolder)
      {
         AddIfSchemaReference(referenceHolder);
      }

      public override void Visit(IOpenApiSchema schema)
      {
         AddIfSchemaReference(schema);
      }

      private void AddIfSchemaReference(object? candidate)
      {
         if (candidate is OpenApiSchemaReference { Reference.Id: { } id })
            ReferencedSchemaIds.Add(id);
      }
   }
}
