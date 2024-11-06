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

      textOrNumberFromString.Switch(text: s => logger.Information("[Switch] String Action: {Text}", s),
                                    number: i => logger.Information("[Switch] Int Action: {Number}", i));

      textOrNumberFromString.SwitchPartially(@default: i => logger.Information("[SwitchPartially] Default Action: {Number}", i),
                                             text: s => logger.Information("[SwitchPartially] String Action: {Text}", s));

      textOrNumberFromString.Switch(logger,
                                    text: static (l, s) => l.Information("[Switch] String Action with state: {Text}", s),
                                    number: static (l, i) => l.Information("[Switch] Int Action with state: {Number}", i));

      textOrNumberFromString.SwitchPartially(logger,
                                             @default: static (l, i) => l.Information("[SwitchPartially] Default Action with state: {Number}", i),
                                             text: static (l, s) => l.Information("[SwitchPartially] String Action with state: {Text}", s));

      var switchResponse = textOrNumberFromInt.Switch(text: static s => $"[Switch] String Func: {s}",
                                                      number: static i => $"[Switch] Int Func: {i}");
      logger.Information("{Response}", switchResponse);

      var switchPartiallyResponse = textOrNumberFromInt.SwitchPartially(@default: static i => $"[SwitchPartially] Default Func: {i}",
                                                                        text: static s => $"[SwitchPartially] String Func: {s}");
      logger.Information("{Response}", switchPartiallyResponse);

      var switchResponseWithContext = textOrNumberFromInt.Switch(123.45,
                                                                 text: static (state, s) => $"[Switch] String Func with state: {state} | {s}",
                                                                 number: static (state, i) => $"[Switch] Int Func with state: {state} | {i}");
      logger.Information("{Response}", switchResponseWithContext);

      var switchPartiallyResponseWithContext = textOrNumberFromInt.SwitchPartially(123.45,
                                                                                   text: static (state, s) => $"[SwitchPartially] String Func with state: {state} | {s}",
                                                                                   @default: static (state, i) => $"[SwitchPartially] Default Func with state: {state} | {i}");
      logger.Information("{Response}", switchPartiallyResponseWithContext);

      var mapResponse = textOrNumberFromString.Map(text: "[Map] Mapped string",
                                                   number: "[Map] Mapped int");
      logger.Information("{Response}", mapResponse);

      var mapPartiallyResponse = textOrNumberFromString.MapPartially(@default: "[MapPartially] Mapped default",
                                                                     text: "[MapPartially] Mapped string");
      logger.Information("{Response}", mapPartiallyResponse);
   }
}
