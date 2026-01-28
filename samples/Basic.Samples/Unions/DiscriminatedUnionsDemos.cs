using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Serilog;

namespace Thinktecture.Unions;

public class DiscriminatedUnionsDemos
{
   public static void Demo(ILogger logger)
   {
      DemoForAdHocUnions(logger);
      SerializationDemoForAdHocUnions(logger);
      DemoForUnions(logger);
      DemoForJurisdiction(logger);
      DemoForPartiallyKnownDate(logger);
      DemoForNestedUnionsAndSwitchMapOverloads(logger);
      DemoForStatelessTypes(logger);
   }

   private static void DemoForAdHocUnions(ILogger logger)
   {
      logger.Information("""


                         ==== Demo for Ad Hoc Unions ====

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

   private static void SerializationDemoForAdHocUnions(ILogger logger)
   {
      TextOrNumberSerializable textOrNumberFromString = "text";
      TextOrNumberSerializable textOrNumberFromInt = 42;

      var json = JsonSerializer.Serialize(textOrNumberFromString);
      var deserializedTextOrNumber = JsonSerializer.Deserialize<TextOrNumberSerializable>(json);
      logger.Information("TextOrNumberSerializable (de)serialization: {Json} -> {TextOrNumber}", json, deserializedTextOrNumber);

      json = JsonSerializer.Serialize(textOrNumberFromInt);
      deserializedTextOrNumber = JsonSerializer.Deserialize<TextOrNumberSerializable>(json);
      logger.Information("TextOrNumberSerializable (de)serialization: {Json} -> {TextOrNumber}", json, deserializedTextOrNumber);
   }

   private static void DemoForUnions(ILogger logger)
   {
      DemoForUnionsUsingClass(logger);
      DemoForUnionsUsingRecord(logger);
      DemoForUnionsUsingGeneric(logger);
   }

   private static void DemoForUnionsUsingClass(ILogger logger)
   {
      logger.Information("""


                         ==== Demo for Union (class) ====

                         """);

      Animal animal = new Animal.Cat("Luna");
      Print(animal);

      animal = Animal.Dog.Create("Milo");
      Print(animal);

      animal = Animal.None.Instance;
      Print(animal);

      void Print(Animal a)
      {
         a.Switch(none: _ => logger.Information("[Switch] None"),
                  dog: d => logger.Information("[Switch] Dog: {Dog}", d),
                  cat: c => logger.Information("[Switch] Cat: {Cat}", c));

         a.SwitchPartially(@default: d => logger.Information("[SwitchPartially] Default: {Animal}", d),
                           none: _ => logger.Information("[SwitchPartially] None"),
                           cat: c => logger.Information("[SwitchPartially] Cat: {Cat}", c.Name));

         var result = a.Map(dog: "Dog",
                            none: "None",
                            cat: "Cat");

         logger.Information("[Map] Result: {Result}", result);
      }
   }

   private static void DemoForUnionsUsingRecord(ILogger logger)
   {
      logger.Information("""


                         ==== Demo for Union (record) ====

                         """);

      AnimalRecord animal = new AnimalRecord.Cat("Luna");
      Print(animal);

      animal = new AnimalRecord.Dog("Milo");
      Print(animal);

      animal = AnimalRecord.None.Instance;
      Print(animal);

      void Print(AnimalRecord a)
      {
         a.Switch(none: _ => logger.Information("[Switch] None"),
                  dog: d => logger.Information("[Switch] Dog: {Dog}", d),
                  cat: c => logger.Information("[Switch] Cat: {Cat}", c));

         a.SwitchPartially(@default: d => logger.Information("[SwitchPartially] Default: {Animal}", d),
                           none: _ => logger.Information("[SwitchPartially] None"),
                           cat: c => logger.Information("[SwitchPartially] Cat: {Cat}", c.Name));

         var result = a.Map(dog: "Dog",
                            none: "None",
                            cat: "Cat");

         logger.Information("[Map] Result: {Result}", result);
      }
   }

   private static void DemoForUnionsUsingGeneric(ILogger logger)
   {
      logger.Information("""


                         ==== Demo for Union (generics) ====

                         """);

      Result<int> success = 42;
      Print(success);

      Result<int> error = "Error message";
      Print(error);

      void Print<T>(Result<T> r)
      {
         r.Switch(failure: f => logger.Information("[Switch] Failure: {Failure}", f),
                  success: s => logger.Information("[Switch] Success: {Success}", s));
      }
   }

   private static void DemoForJurisdiction(ILogger logger)
   {
      logger.Information("""


                         ==== Demo for Jurisdiction ====

                         """);

      // Creating different jurisdictions
      var district = Jurisdiction.District.Create("District 42");
      var country = Jurisdiction.Country.Create("DE");
      var unknown = Jurisdiction.Unknown.Instance;

      // Comparing jurisdictions
      var district42 = Jurisdiction.District.Create("DISTRICT 42");
      logger.Information("district == district42: {IsEqual}", district == district42); // true

      var district43 = Jurisdiction.District.Create("District 43");
      logger.Information("district == district43: {IsEqual}", district == district43); // false

      logger.Information("unknown == Jurisdiction.Unknown.Instance: {IsEqual}", unknown == Jurisdiction.Unknown.Instance); // true

      // Validation examples
      try
      {
         var invalidJuristiction = Jurisdiction.Country.Create("DEU"); // Throws ValidationException
      }
      catch (ValidationException ex)
      {
         logger.Information(ex.Message); // "ISO code must be exactly 2 characters long."
      }

      var description = district.Switch(
         country: c => $"Country: {c}",
         federalState: s => $"Federal state: {s}",
         district: d => $"District: {d}",
         unknown: _ => "Unknown",
         continent: c => $"Continent: {c}"
      );

      logger.Information(description);

      // Json serialization
      var json = JsonSerializer.Serialize<Jurisdiction>(district);
      logger.Information(json); //  {"$type":"District","value":"District 42"}

      var deserializedJurisdiction = JsonSerializer.Deserialize<Jurisdiction>(json);
      logger.Information("Deserialized jurisdiction: {Jurisdiction} ({Type})",
                         deserializedJurisdiction,
                         deserializedJurisdiction?.GetType().Name);
   }

   private static void DemoForPartiallyKnownDate(ILogger logger)
   {
      logger.Information("""


                         ==== Demo for Partially-Known Date ====

                         """);
      // Date with only year known
      var historicalEvent = new PartiallyKnownDate.YearOnly(1776);

      // Date with year and month known
      var approximateBirthdate = new PartiallyKnownDate.YearMonth(1980, 6);

      // Fully known date
      var preciseDate = new PartiallyKnownDate.Date(2023, 12, 31);

      // Implicit conversion from DateOnly to PartiallyKnownDate
      PartiallyKnownDate fullDate = new DateOnly(2024, 3, 15); // Date(2024, 3, 15)

      static string FormatDate(PartiallyKnownDate? date)
      {
         if (date is null)
            return "<<null>>";

         return date.Switch(
            yearOnly: y => y.Year.ToString(),
            yearMonth: ym => $"{ym.Year}-{ym.Month:D2}",
            date: ymd => $"{ymd.Year}-{ymd.Month:D2}-{ymd.Day:D2}"
         );
      }

      logger.Information("Historical event: {Date}", FormatDate(historicalEvent));           // "1776"
      logger.Information("Approximate birthdate: {Date}", FormatDate(approximateBirthdate)); // "1980-06"
      logger.Information("Precise date: {Date}", FormatDate(preciseDate));                   // "2023-12-31"

      // Json serialization
      var json = JsonSerializer.Serialize<PartiallyKnownDate>(preciseDate);
      logger.Information(json); // {"$type":"Date","Month":12,"Day":31,"Year":2023}

      var deserializedDate = JsonSerializer.Deserialize<PartiallyKnownDate>(json);
      logger.Information("Deserialized date: {Date}", FormatDate(deserializedDate)); // "2023-12-31"
   }

   private static void DemoForNestedUnionsAndSwitchMapOverloads(ILogger logger)
   {
      logger.Information("""


                         ==== Demo for Nested Unions and Switch/Map Overloads ====

                         """);

      logger.Information("--- Default Parameter Names ---");

      var apiResponse = new ApiResponse.Failure.Unauthorized();

      // With default parameter naming, nested types include their parent union name
      apiResponse.Switch(
         success: success => logger.Information("[Switch] Success"),
         failureNotFound: notFound => logger.Information("[Switch] Not Found"),
         failureUnauthorized: unauthorized => logger.Information("[Switch] Unauthorized")
      );

      // Non-exhaustive overload (stopped at Failure level)
      apiResponse.Switch(
         success: success => logger.Information("[Switch] Success"),
         failure: HandleFailure);

      void HandleFailure(ApiResponse.Failure failure)
      {
         failure.Switch(
            notFound: notFound => logger.Information("[Switch] Not Found"),
            unauthorized: unauthorized => logger.Information("[Switch] Unauthorized")
         );
      }

      logger.Information("--- Simple Parameter Names ---");

      var apiResponseSimple = new ApiResponseWithSimpleParameterNames.Failure.NotFound();

      // With simple parameter naming, nested types use only their own name
      apiResponseSimple.Switch(
         success: success => logger.Information("[Switch] Success"),
         notFound: notFound => logger.Information("[Switch] Not Found"),           // Simple name
         unauthorized: unauthorized => logger.Information("[Switch] Unauthorized") // Simple name
      );

      // Non-exhaustive overload (stopped at Failure level)
      apiResponseSimple.Switch(
         success: success => logger.Information("[Switch] Success"),
         failure: HandleFailureSimple);

      void HandleFailureSimple(ApiResponseWithSimpleParameterNames.Failure failure)
      {
         failure.Switch(
            notFound: notFound => logger.Information("[Switch] Not Found"),
            unauthorized: unauthorized => logger.Information("[Switch] Unauthorized")
         );
      }
   }

   private static void DemoForStatelessTypes(ILogger logger)
   {
      logger.Information("""


                         ==== Demo for Stateless Types (Memory Optimization) ====

                         """);

      // Creating different API responses
      ApiResponseWithStateless successResponse = new SuccessResponse { Data = "Hello, World!" };
      ApiResponseWithStateless notFoundResponse = new NotFound();
      ApiResponseWithStateless unauthorizedResponse = new Unauthorized();

      logger.Information("--- Type Checking ---");
      logger.Information("Success response - IsSuccess: {IsSuccess}, IsNotFound: {IsNotFound}, IsUnauthorized: {IsUnauthorized}",
                         successResponse.IsSuccess, successResponse.IsNotFound, successResponse.IsUnauthorized);
      logger.Information("NotFound response - IsSuccess: {IsSuccess}, IsNotFound: {IsNotFound}, IsUnauthorized: {IsUnauthorized}",
                         notFoundResponse.IsSuccess, notFoundResponse.IsNotFound, notFoundResponse.IsUnauthorized);
      logger.Information("Unauthorized response - IsSuccess: {IsSuccess}, IsNotFound: {IsNotFound}, IsUnauthorized: {IsUnauthorized}",
                         unauthorizedResponse.IsSuccess, unauthorizedResponse.IsNotFound, unauthorizedResponse.IsUnauthorized);

      logger.Information("--- Accessing Values ---");
      // For stateless types, accessors return default(T)
      var notFoundValue = notFoundResponse.AsNotFound;
      logger.Information("NotFound accessor returns: {Value} (default struct)", notFoundValue);

      var unauthorizedValue = unauthorizedResponse.AsUnauthorized;
      logger.Information("Unauthorized accessor returns: {Value} (default struct)", unauthorizedValue);

      // Regular member still returns the actual instance
      var successValue = successResponse.AsSuccess;
      logger.Information("Success accessor returns: {Data}", successValue.Data);

      logger.Information("--- Switch Pattern Matching ---");
      successResponse.Switch(
         success: s => logger.Information("[Switch] Success with data: {Data}", s.Data),
         notFound: _ => logger.Information("[Switch] Not Found"),
         unauthorized: _ => logger.Information("[Switch] Unauthorized")
      );

      notFoundResponse.Switch(
         success: s => logger.Information("[Switch] Success with data: {Data}", s.Data),
         notFound: _ => logger.Information("[Switch] Not Found"),
         unauthorized: _ => logger.Information("[Switch] Unauthorized")
      );

      unauthorizedResponse.Switch(
         success: s => logger.Information("[Switch] Success with data: {Data}", s.Data),
         notFound: _ => logger.Information("[Switch] Not Found"),
         unauthorized: _ => logger.Information("[Switch] Unauthorized")
      );

      logger.Information("--- Map Pattern Matching ---");
      var statusCode = successResponse.Map(
         success: 200,
         notFound: 404,
         unauthorized: 401
      );
      logger.Information("Success response maps to status code: {StatusCode}", statusCode);

      statusCode = notFoundResponse.Map(
         success: 200,
         notFound: 404,
         unauthorized: 401
      );
      logger.Information("NotFound response maps to status code: {StatusCode}", statusCode);

      statusCode = unauthorizedResponse.Map(
         success: 200,
         notFound: 404,
         unauthorized: 401
      );
      logger.Information("Unauthorized response maps to status code: {StatusCode}", statusCode);

      logger.Information("--- Equality Comparison ---");
      var anotherNotFound = new NotFound();
      logger.Information("notFoundResponse == anotherNotFound: {IsEqual}", notFoundResponse == anotherNotFound);

      var anotherUnauthorized = new Unauthorized();
      logger.Information("unauthorizedResponse == anotherUnauthorized: {IsEqual}", unauthorizedResponse == anotherUnauthorized);

      logger.Information("notFoundResponse == unauthorizedResponse: {IsEqual}", notFoundResponse == unauthorizedResponse);
   }
}
