using System;
using System.Collections.Generic;
using Serilog;

namespace Thinktecture.EnumLikeClass
{
   public class EnumLikeClassDemos
   {
      public static void Demo(ILogger logger)
      {
         DemoForNonValidatableEnum(logger);
         DemoForValidatableEnum(logger);

         logger.Information("Press ENTER to exit");
         Console.ReadLine();
      }

      private static void DemoForNonValidatableEnum(ILogger logger)
      {
         logger.Information("==== Demo for IEnum<T> ====");

         var productTypes = ProductType.Items;
         logger.Information("Product types: {types}", productTypes);

         var productType = ProductType.Get("Groceries");
         logger.Information("Product type: {type}", productType);

         if (ProductType.TryGet("Housewares", out var housewares))
            logger.Information("Product type {type} with TryGet found", housewares);

         string keyOfTheProductType = productType;
         logger.Information("Implicit conversion of ProductType -> string: {key}", keyOfTheProductType);

         try
         {
            ProductType.Get("Unknown");
            logger.Warning("This line won't be reached.");
         }
         catch (KeyNotFoundException)
         {
            logger.Information("KeyNotFoundException is thrown because there is no product type with the key 'Unknown'.");
         }
      }

      private static void DemoForValidatableEnum(ILogger logger)
      {
         logger.Information("==== Demo for IValidatableEnum<T> ====");

         var categories = ProductCategory.Items;
         logger.Information("Categories: {categories}", categories);

         var category = ProductCategory.Get("Fruits");
         logger.Information("Category: {category}", category);

         // Throws "InvalidOperationException" if not valid
         category.EnsureValid();

         if (ProductCategory.TryGet("fruits", out var fruits))
            logger.Information("Category {category} with TryGet found", fruits);

         string keyOfTheCategory = category;
         logger.Information("Implicit conversion of Category -> string: {key}", keyOfTheCategory);

         var unknownCategory = ProductCategory.Get("Grains");
         logger.Information("unknownCategory.Key: {categoryKey}", unknownCategory.Name);
         logger.Information("unknownCategory.isValid: {IsValid}", unknownCategory.IsValid);
      }
   }
}
