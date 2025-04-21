using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Thinktecture.Runtime.Tests.Swashbuckle.Helpers;
using Thinktecture.Runtime.Tests.TestAdHocUnions;
using Thinktecture.Runtime.Tests.TestEnums;
using Thinktecture.Runtime.Tests.TestValueObjects;
using Xunit.Abstractions;
using static VerifyXunit.Verifier;

namespace Thinktecture.Runtime.Tests.Swashbuckle;

public partial class ThinktectureSchemaFilterTests
{
   public class Objects : ThinktectureSchemaFilterTests
   {
      public Objects(ITestOutputHelper testOutputHelper)
         : base(testOutputHelper)
      {
      }

      public static IEnumerable<object[]> TestData =
         EndpointKind.Items.Select(i => new object[] { i });

      [Theory]
      [MemberData(nameof(TestData))]
      public async Task Should_handle_body_parameter(
         EndpointKind endpointKind)
      {
         if (endpointKind == EndpointKind.MinimalApi)
         {
            App.MapPost("/test", ([FromBody] TestClass value) => value);
         }
         else
         {
            _controllerType = typeof(TestController.Objects<TestClass>.Body);
         }

         var openApi = GetOpenApiJsonAsync();

         await Verify(openApi)
            .UseParameters(endpointKind);
      }

      [Theory]
      [MemberData(nameof(TestData))]
      public async Task Should_handle_form_parameter(
         EndpointKind endpointKind)
      {
         if (endpointKind == EndpointKind.MinimalApi)
         {
            App.MapPost("/test", ([FromForm] TestClass value) => value);
         }
         else
         {
            _controllerType = typeof(TestController.Objects<TestClass>.Form);
         }

         var openApi = GetOpenApiJsonAsync();

         await Verify(openApi)
            .UseParameters(endpointKind);
      }

      public class TestClass
      {
         public TestEnum SmartEnum_Class_StringBased { get; set; }
         public TestEnum? SmartEnum_Class_StringBased_Nullable { get; set; }

         public required StructStringEnum SmartEnum_Struct_StringBased { get; set; }
         public StructStringEnum? SmartEnum_Struct_StringBased_Nullable { get; set; }

         public StringBasedReferenceValueObject KeyedValueObject_Class_StringBased { get; set; }
         public StringBasedReferenceValueObject? KeyedValueObject_Class_StringBased_Nullable { get; set; }

         public required StringBasedStructValueObject KeyedValueObject_Struct_StringBased { get; set; }
         public StringBasedStructValueObject? KeyedValueObject_Struct_StringBased_Nullable { get; set; }

         public Boundary ComplexValueObject_Class { get; set; }
         public Boundary? ComplexValueObject_Class_Nullabe { get; set; }

         public required BoundaryStruct ComplexValueObject_Struct { get; set; }
         public BoundaryStruct? ComplexValueObject_Struct_Nullabe { get; set; }

         public required TestUnion_class_string_int AdHocUnion_Class { get; set; }
         public TestUnion_class_string_int? AdHocUnion_Class_Nullable { get; set; }

         public TestClass()
         {
            SmartEnum_Class_StringBased = TestEnum.Item1;
            KeyedValueObject_Class_StringBased = StringBasedReferenceValueObject.Create("Test");
            ComplexValueObject_Class = Boundary.Create(0, 0);
            AdHocUnion_Class = "Test";
         }
      }
   }
}
