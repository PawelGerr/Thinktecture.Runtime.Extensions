using System;
using System.Text;
using Serilog;
using Thinktecture.Runtime.Extensions.Samples.EnumLikeClass;

namespace Thinktecture.Runtime.Extensions.Samples
{
   public class Program
   {
      public static void Main()
      {
         var logger = GetLogger();

         EnumLikeClassDemos.Demo(logger);
      }

      private static ILogger GetLogger()
      {
         return new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();
      }
   }
}
