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
            public SmartEnum_StringBased Get(SmartEnum_StringBased value)
            {
               return value;
            }

            [Route("/")]
            public class Nullable : ControllerBase
            {
               [HttpGet("/test/{value}")]
               public SmartEnum_StringBased? Get(SmartEnum_StringBased? value = null)
               {
                  return value;
               }
            }
         }

         [Route("/")]
         public class QueryString : ControllerBase
         {
            [HttpGet("/test")]
            public SmartEnum_StringBased Get(SmartEnum_StringBased value)
            {
               return value;
            }

            [Route("/")]
            public class Nullable : ControllerBase
            {
               [HttpGet("/test")]
               public SmartEnum_StringBased? Get(SmartEnum_StringBased? value = null)
               {
                  return value;
               }
            }
         }

         [Route("/")]
         public class Body : ControllerBase
         {
            [HttpPost("/test")]
            public SmartEnum_StringBased Get([FromBody] SmartEnum_StringBased value)
            {
               return value;
            }

            [Route("/")]
            public class Nullable : ControllerBase
            {
               [HttpPost("/test")]
               public SmartEnum_StringBased? Get([FromBody] SmartEnum_StringBased? value = null)
               {
                  return value;
               }
            }
         }

         [Route("/")]
         public class Form : ControllerBase
         {
            [HttpPost("/test")]
            public SmartEnum_StringBased Get([FromForm] SmartEnum_StringBased value)
            {
               return value;
            }

            [Route("/")]
            public class Nullable : ControllerBase
            {
               [HttpPost("/test")]
               public SmartEnum_StringBased? Get([FromForm] SmartEnum_StringBased? value = null)
               {
                  return value;
               }
            }
         }
      }

      public static class StringBased_with_BaseClass
      {
         [Route("/")]
         public class Route : ControllerBase
         {
            [HttpGet("/test/{value}")]
            public SmartEnum_StringBased_with_BaseClass Get(SmartEnum_StringBased_with_BaseClass value)
            {
               return value;
            }

            [Route("/")]
            public class Nullable : ControllerBase
            {
               [HttpGet("/test/{value}")]
               public SmartEnum_StringBased_with_BaseClass? Get(SmartEnum_StringBased_with_BaseClass? value = null)
               {
                  return value;
               }
            }
         }

         [Route("/")]
         public class QueryString : ControllerBase
         {
            [HttpGet("/test")]
            public SmartEnum_StringBased_with_BaseClass Get(SmartEnum_StringBased_with_BaseClass value)
            {
               return value;
            }

            [Route("/")]
            public class Nullable : ControllerBase
            {
               [HttpGet("/test")]
               public SmartEnum_StringBased_with_BaseClass? Get(SmartEnum_StringBased_with_BaseClass? value = null)
               {
                  return value;
               }
            }
         }

         [Route("/")]
         public class Body : ControllerBase
         {
            [HttpPost("/test")]
            public SmartEnum_StringBased_with_BaseClass Get([FromBody] SmartEnum_StringBased_with_BaseClass value)
            {
               return value;
            }

            [Route("/")]
            public class Nullable : ControllerBase
            {
               [HttpPost("/test")]
               public SmartEnum_StringBased_with_BaseClass? Get([FromBody] SmartEnum_StringBased_with_BaseClass? value = null)
               {
                  return value;
               }
            }
         }

         [Route("/")]
         public class Form : ControllerBase
         {
            [HttpPost("/test")]
            public SmartEnum_StringBased_with_BaseClass Get([FromForm] SmartEnum_StringBased_with_BaseClass value)
            {
               return value;
            }

            [Route("/")]
            public class Nullable : ControllerBase
            {
               [HttpPost("/test")]
               public SmartEnum_StringBased_with_BaseClass? Get([FromForm] SmartEnum_StringBased_with_BaseClass? value = null)
               {
                  return value;
               }
            }
         }
      }
   }
}
