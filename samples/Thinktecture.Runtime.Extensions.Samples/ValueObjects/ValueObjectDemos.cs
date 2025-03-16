using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text.Json;
using Serilog;
using Thinktecture.SmartEnums;

namespace Thinktecture.ValueObjects;

public class ValueObjectDemos
{
   public static void Demo(ILogger logger)
   {
      DemoForSimpleValueObjects(logger);
      DemoForSimpleValueObjectWithCustomComparer(logger);
      DemoForEndDate(logger);
      DemoForPeriod(logger);
      DemosForAmount(logger);
      DemosForMoney(logger);
      DemosForYearMonth(logger);

      DemoForComplexValueObjects(logger);
      DemoForComplexValueObjectWithCustomComparison(logger);
      DemoForFileUrn(logger);
   }

   private static void DemoForSimpleValueObjects(ILogger logger)
   {
      logger.Information("""


                         ==== Demo for Simple Value Objects ====

                         """);

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

      var validationError = ProductName.Validate("Milk", null, out var milk);

      if (validationError is null)
         logger.Information("Product name '{Name}' created with 'TryCreate'.", milk);

      if (ProductName.TryCreate("Milk", out milk))
         logger.Information("Product name '{Name}' created with 'TryCreate'.", milk);

      if (!ProductName.TryCreate("X", out _, out var error))
         logger.Information("Failed to create a product with name 'X'. Error: {Error}", error.Message);

      // Thanks to setting "NullInFactoryMethodsYieldsNull = true" the method "Create" returns null
      var nullProduct = ProductName.Create(null);
      logger.Information("Null-Product name: {NullProduct}", nullProduct);

      // Thanks to setting "EmptyStringInFactoryMethodsYieldsNull = true" the method "Create" returns null
      var otherNullProductName = OtherProductName.Create(null);
      logger.Information("Null-Product name: {NullProduct}", otherNullProductName);

      // Thanks to setting "EmptyStringInFactoryMethodsYieldsNull = true" the method "Create" returns null
      var otherNullProductName2 = OtherProductName.Create(" ");
      logger.Information("Null-Product name: {NullProduct}", otherNullProductName2);

      var nullValidationError = ProductName.Validate(null, null, out nullProduct);

      if (nullValidationError is null)
         logger.Information("Null-Product name: {NullProduct}", nullProduct);

      if (ProductName.TryCreate(null, out nullProduct))
         logger.Information("Null-Product name: {NullProduct}", nullProduct);

      if (ProductName.TryParse("New product name", null, out var productName))
         logger.Information("Parsed name: {ParsedProductName}", productName);
   }

   private static void DemoForSimpleValueObjectWithCustomComparer(ILogger logger)
   {
      logger.Information("""


                         ==== Demo for Simple Value Objects with custom Comparer ====

                         """);

      var lowerCasedName = ProductNameWithCaseSensitiveComparer.Create("foo");
      var upperCasedName = ProductNameWithCaseSensitiveComparer.Create("FOO");

      logger.Information("With case-sensitive comparer: \"{LowerCasedName}\".Equals(\"{UpperCasedName}\") = {Result}",
                         lowerCasedName,
                         upperCasedName,
                         lowerCasedName.Equals(upperCasedName));

      logger.Information("With case-sensitive comparer: \"{LowerCasedName}\" == \"{UpperCasedName}\" = {Result}",
                         lowerCasedName,
                         upperCasedName,
                         lowerCasedName == upperCasedName);

      logger.Information("With case-sensitive comparer: \"{LowerCasedName}\".CompareTo(\"{UpperCasedName}\") = {Result}",
                         lowerCasedName,
                         upperCasedName,
                         lowerCasedName.CompareTo(upperCasedName));

      logger.Information("With case-sensitive comparer: \"{LowerCasedName}\" > \"{UpperCasedName}\" = {Result}",
                         lowerCasedName,
                         upperCasedName,
                         lowerCasedName > upperCasedName);
   }

   private static void DemosForAmount(ILogger logger)
   {
      logger.Information("""


                         ==== Demo for Amount ====

                         """);

      var formattedValue = Amount.Create(42.1m).ToString("000.00", CultureInfo.InvariantCulture); // "042.10"
      logger.Information("Formatted: {Formatted}", formattedValue);

      var amount = Amount.Create(1);
      var otherAmount = (Amount)2;
      var zero = Amount.Zero;

      logger.Information("Comparison: 1.CompareTo(2) = {Comparison}", amount.CompareTo(otherAmount)); // -1
      logger.Information("Comparison: 1 == 0 = {Comparison}", amount == zero);                        // false

      // Addition / subtraction / multiplication / division / comparison
      logger.Information("{Amount} + {Other} = {Sum}", amount, otherAmount, amount + otherAmount);    // 1 + 2 = 3
      logger.Information("{Amount} > {Other} = {Result}", amount, otherAmount, amount > otherAmount); // 1 > 2 = False

      // Addition with key-member type due to [ValueObject(AdditionOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads)]
      logger.Information("{Amount} + {Other} = {Sum}", amount, 42, amount + 42); // 1 + 42 = 43

      // Comparison with key-member type due to [ValueObject(ComparisonOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads)]
      logger.Information("{Amount} > {Other} = {Result}", amount, 42, amount > 42); // 1 > 42 = False
   }

   private static void DemosForMoney(ILogger logger)
   {
      logger.Information("""


                         ==== Demo for Money ====

                         """);

      // Creating monetary amounts
      var price = Money.Create(19.999m, MoneyRoundingStrategy.Down);   // 19.99 (rounds down)
      var roundedUp = Money.Create(19.991m, MoneyRoundingStrategy.Up); // 20.00 (rounds up)
      var zero = Money.Zero;                                           // 0.00

      // Arithmetic operations
      Money sum = price + roundedUp; // 39.99
      logger.Information("price + roundedUp: {Result}", sum);

      Money difference = roundedUp - price; // 0.01
      logger.Information("roundedUp - price: {Result}", difference);

      Money doubled = price * 2; // 39.98 (multiplication with int)
      logger.Information("price * 2: {Result}", doubled);

      Money tripled = 3 * price; // 59.97 (multiplication with int)
      logger.Information("3 * price: {Result}", tripled);

      // Division or multiplication with decimal need special handling
      var multiplicationResult = Money.Create(price * 1.234m); // 24.66766 => 24.67
      logger.Information("[Decimal] price * 1.234m: {Result}", price * 1.234m);
      logger.Information("[Money] Money.Create(price * 1.234m): {Result}", multiplicationResult);

      // Comparison
      logger.Information("roundedUp > price: {IsGreater}", roundedUp > price); // true
   }

   private static void DemosForYearMonth(ILogger logger)
   {
      logger.Information("""


                         ==== Demo for Year-Month ====

                         """);

      // Creating DayMonth instances
      var birthday = DayMonth.Create(5, 15); // May 15th
      var leapDay = DayMonth.Create(2, 29);  // February 29th (works because we use leap year 2000)

      // Accessing components
      var day = birthday.Day;     // 15
      var month = birthday.Month; // 5

      var date = new DateOnly(2020, 5, 15);
      DayMonth dayMonthFromDate = date;
      logger.Information("DayMonth from DateOnly: {DayMonth}", dayMonthFromDate); // May 15th

      // Comparing dates
      var sameDay = DayMonth.Create(5, 15);
      logger.Information("birthday == sameDay: {IsEqual}", birthday == sameDay);                            // true
      logger.Information("birthday < DayMonth.Create(6, 1): {IsBefore}", birthday < DayMonth.Create(6, 1)); // true
   }

   private static void DemoForEndDate(ILogger logger)
   {
      logger.Information("""


                         ==== Demo for Open-End Date ====

                         """);

      var endDate = OpenEndDate.Create(DateTime.Today);
      var defaultEndDate = default(OpenEndDate);
      var infiniteEndDate = OpenEndDate.Infinite;

      logger.Information("EndDate.Infinite and default(EndDate) are equal: {AreEqual}", infiniteEndDate == defaultEndDate);

      logger.Information("EndDate.Infinite is bigger than today: {AreEqual}", OpenEndDate.Infinite > endDate);

      DateOnly dateOfDefaultDate = defaultEndDate;
      DateOnly dateOfInfiniteDate = infiniteEndDate;

      logger.Information("DateOnly of EndDate.Infinite and default(EndDate) are equal: {AreEqual}", OpenEndDate.Infinite == dateOfDefaultDate);

      logger.Information("EndDate.Infinite and DateOnly.MaxValue are equal: {AreEqual}", infiniteEndDate == DateOnly.MaxValue);
   }

   private static void DemoForPeriod(ILogger logger)
   {
      logger.Information("""


                         ==== Demo for Period ====

                         """);

      // Creating Period instances
      var startDate = new DateOnly(2023, 1, 1);
      var endDate = OpenEndDate.Create(2023, 12, 31);
      var period = Period.Create(startDate, endDate);

      // Validation examples
      try
      {
         var invalidPeriod = Period.Create(
            new DateOnly(2023, 12, 31),
            OpenEndDate.Create(2023, 1, 1)
         ); // Throws ValidationException
      }
      catch (ValidationException ex)
      {
         Console.WriteLine(ex.Message); // "From must be earlier than Until"
      }

      // Equality comparison
      var samePeriod = Period.Create(startDate, endDate);
      var areEqual = period == samePeriod; // True
      logger.Information("period == samePeriod: {AreEqual}", areEqual);

      // Checking if period intersects with another period
      var otherPeriod = Period.Create(new DateOnly(2023, 6, 1),
                                      OpenEndDate.Create(2024, 6, 1));
      var intersects = period.IntersectsWith(otherPeriod); // true
      logger.Information("period.IntersectsWith(otherPeriod): {Intersects}", intersects);
   }

   private static void DemoForComplexValueObjects(ILogger logger)
   {
      logger.Information("""


                         ==== Demo for Complex Value Objects ====

                         """);

      Boundary boundaryWithCreate = Boundary.Create(lower: 1, upper: 2);
      logger.Information("Boundary with Create: {Boundary}", boundaryWithCreate);

      if (Boundary.TryCreate(lower: 1, upper: 2, out var boundaryWithTryCreate))
         logger.Information("Boundary with TryCreate: {Boundary}", boundaryWithTryCreate);

      if (!Boundary.TryCreate(lower: 2, upper: 1, out _, out var error))
         logger.Information("Failed to create Boundary (2, 1). Error: {Error}", error.Message);

      var validationError = Boundary.Validate(lower: 1, upper: 2, out var boundaryWithValidate);

      if (validationError is null)
      {
         logger.Information("Boundary {Boundary} created via Validate", boundaryWithValidate);
      }
      else
      {
         logger.Warning("Failed to create boundary. Validation error: {ValidationError}", validationError.ToString());
      }

      var equal = boundaryWithCreate.Equals(boundaryWithCreate);
      logger.Information("Boundaries are equal: {Equal}", equal);

      // Custom implementation of IValueObjectFactory<Boundary, string>
      var boundaryValidationError = BoundaryWithFactories.Validate("3:4", null, out var boundaryFromString);

      if (boundaryValidationError is null)
      {
         logger.Information("BoundaryWithFactories '{Boundary}' created from string", boundaryFromString);
      }
      else
      {
         logger.Warning("Failed to create BoundaryWithFactories from string. Validation error: {ValidationError}", boundaryValidationError.ToString());
      }

      // Custom implementation of IValueObjectFactory<Boundary, (int Lower, int Upper)>
      boundaryValidationError = BoundaryWithFactories.Validate((5, 6), null, out var boundaryFromTuple);

      if (boundaryValidationError is null)
      {
         logger.Information("BoundaryWithFactories '{Boundary}' created from tuple", boundaryFromTuple);
      }
      else
      {
         logger.Warning("Failed to create BoundaryWithFactories from tuple. Validation error: {ValidationError}", boundaryValidationError.ToString());
      }
   }

   private static void DemoForComplexValueObjectWithCustomComparison(ILogger logger)
   {
      logger.Information("""


                         ==== Demo for Complex Value Object with custom Comparison ====

                         """);

      var item1 = ComplexValueObjectWithCustomEqualityComparison.Create("1", "Item 1");
      var item2 = ComplexValueObjectWithCustomEqualityComparison.Create("1", "Item 2");
      var item3 = ComplexValueObjectWithCustomEqualityComparison.Create("2", "Item 3");

      logger.Information("{Item1} == {Item2}: {Result}", item1, item2, item1 == item2);
      logger.Information("{Item1} == {Item3}: {Result}", item1, item3, item1 == item3);
   }

   private static void DemoForFileUrn(ILogger logger)
   {
      logger.Information("""


                         ==== Demo for File URN ====

                         """);

      // Creating a FileUrn from its components
      var documentLocation = FileUrn.Create("blob storage", "containers/documents/contract.pdf");
      var imageLocation = FileUrn.Create("local file system", "images/profile/user123.jpg");

      // Parsing from string
      var parsed = FileUrn.Parse("blob storage:containers/documents/contract.pdf", null);       // IParsable.Parse
      logger.Information("parsed file urn: {Parsed}", parsed);                                  // { FileStore = blob storage, Urn = containers/documents/contract.pdf }
      logger.Information("documentLocation == parsed: {AreEqual}", documentLocation == parsed); // true

      // Validation
      try
      {
         var invalid = FileUrn.Parse("invalid-format", null);
      }
      catch (FormatException ex)
      {
         logger.Information(ex.Message); // "Invalid FileUrn format. Expected 'fileStore:urn'"
      }

      // Serialization
      var json = JsonSerializer.Serialize(documentLocation);
      logger.Information("Serialized JSON: {Json}", json); // "blob storage:containers/documents/contract.pdf"

      var deserialized = JsonSerializer.Deserialize<FileUrn>(json);
      logger.Information("Deserialized FileUrn: {Deserialized}", deserialized); // blob storage:containers/documents/contract.pdf
   }
}
