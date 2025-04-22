using System;
using System.Globalization;
using System.IO;
using CsvHelper;
using CsvHelper.Configuration;
using Serilog;

namespace Thinktecture.SmartEnums;

public class SmartEnumDemos
{
   public static void Demo(ILogger logger)
   {
      DemoForSmartEnum(logger);
      DemoForSmartEnumWithCustomComparer(logger);
      DemoForValidatableEnum(logger);

      DemoForDailySalesCsvImporterType(logger);
      DemoForMonthlySalesCsvImporterType(logger);
   }

   private static void DemoForSmartEnum(ILogger logger)
   {
      logger.Information("""


                         ==== Demo for SmartEnum<T> ====

                         """);

      // Iteration over all items
      logger.Information("Product types: {Types}", ProductType.Items);

      // Iteration over all items using "abstract static member" Items
      PrintAllItems<ProductType, string, ProductTypeValidationError>(logger);

      // Lookup/conversion from key type (throws if invalid)
      var productType = ProductType.Get("Groceries");
      logger.Information("Product type: {Type}", productType);

      try
      {
         // Throws if the provided value is invalid/unknown
         ProductType.Get("Unknown");
         logger.Warning("This line won't be reached.");
      }
      catch (UnknownSmartEnumIdentifierException)
      {
         logger.Information("UnknownSmartEnumIdentifierException is thrown because there is no product type with the key 'Unknown'.");
      }

      // Conversion to key type (throws if invalid)
      productType = (ProductType)"Groceries";
      logger.Information("Explicitly casted product type: {Type}", productType);

      // Conversion from key type
      if (ProductType.TryGet("Housewares", out var housewares))
         logger.Information("Product type {Type} with TryGet found", housewares);

      // Conversion from key type (this overload is mainly used for writing other libs)
      var validationError = ProductType.Validate("Groceries", null, out var groceries);

      if (validationError is null)
      {
         logger.Information("Product type {Type} found with Validate", groceries);
      }
      else
      {
         logger.Warning("Failed to fetch the product type with Validate. Validation error: {ValidationError}", validationError.ToString());
      }

      // Implicit conversion to key type
      string keyOfTheProductType = productType;
      logger.Information("Implicit conversion of ProductType -> string: {Key}", keyOfTheProductType);

      SwitchWithAction(logger);
      SwitchWithFunc(logger);
      Map(logger);

      // Implements IParsable if the key is a string
      var parsed = ProductType.TryParse("Groceries", null, out var parsedProductType);
      logger.Information("Success: {Success} Parsed: {Parsed}", parsed, parsedProductType);

      // Implements IFormattable if the key is IFormattable
      var formatted = ProductGroup.Apple.ToString("000", CultureInfo.InvariantCulture); // 001
      logger.Information("Formatted: {Formatted}", formatted);

      // Implements IComparable if the key is IComparable
      var comparison = ProductGroup.Apple.CompareTo(ProductGroup.Orange); // -1
      logger.Information("Comparison: {Comparison}", comparison);

      // Implements comparison operators if the key is comparable
      var isBigger = ProductGroup.Apple > ProductGroup.Orange;
      logger.Information("{Apple} > {Orange} = {IsBigger}", ProductGroup.Apple, ProductGroup.Orange, isBigger);

      // Comparison with key-member type due to [EnumGeneration(ComparisonOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads)]
      isBigger = ProductGroup.Apple > 42;
      logger.Information("{Apple} > {42} = {IsBigger}", ProductGroup.Apple, 42, isBigger);

      logger.Information("==== Demo for abstract static members ====");

      Get<ProductType, string, ProductTypeValidationError>(logger, "Groceries");
   }

   private static void Map(ILogger logger)
   {
      var productType = ProductType.Groceries;

      // Map
      var returnValue = productType.Map(groceries: "Map: Groceries",
                                        housewares: "Map: Housewares");
      logger.Information("{ReturnValue}", returnValue);

      // MapPartially
      returnValue = productType.MapPartially(@default: "MapPartially: default",
                                             housewares: "MapPartially: Housewares");
      logger.Information("{ReturnValue}", returnValue);
   }

   private static void SwitchWithFunc(ILogger logger)
   {
      var productType = ProductType.Groceries;

      // Switch with Func<TResult>
      var returnValue = productType.Switch(groceries: static () => "Switch with Func<TResult>: Groceries",
                                           housewares: static () => "Switch with Func<TResult>: Housewares");
      logger.Information("{ReturnValue}", returnValue);

      // Switch with Func<TState, TResult>
      returnValue = productType.Switch(logger,
                                       groceries: static _ => "Switch with Func<TState, TResult>: Groceries",
                                       housewares: static _ => "Switch with Func<TState, TResult>: Housewares");
      logger.Information("{ReturnValue}", returnValue);

      // SwitchPartially with Func<TResult>
      returnValue = productType.SwitchPartially(@default: static item => $"SwitchPartially with Func<TResult>: default ('{item}')",
                                                groceries: static () => "SwitchPartially with Func<TResult>: Groceries");
      logger.Information("{ReturnValue}", returnValue);

      returnValue = productType.SwitchPartially(@default: item => $"SwitchPartially with Func<TResult>: '{item}' (default only)");
      logger.Information("{ReturnValue}", returnValue);

      returnValue = ProductType.Housewares.SwitchPartially(@default: item => $"SwitchPartially with Func<TResult>: default ('{item}')",
                                                           groceries: () => "SwitchPartially with Func<TResult>: Groceries");
      logger.Information("{ReturnValue}", returnValue);

      // SwitchPartially with Func<TState, TResult>
      returnValue = productType.SwitchPartially(logger,
                                                @default: static (_, item) => $"SwitchPartially with Func<TState, TResult>: default ('{item}')",
                                                groceries: static _ => "SwitchPartially with Func<TState, TResult>: Groceries");
      logger.Information("{ReturnValue}", returnValue);

      returnValue = productType.SwitchPartially(logger,
                                                @default: static (_, item) => $"SwitchPartially with Func<TState, TResult>: {item} (default only)");
      logger.Information("{ReturnValue}", returnValue);

      returnValue = ProductType.Housewares.SwitchPartially(logger,
                                                           @default: static (_, item) => $"SwitchPartially with Func<TState, TResult>: default ('{item}')",
                                                           groceries: static _ => "SwitchPartially with Func<TState, TResult>: Groceries");
      logger.Information("{ReturnValue}", returnValue);
   }

   private static void SwitchWithAction(ILogger logger)
   {
      var productType = ProductType.Groceries;

      // Switch with Action
      productType.Switch(groceries: () => logger.Information("Switch with Action: Groceries"),
                         housewares: () => logger.Information("Switch with Action: Housewares"));

      // Switch with Action<TState>
      productType.Switch(logger,
                         groceries: static l => l.Information("Switch with Action: Groceries"),
                         housewares: static l => l.Information("Switch with Action: Housewares"));

      // Switch of a "validatable" enum
      ProductGroupStruct.Get(42).Switch(invalid: invalidItem => Console.WriteLine($"Invalid item: {invalidItem}"),
                                        apple: () => Console.WriteLine("apple"),
                                        orange: () => Console.WriteLine("orange"));

      // SwitchPartially with Action
      productType.SwitchPartially(@default: item => logger.Information("SwitchPartially with Action: default ('{Item}')", item),
                                  groceries: () => logger.Information("SwitchPartially with Action: Groceries"));

      productType.SwitchPartially(groceries: () => logger.Information("SwitchPartially with Action: Groceries (no default)"));
      productType.SwitchPartially(@default: item => logger.Information("SwitchPartially with Action: {Item} (default only)", item));

      ProductType.Housewares.SwitchPartially(@default: item => logger.Information("SwitchPartially with Action: default ('{Item}')", item),
                                             groceries: () => logger.Information("SwitchPartially with Action: Groceries"));

      // Switch of a "validatable" enum
      ProductGroupStruct.Get(42).SwitchPartially(invalid: invalidItem => Console.WriteLine($"SwitchPartially with Action: Invalid item ({invalidItem})"),
                                                 apple: () => Console.WriteLine("SwitchPartially with Action: apple"),
                                                 orange: () => Console.WriteLine("SwitchPartially with Action: orange"));

      // SwitchPartially with Action<TState>
      productType.SwitchPartially(logger,
                                  @default: static (l, item) => l.Information("SwitchPartially with Action<TState>: default ('{Item}')", item),
                                  groceries: static l => l.Information("SwitchPartially with Action<TState>: Groceries"));

      productType.SwitchPartially(logger,
                                  groceries: static l => l.Information("SwitchPartially with Action<TState>: Groceries (no default)"));

      productType.SwitchPartially(logger,
                                  @default: static (l, item) => l.Information("SwitchPartially with Action<TState>: {Item} (default only)", item));

      ProductType.Housewares.SwitchPartially(logger,
                                             @default: static (l, item) => l.Information("SwitchPartially with Action<TState>: default ('{Item}')", item),
                                             groceries: static l => l.Information("SwitchPartially with Action<TState>: Groceries"));
   }

   private static void DemoForSmartEnumWithCustomComparer(ILogger logger)
   {
      logger.Information("""


                         ==== Demo for SmartEnum<T> with custom Comparer ====

                         """);

      var lowerCased = ProductCategoryWithCaseSensitiveComparer.FruitsLowerCased;
      var upperCased = ProductCategoryWithCaseSensitiveComparer.FruitsUpperCased;

      logger.Information("With case-sensitive comparer: \"{LowerCased}\".Equals(\"{UpperCased}\") = {Result}",
                         lowerCased,
                         upperCased,
                         lowerCased.Equals(upperCased));

      logger.Information("With case-sensitive comparer: \"{LowerCased}\" == \"{UpperCase}\" = {Result}",
                         lowerCased,
                         upperCased,
                         lowerCased == upperCased);

      logger.Information("With case-sensitive comparer: \"{LowerCased}\".CompareTo(\"{UpperCased}\") = {Result}",
                         lowerCased,
                         upperCased,
                         lowerCased.CompareTo(upperCased));

      logger.Information("With case-sensitive comparer: \"{LowerCased}\" > \"{UpperCased}\" = {Result}",
                         lowerCased,
                         upperCased,
                         lowerCased > upperCased);
   }

   private static void DemoForValidatableEnum(ILogger logger)
   {
      logger.Information("""


                         ==== Demo for SmartEnum<T>(IsValidatable = true) ====

                         """);

      var categories = ProductCategory.Items;
      logger.Information("Categories: {Categories}", categories);

      var category = ProductCategory.Get("Fruits");
      logger.Information("Category: {Category}", category);

      // Throws "InvalidOperationException" if not valid
      category.EnsureValid();

      if (ProductCategory.TryGet("fruits", out var fruits))
         logger.Information("Category {Category} with TryGet found", fruits);

      string keyOfTheCategory = category;
      logger.Information("Implicit conversion of Category -> string: {Key}", keyOfTheCategory);

      var unknownCategory = ProductCategory.Get("Grains");
      logger.Information("unknownCategory.Key: {CategoryKey}", unknownCategory.Name);
      logger.Information("unknownCategory.isValid: {IsValid}", unknownCategory.IsValid);
   }

   private static void DemoForDailySalesCsvImporterType(ILogger logger)
   {
      logger.Information("""


                         ==== Demo for Daily CSV-Importer-Type ====

                         """);

      var type = SalesCsvImporterType.Daily;

      using var textReader = new StringReader("""
                                              id,datetime,volume
                                              1,20230425 10:45,345.67
                                              """);

      using var csvReader = new CsvReader(textReader, new CsvConfiguration(CultureInfo.InvariantCulture) { HasHeaderRecord = true });

      csvReader.Read();
      csvReader.ReadHeader();

      while (csvReader.Read())
      {
         var articleId = csvReader.GetField<int>(type.ArticleIdIndex);
         var volume = csvReader.GetField<decimal>(type.VolumeIndex);
         var dateTime = type.GetDateTime(csvReader);

         logger.Information("CSV ({Type}): Article-Id={Id}, DateTime={DateTime}, Volume={Volume}", type, articleId, dateTime, volume);
      }
   }

   private static void DemoForMonthlySalesCsvImporterType(ILogger logger)
   {
      logger.Information("""


                         ==== Demo for Monthly CSV-Importer-Type ====

                         """);

      var with_3_columns = true;
      var csvWith3Columns = """
         volume,datetime,id
         123.45,20230426 11:50,2
         """;

      var csvWith4Columns = """
         volume,quantity,id,datetime
         123.45,42,2,2023-04-25
         """;

      var type = SalesCsvImporterType.Monthly;
      var csv = with_3_columns ? csvWith3Columns : csvWith4Columns;

      using var textReader = new StringReader(csv);
      using var csvReader = new CsvReader(textReader, new CsvConfiguration(CultureInfo.InvariantCulture) { HasHeaderRecord = true });

      csvReader.Read();
      csvReader.ReadHeader();

      while (csvReader.Read())
      {
         var articleId = csvReader.GetField<int>(type.ArticleIdIndex);
         var volume = csvReader.GetField<decimal>(type.VolumeIndex);
         var dateTime = type.GetDateTime(csvReader);

         logger.Information("CSV ({Type}): Article-Id={Id}, DateTime={DateTime}, Volume={Volume}", type, articleId, dateTime, volume);
      }
   }

   private static void PrintAllItems<T, TKey, TValidationError>(ILogger logger)
      where T : ISmartEnum<TKey, T, TValidationError>
      where TKey : notnull
      where TValidationError : class, IValidationError<TValidationError>
   {
      logger.Information("Print all items of '{Name}':", typeof(T).Name);

      foreach (var item in T.Items)
      {
         logger.Information("Item: {Item}", item);
      }
   }

   private static void Get<T, TKey, TValidationError>(ILogger logger, TKey key)
      where T : ISmartEnum<TKey, T, TValidationError>
      where TKey : notnull
      where TValidationError : class, IValidationError<TValidationError>
   {
      var item = T.Get(key);

      logger.Information("Key '{Key}' => '{Item}'", key, item);
   }
}
