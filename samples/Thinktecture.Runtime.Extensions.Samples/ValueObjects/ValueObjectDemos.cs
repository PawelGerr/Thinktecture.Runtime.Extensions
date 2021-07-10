using System;
using System.ComponentModel.DataAnnotations;
using Serilog;

namespace Thinktecture.ValueObjects
{
   public class ValueObjectDemos
   {
      public static void Demo(ILogger logger)
      {
         logger.Information("==== Demo for ValueObjectAttribute ====");

         var bread = ProductName.Create("Bread");
         logger.Information("Product name: {Bread}", bread);

         string valueOfTheProductName = bread;
         logger.Information("Implicit conversion of ProductName -> string: {Key}", valueOfTheProductName);

         try
         {
            ProductName.Create("  ");
            logger.Warning("This line won't be reached.");
         }
         catch (ValidationException)
         {
            logger.Information("ValidationException is thrown because a product name cannot be an empty string.");
         }

         var validationResult = ProductName.TryCreate("Milk", out var milk);
         if (validationResult == ValidationResult.Success)
            logger.Information("Product name '{Name}' created with 'TryCreate'.", milk);

         // Thanks to setting "NullInFactoryMethodsYieldsNull = true"
         var nullProduct = ProductName.Create(null);
         logger.Information("Null-Product name: {NullProduct}", nullProduct);

         var nullValidationResult = ProductName.TryCreate(null, out nullProduct);
         if (nullValidationResult == ValidationResult.Success)
            logger.Information("Null-Product name: {NullProduct}", nullProduct);
      }
   }
}
