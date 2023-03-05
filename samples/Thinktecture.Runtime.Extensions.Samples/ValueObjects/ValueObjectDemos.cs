using System.ComponentModel.DataAnnotations;
using System.Globalization;
using Serilog;

namespace Thinktecture.ValueObjects;

public class ValueObjectDemos
{
   public static void Demo(ILogger logger)
   {
      DemoForSimpleValueObjects(logger);
      DemoForComplexValueObjects(logger);
   }

   private static void DemoForSimpleValueObjects(ILogger logger)
   {
      logger.Information("==== Demo for Simple Value Objects ====");

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

      var validationResult = ProductName.Validate("Milk", out var milk);
      if (validationResult == ValidationResult.Success)
         logger.Information("Product name '{Name}' created with 'TryCreate'.", milk);

      if (ProductName.TryCreate("Milk", out milk))
         logger.Information("Product name '{Name}' created with 'TryCreate'.", milk);

      // Thanks to setting "NullInFactoryMethodsYieldsNull = true" the method "Create" returns null
      var nullProduct = ProductName.Create(null);
      logger.Information("Null-Product name: {NullProduct}", nullProduct);

      // Thanks to setting "EmptyStringInFactoryMethodsYieldsNull = true" the method "Create" returns null
      var otherNullProductName = OtherProductName.Create(null);
      logger.Information("Null-Product name: {NullProduct}", otherNullProductName);

      // Thanks to setting "EmptyStringInFactoryMethodsYieldsNull = true" the method "Create" returns null
      var otherNullProductName2 = OtherProductName.Create(" ");
      logger.Information("Null-Product name: {NullProduct}", otherNullProductName2);

      var nullValidationResult = ProductName.Validate(null, out nullProduct);
      if (nullValidationResult == ValidationResult.Success)
         logger.Information("Null-Product name: {NullProduct}", nullProduct);

      if (ProductName.TryCreate(null, out nullProduct))
         logger.Information("Null-Product name: {NullProduct}", nullProduct);

      if (ProductName.TryParse("New product name", null, out var productName))
         logger.Information("Parsed name: {ParsedProductName}", productName);

      var formattedValue = Amount.Create(42).ToString("000", CultureInfo.InvariantCulture); // "042"
      logger.Information("Formatted: {Formatted}", formattedValue);

      var amount = Amount.Create(1);
      var otherAmount = Amount.Create(2);

      var comparison = amount.CompareTo(otherAmount);
      logger.Information("Comparison: {Comparison}", comparison);

      // Addition / subtraction / multiplication / division / comparison
      var sum = amount + otherAmount;
      logger.Information("{Amount} + {Other} = {Sum}", amount, otherAmount, sum);

      logger.Information("{Amount} > {Other} = {Result}", amount, otherAmount, amount > otherAmount);
   }

   private static void DemoForComplexValueObjects(ILogger logger)
   {
      logger.Information("==== Demo for Complex Value Objects ====");

      Boundary boundaryWithCreate = Boundary.Create(lower: 1, upper: 2);
      logger.Information("Boundary with Create: {Boundary}", boundaryWithCreate);

      if (Boundary.TryCreate(lower: 1, upper: 2, out var boundaryWithTryCreate))
         logger.Information("Boundary with TryCreate: {Boundary}", boundaryWithTryCreate);

      var validationResult = Boundary.Validate(lower: 1, upper: 2, out var boundaryWithValidate);

      if (validationResult == ValidationResult.Success)
      {
         logger.Information("Boundary {Boundary} created via Validate", boundaryWithValidate);
      }
      else
      {
         logger.Warning("Failed to create boundary. Validation result: {ValidationResult}", validationResult!.ErrorMessage);
      }

      var equal = boundaryWithCreate.Equals(boundaryWithCreate);
      logger.Information("Boundaries are equal: {Equal}", equal);
   }
}
