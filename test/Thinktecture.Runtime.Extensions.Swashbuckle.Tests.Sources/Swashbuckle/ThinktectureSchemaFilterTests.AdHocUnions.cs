using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Thinktecture.Runtime.Tests.Swashbuckle.Helpers;
using Thinktecture.Runtime.Tests.TestAdHocUnions;
using static VerifyXunit.Verifier;

namespace Thinktecture.Runtime.Tests.Swashbuckle;

public partial class ThinktectureSchemaFilterTests
{
   public class AdHocUnions : ThinktectureSchemaFilterTests
   {
      public AdHocUnions(ITestOutputHelper testOutputHelper)
         : base(testOutputHelper)
      {
      }

      public static IEnumerable<object[]> TestData =
         EndpointKind.Items
                     .CrossJoin([true, false])
                     .CrossJoin([true, false])
                     .Select(i => new object[] { i.Item1, i.Item2, i.Item3 });

      public static IEnumerable<object[]> TestDataWithPolymorphism =
         EndpointKind.Items
                     .CrossJoin([true, false])
                     .CrossJoin([true, false])
                     .CrossJoin([true, false])
                     .Select(i => new object[] { i.Item1, i.Item2, i.Item3, i.Item4 });

      [Theory]
      [MemberData(nameof(TestData))]
      public async Task Should_handle_Class_as_route_parameter(
         EndpointKind endpointKind,
         bool nullable,
         bool nonNullableReferenceTypesAsRequired)
      {
         _nonNullableReferenceTypesAsRequired = nonNullableReferenceTypesAsRequired;

         // TextOrNumberSerializable is IParsable

         if (endpointKind == EndpointKind.MinimalApi)
         {
            if (nullable)
            {
               App.MapGet("/test/{value}", ([FromRoute] TextOrNumberSerializable? value = null) => value);
            }
            else
            {
               App.MapGet("/test/{value}", ([FromRoute] TextOrNumberSerializable value) => value);
            }
         }
         else
         {
            _controllerType = nullable
                                 ? typeof(TestController.AdHocUnion.Class.Route.Nullable)
                                 : typeof(TestController.AdHocUnion.Class.Route);
         }

         var openApi = GetOpenApiJsonAsync();

         await Verify(openApi)
            .UseParameters(endpointKind, nullable, nonNullableReferenceTypesAsRequired);
      }

      [Theory]
      [MemberData(nameof(TestData))]
      public async Task Should_handle_Class_as_query_parameter(
         EndpointKind endpointKind,
         bool nullable,
         bool nonNullableReferenceTypesAsRequired)
      {
         _nonNullableReferenceTypesAsRequired = nonNullableReferenceTypesAsRequired;

         // TextOrNumberSerializable is IParsable

         if (endpointKind == EndpointKind.MinimalApi)
         {
            if (nullable)
            {
               App.MapGet("/test", ([FromQuery] TextOrNumberSerializable? value = null) => value);
            }
            else
            {
               App.MapGet("/test", ([FromQuery] TextOrNumberSerializable value) => value);
            }
         }
         else
         {
            _controllerType = nullable
                                 ? typeof(TestController.AdHocUnion.Class.QueryString.Nullable)
                                 : typeof(TestController.AdHocUnion.Class.QueryString);
         }

         var openApi = GetOpenApiJsonAsync();

         await Verify(openApi)
            .UseParameters(endpointKind, nullable, nonNullableReferenceTypesAsRequired);
      }

      [Theory]
      [MemberData(nameof(TestData))]
      public async Task Should_handle_Class_as_body_parameter(
         EndpointKind endpointKind,
         bool nullable,
         bool nonNullableReferenceTypesAsRequired)
      {
         _nonNullableReferenceTypesAsRequired = nonNullableReferenceTypesAsRequired;

         if (endpointKind == EndpointKind.MinimalApi)
         {
            if (nullable)
            {
               App.MapPost("/test", ([FromBody] TestUnion_class_string_int? value = null) => value);
            }
            else
            {
               App.MapPost("/test", ([FromBody] TestUnion_class_string_int value) => value);
            }
         }
         else
         {
            _controllerType = nullable
                                 ? typeof(TestController.AdHocUnion.Class.Body.Nullable)
                                 : typeof(TestController.AdHocUnion.Class.Body);
         }

         var openApi = GetOpenApiJsonAsync();

         await Verify(openApi)
            .UseParameters(endpointKind, nullable, nonNullableReferenceTypesAsRequired);
      }

      [Theory]
      [MemberData(nameof(TestDataWithPolymorphism))]
      public async Task Should_handle_Class_with_BaseClass_as_body_parameter(
         EndpointKind endpointKind,
         bool nullable,
         bool nonNrtAsRequired,
         bool polymorphism)
      {
         _nonNullableReferenceTypesAsRequired = nonNrtAsRequired;
         _useOneOfForPolymorphism = polymorphism;

         if (endpointKind == EndpointKind.MinimalApi)
         {
            if (nullable)
            {
               App.MapPost("/test", ([FromBody] TestUnion_class_string_int_with_base_class? value = null) => value);
            }
            else
            {
               App.MapPost("/test", ([FromBody] TestUnion_class_string_int_with_base_class value) => value);
            }
         }
         else
         {
            _controllerType = nullable
                                 ? typeof(TestController.AdHocUnion.Class_with_BaseClass.Body.Nullable)
                                 : typeof(TestController.AdHocUnion.Class_with_BaseClass.Body);
         }

         var openApi = GetOpenApiJsonAsync();

         await Verify(openApi)
            .UseParameters(endpointKind, nullable, nonNrtAsRequired, polymorphism);
      }

      [Theory]
      [MemberData(nameof(TestData))]
      public async Task Should_handle_Class_as_form_parameter(
         EndpointKind endpointKind,
         bool nullable,
         bool nonNullableReferenceTypesAsRequired)
      {
         _nonNullableReferenceTypesAsRequired = nonNullableReferenceTypesAsRequired;

         if (endpointKind == EndpointKind.MinimalApi)
         {
            if (nullable)
            {
               App.MapPost("/test", ([FromForm] TestUnion_class_string_int? value = null) => value);
            }
            else
            {
               App.MapPost("/test", ([FromForm] TestUnion_class_string_int value) => value);
            }
         }
         else
         {
            _controllerType = nullable
                                 ? typeof(TestController.AdHocUnion.Class.Form.Nullable)
                                 : typeof(TestController.AdHocUnion.Class.Form);
         }

         var openApi = GetOpenApiJsonAsync();

         await Verify(openApi)
            .UseParameters(endpointKind, nullable, nonNullableReferenceTypesAsRequired);
      }
   }
}
