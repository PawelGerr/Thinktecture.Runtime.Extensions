using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Thinktecture.Runtime.Tests.Swashbuckle.Helpers;
using Thinktecture.Runtime.Tests.TestValueObjects;
using Xunit.Abstractions;
using static VerifyXunit.Verifier;

namespace Thinktecture.Runtime.Tests.Swashbuckle;

public partial class ThinktectureSchemaFilterTests
{
   public class KeyedValueObjects : ThinktectureSchemaFilterTests
   {
      public KeyedValueObjects(ITestOutputHelper testOutputHelper)
         : base(testOutputHelper)
      {
      }

      public static IEnumerable<object[]> TestData =
         EndpointKind.Items
                     .CrossJoin([true, false])
                     .CrossJoin([true, false])
                     .Select(i => new object[] { i.Item1, i.Item2, i.Item3 });

      [Theory]
      [MemberData(nameof(TestData))]
      public async Task Should_handle_Class_StringBased_as_route_parameter(
         EndpointKind endpointKind,
         bool nullable,
         bool nonNullableReferenceTypesAsRequired)
      {
         _nonNullableReferenceTypesAsRequired = nonNullableReferenceTypesAsRequired;

         if (endpointKind == EndpointKind.MinimalApi)
         {
            if (nullable)
            {
               App.MapGet("/test/{value}", (StringBasedReferenceValueObject? value = null) => value);
            }
            else
            {
               App.MapGet("/test/{value}", (StringBasedReferenceValueObject value) => value);
            }
         }
         else
         {
            _controllerType = nullable
                                 ? typeof(TestController.KeyedValueObjectClass.StringBased.Route.Nullable)
                                 : typeof(TestController.KeyedValueObjectClass.StringBased.Route);
         }

         var openApi = GetOpenApiJsonAsync();

         await Verify(openApi)
            .UseParameters(endpointKind, nullable, nonNullableReferenceTypesAsRequired);
      }

      [Theory]
      [MemberData(nameof(TestData))]
      public async Task Should_handle_Class_StringBased_as_query_parameter(
         EndpointKind endpointKind,
         bool nullable,
         bool nonNullableReferenceTypesAsRequired)
      {
         _nonNullableReferenceTypesAsRequired = nonNullableReferenceTypesAsRequired;

         if (endpointKind == EndpointKind.MinimalApi)
         {
            if (nullable)
            {
               App.MapGet("/test", (StringBasedReferenceValueObject? value = null) => value);
            }
            else
            {
               App.MapGet("/test", (StringBasedReferenceValueObject value) => value);
            }
         }
         else
         {
            _controllerType = nullable
                                 ? typeof(TestController.KeyedValueObjectClass.StringBased.QueryString.Nullable)
                                 : typeof(TestController.KeyedValueObjectClass.StringBased.QueryString);
         }

         var openApi = GetOpenApiJsonAsync();

         await Verify(openApi)
            .UseParameters(endpointKind, nullable, nonNullableReferenceTypesAsRequired);
      }

      [Theory]
      [MemberData(nameof(TestData))]
      public async Task Should_handle_Class_StringBased_as_body_parameter(
         EndpointKind endpointKind,
         bool nullable,
         bool nonNullableReferenceTypesAsRequired)
      {
         _nonNullableReferenceTypesAsRequired = nonNullableReferenceTypesAsRequired;

         if (endpointKind == EndpointKind.MinimalApi)
         {
            if (nullable)
            {
               App.MapPost("/test", ([FromBody] StringBasedReferenceValueObject? value = null) => value);
            }
            else
            {
               App.MapPost("/test", ([FromBody] StringBasedReferenceValueObject value) => value);
            }
         }
         else
         {
            _controllerType = nullable
                                 ? typeof(TestController.KeyedValueObjectClass.StringBased.Body.Nullable)
                                 : typeof(TestController.KeyedValueObjectClass.StringBased.Body);
         }

         var openApi = GetOpenApiJsonAsync();

         await Verify(openApi)
            .UseParameters(endpointKind, nullable, nonNullableReferenceTypesAsRequired);
      }

      [Theory]
      [MemberData(nameof(TestData))]
      public async Task Should_handle_Class_StringBased_as_form_parameter(
         EndpointKind endpointKind,
         bool nullable,
         bool nonNullableReferenceTypesAsRequired)
      {
         _nonNullableReferenceTypesAsRequired = nonNullableReferenceTypesAsRequired;

         if (endpointKind == EndpointKind.MinimalApi)
         {
            if (nullable)
            {
               App.MapPost("/test", ([FromForm] StringBasedReferenceValueObject? value = null) => value);
            }
            else
            {
               App.MapPost("/test", ([FromForm] StringBasedReferenceValueObject value) => value);
            }
         }
         else
         {
            _controllerType = nullable
                                 ? typeof(TestController.KeyedValueObjectClass.StringBased.Form.Nullable)
                                 : typeof(TestController.KeyedValueObjectClass.StringBased.Form);
         }

         var openApi = GetOpenApiJsonAsync();

         await Verify(openApi)
            .UseParameters(endpointKind, nullable, nonNullableReferenceTypesAsRequired);
      }

      [Theory]
      [MemberData(nameof(TestData))]
      public async Task Should_handle_Struct_StringBased_as_route_parameter(
         EndpointKind endpointKind,
         bool nullable,
         bool nonNullableReferenceTypesAsRequired)
      {
         _nonNullableReferenceTypesAsRequired = nonNullableReferenceTypesAsRequired;

         if (endpointKind == EndpointKind.MinimalApi)
         {
            if (nullable)
            {
               App.MapGet("/test/{value}", (StringBasedStructValueObject? value = null) => value);
            }
            else
            {
               App.MapGet("/test/{value}", (StringBasedStructValueObject value) => value);
            }
         }
         else
         {
            _controllerType = nullable
                                 ? typeof(TestController.KeyedValueObjectStruct.StringBased.Route.Nullable)
                                 : typeof(TestController.KeyedValueObjectStruct.StringBased.Route);
         }

         var openApi = GetOpenApiJsonAsync();

         await Verify(openApi)
            .UseParameters(endpointKind, nullable, nonNullableReferenceTypesAsRequired);
      }

      [Theory]
      [MemberData(nameof(TestData))]
      public async Task Should_handle_Struct_StringBased_as_query_parameter(
         EndpointKind endpointKind,
         bool nullable,
         bool nonNullableReferenceTypesAsRequired)
      {
         _nonNullableReferenceTypesAsRequired = nonNullableReferenceTypesAsRequired;

         if (endpointKind == EndpointKind.MinimalApi)
         {
            if (nullable)
            {
               App.MapGet("/test", (StringBasedStructValueObject? value = null) => value);
            }
            else
            {
               App.MapGet("/test", (StringBasedStructValueObject value) => value);
            }
         }
         else
         {
            _controllerType = nullable
                                 ? typeof(TestController.KeyedValueObjectStruct.StringBased.QueryString.Nullable)
                                 : typeof(TestController.KeyedValueObjectStruct.StringBased.QueryString);
         }

         var openApi = GetOpenApiJsonAsync();

         await Verify(openApi)
            .UseParameters(endpointKind, nullable, nonNullableReferenceTypesAsRequired);
      }
   }
}
