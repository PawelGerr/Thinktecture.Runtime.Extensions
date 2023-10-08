![Build](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/workflows/CI/badge.svg?branch=master)
![TestResults](https://gist.githubusercontent.com/PawelGerr/043909cfb348b36187d02222da1f372e/raw/badge.svg)

[![Thinktecture.Runtime.Extensions](https://img.shields.io/nuget/vpre/Thinktecture.Runtime.Extensions.svg?maxAge=60&label=Thinktecture.Runtime.Extensions)](https://www.nuget.org/packages/Thinktecture.Runtime.Extensions/)  
[![Thinktecture.Runtime.Extensions.EntityFrameworkCore5](https://img.shields.io/nuget/vpre/Thinktecture.Runtime.Extensions.EntityFrameworkCore5.svg?maxAge=60&label=Thinktecture.Runtime.Extensions.EntityFrameworkCore5)](https://www.nuget.org/packages/Thinktecture.Runtime.Extensions.EntityFrameworkCore5/)  
[![Thinktecture.Runtime.Extensions.EntityFrameworkCore6](https://img.shields.io/nuget/vpre/Thinktecture.Runtime.Extensions.EntityFrameworkCore6.svg?maxAge=60&label=Thinktecture.Runtime.Extensions.EntityFrameworkCore6)](https://www.nuget.org/packages/Thinktecture.Runtime.Extensions.EntityFrameworkCore6/)  
[![Thinktecture.Runtime.Extensions.EntityFrameworkCore7](https://img.shields.io/nuget/vpre/Thinktecture.Runtime.Extensions.EntityFrameworkCore7.svg?maxAge=60&label=Thinktecture.Runtime.Extensions.EntityFrameworkCore7)](https://www.nuget.org/packages/Thinktecture.Runtime.Extensions.EntityFrameworkCore7/)  
[![Thinktecture.Runtime.Extensions.EntityFrameworkCore8](https://img.shields.io/nuget/vpre/Thinktecture.Runtime.Extensions.EntityFrameworkCore8.svg?maxAge=60&label=Thinktecture.Runtime.Extensions.EntityFrameworkCore8)](https://www.nuget.org/packages/Thinktecture.Runtime.Extensions.EntityFrameworkCore8/)  
[![Thinktecture.Runtime.Extensions.Json](https://img.shields.io/nuget/vpre/Thinktecture.Runtime.Extensions.Json.svg?maxAge=60&label=Thinktecture.Runtime.Extensions.Json)](https://www.nuget.org/packages/Thinktecture.Runtime.Extensions.Newtonsoft.Json/)  
[![Thinktecture.Runtime.Extensions.Newtonsoft.Json](https://img.shields.io/nuget/vpre/Thinktecture.Runtime.Extensions.Newtonsoft.Json.svg?maxAge=60&label=Thinktecture.Runtime.Extensions.Newtonsoft.Json)](https://www.nuget.org/packages/Thinktecture.Runtime.Extensions.Newtonsoft.Json/)  
[![Thinktecture.Runtime.Extensions.MessagePack](https://img.shields.io/nuget/vpre/Thinktecture.Runtime.Extensions.MessagePack.svg?maxAge=60&label=Thinktecture.Runtime.Extensions.MessagePack)](https://www.nuget.org/packages/Thinktecture.Runtime.Extensions.MessagePack/)  
[![Thinktecture.Runtime.Extensions.AspNetCore](https://img.shields.io/nuget/vpre/Thinktecture.Runtime.Extensions.AspNetCore.svg?maxAge=60&label=Thinktecture.Runtime.Extensions.AspNetCore)](https://www.nuget.org/packages/Thinktecture.Runtime.Extensions.AspNetCore/)

This library provides some interfaces, classes, [Roslyn Source Generators](https://docs.microsoft.com/en-us/dotnet/csharp/roslyn-sdk/source-generators-overview), Roslyn Analyzers and Roslyn CodeFixes for implementation of **Smart Enums** and **Value Objects**.

# Documentation

See [wiki](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki) for more documentation.

I recently started writing down some **[ideas and real-world use cases](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Value-Objects#real-world-use-cases-and-ideas)** I used in the past to show the developers the benefits of value objects and smart enums. More examples will come very soon!

Smart Enums:
* [CSV-Importer-Type](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Smart-Enums#csv-importer-type)

Value objects:
* [Open-ended End Date](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Value-Objects#open-ended-end-date)
* [(Always-positive) Amount](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Value-Objects#always-positive-amount)

# Requirements

* Version 7:
  * C# 11 (or higher) for generated code
  * SDK 7.0.401 (or higher) for building projects
* Version 6:
  * C# 11 (or higher) for generated code
  * SDK 7.0.202 (or higher) for building projects
* Version 5:
  * C# 9 (or higher) for generated code
  * SDK 6.0.300 (or higher) for building projects

# Migrations
* [Migration from v6 to v7](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Migration-from-v6-to-v7)

# Smart Enums

Install: `Install-Package Thinktecture.Runtime.Extensions`

Documentation: [Smart Enums](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Smart-Enums)

Features:
* Roslyn Analyzers and CodeFixes help the developers to implement the Smart Enums correctly
* [Allows iteration over all items](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Smart-Enums#what-is-implemented-for-you)
* [Allows custom properties and methods](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Smart-Enums#adding-behavior)
* [Switch-case/Map](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Smart-Enums#switch-casemap)
* Provides appropriate constructor, based on the specified properties/fields
* Provides means for lookup, cast and type conversion from key-type to Smart Enum and vice versa
* Provides proper implementation of `Equals`, `GetHashCode`, `ToString` and equality comparison via `==` and `!=`
* Provides implementation of `IComparable`, `IComparable<T>`, `IFormattable`, `IParsable<T>` and comparison operators `<`, `<=`, `>`, `>=` (if applicable to the underlying type)
* [Choice between always-valid and maybe-valid Smart Enum](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Smart-Enums#always-valid-vs-maybe-valid-smart-enum)
* [Makes use of abstract static members](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Smart-Enums#make-use-of-abstract-static-members)
* [Derived types can be generic](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Smart-Enums#generic-derived-types)
* [Allows custom validation of constructor arguments](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Smart-Enums#validation-of-the-constructor-arguments)
* [Allows changing the property name `Key`](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Smart-Enums#changing-the-key-property-name), which holds the underlying value - thanks to [Roslyn Source Generator](https://docs.microsoft.com/en-us/dotnet/csharp/roslyn-sdk/source-generators-overview)
* [Allows custom key comparer](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Smart-Enums#custom-key-comparer)
* [JSON support](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Smart-Enums#json-serialization) (`System.Text.Json` and `Newtonsoft.Json`)
* [Support for Minimal Web Api Parameter Binding and ASP.NET Core Model Binding](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Smart-Enums#support-for-minimal-web-api-parameter-binding-and-aspnet-core-model-binding)
* [Entity Framework Core support](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Smart-Enums#support-for-entity-framework-core) (`ValueConverter`)
* [MessagePack support](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Smart-Enums#messagepack-serialization) (`IMessagePackFormatter`)
* [Logging for debugging or getting insights](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Smart-Enums#logging-v610-or-higher)

Definition of a 2 Smart Enums without any custom properties and methods. All other features mentioned above are generated by the [Roslyn Source Generators](https://docs.microsoft.com/en-us/dotnet/csharp/roslyn-sdk/source-generators-overview) in the background.

```C#
// Smart Enum with a string as the underlying type
public sealed partial class ProductType : IEnum<string>
{
   public static readonly ProductType Groceries = new("Groceries");
   public static readonly ProductType Housewares = new("Housewares");
}

// Smart Enum with an int as the underlying type
public sealed partial class ProductGroup : IEnum<int>
{
   public static readonly ProductGroup Apple = new(1);
   public static readonly ProductGroup Orange = new(2);
}
```

Behind the scenes a Roslyn Source Generator, which comes with the library, generates additional code. Some of the features that are now available are ...

```C#
// a private constructor which takes the key and additional members (if we had any)
public sealed partial class ProductType : IEnum<string>
{
   public static readonly ProductType Groceries = new("Groceries");
   ...

------------

// a property for iteration over all items
IReadOnlyList<ProductType> allTypes = ProductType.Items;

------------

// getting the item with specific name, i.e. its key
// throw UnknownEnumIdentifierException if the provided key doesn't match to any item
ProductType productType = ProductType.Get("Groceries");

// Alternatively, using an explicit cast (behaves the same as with Get)
ProductType productType = (ProductType)"Groceries";

------------

// the same as above but returns a bool instead of throwing an exception (dictionary-style)
bool found = ProductType.TryGet("Groceries", out ProductType productType);

------------

// similar to TryGet but returns a ValidationResult instead of a boolean.
ValidationResult? validationResult = ProductType.Validate("Groceries", out productType);

if (validationResult == ValidationResult.Success)
{
    logger.Information("Product type {Type} found with Validate", productType);
}
else
{
    logger.Warning("Failed to fetch the product type with Validate. Validation result: {ValidationResult}", validationResult.ErrorMessage);
}

------------

// implicit conversion to the type of the key
string key = ProductType.Groceries; // "Groceries"

------------

// Equality comparison with 'Equals' 
// which compares the keys using default or custom 'IEqualityComparer<T>'
bool equal = ProductType.Groceries.Equals(ProductType.Groceries);

------------

// Equality comparison with '==' and '!='
bool equal = ProductType.Groceries == ProductType.Groceries;
bool notEqual = ProductType.Groceries != ProductType.Groceries;

------------

// Hash code
int hashCode = ProductType.Groceries.GetHashCode();

------------

// 'ToString' implementation
string key = ProductType.Groceries.ToString(); // "Groceries"

------------

ILogger logger = ...;

// Switch-case (Action)
productType.Switch(ProductType.Groceries, () => logger.Information("Switch with Action: Groceries"),
                   ProductType.Housewares, () => logger.Information("Switch with Action: Housewares"));
                   
// Switch-case with parameter (Action<TParam>) to prevent closures
productType.Switch(logger,
                   ProductType.Groceries, static l => l.Information("Switch with Action: Groceries"),
                   ProductType.Housewares, static l => l.Information("Switch with Action: Housewares"));

// Switch case returning a value (Func<TResult>)
var returnValue = productType.Switch(ProductType.Groceries, static () => "Switch with Func<T>: Groceries",
                                     ProductType.Housewares, static () => "Switch with Func<T>: Housewares");

// Switch case with parameter returning a value (Func<TParam, TResult>) to prevent closures
returnValue = productType.Switch(logger,
                                 ProductType.Groceries, static l => "Switch with Func<T>: Groceries",
                                 ProductType.Housewares, static l => "Switch with Func<T>: Housewares");

// Maps an item to another instance
returnValue = productType.Map(ProductType.Groceries, "Map: Groceries",
                              ProductType.Housewares, "Map: Housewares");

------------

// Implements IParsable<T> which is especially helpful with minimal web apis.
// This feature can be disabled if it doesn't make sense (see EnumGenerationAttribute).
bool parsed = ProductType.TryParse("Groceries", null, out var parsedProductType);

------------

// Implements IFormattable if the underlyng type (like int) is an IFormattable itself.
// This feature can be disabled if it doesn't make sense (see EnumGenerationAttribute).
var formatted = ProductGroup.Apple.ToString("000", CultureInfo.InvariantCulture); // 001

------------

// Implements IComparable and IComparable<T> if the underlyng type (like int) is an IComparable itself.
// This feature can be disabled if it doesn't make sense (see EnumGenerationAttribute).
var comparison = ProductGroup.Apple.CompareTo(ProductGroup.Orange); // -1

// Implements comparison operators (<,<=,>,>=) if the underlyng type (like int) has comparison operators itself.
// This feature can be disabled if it doesn't make sense (see EnumGenerationAttribute).
var isBigger = ProductGroup.Apple > ProductGroup.Orange;       
```

Definition of a new Smart Enum with 1 custom property `RequiresFoodVendorLicense` and 1 method `Do` with different behaviors for different enum items.

```C#
public partial class ProductType : IEnum<string>
{
   public static readonly ProductType Groceries = new("Groceries",  requiresFoodVendorLicense: true);
   public static readonly ProductType Housewares = new HousewaresProductType();

   public bool RequiresFoodVendorLicense { get; }

   public virtual void Do()
   {
      // do default stuff
   }

   private class HousewaresProductType : ProductType
   {
      public HousewaresProductType()
         : base("Housewares", requiresFoodVendorLicense: false)
      {
      }

      public override void Do()
      {
         // do special stuff
      }
   }
}
```

# Value Objects

Install: `Install-Package Thinktecture.Runtime.Extensions`

Documentation: [Value Objects](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Value-Objects)

Features:
* Roslyn Analyzers and CodeFixes help the developers to implement the Value Objects correctly
* Allows custom properties and methods
* Provides appropriate factory methods for creation of new value objects based on the specified properties/fields
* Allows custom [validation](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Value-Objects#validation) of [constructor](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Value-Objects#validation-of-the-constructor-arguments) and [factory method](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Value-Objects#validation-of-the-factory-method-arguments) arguments
* Additional features for [simple Value Objects (1 "key"-property/field)](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Value-Objects#simple-value-objects) and [complex Value Objects (2 properties/fields or more)](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Value-Objects#complex-value-objects)
* Simple Value Objects: allows cast and type conversion from key-type to Value Object and vice versa
* Simple Value Objects: provides an implementation of `IFormattable` if the key-property/field is an `IFormattable`
* Provides proper implementation of `Equals`, `GetHashCode`, `ToString` and equality comparison via `==` and `!=`
* Provides implementation of `IComparable`, `IComparable<T>`, `IFormattable`, `IParsable<T>` and comparison operators `<`, `<=`, `>`, `>=`
* [Allows custom equality comparison](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Value-Objects#custom-comparer) and [custom comparer](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Value-Objects#custom-comparer)
* Handling of [null](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Value-Objects#null-in-factory-methods-yields-null) and [empty strings](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Value-Objects#empty-string-in-factory-methods-yields-null)
* [JSON support](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Value-Objects#json-serialization) (`System.Text.Json` and `Newtonsoft.Json`)
* [Support for Minimal Web Api Parameter Binding and ASP.NET Core Model Binding](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Value-Objects#support-for-minimal-web-api-parameter-binding-and-aspnet-core-model-binding)
* [Entity Framework Core support](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Value-Objects#support-for-entity-framework-core) (`ValueConverter`)
* [MessagePack support](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Value-Objects#messagepack-serialization) (`IMessagePackFormatter`)
* [Logging for debugging or getting insights](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Value-Objects#logging-v610-or-higher)

## Simple Value Object

Definition of 2 value objects, one with 1 `string` property `Value` and the other with an `int`.

```C#
[ValueObject]
public sealed partial class ProductName
{
   public string Value { get; }
}

[ValueObject]
public sealed partial class Amount
{
   private readonly int _value;
}
```

After the implementation of the `ProductName`, a Roslyn source generator kicks in and implements the rest. Following API is available from now on.

```C#
// Factory method for creation of new instances.
// Throws ValidationException if the validation fails
ProductName bread = ProductName.Create("Bread");

// Alternatively, using an explicit cast (behaves the same as with Create)
ProductName bread = (ProductName)"Bread"; // is the same as calling "ProductName.Create"

-----------

// the same as above but returns a bool instead of throwing an exception (dictionary-style)
bool created = ProductName.TryCreate("Milk", out ProductName milk);

-----------

// similar to TryCreate but returns a ValidationResult instead of a boolean.
ValidationResult? validationResult = ProductName.Validate("Milk", out var milk);

if (validationResult == ValidationResult.Success)
{
    logger.Information("Product name {Name} created", milk);
}
else
{
    logger.Warning("Failed to create product name. Validation result: {ValidationResult}", validationResult.ErrorMessage);
}

-----------

// implicit conversion to the type of the key member
string valueOfTheProductName = bread; // "Bread"

-----------

// Equality comparison with 'Equals'
// which compares the key members using default or custom 'IEqualityComparer<T>'
bool equal = bread.Equals(bread);

-----------

// Equality comparison with '==' and '!='
bool equal = bread == bread;
bool notEqual = bread != bread;

-----------

// Hash code
int hashCode = bread.GetHashCode();

-----------

// 'ToString' implementation
string value = bread.ToString(); // "Bread"

------------

// Implements IParsable<T> which is especially helpful with minimal web apis.
// This feature can be disabled if it doesn't make sense (see ValueObjectAttribute).
bool success = ProductName.TryParse("New product name", null, out var productName);

------------

// Implements "IFormattable" if the key member is an "IFormattable".
// This feature can be disabled if it doesn't make sense (see ValueObjectAttribute).
var amount = Amount.Create(42);
string formattedValue = amount.ToString("000", CultureInfo.InvariantCulture); // "042"

------------

// Implements "IComparable<ProductName>" if the key member is an "IComparable"
// This feature can be disabled if it doesn't make sense (see ValueObjectAttribute).
var amount = Amount.Create(1);
var otherAmount = Amount.Create(2);

var comparison = amount.CompareTo(otherAmount); // -1

// Implements comparison operators (<,<=,>,>=) if the key member has comparison operators itself.
// This feature can be disabled if it doesn't make sense (see ValueObjectAttribute).
var isBigger = amount > otherAmount;  

------------

// Implements addition / subtraction / multiplication / division if the key member supports operators
// This feature can be disabled if it doesn't make sense (see ValueObjectAttribute).
var sum = amount + otherAmount;
```

## Complex Value Objects

A complex value object is considered a `class` or a `readonly struct` with a `ValueObjectAttribute` and with more than 1 "assignable" properties/fields. The main use case is to *manage* multiple values as a whole.

A simple example would be a `Boundary` with 2 properties, one is the lower boundary and the other is the upper boundary. Yet again, we skip the validation at the moment.

```C#
[ValueObject]
public sealed partial class Boundary
{
   public decimal Lower { get; }
   public decimal Upper { get; }
}
```

The rest is implemented by a Roslyn source generator, providing the following API:

```C#
// Factory method for creation of new instances.
// Throws ValidationException if the validation fails
Boundary boundary = Boundary.Create(lower: 1, upper: 2);

-----------

// the same as above but returns a bool instead of throwing an exception (dictionary-style)
bool created = Boundary.TryCreate(lower: 1, upper: 2, out Boundary boundary);

-----------

// similar to TryCreate but returns a ValidationResult instead of a boolean.
ValidationResult? validationResult = Boundary.Validate(lower: 1, upper: 2, out Boundary boundary);

if (validationResult == ValidationResult.Success)
{
    logger.Information("Boundary {Boundary} created", boundary);
}
else
{
    logger.Warning("Failed to create boundary. Validation result: {ValidationResult}", validationResult.ErrorMessage);
}

-----------

// Equality comparison with 'Equals'
// which compares the members using default or custom comparers.
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

