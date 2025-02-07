![Build](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/workflows/CI/badge.svg?branch=master)
![TestResults](https://gist.githubusercontent.com/PawelGerr/043909cfb348b36187d02222da1f372e/raw/badge.svg)
![NuGet Downloads](https://img.shields.io/nuget/dt/Thinktecture.Runtime.Extensions)

[![Thinktecture.Runtime.Extensions](https://img.shields.io/nuget/v/Thinktecture.Runtime.Extensions.svg?maxAge=60&label=Thinktecture.Runtime.Extensions)](https://www.nuget.org/packages/Thinktecture.Runtime.Extensions/)  
[![Thinktecture.Runtime.Extensions.EntityFrameworkCore6](https://img.shields.io/nuget/v/Thinktecture.Runtime.Extensions.EntityFrameworkCore6.svg?maxAge=60&label=Thinktecture.Runtime.Extensions.EntityFrameworkCore6)](https://www.nuget.org/packages/Thinktecture.Runtime.Extensions.EntityFrameworkCore6/)  
[![Thinktecture.Runtime.Extensions.EntityFrameworkCore7](https://img.shields.io/nuget/v/Thinktecture.Runtime.Extensions.EntityFrameworkCore7.svg?maxAge=60&label=Thinktecture.Runtime.Extensions.EntityFrameworkCore7)](https://www.nuget.org/packages/Thinktecture.Runtime.Extensions.EntityFrameworkCore7/)  
[![Thinktecture.Runtime.Extensions.EntityFrameworkCore8](https://img.shields.io/nuget/v/Thinktecture.Runtime.Extensions.EntityFrameworkCore8.svg?maxAge=60&label=Thinktecture.Runtime.Extensions.EntityFrameworkCore8)](https://www.nuget.org/packages/Thinktecture.Runtime.Extensions.EntityFrameworkCore8/)  
[![Thinktecture.Runtime.Extensions.EntityFrameworkCore9](https://img.shields.io/nuget/v/Thinktecture.Runtime.Extensions.EntityFrameworkCore9.svg?maxAge=60&label=Thinktecture.Runtime.Extensions.EntityFrameworkCore9)](https://www.nuget.org/packages/Thinktecture.Runtime.Extensions.EntityFrameworkCore9/)  
[![Thinktecture.Runtime.Extensions.Json](https://img.shields.io/nuget/v/Thinktecture.Runtime.Extensions.Json.svg?maxAge=60&label=Thinktecture.Runtime.Extensions.Json)](https://www.nuget.org/packages/Thinktecture.Runtime.Extensions.Json/)  
[![Thinktecture.Runtime.Extensions.Newtonsoft.Json](https://img.shields.io/nuget/v/Thinktecture.Runtime.Extensions.Newtonsoft.Json.svg?maxAge=60&label=Thinktecture.Runtime.Extensions.Newtonsoft.Json)](https://www.nuget.org/packages/Thinktecture.Runtime.Extensions.Newtonsoft.Json/)  
[![Thinktecture.Runtime.Extensions.MessagePack](https://img.shields.io/nuget/v/Thinktecture.Runtime.Extensions.MessagePack.svg?maxAge=60&label=Thinktecture.Runtime.Extensions.MessagePack)](https://www.nuget.org/packages/Thinktecture.Runtime.Extensions.MessagePack/)  
[![Thinktecture.Runtime.Extensions.AspNetCore](https://img.shields.io/nuget/v/Thinktecture.Runtime.Extensions.AspNetCore.svg?maxAge=60&label=Thinktecture.Runtime.Extensions.AspNetCore)](https://www.nuget.org/packages/Thinktecture.Runtime.Extensions.AspNetCore/)

This library provides some interfaces, classes, [Roslyn Source Generators](https://docs.microsoft.com/en-us/dotnet/csharp/roslyn-sdk/source-generators-overview), Roslyn Analyzers and Roslyn CodeFixes for implementation of **Smart Enums**, **Value Objects** and **Discriminated Unions**.

* [Requirements](#requirements)
* [Migrations](#migrations)
* [Smart Enum](#smart-enums)
* [Value Objects](#value-objects)
* [Discriminated Unions](#discriminated-unions) (requires version 8.x.x)
    * [Ad hoc unions](#ad-hoc-unions)
    * [Regular unions](#regular-unions)

# Documentation

See [wiki](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki) for more documentation.


# Requirements

* Version 8:
    * C# 11 (or higher) for generated code
    * SDK 8.0.400 (or higher) for building projects
* Version 7:
    * C# 11 (or higher) for generated code
    * SDK 7.0.401 (or higher) for building projects

# Migrations

* [Migration from v7 to v8](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Migration-from-v7-to-v8)
* [Migration from v6 to v7](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Migration-from-v6-to-v7)

# Ideas and real-world use cases

Smart Enums:

* [Shipping Method](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Smart-Enums#shipping-method)
* [CSV-Importer-Type](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Smart-Enums#csv-importer-type)

Value objects:

* [Open-ended End Date](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Value-Objects#open-ended-end-date)
* [(Always-positive) Amount](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Value-Objects#always-positive-amount)

# Smart Enums

Smart Enums provide a powerful alternative to traditional C# enums, offering type-safety, extensibility, and rich behavior.
Unlike regular C# enums which are limited to numeric values and lack extensibility, Smart Enums can:
* Use any type as the underlying type (e.g., strings, integers) or none at all
* Include additional fields, properties and behavior
* Use polymorphism to define custom behavior for each value
* Prevent creation of invalid values
* Integrate seamlessly with JSON serializers, MessagePack, Entity Framework Core and ASP.NET Core

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

// Direct mapping to values
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

Features:

* Roslyn Analyzers and CodeFixes help the developers to implement the Value Objects correctly
* Choice between [Simple Value Objects](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Value-Objects#simple-value-objects) and [Complex Value Objects](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Value-Objects#complex-value-objects)
* Allows custom fields, properties and methods
* Provides appropriate factory methods for creation of new value objects based on the specified properties/fields
* [Factory methods can be renamed](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Value-Objects#rename-factory-methods)
* Allows custom [validation](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Value-Objects#validation) of [constructor](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Value-Objects#validation-of-the-constructor-arguments) and [factory method](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Value-Objects#validation-of-the-factory-method-arguments) arguments
* Allows [custom type to pass validation error(s)](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Value-Objects#custom-type-for-validation-errors)
* *[Simple Value Objects only]* Allows cast and type conversion from key-member type to Value Object and vice versa
* *[Simple Value Objects only]* Provides an implementation of `IFormattable` if the key member is an `IFormattable`
* Provides proper implementation of `Equals`, `GetHashCode`, `ToString` and equality comparison via `==` and `!=`
* Provides implementation of `IComparable`, `IComparable<T>`, `IFormattable`, `IParsable<T>` and comparison operators `<`, `<=`, `>`, `>=`
* [Allows custom equality comparison](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Value-Objects#custom-equality-comparer) and [custom comparer](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Value-Objects#custom-comparer)
* Configurable handling of [null](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Value-Objects#null-in-factory-methods-yields-null) and [empty strings](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Value-Objects#empty-string-in-factory-methods-yields-null)
* [JSON support](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Value-Objects#json-serialization) (`System.Text.Json` and `Newtonsoft.Json`)
* [Support for Minimal Api Parameter Binding and ASP.NET Core Model Binding](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Value-Objects#support-for-minimal-api-parameter-binding-and-aspnet-core-model-binding)
* [Entity Framework Core support](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Value-Objects#support-for-entity-framework-core) (`ValueConverter`)
* [MessagePack support](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Value-Objects#messagepack-serialization) (`IMessagePackFormatter`)
* [Logging for debugging or getting insights](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Value-Objects#logging-v610-or-higher)

## Simple Value Object

A simple value object has 1 field/property only, i.e., it is kind of wrapper for another (primitive) type. The main use case is to prevent creation of values/instances which are considered invalid according to some rules.
In DDD (domain-driven design), working with primitive types, like string, directly is called primitive obsession and should be avoided.

Most simple value objects with a key member of type `string` and another one (which is a struct) with an `int`.

```C#
[ValueObject<string>]
public sealed partial class ProductName
{
}

[ValueObject<int>]
public readonly partial struct Amount
{
}
```

After the implementation of a value object, a Roslyn source generator kicks in and implements the rest. Following API is available from now on.

```C#
// Factory method for creation of new instances.
// Throws ValidationException if the validation fails (if we had any)
ProductName apple = ProductName.Create("Apple");

// Alternatively, using an explicit cast, which behaves the same way as calling "ProductName.Create"
ProductName apple = (ProductName)"Apple";

-----------

// The same as above but returns a bool instead of throwing an exception (dictionary-style)
bool created = ProductName.TryCreate("Chocolate", out ProductName? chocolate);

-----------

// Similar to TryCreate but returns a ValidationError instead of a boolean.
ValidationError? validationError = ProductName.Validate("Chocolate", null, out var chocolate);

if (validationError is null)
{
    logger.Information("Product name {Name} created", chocolate);
}
else
{
    logger.Warning("Failed to create product name. Validation result: {validationError}", validationError.ToString());
}

-----------

// Implicit conversion to the type of the key member
string valueOfTheProductName = apple; // "Apple"

-----------

// Equality comparison compares the key member using default comparer by default.
// Key members of type `string` are compared with 'StringComparer.OrdinalIgnoreCase' by default.
bool equal = apple.Equals(apple);

-----------

// Equality comparison operators: '==' and '!='
bool equal = apple == apple;
bool notEqual = apple != apple;

-----------

// Hash code: combined hash code of type and key member. 
// Strings are using 'StringComparer.OrdinalIgnoreCase' by default.
int hashCode = apple.GetHashCode();

-----------

// 'ToString' implementation return the string representation of the key member
string value = apple.ToString(); // "Apple"

------------

// Implements IParsable<T> which is especially helpful with minimal apis.
bool success = ProductName.TryParse("New product name", null, out var productName);

ProductName productName = ProductName.Parse("New product name", null);

------------

// Implements "IFormattable" if the key member is an "IFormattable".
Amount amount = Amount.Create(42);
string formattedValue = amount.ToString("000", CultureInfo.InvariantCulture); // "042"

------------

// Implements "IComparable<ProductName>" if the key member is an "IComparable",
// or if custom comparer is provided.
Amount amount = Amount.Create(1);
Amount otherAmount = Amount.Create(2);

int comparison = amount.CompareTo(otherAmount); // -1

------------

// Implements comparison operators (<,<=,>,>=) if the key member has comparison operators itself.
bool isBigger = amount > otherAmount;

// Implements comparison operators to compare the value object with an instance of key-member-type,
// if "ComparisonOperators" is set "OperatorsGeneration.DefaultWithKeyTypeOverloads"
bool isBigger = amount > 2;

------------

// Implements addition / subtraction / multiplication / division if the key member supports corresponding operators
Amount sum = amount + otherAmount;

// Implements operators that accept an instance of key-member-type,
// if the "OperatorsGeneration" is set "DefaultWithKeyTypeOverloads"
Amount sum = amount + 2;

------------

// Provides a static default value "Empty" (similar to "Guid.Empty"), if the value object is a struct
Amount defaultValue = Amount.Empty; // same as "Amount defaultValue = default;"
```

## Complex Value Objects

A complex value object is an immutable `class` or a `readonly struct` with a `ComplexValueObjectAttribute`. Complex value object usually has multiple readonly fields/properties.

A simple example would be a `Boundary` with 2 properties. One property is the lower boundary and the other is the upper boundary. Yet again, we skip the validation at the moment.

```C#
[ComplexValueObject]
public sealed partial class Boundary
{
   public decimal Lower { get; }
   public decimal Upper { get; }
}
```

The rest is implemented by a Roslyn source generator, providing the following API:

```C#
// Factory method for creation of new instances.
// Throws ValidationException if the validation fails (if we had any)
Boundary boundary = Boundary.Create(lower: 1, upper: 2);

-----------

// the same as above but returns a bool instead of throwing an exception (dictionary-style)
bool created = Boundary.TryCreate(lower: 1, upper: 2, out Boundary? boundary);

-----------

// similar to TryCreate but returns a ValidationError instead of a boolean.
ValidationError? validationError = Boundary.Validate(lower: 1, upper: 2, out Boundary? boundary);

if (validationError is null)
{
    logger.Information("Boundary {Boundary} created", boundary);
}
else
{
    logger.Warning("Failed to create boundary. Validation result: {validationError}", validationError.ToString());
}

-----------

// Equality comparison compares the members using default or custom comparers.
// Strings are compared with 'StringComparer.OrdinalIgnoreCase' by default.
bool equal = boundary.Equals(boundary);

-----------

// Equality comparison with '==' and '!='
bool equal = boundary == boundary;
bool notEqual = boundary != boundary;

-----------

// Hash code of the members according default or custom comparers
int hashCode = boundary.GetHashCode();

-----------

// 'ToString' implementation
string value = boundary.ToString(); // "{ Lower = 1, Upper = 2 }"
```

# Discriminated Unions

Install: `Install-Package Thinktecture.Runtime.Extensions` (requires version 8.x.x)

Documentation: [Discriminated Unions](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Discriminated-Unions)

There are 2 types of unions: `ad hoc union` and `"regular" unions`

## Ad hoc unions

Features:

* Roslyn Analyzers and CodeFixes help the developers to implement the unions correctly
* Provides proper implementation of `Equals`, `GetHashCode`, `ToString` and equality comparison via `==` and `!=`
* Switch-Case/Map
* Renaming of properties
* Definition of nullable reference types

Definition of a basic union with 2 types using a `class`, a `struct` or `ref struct`:

```csharp
// class
[Union<string, int>]
public partial class TextOrNumber;

// struct
[Union<string, int>]
public partial struct TextOrNumber;

// ref struct
[Union<string, int>]
public ref partial struct TextOrNumber;

// Up to 5 types
[Union<string, int, bool, Guid, char>]
public partial class MyUnion;
```

Behind the scenes a Roslyn Source Generator generates additional code. Some of the features that are now available are ...

```csharp
// Implicit conversion from one of the defined generics.
TextOrNumber textOrNumberFromString = "text";
TextOrNumber textOrNumberFromInt = 42;

// Check the type of the value.
// By default, the properties are named using the name of the type (`String`, `Int32`)
bool isText = textOrNumberFromString.IsString;
bool isNumber = textOrNumberFromString.IsInt32;

// Getting the typed value.
// Throws "InvalidOperationException" if the current value doesn't match the calling property.
// By default, the properties are named using the name of the type (`String`, `Int32`)
string text = textOrNumberFromString.AsString;
int number = textOrNumberFromInt.AsInt32;

// Alternative approach is to use explicit cast.
// Behavior is identical to methods "As..."
string text = (string)textOrNumberFromString;
int number = (int)textOrNumberFromInt;

// Getting the value as object, i.e. untyped.
object value = textOrNumberFromString.Value;

// Implementation of Equals, GetHashCode and ToString
// PLEASE NOTE: Strings are compared using "StringComparison.OrdinalIgnoreCase" by default! (configurable)
bool equals = textOrNumberFromInt.Equals(textOrNumberFromString);
int hashCode = textOrNumberFromInt.GetHashCode();
string toString = textOrNumberFromInt.ToString();

// Equality comparison operators
bool equal = textOrNumberFromInt == textOrNumberFromString;
bool notEqual = textOrNumberFromInt != textOrNumberFromString;
```

There are multiple overloads of *switch-cases*: with `Action`, `Func<T>` and concrete values.
To prevent *closures*, you can pass a value to method `Switch`, which is going to be passed to provided callback (`Action`/`Func<T>`).

By default, the names of the method arguments are named after the type specified by `UnionAttribute<T1, T2>`.
Reserved C# keywords (like `string`) must string with `@` (like `@string`, `@default`, etc.).

```csharp
// With "Action"
textOrNumberFromString.Switch(@string: s => logger.Information("[Switch] String Action: {Text}", s),
                              int32: i => logger.Information("[Switch] Int Action: {Number}", i));

// With "Action". Logger is passed as additional parameter to prevent closures.
textOrNumberFromString.Switch(logger,
                              @string: static (l, s) => l.Information("[Switch] String Action with logger: {Text}", s),
                              int32: static (l, i) => l.Information("[Switch] Int Action with logger: {Number}", i));

// With "Func<T>"
var switchResponse = textOrNumberFromInt.Switch(@string: static s => $"[Switch] String Func: {s}",
                                                int32: static i => $"[Switch] Int Func: {i}");

// With "Func<T>" and additional argument to prevent closures.
var switchResponseWithContext = textOrNumberFromInt.Switch(123.45,
                                                           @string: static (value, s) => $"[Switch] String Func with value: {ctx} | {s}",
                                                           int32: static (value, i) => $"[Switch] Int Func with value: {ctx} | {i}");

// Use `Map` instead of `Switch` to return concrete values directly.
var mapResponse = textOrNumberFromString.Map(@string: "[Map] Mapped string",
                                             int32: "[Map] Mapped int");
```

Use `T1Name`/`T2Name` of the `UnionAttribute` to get more meaningful names.

```csharp
[Union<string, int>(T1Name = "Text",
                    T2Name = "Number")]
public partial class TextOrNumber;
```

The properties and method arguments are renamed accordingly:

```csharp
bool isText = textOrNumberFromString.IsText;
bool isNumber = textOrNumberFromString.IsNumber;

string text = textOrNumberFromString.AsText;
int number = textOrNumberFromInt.AsNumber;

textOrNumberFromString.Switch(text: s => logger.Information("[Switch] String Action: {Text}", s),
                              number: i => logger.Information("[Switch] Int Action: {Number}", i));
```

## Regular unions

Features:

* Roslyn Analyzers and CodeFixes help the developers to implement the unions correctly
* Can be a `class` or `record`
* Switch-Case/Map
* Supports generics
* Derived types can be simple classes or something complex like a [value object](#value-objects).

Simple union using a class and a value object:

```csharp
[Union]
public partial class Animal
{
   [ValueObject<string>]
   public partial class Dog : Animal;

   public sealed class Cat : Animal;
}

```

Similar example as above but with `records`:

```csharp
[Union]
public partial record AnimalRecord
{
   public sealed record Dog(string Name) : AnimalRecord;

   public sealed record Cat(string Name) : AnimalRecord;
}
```

A union type (i.e. the base class) with a property:

```csharp
[Union]
public partial class Animal
{
   public string Name { get; }

   private Animal(string name)
   {
      Name = name;
   }

   public sealed class Dog(string Name) : Animal(Name);

   public sealed class Cat(string Name) : Animal(Name);
}
```

A `record` with a generic:

```csharp
[Union]
public partial record Result<T>
{
   public record Success(T Value) : Result<T>;

   public record Failure(string Error) : Result<T>;

   public static implicit operator Result<T>(T value) => new Success(value);
   public static implicit operator Result<T>(string error) => new Failure(error);
}
```

One of the main purposes for a regular union is their exhaustiveness, i.e. all member types are accounted for in a switch/map:

```csharp
Animal animal = new Animal.Dog("Milo");

animal.Switch(dog: d => logger.Information("Dog: {Dog}", d),  
              cat: c => logger.Information("Cat: {Cat}", c));

var result = animal.Map(dog: "Dog",
                        cat: "Cat");
```

Use flags `SwitchMethods` and `MapMethods` for generation of `SwitchPartially`/`MapPartially`:

```csharp
[Union(SwitchMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads,
       MapMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads)]
public partial record AnimalRecord
{
   public sealed record Dog(string Name) : AnimalRecord;

   public sealed record Cat(string Name) : AnimalRecord;
}

---------------------------
    
Animal animal = new Animal.Dog("Milo");

animal.SwitchPartially(@default: a => logger.Information("Default: {Animal}", a),
                       cat: c => logger.Information("Cat: {Cat}", c.Name));
```
