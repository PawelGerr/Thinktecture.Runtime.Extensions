using Microsoft.AspNetCore.Mvc;
using Thinktecture.Runtime.Tests.TestValueObjects;

namespace Thinktecture.Runtime.Tests.Swashbuckle.Helpers;

public static partial class TestController
{
   public static class KeyedValueObjectClass
   {
      public static class StringBased
      {
         [Route("/")]
         public class Route : ControllerBase
         {
            [HttpGet("/test/{value}")]
            public StringBasedReferenceValueObject Get(StringBasedReferenceValueObject value)
            {
               return value;
            }

            [Route("/")]
            public class Nullable : ControllerBase
            {
               [HttpGet("/test/{value}")]
               public StringBasedReferenceValueObject? Get(StringBasedReferenceValueObject? value = null)
               {
                  return value;
               }
            }
         }

         [Route("/")]
         public class QueryString : ControllerBase
         {
            [HttpGet("/test")]
            public StringBasedReferenceValueObject Get(StringBasedReferenceValueObject value)
            {
               return value;
            }

            [Route("/")]
            public class Nullable : ControllerBase
            {
               [HttpGet("/test")]
               public StringBasedReferenceValueObject? Get(StringBasedReferenceValueObject? value = null)
               {
                  return value;
               }
            }
         }

         [Route("/")]
         public class Body : ControllerBase
         {
            [HttpPost("/test")]
            public StringBasedReferenceValueObject Get([FromBody] StringBasedReferenceValueObject value)
            {
               return value;
            }

            [Route("/")]
            public class Nullable : ControllerBase
            {
               [HttpPost("/test")]
               public StringBasedReferenceValueObject? Get([FromBody] StringBasedReferenceValueObject? value = null)
               {
                  return value;
               }
            }
         }

         [Route("/")]
         public class Form : ControllerBase
         {
            [HttpPost("/test")]
            public StringBasedReferenceValueObject Get([FromForm] StringBasedReferenceValueObject value)
            {
               return value;
            }

            [Route("/")]
            public class Nullable : ControllerBase
            {
               [HttpPost("/test")]
               public StringBasedReferenceValueObject? Get([FromForm] StringBasedReferenceValueObject? value = null)
               {
                  return value;
               }
            }
         }
      }

      // ReSharper disable once InconsistentNaming
      public static class StringBased_with_BaseClass
      {
         [Route("/")]
         public class Route : ControllerBase
         {
            [HttpGet("/test/{value}")]
            public StringBasedReferenceValueObject_with_BaseClass Get(StringBasedReferenceValueObject_with_BaseClass value)
            {
               return value;
            }

            [Route("/")]
            public class Nullable : ControllerBase
            {
               [HttpGet("/test/{value}")]
               public StringBasedReferenceValueObject_with_BaseClass? Get(StringBasedReferenceValueObject_with_BaseClass? value = null)
               {
                  return value;
               }
            }
         }

         [Route("/")]
         public class QueryString : ControllerBase
         {
            [HttpGet("/test")]
            public StringBasedReferenceValueObject_with_BaseClass Get(StringBasedReferenceValueObject_with_BaseClass value)
            {
               return value;
            }

            [Route("/")]
            public class Nullable : ControllerBase
            {
               [HttpGet("/test")]
               public StringBasedReferenceValueObject_with_BaseClass? Get(StringBasedReferenceValueObject_with_BaseClass? value = null)
               {
                  return value;
               }
            }
         }

         [Route("/")]
         public class Body : ControllerBase
         {
            [HttpPost("/test")]
            public StringBasedReferenceValueObject_with_BaseClass Get([FromBody] StringBasedReferenceValueObject_with_BaseClass value)
            {
               return value;
            }

            [Route("/")]
            public class Nullable : ControllerBase
            {
               [HttpPost("/test")]
               public StringBasedReferenceValueObject_with_BaseClass? Get([FromBody] StringBasedReferenceValueObject_with_BaseClass? value = null)
               {
                  return value;
               }
            }
         }

         [Route("/")]
         public class Form : ControllerBase
         {
            [HttpPost("/test")]
            public StringBasedReferenceValueObject_with_BaseClass Get([FromForm] StringBasedReferenceValueObject_with_BaseClass value)
            {
               return value;
            }

            [Route("/")]
            public class Nullable : ControllerBase
            {
               [HttpPost("/test")]
               public StringBasedReferenceValueObject_with_BaseClass? Get([FromForm] StringBasedReferenceValueObject_with_BaseClass? value = null)
               {
                  return value;
               }
            }
         }
      }
   }

   public static class KeyedValueObjectStruct
   {
      public static class StringBased
      {
         [Route("/")]
         public class Route : ControllerBase
         {
            [HttpGet("/test/{value}")]
            public StringBasedStructValueObject Get(StringBasedStructValueObject value)
            {
               return value;
            }

            [Route("/")]
            public class Nullable : ControllerBase
            {
               [HttpGet("/test/{value}")]
               public StringBasedStructValueObject? Get(StringBasedStructValueObject? value = null)
               {
                  return value;
               }
            }
         }

         [Route("/")]
         public class QueryString : ControllerBase
         {
            [HttpGet("/test")]
            public StringBasedStructValueObject Get(StringBasedStructValueObject value)
            {
               return value;
            }

            [Route("/")]
            public class Nullable : ControllerBase
            {
               [HttpGet("/test")]
               public StringBasedStructValueObject? Get(StringBasedStructValueObject? value = null)
               {
                  return value;
               }
            }
         }
      }
   }
}
