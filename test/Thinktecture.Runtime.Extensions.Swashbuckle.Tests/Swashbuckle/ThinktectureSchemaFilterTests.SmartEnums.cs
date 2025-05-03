using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Thinktecture.Runtime.Tests.Swashbuckle.Helpers;
using Thinktecture.Runtime.Tests.TestEnums;
using Thinktecture.Swashbuckle;
using Xunit.Abstractions;
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
