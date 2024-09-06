using Serilog;

namespace Thinktecture.DiscriminatedUnions;

public class DiscriminatedUnionsDemos
{
   public static void Demo(ILogger logger)
   {
      logger.Information("""


                         ==== Demo for Union ====

                         """);

      TextOrNumber textOrNumberFromString = "text";
      logger.Information("Implicitly casted from string: {TextOrNumberFromString}", textOrNumberFromString);

      TextOrNumber textOrNumberFromInt = 42;
      logger.Information("Implicitly casted from int: {TextOrNumberFromInt}", textOrNumberFromInt);

      logger.Information("TextOrNumber from string: IsText = {IsText}", textOrNumberFromString.IsText);
      logger.Information("TextOrNumber from string: IsNumber = {IsNumber}", textOrNumberFromString.IsNumber);
      logger.Information("TextOrNumber from int: IsText = {IsText}", textOrNumberFromInt.IsText);
      logger.Information("TextOrNumber from int: IsNumber = {IsNumber}", textOrNumberFromInt.IsNumber);

      logger.Information("TextOrNumber from string: AsText = {AsText}", textOrNumberFromString.AsText);
      logger.Information("TextOrNumber from int: AsNumber = {AsNumber}", textOrNumberFromInt.AsNumber);

      logger.Information("TextOrNumber from string: Value = {Value}", textOrNumberFromString.Value);
      logger.Information("TextOrNumber from int: Value = {Value}", textOrNumberFromInt.Value);

      textOrNumberFromString.Switch(text: s => logger.Information("String Action: {Text}", s),
                                    number: i => logger.Information("Int Action: {Number}", i));

      textOrNumberFromString.Switch(logger,
                                    text: static (l, s) => l.Information("String Action with context: {Text}", s),
                                    number: static (l, i) => l.Information("Int Action with context: {Number}", i));

      var switchResponse = textOrNumberFromInt.Switch(text: static s => $"String Func: {s}",
                                                      number: static i => $"Int Func: {i}");
      logger.Information("{Response}", switchResponse);

      var switchResponseWithContext = textOrNumberFromInt.Switch(123.45,
                                                                 text: static (ctx, s) => $"String Func with context: {ctx} | {s}",
                                                                 number: static (ctx, i) => $"Int Func with context: {ctx} | {i}");
      logger.Information("{Response}", switchResponseWithContext);

      var mapResponse = textOrNumberFromString.Map(text: "Mapped string",
                                                   number: "Mapped int");
      logger.Information("{Response}", mapResponse);
   }
}
