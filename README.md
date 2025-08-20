![Build](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/workflows/CI/badge.svg)
![TestResults](https://gist.githubusercontent.com/PawelGerr/043909cfb348b36187d02222da1f372e/raw/badge.svg)
![NuGet Downloads](https://img.shields.io/nuget/dt/Thinktecture.Runtime.Extensions)

[![Thinktecture.Runtime.Extensions](https://img.shields.io/nuget/v/Thinktecture.Runtime.Extensions.svg?maxAge=60&label=Thinktecture.Runtime.Extensions)](https://www.nuget.org/packages/Thinktecture.Runtime.Extensions/)  
[![Thinktecture.Runtime.Extensions.EntityFrameworkCore7](https://img.shields.io/nuget/v/Thinktecture.Runtime.Extensions.EntityFrameworkCore7.svg?maxAge=60&label=Thinktecture.Runtime.Extensions.EntityFrameworkCore7)](https://www.nuget.org/packages/Thinktecture.Runtime.Extensions.EntityFrameworkCore7/)  
[![Thinktecture.Runtime.Extensions.EntityFrameworkCore8](https://img.shields.io/nuget/v/Thinktecture.Runtime.Extensions.EntityFrameworkCore8.svg?maxAge=60&label=Thinktecture.Runtime.Extensions.EntityFrameworkCore8)](https://www.nuget.org/packages/Thinktecture.Runtime.Extensions.EntityFrameworkCore8/)  
[![Thinktecture.Runtime.Extensions.EntityFrameworkCore9](https://img.shields.io/nuget/v/Thinktecture.Runtime.Extensions.EntityFrameworkCore9.svg?maxAge=60&label=Thinktecture.Runtime.Extensions.EntityFrameworkCore9)](https://www.nuget.org/packages/Thinktecture.Runtime.Extensions.EntityFrameworkCore9/)  
[![Thinktecture.Runtime.Extensions.Json](https://img.shields.io/nuget/v/Thinktecture.Runtime.Extensions.Json.svg?maxAge=60&label=Thinktecture.Runtime.Extensions.Json)](https://www.nuget.org/packages/Thinktecture.Runtime.Extensions.Json/)  
[![Thinktecture.Runtime.Extensions.Newtonsoft.Json](https://img.shields.io/nuget/v/Thinktecture.Runtime.Extensions.Newtonsoft.Json.svg?maxAge=60&label=Thinktecture.Runtime.Extensions.Newtonsoft.Json)](https://www.nuget.org/packages/Thinktecture.Runtime.Extensions.Newtonsoft.Json/)  
[![Thinktecture.Runtime.Extensions.MessagePack](https://img.shields.io/nuget/v/Thinktecture.Runtime.Extensions.MessagePack.svg?maxAge=60&label=Thinktecture.Runtime.Extensions.MessagePack)](https://www.nuget.org/packages/Thinktecture.Runtime.Extensions.MessagePack/)  
[![Thinktecture.Runtime.Extensions.AspNetCore](https://img.shields.io/nuget/v/Thinktecture.Runtime.Extensions.AspNetCore.svg?maxAge=60&label=Thinktecture.Runtime.Extensions.AspNetCore)](https://www.nuget.org/packages/Thinktecture.Runtime.Extensions.AspNetCore/)  
[![Thinktecture.Runtime.Extensions.Swashbuckle](https://img.shields.io/nuget/v/Thinktecture.Runtime.Extensions.Swashbuckle.svg?maxAge=60&label=Thinktecture.Runtime.Extensions.Swashbuckle)](https://www.nuget.org/packages/Thinktecture.Runtime.Extensions.Swashbuckle/)

This library provides some interfaces, classes, [Roslyn Source Generators](https://docs.microsoft.com/en-us/dotnet/csharp/roslyn-sdk/source-generators-overview), Roslyn Analyzers and Roslyn CodeFixes for implementation of **Smart Enums**, **Value Objects** and **Discriminated Unions**.

* [Requirements](#requirements)
* [Migrations](#migrations)
* [Smart Enums](#smart-enums)
* [Value Objects](#value-objects)
* [Discriminated Unions](#discriminated-unions)
    * [Ad hoc unions](#ad-hoc-unions)
    * [Regular unions](#regular-unions)

# Documentation

See [wiki](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki) for more documentation.

**Value Objects articles**:
* [Value Objects: Solving Primitive Obsession in .NET](https://www.thinktecture.com/en/net/value-objects-solving-primitive-obsession-in-net/)
* [Handling Complexity: Introducing Complex Value Objects in .NET](https://www.thinktecture.com/en/net/handling-complexity-introducing-complex-value-objects-in-dotnet/)
* [Value Objects in .NET: Integration with Frameworks and Libraries](https://www.thinktecture.com/en/net/value-objects-integration-with-frameworks-and-libraries/)

**Smart Enums articles**:
* [Smart Enums: Beyond Traditional Enumerations in .NET](https://www.thinktecture.com/en/net/smart-enums-beyond-traditional-enumerations-in-dotnet/)

**Discriminated Unions articles**:
* [Discriminated Unions: Representation of Alternative Types in .NET](https://www.thinktecture.com/net/discriminated-unions-representation-of-alternative-types-in-dotnet/)

# Requirements

* C# 11 (or higher) for generated code
* SDK 8.0.400 (or higher) for building projects

# Migrations

* [Migration from v8 to v9](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Migration-from-v8-to-v9)
* [Migration from v7 to v8](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Migration-from-v7-to-v8)
* [Migration from v6 to v7](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Migration-from-v6-to-v7)

# Ideas and real-world use cases

Smart Enums:

* [Shipping Method](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Smart-Enums#shipping-method)
* [CSV-Importer-Type](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Smart-Enums#csv-importer-type)
* [Discriminator in a JSON Converter](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Smart-Enums#discriminator-in-a-json-converter)
* [Dispatcher in a Web API](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Smart-Enums#dispatcher-in-a-web-api)

Value objects:

* [ISBN](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Value-Objects#isbn-international-standard-book-number)
* [Open-ended End Date](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Value-Objects#open-ended-end-date)
* [Recurring Dates (Day-Month)](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Value-Objects#recurring-dates-day-month)
* [Period](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Value-Objects#period)
* [(Always-positive) Amount](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Value-Objects#always-positive-amount)
* [Monetary Amount with Specific Rounding](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Value-Objects#monetary-amount-with-specific-rounding)
* [FileUrn - Composite Identifier with String Serialization](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Value-Objects#fileurn---composite-identifier-with-string-serialization)
* [Jurisdiction (combination of value objects and union types)](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Value-Objects#jurisdiction)

Discriminated Unions:

* [Partially Known Date](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Discriminated-Unions#partially-known-date)
* [Jurisdiction (combination of value objects and union types)](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Discriminated-Unions#jurisdiction)
* [Message Processing State Management](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Discriminated-Unions#message-processing-state-management)

# Smart Enums

Smart Enums provide a powerful alternative to traditional C# enums, offering type-safety, extensibility, and rich behavior.
Unlike regular C# enums which are limited to numeric values and lack extensibility, Smart Enums can:
* Use any type as the underlying type (e.g., strings, integers) or none at all
* Include additional fields, properties and behavior
* Use polymorphism to define custom behavior for each value
* Prevent creation of invalid values
* Integrate seamlessly with JSON serializers, MessagePack, Entity Framework Core, ASP.NET Core and Swashbuckle (OpenAPI)

Install: `Install-Package Thinktecture.Runtime.Extensions`

Documentation: [Smart Enums](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Smart-Enums)

Some of the Key Features are:

* Choice between always-valid and maybe-valid Smart Enum
* Reflection-free iteration over all items
* Fast lookup/conversion from underlying type to Smart Enum and vice versa
* Allows custom properties and methods
* Exhaustive pattern matching with `Switch`/`Map` methods
* Provides appropriate constructor, based on the specified properties/fields
* Proper implementation of `Equals`, `GetHashCode`, `ToString` and equality operators
* Provides implementation of `IComparable`, `IComparable<T>`, `IFormattable`, `IParsable<T>` and comparison operators `<`, `<=`, `>`, `>=` (if applicable to the underlying type)
* Custom comparer and equality comparer

Roslyn Analyzers and CodeFixes help the developers to implement the Smart Enums correctly

Provides support for:
* JSON (System.Text.Json and Newtonsoft)
* Minimal Api Parameter Binding and ASP.NET Core Model Binding
* Entity Framework Core
* MessagePack

Definition of a Smart Enum with custom properties and methods.

```C#
[SmartEnum<string>]
public partial class ShippingMethod
{
   public static readonly ShippingMethod Standard = new(
      "STANDARD",
      basePrice: 5.99m,
      weightMultiplier: 0.5m,
      estimatedDays: 5,
      requiresSignature: false);

   public static readonly ShippingMethod Express = new(
      "EXPRESS",
      basePrice: 15.99m,
      weightMultiplier: 0.75m,
      estimatedDays: 2,
      requiresSignature: true);

   public static readonly ShippingMethod NextDay = new(
      "NEXT_DAY",
      basePrice: 29.99m,
      weightMultiplier: 1.0m,
      estimatedDays: 1,
      requiresSignature: true);

   private readonly decimal _basePrice;
   private readonly decimal _weightMultiplier;
   private readonly int _estimatedDays;

   public bool RequiresSignature { get; }

   public decimal CalculatePrice(decimal orderWeight)
   {
      return _basePrice + (orderWeight * _weightMultiplier);
   }

   public DateTime GetEstimatedDeliveryDate()
   {
      return DateTime.Today.AddDays(_estimatedDays);
   }
}
```

Behind the scenes a Roslyn Source Generator generates additional code. Some of the features that are now available are ...

### Basic Operations

```C#
[SmartEnum<string>]
public partial class ProductType
{
    // The source generator creates a private constructor
    public static readonly ProductType Groceries = new("Groceries");
}

// Enumeration over all defined items
IReadOnlyList<ProductType> allTypes = ProductType.Items;

// Value retrieval
ProductType productType = ProductType.Get("Groceries");        // Get by key (throws if not found)
ProductType productType = (ProductType)"Groceries";            // Same as above but by using a cast
bool found = ProductType.TryGet("Groceries", out var productType);  // Safe retrieval (returns false if not found)

// Validation with detailed error information
ValidationError? error = ProductType.Validate("Groceries", null, out ProductType? productType);

// IParsable<T> (useful for Minimal APIs)
bool parsed = ProductType.TryParse("Groceries", null, out ProductType? parsedType);

// IFormattable (e.g. for numeric keys)
string formatted = ProductGroup.Fruits.ToString("000", CultureInfo.InvariantCulture);  // "001"

// IComparable
int comparison = ProductGroup.Fruits.CompareTo(ProductGroup.Vegetables);
bool isGreater = ProductGroup.Fruits > ProductGroup.Vegetables;  // Comparison operators
```

### Type Conversion and Equality

```C#
// Implicit conversion to key type
string key = ProductType.Groceries;  // Returns "Groceries"

// Equality comparison
bool equal = ProductType.Groceries.Equals(ProductType.Groceries);
bool equal = ProductType.Groceries == ProductType.Groceries;  // Operator overloading
bool notEqual = ProductType.Groceries != ProductType.Housewares;

// Methods inherited from Object
int hashCode = ProductType.Groceries.GetHashCode();
string key = ProductType.Groceries.ToString();  // Returns "Groceries"

// TypeConverter
var converter = TypeDescriptor.GetConverter(typeof(ProductType));
string? keyStr = (string?)converter.ConvertTo(ProductType.Groceries, typeof(string));
ProductType? converted = (ProductType?)converter.ConvertFrom("Groceries");
```

### Pattern Matching with Switch/Map

All `Switch`/`Map` methods are exhaustive by default ensuring all cases are handled correctly.

```C#
ProductType productType = ProductType.Groceries;

// Execute different actions based on the enum value (void return)
productType.Switch(
    groceries: () => Console.WriteLine("Processing groceries order"),
    housewares: () => Console.WriteLine("Processing housewares order")
);

// Transform enum values into different types
string department = productType.Switch(
    groceries: () => "Food and Beverages",
    housewares: () => "Home and Kitchen"
);

// Direct mapping to values - clean and concise
decimal discount = productType.Map(
    groceries: 0.05m,    // 5% off groceries
    housewares: 0.10m    // 10% off housewares
);
```

For optimal performance Smart Enums provide overloads that prevent closures.

```csharp
ILogger logger = ...;

// Prevent closures by passing the parameter as first method argument
productType.Switch(logger,
    groceries: static l => l.LogInformation("Processing groceries order"),
    housewares: static l => l.LogInformation("Processing housewares order")
);

// Use a tuple to pass multiple values
var context = (Logger: logger, OrderId: "123");

productType.Switch(context,
    groceries: static ctx => ctx.Logger.LogInformation("Processing groceries order {OrderId}", ctx.OrderId),
    housewares: static ctx => ctx.Logger.LogInformation("Processing housewares order {OrderId}", ctx.OrderId)
);
```

# Value Objects

Install: `Install-Package Thinktecture.Runtime.Extensions`

Documentation: [Value Objects](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Value-Objects)

Value objects help solve several common problems in software development:

1. **Type Safety**: Prevent mixing up different concepts that share the same primitive type
   ```csharp
   // Problem: Easy to accidentally swap parameters
   void ProcessOrder(int customerId, int orderId) { ... }
   ProcessOrder(orderId, customerId); // Compiles but wrong!
   
   // Solution: Value objects make it type-safe
   [ValueObject<int>]
   public partial struct CustomerId { }
   
   [ValueObject<int>]
   public partial struct OrderId { }
   
   void ProcessOrder(CustomerId customerId, OrderId orderId) { ... }
   ProcessOrder(orderId, customerId); // Won't compile!
   ```

2. **Built-in Validation**: Ensure data consistency at creation time
   ```csharp
   [ValueObject<decimal>]
   public partial struct Amount
   {
       static partial void ValidateFactoryArguments(ref ValidationError? validationError, ref decimal value)
       {
           if (value < 0)
           {
               validationError = new ValidationError("Amount cannot be negative");
               return;
           }
           // Normalize to two decimal places
           value = Math.Round(value, 2);
       }
   }
   
   var amount = Amount.Create(100.50m);     // Success: 100.50
   var invalid = Amount.Create(-50m);       // Throws ValidationException
   ```

3. **Immutability**: Prevent accidental modifications and ensure thread safety

4. **Complex Value Objects**: Encapsulate multiple related values with validation
   ```csharp
   [ComplexValueObject]
   public partial class DateRange
   {
       public DateOnly Start { get; }
       public DateOnly End { get; }

       static partial void ValidateFactoryArguments(
           ref ValidationError? validationError,
           ref DateOnly start,
           ref DateOnly end)
       {
           if (end < start)
           {
               validationError = new ValidationError(
                   $"End date '{end}' cannot be before start date '{start}'");
               return;
           }

           // Ensure dates are not in the past
           var today = DateOnly.FromDateTime(DateTime.Today);
           if (start < today)
           {
               validationError = new ValidationError("Start date cannot be in the past");
               return;
           }
       }

       public int DurationInDays => End.DayNumber - Start.DayNumber + 1;
       
       public bool Contains(DateOnly date) => date >= Start && date <= End;
   }

   // Usage
   var range = DateRange.Create(
       start: DateOnly.FromDateTime(DateTime.Today),
       end: DateOnly.FromDateTime(DateTime.Today.AddDays(7))
   );

   Console.WriteLine(range.DurationInDays);    // 8
   Console.WriteLine(range.Contains(range.Start));  // true
   ```

Key Features:
* Two types of value objects:
    * Simple value objects (wrapper around a single value with validation)
    * Complex value objects (multiple properties representing a single concept)
* Comprehensive validation support with descriptive error messages
* Framework integration:
    * JSON serialization (System.Text.Json and Newtonsoft.Json)
    * Entity Framework Core support
    * ASP.NET Core Model Binding
    * Swashbuckle (OpenAPI)
    * MessagePack serialization
* Rich feature set:
    * Type conversion and comparison operators
    * Custom equality comparison
    * Proper implementation of standard interfaces (IComparable, IFormattable, etc.)
    * Configurable null and empty string handling
* Development support:
    * Roslyn Analyzers and CodeFixes for correct implementation
    * Logging for debugging and insights

For more examples and detailed documentation, see the [wiki](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Value-Objects).

# Discriminated Unions

Install: `Install-Package Thinktecture.Runtime.Extensions`

Documentation: [Discriminated Unions](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Discriminated-Unions)

Discriminated unions are a powerful feature that allows a type to hold a value that could be one of several different types. They provide type safety, exhaustive pattern matching, and elegant handling of complex domain scenarios. Key benefits include:

* Type-safe representation of values that can be one of several types
* Exhaustive pattern matching ensuring all cases are handled
* Elegant modeling of domain concepts with multiple states
* Clean handling of success/failure scenarios without exceptions

The library provides two types of unions to suit different needs:

## Ad hoc unions

Perfect for simple scenarios where you need to combine a few types quickly. Features:

* Type-safe combination of up to 5 different types
* Implicit conversions and type checking
* Exhaustive pattern matching with Switch/Map methods
* Built-in equality comparison
* Support for class, struct, or ref struct implementations

```csharp
// Quick combination of types
[Union<string, int>]
public partial class TextOrNumber;

// Create and use the union
TextOrNumber value = "Hello";           // Implicit conversion
TextOrNumber number = 42;               // Works with any defined type

// Type-safe access
if (value.IsString)
{
    string text = value.AsString;       // Type-safe access
    Console.WriteLine(text);
}

// Exhaustive pattern matching
var result = value.Switch(
    @string: text => $"Text: {text}",
    int32: num => $"Number: {num}"
);

// Custom property names for clarity
[Union<string, int>(T1Name = "Text", T2Name = "Number")]
public partial class BetterNamed;
// Now use .IsText, .IsNumber, .AsText, .AsNumber
```

## Regular unions

Ideal for modeling domain concepts and complex hierarchies. Features:

* Inheritance-based approach for complex scenarios
* Support for both classes and records
* Integration with value objects
* Generic type support
* Exhaustive pattern matching

Perfect for modeling domain concepts:

```csharp
// Model domain concepts clearly
[Union]
public partial record OrderStatus
{
    public record Pending : OrderStatus;
    public record Processing(DateTime StartedAt) : OrderStatus;
    public record Completed(DateTime CompletedAt, string TrackingNumber) : OrderStatus;
    public record Cancelled(string Reason) : OrderStatus;
}

// Generic result type for error handling
[Union]
public partial record Result<T>
{
    public record Success(T Value) : Result<T>;
    public record Failure(string Error) : Result<T>;

    // Implicit conversions from T and string are implemented automatically 
}

// Usage
Result<int> result = await GetDataAsync();

var message = result.Switch(
    success: s => $"Got value: {s.Value}",
    failure: f => $"Error: {f.Error}"
);
```
