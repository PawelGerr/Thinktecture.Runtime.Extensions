using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;
using Thinktecture.Internal;

namespace Thinktecture.Swashbuckle.Internal;

/// <summary>
/// This is an internal API that supports the Thinktecture.Runtime.Extensions infrastructure and not subject to
/// the same compatibility standards as public APIs. It may be changed or removed without notice in
/// any release. You should only use it directly in your code with extreme caution and knowing that
/// doing so can result in application failures when updating to a new Thinktecture.Runtime.Extensions release.
/// </summary>
public partial class ThinktectureRequestBodyFilter : IRequestBodyFilter
{
   private readonly SwaggerGenOptions _swaggerGenOptions;

   /// <summary>
   /// This is an internal API that supports the Thinktecture.Runtime.Extensions infrastructure and not subject to
   /// the same compatibility standards as public APIs. It may be changed or removed without notice in
   /// any release. You should only use it directly in your code with extreme caution and knowing that
   /// doing so can result in application failures when updating to a new Thinktecture.Runtime.Extensions release.
   /// </summary>
   public ThinktectureRequestBodyFilter(
      SwaggerGenOptions swaggerGenOptions)
   {
      _swaggerGenOptions = swaggerGenOptions;
   }

   /// <inheritdoc />
   public void Apply(IOpenApiRequestBody requestBody, RequestBodyFilterContext context)
   {
      if (context.FormParameterDescriptions is null)
         return;

      ReplaceExpandedSchema(requestBody, context);
   }

   private void ReplaceExpandedSchema(IOpenApiRequestBody requestBody, RequestBodyFilterContext context)
   {
      var rootNodes = BuildTree(context);

      foreach (var root in rootNodes)
      {
         ReplaceExpandedSchema(context, requestBody, root);
      }
   }

   private void ReplaceExpandedSchema(
      RequestBodyFilterContext context,
      IOpenApiRequestBody requestBody,
      Node node)
   {
      var metadata = MetadataLookup.Find(node.Type);

      if (metadata is null)
      {
         var children = node.Switch(
            nodeNamedLeaf: _ => [],
            nodeNamedContainer: n => n.Children,
            nodeRoot: n => n.Children);

         foreach (var child in children)
         {
            ReplaceExpandedSchema(context, requestBody, child);
         }
      }
      else if (metadata is Metadata.Keyed or Metadata.AdHocUnion)
      {
         var parameters = node.FlattenParameters();
         ReplaceExpandedSchema(requestBody, context, parameters, node.Name, metadata);
      }
   }

   private void ReplaceExpandedSchema(
      IOpenApiRequestBody requestBody,
      RequestBodyFilterContext context,
      IReadOnlyList<ApiParameterDescription> parameters,
      string newParameterName,
      Metadata metadata)
   {
      if (requestBody.Content is null)
         return;

      var schema = context.SchemaGenerator.GenerateSchema(metadata.Type, context.SchemaRepository);

      if (schema is not OpenApiSchemaReference)
         return;

      foreach (var (_, mediaType) in requestBody.Content)
      {
         ReplaceExpandedSchema(mediaType, parameters, newParameterName, schema);
      }
   }

   private void ReplaceExpandedSchema(
      OpenApiMediaType mediaType,
      IReadOnlyList<ApiParameterDescription> parameters,
      string newParameterName,
      IOpenApiSchema schema)
   {
      var schemaReference = schema as OpenApiSchemaReference;

      if (schemaReference is not null && IsSameSchema(mediaType.Schema, schemaReference))
         return;

      var propertySchemas = mediaType.Schema?
                                     .Properties?
                                     .Where(kvp => parameters.Any(parameter => kvp.Key == parameter.Name))
                                     .ToList();

      if (propertySchemas is null || propertySchemas.Count == 0)
         return;

      if (propertySchemas.Count == 1)
      {
         var propertySchema = propertySchemas[0].Value;

         if (propertySchema is OpenApiSchemaReference propertySchemaReference && propertySchemaReference.Id == schemaReference?.Id)
            return;

         if (propertySchema.AllOf?.Count == 1)
         {
            var allOfSchema = propertySchema.AllOf[0];

            if (allOfSchema is OpenApiSchemaReference allOfSchemaReference && allOfSchemaReference.Reference?.Id == schemaReference?.Id)
               return;

            propertySchema.AllOf[0] = schema;
            return;
         }
      }

      foreach (var propertySchema in propertySchemas)
      {
         mediaType.Schema?.Properties?.Remove(propertySchema.Key);
         mediaType.Encoding?.Remove(propertySchema.Key);
      }

      mediaType.Schema ??= new OpenApiSchema();

      if (mediaType.Schema is OpenApiSchema mediaTypeSchema)
      {
         mediaTypeSchema.Properties ??= new Dictionary<string, IOpenApiSchema>();
         mediaTypeSchema.Properties[newParameterName] = _swaggerGenOptions.SchemaGeneratorOptions.UseAllOfToExtendReferenceSchemas
                                                           ? new OpenApiSchema { AllOf = [schema] }
                                                           : schema;
      }

      mediaType.Encoding ??= new Dictionary<string, OpenApiEncoding>();
      mediaType.Encoding[newParameterName] = new OpenApiEncoding { Style = ParameterStyle.Form };
   }

   private static bool IsSameSchema(
      IOpenApiSchema? schema,
      OpenApiSchemaReference expected)
   {
      if (schema is OpenApiSchemaReference schemaReference && schemaReference.Id == expected.Id)
         return true;

      if (schema?.AllOf?.Count != 1)
         return false;

      var allOfSchema = schema.AllOf[0];

      if (allOfSchema is OpenApiSchemaReference allOfSchemaReference && allOfSchemaReference.Id == expected.Id)
         return true;

      return false;
   }

   private static List<Node.Root> BuildTree(RequestBodyFilterContext context)
   {
      var rootNodes = new List<Node.Root>();

      foreach (var rootParameters in context.FormParameterDescriptions
                                            .GroupBy(d => d.ParameterDescriptor))
      {
         var rootNode = new Node.Root
                        {
                           Name = rootParameters.Key.Name,
                           ParameterDescriptor = rootParameters.Key
                        };
         rootNodes.Add(rootNode);

         foreach (var parameters in rootParameters
                     .GroupBy(p => p.ModelMetadata.ContainerType is null ? null : p.ModelMetadata.ContainerMetadata))
         {
            foreach (var parameter in parameters)
            {
               BuildTree(parameters.Key, parameter, rootNode);
            }
         }
      }

      return rootNodes;
   }

   private static void BuildTree(
      ModelMetadata? containerMetadata,
      ApiParameterDescription descriptor,
      Node.Root rootNode)
   {
      var path = descriptor.Name.Split('.');
      var currentNodes = (Nodes: rootNode.Children, Path: (string?)null);

      for (var i = 0; i < path.Length; i++)
      {
         var name = path[i];
         var node = currentNodes.Nodes.FirstOrDefault(n => n.Name == name);

         if (node is not null)
         {
            currentNodes = (((Node.Named.Container)node).Children, node.Path);
            continue;
         }

         if (i == path.Length - 1)
         {
            currentNodes.Nodes.Add(new Node.Named.Leaf
                                   {
                                      Name = name,
                                      Path = currentNodes.Path is null ? name : $"{currentNodes.Path}.{name}",
                                      Parameter = descriptor
                                   });

            return;
         }

         var container = new Node.Named.Container
                         {
                            Name = name,
                            Path = currentNodes.Path is null ? name : $"{currentNodes.Path}.{name}",
                            Metadata = containerMetadata
                                       ?? throw new Exception($"Container metadata of {nameof(ApiParameterDescription)} '{descriptor.Name}' is null")
                         };
         currentNodes.Nodes.Add(container);

         currentNodes = (container.Children, container.Path);
      }
   }

   [Union]
   private abstract partial class Node
   {
      public required string Name { get; set; }
      public abstract Type Type { get; }

      protected abstract void FlattenParameters(List<ApiParameterDescription> nodes);

      public List<ApiParameterDescription> FlattenParameters()
      {
         var parameters = new List<ApiParameterDescription>();
         FlattenParameters(parameters);

         return parameters;
      }

      public sealed class Root : Node
      {
         public required ParameterDescriptor ParameterDescriptor { get; init; }
         public List<Named> Children { get; } = [];

         public override Type Type => ParameterDescriptor.ParameterType;

         protected override void FlattenParameters(List<ApiParameterDescription> nodes)
         {
            foreach (var child in Children)
            {
               child.FlattenParameters(nodes);
            }
         }
      }

      public abstract class Named : Node
      {
         public required string Path { get; init; }

         private Named()
         {
         }

         public sealed class Container : Named
         {
            public required ModelMetadata Metadata { get; init; }
            public List<Named> Children { get; } = [];

            public override Type Type => Metadata.ModelType;

            public override string ToString()
            {
               return Name;
            }

            protected override void FlattenParameters(List<ApiParameterDescription> nodes)
            {
               foreach (var child in Children)
               {
                  child.FlattenParameters(nodes);
               }
            }
         }

         public sealed class Leaf : Named
         {
            public required ApiParameterDescription Parameter { get; init; }

            public override Type Type => Parameter.ModelMetadata.ModelType;

            protected override void FlattenParameters(List<ApiParameterDescription> parameters)
            {
               parameters.Add(Parameter);
            }

            public override string ToString()
            {
               return Name;
            }
         }
      }
   }
}
