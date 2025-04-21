using Microsoft.AspNetCore.Mvc;

namespace Thinktecture.Runtime.Tests.Swashbuckle.Helpers;

public static partial class TestController
{
   public static class Objects<T>
   {
      [Route("/")]
      public class Route : ControllerBase
      {
         [HttpGet("/test/{value}")]
         public T Get([FromRoute] T value)
         {
            return value;
         }
      }

      [Route("/")]
      public class QueryString : ControllerBase
      {
         [HttpGet("/test")]
         public T Get([FromQuery] T value)
         {
            return value;
         }
      }

      [Route("/")]
      public class Body : ControllerBase
      {
         [HttpPost("/test")]
         public T Get([FromBody] T value)
         {
            return value;
         }
      }

      [Route("/")]
      public class Form : ControllerBase
      {
         [HttpPost("/test")]
         public T Get([FromForm] T value)
         {
            return value;
         }
      }
   }
}
