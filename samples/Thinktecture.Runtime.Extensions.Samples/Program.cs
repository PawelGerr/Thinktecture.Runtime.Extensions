using System;
using System.Text;
using Serilog;
using Thinktecture.EmptyClass;
using Thinktecture.EnumLikeClasses;

namespace Thinktecture
{
   internal class Program
   {
      public static void Main()
      {
         var logger = GetLogger();

         EnumLikeClassDemos.Demo(logger);
         EmptyActionDemos.Demo();
         EmptyCollectionsDemos.Demo();
      }

      private static ILogger GetLogger()
      {
         return new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();
      }
   }
}
