using Serilog;
using Thinktecture.EmptyClass;
using Thinktecture.SmartEnums;
using Thinktecture.ValueObjects;

namespace Thinktecture;

internal class Program
{
   public static void Main()
   {
      var logger = GetLogger();

      SmartEnumDemos.Demo(logger);
      ValueObjectDemos.Demo(logger);
      EmptyActionDemos.Demo();
      EmptyCollectionsDemos.Demo();
   }

   private static ILogger GetLogger()
   {
      return new LoggerConfiguration()
             .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}")
             .CreateLogger();
   }
}
