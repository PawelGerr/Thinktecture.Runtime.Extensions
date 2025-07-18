using Microsoft.AspNetCore.Mvc;
using Thinktecture.Runtime.Tests.TestAdHocUnions;

namespace Thinktecture.Runtime.Tests.Swashbuckle.Helpers;

public static partial class TestController
{
   public static class AdHocUnion
   {
      public static class Class
      {
         [Route("/")]
         public class Route : ControllerBase
         {
            [HttpGet("/test/{value}")]
            public TextOrNumberSerializable Get([FromRoute] TextOrNumberSerializable value)
            {
               return value;
            }

            [Route("/")]
            public class Nullable : ControllerBase
            {
               [HttpGet("/test/{value}")]
               public TextOrNumberSerializable? Get([FromRoute] TextOrNumberSerializable? value = null)
               {
                  return value;
               }
            }
         }

         [Route("/")]
         public class QueryString : ControllerBase
         {
            [HttpGet("/test")]
            public TextOrNumberSerializable Get([FromQuery] TextOrNumberSerializable value)
            {
               return value;
            }

            [Route("/")]
            public class Nullable : ControllerBase
            {
               [HttpGet("/test")]
               public TextOrNumberSerializable? Get([FromQuery] TextOrNumberSerializable? value = null)
               {
                  return value;
               }
            }
         }

         [Route("/")]
         public class Body : ControllerBase
         {
            [HttpPost("/test")]
            public TestUnion_class_string_int Get([FromBody] TestUnion_class_string_int value)
            {
               return value;
            }

            [Route("/")]
            public class Nullable : ControllerBase
            {
               [HttpPost("/test")]
               public TestUnion_class_string_int? Get([FromBody] TestUnion_class_string_int? value = null)
               {
                  return value;
               }
            }
         }

         [Route("/")]
         public class Form : ControllerBase
         {
            [HttpPost("/test")]
            public TestUnion_class_string_int Get([FromForm] TestUnion_class_string_int value)
            {
               return value;
            }

            [Route("/")]
            public class Nullable : ControllerBase
            {
               [HttpPost("/test")]
               public TestUnion_class_string_int? Get([FromForm] TestUnion_class_string_int? value = null)
               {
                  return value;
               }
            }
         }
      }

      // ReSharper disable once InconsistentNaming
      public static class Class_with_BaseClass
      {
         [Route("/")]
         public class Body : ControllerBase
         {
            [HttpPost("/test")]
            public TestUnion_class_string_int_with_base_class Get([FromBody] TestUnion_class_string_int_with_base_class value)
            {
               return value;
            }

            [Route("/")]
            public class Nullable : ControllerBase
            {
               [HttpPost("/test")]
               public TestUnion_class_string_int_with_base_class? Get([FromBody] TestUnion_class_string_int_with_base_class? value = null)
               {
                  return value;
               }
            }
         }
      }
   }
}
