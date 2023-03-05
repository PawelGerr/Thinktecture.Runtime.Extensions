using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using Serilog;

namespace Thinktecture.SmartEnums;

public class SmartEnumDemos
{
   public static void Demo(ILogger logger)
   {
      DemoForNonValidatableEnum(logger);
      DemoForValidatableEnum(logger);
   }

   private static void DemoForNonValidatableEnum(ILogger logger)
   {
      logger.Information("==== Demo for IEnum<T> ====");

      logger.Information("Product types: {Types}", ProductType.Items);

      var productType = ProductType.Get("Groceries");
      logger.Information("Product type: {Type}", productType);

      productType = (ProductType)"Groceries";
      logger.Information("Explicitly casted product type: {Type}", productType);

      if (ProductType.TryGet("Housewares", out var housewares))
         logger.Information("Product type {Type} with TryGet found", housewares);

      var validationResult = ProductType.Validate("Groceries", out var groceries);

      if (validationResult == ValidationResult.Success)
      {
         logger.Information("Product type {Type} found with Validate", groceries);
      }
      else
      {
         logger.Warning("Failed to fetch the product type with Validate. Validation result: {ValidationResult}", validationResult!.ErrorMessage);
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

      returnValue = productType.Switch(logger,
                                       ProductType.Groceries, static l => "Switch with Func<T>: Groceries",
                                       ProductType.Housewares, static l => "Switch with Func<T>: Housewares");

      logger.Information(returnValue);

      var formatted = ProductGroup.Apple.ToString("000", CultureInfo.InvariantCulture); // 001
      logger.Information("Formatted: {Formatted}", formatted);

      var comparison = ProductGroup.Apple.CompareTo(ProductGroup.Orange); // -1
      logger.Information("Comparison: {Comparison}", comparison);

      logger.Information("==== Demo for abstract static members ====");

      PrintAllItems<ProductType, string>(logger);

      Get<ProductType, string>(logger, "Groceries");
   }

   private static void PrintAllItems<T, TKey>(ILogger logger)
      where T : IEnum<TKey, T>, IEnum<TKey>
      where TKey : notnull
   {
      logger.Information("Print all items of '{Name}':", typeof(T).Name);

      foreach (var item in T.Items)
      {
         logger.Information("Item: {Item}", item);
      }
   }

   private static void Get<T, TKey>(ILogger logger, TKey key)
      where T : IEnum<TKey, T>, IEnum<TKey>
      where TKey : notnull
   {
      var item = T.Get(key);

      logger.Information("Key '{Key}' => '{Item}'", key, item);
   }

   private static void DemoForValidatableEnum(ILogger logger)
   {
      logger.Information("==== Demo for IValidatableEnum<T> ====");

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
}
