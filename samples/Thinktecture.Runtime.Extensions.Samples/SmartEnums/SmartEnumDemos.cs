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
      logger.Information("==== Demo for SmartEnum<T> ====");

      logger.Information("Product types: {Types}", ProductType.Items);

      var productType = ProductType.Get("Groceries");
      logger.Information("Product type: {Type}", productType);

      productType = (ProductType)"Groceries";
      logger.Information("Explicitly casted product type: {Type}", productType);

      if (ProductType.TryGet("Housewares", out var housewares))
         logger.Information("Product type {Type} with TryGet found", housewares);

      var validationError = ProductType.Validate("Groceries", null, out var groceries);

      if (validationError is null)
      {
         logger.Information("Product type {Type} found with Validate", groceries);
      }
      else
      {
         logger.Warning("Failed to fetch the product type with Validate. Validation error: {ValidationError}", validationError.ToString());
      }

      string keyOfTheProductType = productType;
      logger.Information("Implicit conversion of ProductType -> string: {Key}", keyOfTheProductType);

      try
      {
         ProductType.Get("Unknown");
         logger.Warning("This line won't be reached.");
      }
      catch (UnknownEnumIdentifierException)
      {
         logger.Information("UnknownEnumIdentifierException is thrown because there is no product type with the key 'Unknown'.");
      }

      productType.Switch(ProductType.Groceries, () => logger.Information("Switch with Action: Groceries"),
                         ProductType.Housewares, () => logger.Information("Switch with Action: Housewares"));

      productType.Switch(logger,
                         ProductType.Groceries, static l => l.Information("Switch with Action: Groceries"),
                         ProductType.Housewares, static l => l.Information("Switch with Action: Housewares"));

      var returnValue = productType.Switch(ProductType.Groceries, static () => "Switch with Func<T>: Groceries",
                                           ProductType.Housewares, static () => "Switch with Func<T>: Housewares");
      logger.Information("{ReturnValue}", returnValue);

      returnValue = productType.Switch(logger,
                                       ProductType.Groceries, static _ => "Switch with Func<T>: Groceries",
                                       ProductType.Housewares, static _ => "Switch with Func<T>: Housewares");

      logger.Information("{ReturnValue}", returnValue);

      returnValue = productType.Map(ProductType.Groceries, "Map: Groceries",
                                    ProductType.Housewares, "Map: Housewares");

      logger.Information("{ReturnValue}", returnValue);

      var parsed = ProductType.TryParse("Groceries", null, out var parsedProductType);
      logger.Information("Success: {Success} Parsed: {Parsed}", parsed, parsedProductType);

      var formatted = ProductGroup.Apple.ToString("000", CultureInfo.InvariantCulture); // 001
      logger.Information("Formatted: {Formatted}", formatted);

      var comparison = ProductGroup.Apple.CompareTo(ProductGroup.Orange); // -1
      logger.Information("Comparison: {Comparison}", comparison);

      var isBigger = ProductGroup.Apple > ProductGroup.Orange;
      logger.Information("{Apple} > {Orange} = {IsBigger}", ProductGroup.Apple, ProductGroup.Orange, isBigger);

      // Comparison with key-member type due to [EnumGeneration(ComparisonOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads)]
      isBigger = ProductGroup.Apple > 42;
      logger.Information("{Apple} > {42} = {IsBigger}", ProductGroup.Apple, 42, isBigger);

      logger.Information("==== Demo for abstract static members ====");

      PrintAllItems<ProductType, string, ProductTypeValidationError>(logger);

      Get<ProductType, string, ProductTypeValidationError>(logger, "Groceries");
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
      where T : IEnum<TKey, T, TValidationError>
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
      where T : IEnum<TKey, T, TValidationError>
      where TKey : notnull
      where TValidationError : class, IValidationError<TValidationError>
   {
      var item = T.Get(key);

      logger.Information("Key '{Key}' => '{Item}'", key, item);
   }
}
