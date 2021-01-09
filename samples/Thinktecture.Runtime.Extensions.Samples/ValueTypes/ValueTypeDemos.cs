using System;
using Serilog;

namespace Thinktecture.ValueTypes
{
   public class ValueTypeDemos
   {
      public static void Demo(ILogger logger)
      {
         logger.Information("==== Demo for ValueTypeAttribute ====");

         var bread = ProductName.Create("Bread");
         logger.Information($"Product name: {bread}", bread);

         string valueOfTheProductName = bread;
         logger.Information("Implicit conversion of ProductName -> string: {key}", valueOfTheProductName);

         try
         {
            ProductName.Create("  ");
            logger.Warning("This line won't be reached.");
         }
         catch (ArgumentException)
         {
            logger.Information("ArgumentException is thrown because a product name cannot be an empty string.");
         }

         if (ProductName.TryCreate("Milk", out var milk))
            logger.Information("Product name '{name}' created with 'TryCreate'.", milk);
      }
   }
}
