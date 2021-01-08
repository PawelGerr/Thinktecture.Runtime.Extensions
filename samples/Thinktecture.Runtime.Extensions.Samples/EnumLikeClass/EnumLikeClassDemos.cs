using System;
using Serilog;

namespace Thinktecture.EnumLikeClass
{
   public class EnumLikeClassDemos
   {
      public static void Demo(ILogger logger)
      {
         var categories = ProductCategory.Items;
         logger.Information("Catgories: {categories}", categories);

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

         logger.Information("Press ENTER to exit");
         Console.ReadLine();
      }
   }
}
