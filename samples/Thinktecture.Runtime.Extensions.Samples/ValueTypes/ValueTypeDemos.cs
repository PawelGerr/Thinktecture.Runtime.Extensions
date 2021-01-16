using System;
using System.ComponentModel.DataAnnotations;
using Serilog;

namespace Thinktecture.ValueTypes
{
   public class ValueTypeDemos
   {
      public static void Demo(ILogger logger)
      {
         logger.Information("==== Demo for ValueTypeAttribute ====");

         var bread = ProductName.Create("Bread");
         logger.Information("Product name: {Bread}", bread);

         string valueOfTheProductName = bread;
         logger.Information("Implicit conversion of ProductName -> string: {Key}", valueOfTheProductName);

         try
         {
            ProductName.Create("  ");
            logger.Warning("This line won't be reached.");
         }
         catch (ArgumentException)
         {
            logger.Information("ArgumentException is thrown because a product name cannot be an empty string.");
         }

         var validationResult = ProductName.TryCreate("Milk", out var milk);
         if (validationResult == ValidationResult.Success)
            logger.Information("Product name '{Name}' created with 'TryCreate'.", milk);
      }
   }
}
