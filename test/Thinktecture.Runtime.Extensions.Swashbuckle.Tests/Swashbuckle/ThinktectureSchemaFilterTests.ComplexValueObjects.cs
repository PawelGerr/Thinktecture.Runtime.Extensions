using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Thinktecture.Runtime.Tests.Swashbuckle.Helpers;
using Thinktecture.Runtime.Tests.TestValueObjects;
using Thinktecture.Swashbuckle;
using Xunit.Abstractions;
using static VerifyXunit.Verifier;

namespace Thinktecture.Runtime.Tests.Swashbuckle;

public partial class ThinktectureSchemaFilterTests
{
   public class ComplexValueObjects : ThinktectureSchemaFilterTests
   {
      public ComplexValueObjects(ITestOutputHelper testOutputHelper)
         : base(testOutputHelper)
      {
      }

      public static IEnumerable<object[]> TestData =
         EndpointKind.Items
                     .CrossJoin([true, false])
                     .Select(i => new object[] { i.Item1, i.Item2 });

      public static IEnumerable<object[]> TestDataWithPolymorphism =
         EndpointKind.Items
                     .CrossJoin([true, false])
                     .CrossJoin([true, false])
                     .Select(i => new object[] { i.Item1, i.Item2, i.Item3 });

      public static IEnumerable<object[]> TestDataWithRequiredMemberEvaluator =
         EndpointKind.Items
                     .CrossJoin([true, false])
                     .CrossJoin(RequiredMemberEvaluator.Items)
                     .CrossJoin([true, false])
                     .Select(i => new object[] { i.Item1, i.Item2, i.Item3, i.Item4 });

      [Theory]
      [MemberData(nameof(TestData))]
      public async Task Should_handle_Class_as_route_parameter(
         EndpointKind endpointKind,
         bool nullable)
      {
         // BoundaryWithFactories is IParsable

         if (endpointKind == EndpointKind.MinimalApi)
         {
            if (nullable)
            {
               App.MapGet("/test/{value}", ([FromRoute] BoundaryWithFactories? value = null) => value);
            }
            else
            {
               App.MapGet("/test/{value}", ([FromRoute] BoundaryWithFactories value) => value);
            }
         }
         else
         {
            _controllerType = nullable
                                 ? typeof(TestController.ComplexValueObjectClass.Class.Route.Nullable)
                                 : typeof(TestController.ComplexValueObjectClass.Class.Route);
         }

         var openApi = GetOpenApiJsonAsync();

         await Verify(openApi)
            .UseParameters(endpointKind, nullable);
      }

      [Theory]
      [MemberData(nameof(TestData))]
      public async Task Should_handle_Class_as_query_parameter(
         EndpointKind endpointKind,
         bool nullable)
      {
         // BoundaryWithFactories is IParsable

         if (endpointKind == EndpointKind.MinimalApi)
         {
            if (nullable)
            {
               App.MapGet("/test", ([FromQuery] BoundaryWithFactories? value = null) => value);
            }
            else
            {
               App.MapGet("/test", ([FromQuery] BoundaryWithFactories value) => value);
            }
         }
         else
         {
            _controllerType = nullable
                                 ? typeof(TestController.ComplexValueObjectClass.Class.QueryString.Nullable)
                                 : typeof(TestController.ComplexValueObjectClass.Class.QueryString);
         }

         var openApi = GetOpenApiJsonAsync();

         await Verify(openApi)
            .UseParameters(endpointKind, nullable);
      }

      [Theory]
      [MemberData(nameof(TestData))]
      public async Task Should_handle_Class_as_body_parameter(
         EndpointKind endpointKind,
         bool nullable)
      {
         if (endpointKind == EndpointKind.MinimalApi)
         {
            if (nullable)
            {
               App.MapPost("/test", ([FromBody] Boundary? value = null) => value);
            }
            else
            {
               App.MapPost("/test", ([FromBody] Boundary value) => value);
            }
         }
         else
         {
            _controllerType = nullable
                                 ? typeof(TestController.ComplexValueObjectClass.Class.Body.Nullable)
                                 : typeof(TestController.ComplexValueObjectClass.Class.Body);
         }

         var openApi = GetOpenApiJsonAsync();

         await Verify(openApi)
            .UseParameters(endpointKind, nullable);
      }

      [Theory]
      [MemberData(nameof(TestDataWithRequiredMemberEvaluator))]
      public async Task Should_handle_required_properties_as_body_parameter(
         EndpointKind endpointKind,
         bool nullable,
         RequiredMemberEvaluator requiredMemberEvaluator,
         bool nonNullableReferenceTypesAsRequired)
      {
         _requiredMemberEvaluator = requiredMemberEvaluator;
         _nonNullableReferenceTypesAsRequired = nonNullableReferenceTypesAsRequired;

         if (endpointKind == EndpointKind.MinimalApi)
         {
            if (nullable)
            {
               App.MapPost("/test", ([FromBody] ValueObjectWithRequiredProperties? value = null) => value);
            }
            else
            {
               App.MapPost("/test", ([FromBody] ValueObjectWithRequiredProperties value) => value);
            }
         }
         else
         {
            _controllerType = nullable
                                 ? typeof(TestController.ComplexValueObjectClass.Class.BodyWithRequiredProperties.Nullable)
                                 : typeof(TestController.ComplexValueObjectClass.Class.BodyWithRequiredProperties);
         }

         var openApi = GetOpenApiJsonAsync();

         await Verify(openApi)
            .UseParameters(endpointKind, nullable, requiredMemberEvaluator, nonNullableReferenceTypesAsRequired);
      }

      [Theory]
      [MemberData(nameof(TestDataWithPolymorphism))]
      public async Task Should_handle_Class_with_BaseClass_as_body_parameter(
         EndpointKind endpointKind,
         bool nullable,
         bool polimorphism)
      {
         _useOneOfForPolymorphism = polimorphism;

         if (endpointKind == EndpointKind.MinimalApi)
         {
            if (nullable)
            {
               App.MapPost("/test", ([FromBody] Boundary_with_BaseClass? value = null) => value);
            }
            else
            {
               App.MapPost("/test", ([FromBody] Boundary_with_BaseClass value) => value);
            }
         }
         else
         {
            _controllerType = nullable
                                 ? typeof(TestController.ComplexValueObjectClass.Class_with_BaseClass.Body.Nullable)
                                 : typeof(TestController.ComplexValueObjectClass.Class_with_BaseClass.Body);
         }

         var openApi = GetOpenApiJsonAsync();

         await Verify(openApi)
            .UseParameters(endpointKind, nullable, polimorphism);
      }

      [Theory]
      [MemberData(nameof(TestData))]
      public async Task Should_handle_Class_as_form_parameter(
         EndpointKind endpointKind,
         bool nullable)
      {
         if (endpointKind == EndpointKind.MinimalApi)
         {
            if (nullable)
            {
               App.MapPost("/test", ([FromForm] Boundary? value = null) => value);
            }
            else
            {
               App.MapPost("/test", ([FromForm] Boundary value) => value);
            }
         }
         else
         {
            _controllerType = nullable
                                 ? typeof(TestController.ComplexValueObjectClass.Class.Form.Nullable)
                                 : typeof(TestController.ComplexValueObjectClass.Class.Form);
         }

         var openApi = GetOpenApiJsonAsync();

         await Verify(openApi)
            .UseParameters(endpointKind, nullable);
      }

      [Theory]
      [MemberData(nameof(TestData))]
      public async Task Should_handle_Struct_as_route_parameter(
         EndpointKind endpointKind,
         bool nullable)
      {
         if (endpointKind == EndpointKind.MinimalApi)
         {
            if (nullable)
            {
               App.MapGet("/test/{value}", (BoundaryStructWithFactories? value = null) => value);
            }
            else
            {
               App.MapGet("/test/{value}", (BoundaryStructWithFactories value) => value);
            }
         }
         else
         {
            _controllerType = nullable
                                 ? typeof(TestController.ComplexValueObjectClass.Struct.Route.Nullable)
                                 : typeof(TestController.ComplexValueObjectClass.Struct.Route);
         }

         var openApi = GetOpenApiJsonAsync();

         await Verify(openApi)
            .UseParameters(endpointKind, nullable);
      }

      [Theory]
      [MemberData(nameof(TestData))]
      public async Task Should_handle_Struct_as_query_parameter(
         EndpointKind endpointKind,
         bool nullable)
      {
         // BoundaryStructWithFactories is IParsable

         if (endpointKind == EndpointKind.MinimalApi)
         {
            if (nullable)
            {
               App.MapGet("/test", (BoundaryStructWithFactories? value = null) => value);
            }
            else
            {
               App.MapGet("/test", (BoundaryStructWithFactories value) => value);
            }
         }
         else
         {
            _controllerType = nullable
                                 ? typeof(TestController.ComplexValueObjectClass.Struct.QueryString.Nullable)
                                 : typeof(TestController.ComplexValueObjectClass.Struct.QueryString);
         }

         var openApi = GetOpenApiJsonAsync();

         await Verify(openApi)
            .UseParameters(endpointKind, nullable);
      }
   }
}
