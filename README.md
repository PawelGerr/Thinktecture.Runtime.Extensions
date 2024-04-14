![Build](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/workflows/CI/badge.svg?branch=master)
![TestResults](https://gist.githubusercontent.com/PawelGerr/043909cfb348b36187d02222da1f372e/raw/badge.svg)
![NuGet Downloads](https://img.shields.io/nuget/dt/Thinktecture.Runtime.Extensions)


[![Thinktecture.Runtime.Extensions](https://img.shields.io/nuget/vpre/Thinktecture.Runtime.Extensions.svg?maxAge=60&label=Thinktecture.Runtime.Extensions)](https://www.nuget.org/packages/Thinktecture.Runtime.Extensions/)  
[![Thinktecture.Runtime.Extensions.EntityFrameworkCore6](https://img.shields.io/nuget/vpre/Thinktecture.Runtime.Extensions.EntityFrameworkCore6.svg?maxAge=60&label=Thinktecture.Runtime.Extensions.EntityFrameworkCore6)](https://www.nuget.org/packages/Thinktecture.Runtime.Extensions.EntityFrameworkCore6/)  
[![Thinktecture.Runtime.Extensions.EntityFrameworkCore7](https://img.shields.io/nuget/vpre/Thinktecture.Runtime.Extensions.EntityFrameworkCore7.svg?maxAge=60&label=Thinktecture.Runtime.Extensions.EntityFrameworkCore7)](https://www.nuget.org/packages/Thinktecture.Runtime.Extensions.EntityFrameworkCore7/)  
[![Thinktecture.Runtime.Extensions.EntityFrameworkCore8](https://img.shields.io/nuget/vpre/Thinktecture.Runtime.Extensions.EntityFrameworkCore8.svg?maxAge=60&label=Thinktecture.Runtime.Extensions.EntityFrameworkCore8)](https://www.nuget.org/packages/Thinktecture.Runtime.Extensions.EntityFrameworkCore8/)  
[![Thinktecture.Runtime.Extensions.Json](https://img.shields.io/nuget/vpre/Thinktecture.Runtime.Extensions.Json.svg?maxAge=60&label=Thinktecture.Runtime.Extensions.Json)](https://www.nuget.org/packages/Thinktecture.Runtime.Extensions.Json/)  
[![Thinktecture.Runtime.Extensions.Newtonsoft.Json](https://img.shields.io/nuget/vpre/Thinktecture.Runtime.Extensions.Newtonsoft.Json.svg?maxAge=60&label=Thinktecture.Runtime.Extensions.Newtonsoft.Json)](https://www.nuget.org/packages/Thinktecture.Runtime.Extensions.Newtonsoft.Json/)  
[![Thinktecture.Runtime.Extensions.MessagePack](https://img.shields.io/nuget/vpre/Thinktecture.Runtime.Extensions.MessagePack.svg?maxAge=60&label=Thinktecture.Runtime.Extensions.MessagePack)](https://www.nuget.org/packages/Thinktecture.Runtime.Extensions.MessagePack/)  
[![Thinktecture.Runtime.Extensions.AspNetCore](https://img.shields.io/nuget/vpre/Thinktecture.Runtime.Extensions.AspNetCore.svg?maxAge=60&label=Thinktecture.Runtime.Extensions.AspNetCore)](https://www.nuget.org/packages/Thinktecture.Runtime.Extensions.AspNetCore/)

This library provides some interfaces, classes, [Roslyn Source Generators](https://docs.microsoft.com/en-us/dotnet/csharp/roslyn-sdk/source-generators-overview), Roslyn Analyzers and Roslyn CodeFixes for implementation of **Smart Enums** and **Value Objects**.

# Documentation for version 7

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
* [Allows custom properties and methods](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Smart-Enums#custom-fields-properties-and-methods)
* [Switch-case/Map](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Smart-Enums#switch-casemap)
* Provides appropriate constructor, based on the specified properties/fields
* Provides means for lookup, cast and type conversion from key-type to Smart Enum and vice versa
* Provides proper implementation of `Equals`, `GetHashCode`, `ToString` and equality comparison via `==` and `!=`
* Provides implementation of `IComparable`, `IComparable<T>`, `IFormattable`, `IParsable<T>` and comparison operators `<`, `<=`, `>`, `>=` (if applicable to the underlying type)
* [Choice between always-valid and maybe-valid Smart Enum](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Smart-Enums#always-valid-vs-maybe-valid-smart-enum)
* Smart Enum can also be keyless, i.e. without a key member
* [Makes use of abstract static members](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Smart-Enums#make-use-of-abstract-static-members)
* [Derived types can be generic](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Smart-Enums#generic-derived-types)
* [Allows custom validation of constructor arguments](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Smart-Enums#validation-of-the-constructor-arguments)
* [Allows changing the key member name, kind and access modifier](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Smart-Enums#key-member-generation), which holds the underlying value - thanks to [Roslyn Source Generator](https://docs.microsoft.com/en-us/dotnet/csharp/roslyn-sdk/source-generators-overview)
* Allows [custom key equality comparer](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Smart-Enums#custom-equality-comparer) and [custom comparer](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Smart-Enums#custom-comparer)
* [JSON support](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Smart-Enums#json-serialization) (`System.Text.Json` and `Newtonsoft.Json`)
* [Support for Minimal Api Parameter Binding and ASP.NET Core Model Binding](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Smart-Enums#support-for-minimal-api-parameter-binding-and-aspnet-core-model-binding)
* [Entity Framework Core support](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Smart-Enums#support-for-entity-framework-core) (`ValueConverter`)
* [MessagePack support](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Smart-Enums#messagepack-serialization) (`IMessagePackFormatter`)
* [Logging for debugging or getting insights](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Smart-Enums#logging-v610-or-higher)

Definition of a 2 Smart Enums without any custom properties and methods. All other features mentioned above are generated by the [Roslyn Source Generators](https://docs.microsoft.com/en-us/dotnet/csharp/roslyn-sdk/source-generators-overview) in the background.

```C#
// Smart Enum with a string as the underlying type
[SmartEnum<string>]
public sealed partial class ProductType
{
   public static readonly ProductType Groceries = new("Groceries");
   public static readonly ProductType Housewares = new("Housewares");
}

// Smart Enum with an int as the underlying type
[SmartEnum<int>]
public sealed partial class ProductGroup
{
   public static readonly ProductGroup Apple = new(1);
   public static readonly ProductGroup Orange = new(2);
}

// Smart Enum without identifier (keyless)
[SmartEnum]
public sealed partial class SalesCsvImporterType
{
   public static readonly SalesCsvImporterType Daily = new(articleIdIndex: 0, volumeIndex: 2);
   public static readonly SalesCsvImporterType Monthly = new(articleIdIndex: 2, volumeIndex: 0);

   public int ArticleIdIndex { get; }
   public int VolumeIndex { get; }
}
```

Behind the scenes a Roslyn Source Generator generates additional code. Some of the features that are now available are ...

```C#
// A private constructor which takes the key "Groceries" and additional members (if we had any)
[SmartEnum<string>]
public sealed partial class ProductType
{
   public static readonly ProductType Groceries = new("Groceries");
   ...

------------

// A property for iteration over all items
IReadOnlyList<ProductType> allTypes = ProductType.Items;

------------

// Getting the item with specific name, i.e. its key.
// Throws UnknownEnumIdentifierException if the provided key doesn't belong to any item
ProductType productType = ProductType.Get("Groceries");

// Alternatively, using an explicit cast (behaves the same as "Get")
ProductType productType = (ProductType)"Groceries";

------------

// the same as above but returns a bool instead of throwing an exception (dictionary-style)
bool found = ProductType.TryGet("Groceries", out ProductType? productType);

------------

// similar to TryGet but accepts `IFormatProvider` and returns a ValidationError instead of a boolean.
ValidationError? validationError = ProductType.Validate("Groceries", null, out ProductType? productType);

if (validationError is null)
{
    logger.Information("Product type {Type} found with Validate", productType);
}
else
{
    logger.Warning("Failed to fetch the product type with Validate. Validation error: {validationError}", validationError.ToString());
}

------------

// implicit conversion to the type of the key
string key = ProductType.Groceries; // "Groceries"

------------

// Equality comparison
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

// Switch-case (with "Action")
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
var returnValue = productType.Switch(logger,
                                     ProductType.Groceries, static l => "Switch with Func<T>: Groceries",
                                     ProductType.Housewares, static l => "Switch with Func<T>: Housewares");

// Map an item to another instance
returnValue = productType.Map(ProductType.Groceries, "Map: Groceries",
                              ProductType.Housewares, "Map: Housewares");
------------

// Implements IParsable<T> which is especially helpful with minimal apis.
bool parsed = ProductType.TryParse("Groceries", null, out ProductType? parsedProductType);

------------

// Implements IFormattable if the underlyng type (like int) is an IFormattable itself.
var formatted = ProductGroup.Fruits.ToString("000", CultureInfo.InvariantCulture); // 001

------------

// Implements IComparable and IComparable<T> if the key member type (like int) is an IComparable itself.
var comparison = ProductGroup.Fruits.CompareTo(ProductGroup.Vegetables); // -1

// Implements comparison operators (<,<=,>,>=) if the underlyng type (like int) has comparison operators itself.
var isBigger = ProductGroup.Fruits > ProductGroup.Vegetables;       
```

Definition of a new Smart Enum with 1 custom property `RequiresFoodVendorLicense` and 1 method `Do` with different behaviors for different enum items.

```C#
[SmartEnum<string>]
public partial class ProductType
{
   public static readonly ProductType Groceries = new("Groceries",  requiresFoodVendorLicense: true);
   public static readonly ProductType Housewares = new HousewaresProductType();

   public bool RequiresFoodVendorLicense { get; }

   public virtual void Do()
   {
      // do default stuff
   }

   private sealed class HousewaresProductType : ProductType
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

