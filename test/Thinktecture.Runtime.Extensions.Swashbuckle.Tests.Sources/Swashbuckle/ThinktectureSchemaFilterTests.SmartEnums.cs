using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Thinktecture.Runtime.Tests.Swashbuckle.Helpers;
using Thinktecture.Runtime.Tests.TestEnums;
using Thinktecture.Swashbuckle;
using static VerifyXunit.Verifier;

namespace Thinktecture.Runtime.Tests.Swashbuckle;

public partial class ThinktectureSchemaFilterTests
{
   public class SmartEnums : ThinktectureSchemaFilterTests
   {
      public SmartEnums(ITestOutputHelper testOutputHelper)
         : base(testOutputHelper)
      {
      }

      public static IEnumerable<object[]> TestData =
         SmartEnumSchemaFilter.Items.Where(f => f != SmartEnumSchemaFilter.FromDependencyInjection)
                              .CrossJoin(EndpointKind.Items)
                              .CrossJoin([true, false])
                              .CrossJoin([true, false])
                              .Select(i => new object[] { i.Item1, i.Item2, i.Item3, i.Item4 });

      public static IEnumerable<object[]> TestDataWithPolymorphism =
         SmartEnumSchemaFilter.Items.Where(f => f != SmartEnumSchemaFilter.FromDependencyInjection)
                              .CrossJoin(EndpointKind.Items)
                              .CrossJoin([true, false])
                              .CrossJoin([true, false])
                              .CrossJoin([true, false])
                              .Select(i => new object[] { i.Item1, i.Item2, i.Item3, i.Item4, i.Item5 });

      // The OneOf/AnyOf/AllOf strategies build a subschema per item. Only these strategies are exercised here because
      // the Default strategy already has numeric-key coverage and does not stringify the item value.
      public static IEnumerable<object[]> NonDefaultSmartEnumStrategies =
         new[] { SmartEnumSchemaFilter.OneOf, SmartEnumSchemaFilter.AnyOf, SmartEnumSchemaFilter.AllOf }
            .Select(f => new object[] { f });

      [Theory]
      [MemberData(nameof(TestData))]
      public async Task Should_handle_SmartEnumClass_StringBased_as_route_parameter(
         SmartEnumSchemaFilter smartEnumFilter,
         EndpointKind endpointKind,
         bool nullable,
         bool nonNullableReferenceTypesAsRequired)
      {
         _smartEnumFilter = smartEnumFilter;
         _nonNullableReferenceTypesAsRequired = nonNullableReferenceTypesAsRequired;

         if (endpointKind == EndpointKind.MinimalApi)
         {
            if (nullable)
            {
               App.MapGet("/test/{value}", (SmartEnum_StringBased? value = null) => value);
            }
            else
            {
               App.MapGet("/test/{value}", (SmartEnum_StringBased value) => value);
            }
         }
         else
         {
            _controllerType = nullable
                                 ? typeof(TestController.SmartEnumClass.StringBased.Route.Nullable)
                                 : typeof(TestController.SmartEnumClass.StringBased.Route);
         }

         var openApi = GetOpenApiJsonAsync();

         await Verify(openApi)
            .UseParameters(smartEnumFilter, endpointKind, nullable, nonNullableReferenceTypesAsRequired);
      }

      [Theory]
      [MemberData(nameof(TestData))]
      public async Task Should_handle_SmartEnumClass_StringBased_as_query_parameter(
         SmartEnumSchemaFilter smartEnumFilter,
         EndpointKind endpointKind,
         bool nullable,
         bool nonNullableReferenceTypesAsRequired)
      {
         _smartEnumFilter = smartEnumFilter;
         _nonNullableReferenceTypesAsRequired = nonNullableReferenceTypesAsRequired;

         if (endpointKind == EndpointKind.MinimalApi)
         {
            if (nullable)
            {
               App.MapGet("/test", (SmartEnum_StringBased? value = null) => value);
            }
            else
            {
               App.MapGet("/test", (SmartEnum_StringBased value) => value);
            }
         }
         else
         {
            _controllerType = nullable
                                 ? typeof(TestController.SmartEnumClass.StringBased.QueryString.Nullable)
                                 : typeof(TestController.SmartEnumClass.StringBased.QueryString);
         }

         var openApi = GetOpenApiJsonAsync();

         await Verify(openApi)
            .UseParameters(smartEnumFilter, endpointKind, nullable, nonNullableReferenceTypesAsRequired);
      }

      [Theory]
      [MemberData(nameof(TestData))]
      public async Task Should_handle_SmartEnumClass_StringBased_as_body_parameter(
         SmartEnumSchemaFilter smartEnumFilter,
         EndpointKind endpointKind,
         bool nullable,
         bool nonNullableReferenceTypesAsRequired)
      {
         _smartEnumFilter = smartEnumFilter;
         _nonNullableReferenceTypesAsRequired = nonNullableReferenceTypesAsRequired;

         if (endpointKind == EndpointKind.MinimalApi)
         {
            if (nullable)
            {
               App.MapPost("/test", ([FromBody] SmartEnum_StringBased? value = null) => value);
            }
            else
            {
               App.MapPost("/test", ([FromBody] SmartEnum_StringBased value) => value);
            }
         }
         else
         {
            _controllerType = nullable
                                 ? typeof(TestController.SmartEnumClass.StringBased.Body.Nullable)
                                 : typeof(TestController.SmartEnumClass.StringBased.Body);
         }

         var openApi = GetOpenApiJsonAsync();

         await Verify(openApi)
            .UseParameters(smartEnumFilter, endpointKind, nullable, nonNullableReferenceTypesAsRequired);
      }

      [Theory]
      [MemberData(nameof(NonDefaultSmartEnumStrategies))]
      public async Task Should_handle_SmartEnumClass_IntBased_as_body_parameter(
         SmartEnumSchemaFilter smartEnumFilter)
      {
         // Regression: for a numeric-keyed Smart Enum the OneOf/AnyOf/AllOf strategies must document the item value
         // with its real JSON type. Before the fix the value was stringified via "JsonNode.ToString()" and emitted as
         // "const": "1" (a string) although the schema type is "integer". The verified output must show the number 1
         // (via a single-value "enum"), not the string "1".
         _smartEnumFilter = smartEnumFilter;

         App.MapPost("/test", ([FromBody] SmartEnum_IntBased value) => value);

         var openApi = GetOpenApiJsonAsync();

         await Verify(openApi)
            .UseParameters(smartEnumFilter);
      }

      [Theory]
      [MemberData(nameof(TestDataWithPolymorphism))]
      public async Task Should_handle_SmartEnumClass_with_BaseClass_as_body_parameter(
         SmartEnumSchemaFilter smartEnumFilter,
         EndpointKind endpointKind,
         bool nullable,
         bool nonNrtAsRequired,
         bool polymorphism)
      {
         _smartEnumFilter = smartEnumFilter;
         _nonNullableReferenceTypesAsRequired = nonNrtAsRequired;
         _useOneOfForPolymorphism = polymorphism;

         if (endpointKind == EndpointKind.MinimalApi)
         {
            if (nullable)
            {
               App.MapPost("/test", ([FromBody] SmartEnum_StringBased_with_BaseClass? value = null) => value);
            }
            else
            {
               App.MapPost("/test", ([FromBody] SmartEnum_StringBased_with_BaseClass value) => value);
            }
         }
         else
         {
            _controllerType = nullable
                                 ? typeof(TestController.SmartEnumClass.StringBased_with_BaseClass.Body.Nullable)
                                 : typeof(TestController.SmartEnumClass.StringBased_with_BaseClass.Body);
         }

         var openApi = GetOpenApiJsonAsync();

         await Verify(openApi)
            .UseParameters(smartEnumFilter, endpointKind, nullable, nonNrtAsRequired, polymorphism);
      }

      [Fact]
      public async Task Should_document_query_parameter_of_keyed_SmartEnum_with_serialization_only_factory_using_key_type()
      {
         // The Smart Enum has a string key but an object factory that serializes as int for System.Text.Json only
         // (UseForModelBinding is not set). ThinktectureModelBinderProvider therefore binds via the string key, so
         // the query parameter must be documented as string. Before the fix the parameter filter reused the type's
         // own schema, which the object factory rewrites to the int serialization format, documenting it as integer.
         App.MapGet("/test", (SmartEnum_StringBased_WithIntObjectFactoryForJson value) => value);

         var openApi = await GetOpenApiJsonAsync();

         using var doc = JsonDocument.Parse(openApi);
         var parameterSchema = doc.RootElement
                                  .GetProperty("paths")
                                  .GetProperty("/test")
                                  .GetProperty("get")
                                  .GetProperty("parameters")[0]
                                  .GetProperty("schema");

         var resolvedType = ResolveSchemaType(parameterSchema, doc.RootElement);

         resolvedType.Should().Be("string");
      }

      // Resolves the JSON "type" of an OpenAPI schema element, following a single-item "allOf" wrapper (added by
      // UseAllOfToExtendReferenceSchemas) and "$ref" references into "#/components/schemas".
      private static string? ResolveSchemaType(
         JsonElement schema,
         JsonElement root)
      {
         if (schema.TryGetProperty("type", out var type))
            return type.GetString();

         if (schema.TryGetProperty("allOf", out var allOf) && allOf.GetArrayLength() > 0)
            return ResolveSchemaType(allOf[0], root);

         if (schema.TryGetProperty("$ref", out var reference))
         {
            var componentName = reference.GetString()!.Substring("#/components/schemas/".Length);
            var component = root.GetProperty("components").GetProperty("schemas").GetProperty(componentName);
            return ResolveSchemaType(component, root);
         }

         return null;
      }

      [Fact]
      public async Task Should_handle_SmartEnumClass_StringBased_as_body_parameter_with_varnames()
      {
         _smartEnumFilter = SmartEnumSchemaFilter.Default;
         _smartEnumExtension = SmartEnumSchemaExtension.VarNamesFromStringRepresentation;

         App.MapPost("/test", ([FromBody] SmartEnum_CustomToString value) => value);

         var openApi = GetOpenApiJsonAsync();

         await Verify(openApi);
      }

      [Fact]
      public async Task Should_handle_SmartEnumClass_StringBased_as_body_parameter_with_varnames_having_duplicates()
      {
         _smartEnumFilter = SmartEnumSchemaFilter.Default;
         _smartEnumExtension = SmartEnumSchemaExtension.VarNamesFromStringRepresentation;

         App.MapPost("/test", ([FromBody] SmartEnum_NameDuplicates value) => value);

         var openApi = GetOpenApiJsonAsync();

         await Verify(openApi);
      }

      [Fact]
      public async Task Should_handle_SmartEnumClass_StringBased_as_body_parameter_with_varnames_from_dotnet_identifiers()
      {
         _smartEnumFilter = SmartEnumSchemaFilter.Default;
         _smartEnumExtension = SmartEnumSchemaExtension.VarNamesFromDotnetIdentifiers;

         App.MapPost("/test", ([FromBody] SmartEnum_NameDuplicates value) => value);

         var openApi = GetOpenApiJsonAsync();

         await Verify(openApi);
      }

      [Fact]
      public async Task Should_handle_keyed_SmartEnum_nested_in_generic_class_as_body_parameter()
      {
         App.MapPost("/test", ([FromBody] SmartEnums_NestedInGenericClass.GenericOuter<int>.KeyedSmartEnum value) => value);

         await Verify(GetOpenApiJsonAsync());
      }

      [Fact]
      public async Task Should_handle_EnumBased_SmartEnum_as_body_parameter()
      {
         App.MapPost("/test", ([FromBody] SmartEnum_EnumBased value) => value);

         await Verify(GetOpenApiJsonAsync());
      }

      [Fact]
      public async Task Should_handle_EnumBased_SmartEnum_with_string_enum_converter_as_body_parameter()
      {
         _useStringEnumConverter = true;

         App.MapPost("/test", ([FromBody] SmartEnum_EnumBased value) => value);

         await Verify(GetOpenApiJsonAsync());
      }

      [Fact]
      public async Task Should_keep_orphaned_key_type_component_when_orphan_removal_is_disabled()
      {
         // Opt-out of the document filter: the key enum that Swashbuckle registers while generating the Smart Enum
         // stays in the document as an (unreferenced) orphan. Contrast with Should_handle_EnumBased_SmartEnum_as_body_parameter.
         _keepOrphanedKeyTypeSchemas = true;

         App.MapPost("/test", ([FromBody] SmartEnum_EnumBased value) => value);

         await Verify(GetOpenApiJsonAsync());
      }

      [Fact]
      public async Task Should_keep_EnumBased_SmartEnum_key_component_when_key_enum_is_also_used_directly()
      {
         // The Smart Enum endpoint causes the key enum to be registered as a component that would normally be
         // pruned as an orphan. The second endpoint references the raw key enum directly, so the document filter
         // must retain "SmartEnum_EnumKey" instead of removing it.
         App.MapPost("/smart-enum", ([FromBody] SmartEnum_EnumBased value) => value);
         App.MapPost("/raw-enum", ([FromBody] SmartEnum_EnumKey value) => value);

         await Verify(GetOpenApiJsonAsync());
      }

      [Theory]
      [MemberData(nameof(TestData))]
      public async Task Should_handle_SmartEnumClass_StringBased_as_form_parameter(
         SmartEnumSchemaFilter smartEnumFilter,
         EndpointKind endpointKind,
         bool nullable,
         bool nonNullableReferenceTypesAsRequired)
      {
         _smartEnumFilter = smartEnumFilter;
         _nonNullableReferenceTypesAsRequired = nonNullableReferenceTypesAsRequired;

         if (endpointKind == EndpointKind.MinimalApi)
         {
            if (nullable)
            {
               App.MapPost("/test", ([FromForm] SmartEnum_StringBased? value = null) => value);
            }
            else
            {
               App.MapPost("/test", ([FromForm] SmartEnum_StringBased value) => value);
            }
         }
         else
         {
            _controllerType = nullable
                                 ? typeof(TestController.SmartEnumClass.StringBased.Form.Nullable)
                                 : typeof(TestController.SmartEnumClass.StringBased.Form);
         }

         var openApi = GetOpenApiJsonAsync();

         await Verify(openApi)
            .UseParameters(smartEnumFilter, endpointKind, nullable, nonNullableReferenceTypesAsRequired);
      }
   }
}
