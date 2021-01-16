using System;
using System.Collections.Generic;
using Serilog;

namespace Thinktecture.EnumLikeClasses
{
   public class EnumLikeClassDemos
   {
      public static void Demo(ILogger logger)
      {
         DemoForNonValidatableEnum(logger);
         DemoForValidatableEnum(logger);
      }

      private static void DemoForNonValidatableEnum(ILogger logger)
      {
         logger.Information("==== Demo for IEnum<T> ====");

         var productTypes = ProductType.Items;
         logger.Information("Product types: {Types}", productTypes);

         var productType = ProductType.Get("Groceries");
         logger.Information("Product type: {Type}", productType);

         productType = (ProductType)"Groceries";
         logger.Information("Explicitly casted product type: {Type}", productType);

         if (ProductType.TryGet("Housewares", out var housewares))
            logger.Information("Product type {Type} with TryGet found", housewares);

         string keyOfTheProductType = productType;
         logger.Information("Implicit conversion of ProductType -> string: {Key}", keyOfTheProductType);

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
}
