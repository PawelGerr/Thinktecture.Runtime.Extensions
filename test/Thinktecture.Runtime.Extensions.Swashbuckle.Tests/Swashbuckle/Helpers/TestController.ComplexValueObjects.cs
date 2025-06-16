using Microsoft.AspNetCore.Mvc;
using Thinktecture.Runtime.Tests.TestValueObjects;

namespace Thinktecture.Runtime.Tests.Swashbuckle.Helpers;

public static partial class TestController
{
   public static class ComplexValueObjectClass
   {
      public static class Class
      {
         [Route("/")]
         public class Route : ControllerBase
         {
            [HttpGet("/test/{value}")]
            public BoundaryWithFactories Get([FromRoute] BoundaryWithFactories value)
            {
               return value;
            }

            [Route("/")]
            public class Nullable : ControllerBase
            {
               [HttpGet("/test/{value}")]
               public BoundaryWithFactories? Get([FromRoute] BoundaryWithFactories? value = null)
               {
                  return value;
               }
            }
         }

         [Route("/")]
         public class QueryString : ControllerBase
         {
            [HttpGet("/test")]
            public BoundaryWithFactories Get([FromQuery] BoundaryWithFactories value)
            {
               return value;
            }

            [Route("/")]
            public class Nullable : ControllerBase
            {
               [HttpGet("/test")]
               public BoundaryWithFactories? Get([FromQuery] BoundaryWithFactories? value = null)
               {
                  return value;
               }
            }
         }

         [Route("/")]
         public class Body : ControllerBase
         {
            [HttpPost("/test")]
            public Boundary Get([FromBody] Boundary value)
            {
               return value;
            }

            [Route("/")]
            public class Nullable : ControllerBase
            {
               [HttpPost("/test")]
               public Boundary? Get([FromBody] Boundary? value = null)
               {
                  return value;
               }
            }
         }

         [Route("/")]
         public class BodyWithRequiredProperties : ControllerBase
         {
            [HttpPost("/test")]
            public ValueObjectWithRequiredProperties Get([FromBody] ValueObjectWithRequiredProperties value)
            {
               return value;
            }

            [Route("/")]
            public class Nullable : ControllerBase
            {
               [HttpPost("/test")]
               public ValueObjectWithRequiredProperties? Get([FromBody] ValueObjectWithRequiredProperties? value = null)
               {
                  return value;
               }
            }
         }

         [Route("/")]
         public class Form : ControllerBase
         {
            [HttpPost("/test")]
            public Boundary Get([FromForm] Boundary value)
            {
               return value;
            }

            [Route("/")]
            public class Nullable : ControllerBase
            {
               [HttpPost("/test")]
               public Boundary? Get([FromForm] Boundary? value = null)
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
            public Boundary_with_BaseClass Get([FromBody] Boundary_with_BaseClass value)
            {
               return value;
            }

            [Route("/")]
            public class Nullable : ControllerBase
            {
               [HttpPost("/test")]
               public Boundary_with_BaseClass? Get([FromBody] Boundary_with_BaseClass? value = null)
               {
                  return value;
               }
            }
         }
      }

      public static class Struct
      {
         [Route("/")]
         public class Route : ControllerBase
         {
            [HttpGet("/test/{value}")]
            public BoundaryStructWithFactories Get([FromRoute] BoundaryStructWithFactories value)
            {
               return value;
            }

            [Route("/")]
            public class Nullable : ControllerBase
            {
               [HttpGet("/test/{value}")]
               public BoundaryStructWithFactories? Get([FromRoute] BoundaryStructWithFactories? value = null)
               {
                  return value;
               }
            }
         }

         [Route("/")]
         public class QueryString : ControllerBase
         {
            [HttpGet("/test")]
            public BoundaryStructWithFactories Get([FromQuery] BoundaryStructWithFactories value)
            {
               return value;
            }

            [Route("/")]
            public class Nullable : ControllerBase
            {
               [HttpGet("/test")]
               public BoundaryStructWithFactories? Get([FromQuery] BoundaryStructWithFactories? value = null)
               {
                  return value;
               }
            }
         }
      }
   }
}
