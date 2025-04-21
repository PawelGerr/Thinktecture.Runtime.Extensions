using Microsoft.AspNetCore.Mvc;
using Thinktecture.Runtime.Tests.TestEnums;

namespace Thinktecture.Runtime.Tests.Swashbuckle.Helpers;

public static partial class TestController
{
   public static class SmartEnumClass
   {
      public static class StringBased
      {
         [Route("/")]
         public class Route : ControllerBase
         {
            [HttpGet("/test/{value}")]
            public TestEnum Get(TestEnum value)
            {
               return value;
            }

            [Route("/")]
            public class Nullable : ControllerBase
            {
               [HttpGet("/test/{value}")]
               public TestEnum? Get(TestEnum? value = null)
               {
                  return value;
               }
            }
         }

         [Route("/")]
         public class QueryString : ControllerBase
         {
            [HttpGet("/test")]
            public TestEnum Get(TestEnum value)
            {
               return value;
            }

            [Route("/")]
            public class Nullable : ControllerBase
            {
               [HttpGet("/test")]
               public TestEnum? Get(TestEnum? value = null)
               {
                  return value;
               }
            }
         }

         [Route("/")]
         public class Body : ControllerBase
         {
            [HttpPost("/test")]
            public TestEnum Get([FromBody] TestEnum value)
            {
               return value;
            }

            [Route("/")]
            public class Nullable : ControllerBase
            {
               [HttpPost("/test")]
               public TestEnum? Get([FromBody] TestEnum? value = null)
               {
                  return value;
               }
            }
         }

         [Route("/")]
         public class Form : ControllerBase
         {
            [HttpPost("/test")]
            public TestEnum Get([FromForm] TestEnum value)
            {
               return value;
            }

            [Route("/")]
            public class Nullable : ControllerBase
            {
               [HttpPost("/test")]
               public TestEnum? Get([FromForm] TestEnum? value = null)
               {
                  return value;
               }
            }
         }
      }
   }

   public static class SmartEnumStruct
   {
      public static class StringBased
      {
         [Route("/")]
         public class Route : ControllerBase
         {
            [HttpGet("/test/{value}")]
            public StructStringEnum Get(StructStringEnum value)
            {
               return value;
            }

            [Route("/")]
            public class Nullable : ControllerBase
            {
               [HttpGet("/test/{value}")]
               public StructStringEnum? Get(StructStringEnum? value = null)
               {
                  return value;
               }
            }
         }

         [Route("/")]
         public class QueryString : ControllerBase
         {
            [HttpGet("/test")]
            public StructStringEnum Get(StructStringEnum value)
            {
               return value;
            }

            [Route("/")]
            public class Nullable : ControllerBase
            {
               [HttpGet("/test")]
               public StructStringEnum? Get(StructStringEnum? value = null)
               {
                  return value;
               }
            }
         }
      }
   }
}
