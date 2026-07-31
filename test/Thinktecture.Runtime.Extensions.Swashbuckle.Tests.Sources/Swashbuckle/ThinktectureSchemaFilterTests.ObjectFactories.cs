using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Thinktecture.Runtime.Tests.Swashbuckle.Helpers;

namespace Thinktecture.Runtime.Tests.Swashbuckle;

public partial class ThinktectureSchemaFilterTests
{
   public class ObjectFactories : ThinktectureSchemaFilterTests
   {
      public ObjectFactories(ITestOutputHelper testOutputHelper)
         : base(testOutputHelper)
      {
      }

      [Fact]
      public async Task Should_treat_complex_value_object_with_SystemTextJson_only_object_factory_as_string()
      {
         App.MapPost("/test", ([FromBody] ComplexValueObjectWithSystemTextJsonOnlyObjectFactory value) => value);

         var openApi = await GetOpenApiJsonAsync();

         using var document = JsonDocument.Parse(openApi);

         var schema = document.RootElement
                              .GetProperty("components")
                              .GetProperty("schemas")
                              .GetProperty(nameof(ComplexValueObjectWithSystemTextJsonOnlyObjectFactory));

         schema.GetProperty("type").GetString().Should().Be("string");
         schema.TryGetProperty("properties", out _).Should().BeFalse();
      }

      [Fact]
      public async Task Should_prefer_SystemTextJson_object_factory_over_Newtonsoft_when_both_are_present()
      {
         // Regression: the type carries a System.Text.Json string factory and a last-declared Newtonsoft.Json
         // int factory. Swashbuckle documents the System.Text.Json wire format, so the schema must be "string".
         // Before the fix the filter matched both frameworks and picked the last-declared (Newtonsoft) factory,
         // producing "integer".
         App.MapPost("/test", ([FromBody] ComplexValueObjectWithFrameworkSpecificObjectFactories value) => value);

         var openApi = await GetOpenApiJsonAsync();

         using var document = JsonDocument.Parse(openApi);

         var schema = document.RootElement
                              .GetProperty("components")
                              .GetProperty("schemas")
                              .GetProperty(nameof(ComplexValueObjectWithFrameworkSpecificObjectFactories));

         schema.GetProperty("type").GetString().Should().Be("string");
         schema.TryGetProperty("properties", out _).Should().BeFalse();
      }

      [Fact]
      public async Task Should_ignore_Newtonsoft_only_object_factory_when_Newtonsoft_support_is_not_registered()
      {
         // The test host uses the default System.Text.Json pipeline (Swashbuckle.AspNetCore.Newtonsoft is not
         // registered). A factory flagged only for Newtonsoft.Json does not affect the actual payload, so the
         // schema must stay the complex value object's own object schema instead of the factory's string.
         App.MapPost("/test", ([FromBody] ComplexValueObjectWithNewtonsoftJsonOnlyObjectFactory value) => value);

         var openApi = await GetOpenApiJsonAsync();

         using var document = JsonDocument.Parse(openApi);

         var schema = document.RootElement
                              .GetProperty("components")
                              .GetProperty("schemas")
                              .GetProperty(nameof(ComplexValueObjectWithNewtonsoftJsonOnlyObjectFactory));

         schema.GetProperty("type").GetString().Should().Be("object");
         schema.GetProperty("properties").TryGetProperty("property1", out _).Should().BeTrue();
      }

      [Fact]
      public async Task Should_use_Newtonsoft_only_object_factory_when_Newtonsoft_support_is_registered()
      {
         // The inverse of the test above: with Swashbuckle.AspNetCore.Newtonsoft registered, the documented pipeline
         // serializes with Newtonsoft.Json, so the factory flagged only for Newtonsoft.Json defines the wire format.
         _addSwaggerGenNewtonsoftSupport = true;

         App.MapPost("/test", ([FromBody] ComplexValueObjectWithNewtonsoftJsonOnlyObjectFactory value) => value);

         var openApi = await GetOpenApiJsonAsync();

         using var document = JsonDocument.Parse(openApi);

         var schema = document.RootElement
                              .GetProperty("components")
                              .GetProperty("schemas")
                              .GetProperty(nameof(ComplexValueObjectWithNewtonsoftJsonOnlyObjectFactory));

         schema.GetProperty("type").GetString().Should().Be("string");
         schema.TryGetProperty("properties", out _).Should().BeFalse();
      }

      [Fact]
      public async Task Should_ignore_MessagePack_only_object_factory_when_Newtonsoft_support_is_registered()
      {
         // Newtonsoft support is registered on purpose, so both lookups in GetSerializationType are active. A factory
         // flagged for neither System.Text.Json nor Newtonsoft.Json must be rejected by both of them.
         _addSwaggerGenNewtonsoftSupport = true;

         App.MapPost("/test", ([FromBody] ComplexValueObjectWithMessagePackOnlyObjectFactory value) => value);

         var openApi = await GetOpenApiJsonAsync();

         using var document = JsonDocument.Parse(openApi);

         var schema = document.RootElement
                              .GetProperty("components")
                              .GetProperty("schemas")
                              .GetProperty(nameof(ComplexValueObjectWithMessagePackOnlyObjectFactory));

         schema.GetProperty("type").GetString().Should().Be("object");
         schema.GetProperty("properties").TryGetProperty("property1", out _).Should().BeTrue();
      }

      [Fact]
      public async Task Should_ignore_ref_struct_object_factory_when_documenting_schema()
      {
         // Regression: the object factory of this type has the ref struct ReadOnlySpan<byte> as its value type and is
         // flagged for all serialization frameworks. Only ReadOnlySpan<char> has a span-based converter, so every other
         // ref-struct factory must be ignored by both lookups. Newtonsoft support is registered so both of them run.
         // Before the fix, the schema generator was handed ReadOnlySpan<byte> and schema generation broke.
         _addSwaggerGenNewtonsoftSupport = true;

         App.MapPost("/test", ([FromBody] TestValueObjects.ClassWithByteSpanObjectFactoryMetadata value) => value);

         var openApi = await GetOpenApiJsonAsync();

         using var document = JsonDocument.Parse(openApi);

         var schema = document.RootElement
                              .GetProperty("components")
                              .GetProperty("schemas")
                              .GetProperty(nameof(TestValueObjects.ClassWithByteSpanObjectFactoryMetadata));

         schema.GetProperty("type").GetString().Should().Be("object");
      }

      [Fact]
      public async Task Should_treat_complex_value_object_with_ReadOnlySpan_based_object_factory_as_string()
      {
         // Regression: a ReadOnlySpan<char>-based object factory serializes as a JSON string, so the schema must be
         // "string". Before the fix, GetSerializationType returned typeof(ReadOnlySpan<char>) and the schema generator
         // was asked to build a schema for a ref struct, producing a wrong (non-string) schema or failing.
         App.MapPost("/test", ([FromBody] TestValueObjects.ComplexValueObjectWithReadOnlySpanBasedObjectFactoryForJson value) => value);

         var openApi = await GetOpenApiJsonAsync();

         using var document = JsonDocument.Parse(openApi);

         var schema = document.RootElement
                              .GetProperty("components")
                              .GetProperty("schemas")
                              .GetProperty(nameof(TestValueObjects.ComplexValueObjectWithReadOnlySpanBasedObjectFactoryForJson));

         schema.GetProperty("type").GetString().Should().Be("string");
         schema.TryGetProperty("properties", out _).Should().BeFalse();
      }
   }
}
