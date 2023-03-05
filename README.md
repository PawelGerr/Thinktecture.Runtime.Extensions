![Build](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/workflows/CI/badge.svg?branch=master)
![TestResults](https://gist.githubusercontent.com/PawelGerr/043909cfb348b36187d02222da1f372e/raw/badge.svg)

[![Thinktecture.Runtime.Extensions](https://img.shields.io/nuget/vpre/Thinktecture.Runtime.Extensions.svg?maxAge=60&label=Thinktecture.Runtime.Extensions)](https://www.nuget.org/packages/Thinktecture.Runtime.Extensions/)  
[![Thinktecture.Runtime.Extensions.EntityFrameworkCore](https://img.shields.io/nuget/v/Thinktecture.Runtime.Extensions.EntityFrameworkCore.svg?maxAge=60&label=Thinktecture.Runtime.Extensions.EntityFrameworkCore+(DEPRECATED+in+v5))](https://www.nuget.org/packages/Thinktecture.Runtime.Extensions.EntityFrameworkCore/)  
[![Thinktecture.Runtime.Extensions.EntityFrameworkCore5](https://img.shields.io/nuget/vpre/Thinktecture.Runtime.Extensions.EntityFrameworkCore5.svg?maxAge=60&label=Thinktecture.Runtime.Extensions.EntityFrameworkCore5)](https://www.nuget.org/packages/Thinktecture.Runtime.Extensions.EntityFrameworkCore5/)  
[![Thinktecture.Runtime.Extensions.EntityFrameworkCore6](https://img.shields.io/nuget/vpre/Thinktecture.Runtime.Extensions.EntityFrameworkCore6.svg?maxAge=60&label=Thinktecture.Runtime.Extensions.EntityFrameworkCore6)](https://www.nuget.org/packages/Thinktecture.Runtime.Extensions.EntityFrameworkCore6/)  
[![Thinktecture.Runtime.Extensions.EntityFrameworkCore7](https://img.shields.io/nuget/vpre/Thinktecture.Runtime.Extensions.EntityFrameworkCore7.svg?maxAge=60&label=Thinktecture.Runtime.Extensions.EntityFrameworkCore7)](https://www.nuget.org/packages/Thinktecture.Runtime.Extensions.EntityFrameworkCore7/)  
[![Thinktecture.Runtime.Extensions.Json](https://img.shields.io/nuget/vpre/Thinktecture.Runtime.Extensions.Json.svg?maxAge=60&label=Thinktecture.Runtime.Extensions.Json)](https://www.nuget.org/packages/Thinktecture.Runtime.Extensions.Newtonsoft.Json/)  
[![Thinktecture.Runtime.Extensions.Newtonsoft.Json](https://img.shields.io/nuget/vpre/Thinktecture.Runtime.Extensions.Newtonsoft.Json.svg?maxAge=60&label=Thinktecture.Runtime.Extensions.Newtonsoft.Json)](https://www.nuget.org/packages/Thinktecture.Runtime.Extensions.Newtonsoft.Json/)  
[![Thinktecture.Runtime.Extensions.MessagePack](https://img.shields.io/nuget/vpre/Thinktecture.Runtime.Extensions.MessagePack.svg?maxAge=60&label=Thinktecture.Runtime.Extensions.MessagePack)](https://www.nuget.org/packages/Thinktecture.Runtime.Extensions.MessagePack.Json/)  
[![Thinktecture.Runtime.Extensions.AspNetCore](https://img.shields.io/nuget/vpre/Thinktecture.Runtime.Extensions.AspNetCore.svg?maxAge=60&label=Thinktecture.Runtime.Extensions.AspNetCore)](https://www.nuget.org/packages/Thinktecture.Runtime.Extensions.AspNetCore/)

This library provides some interfaces, classes, [Roslyn Source Generators](https://docs.microsoft.com/en-us/dotnet/csharp/roslyn-sdk/source-generators-overview), Roslyn Analyzers and Roslyn CodeFixes for implementation of **Smart Enums** and **Value Objects**.

See [wiki](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki) for more documentation.

# Requirements

* Version 6:
  * C# 11 (or higher) for generated code
  * SDK 7.0.102 (or higher) for building projects
* Version 5:
  * C# 9 (or higher) for generated code
  * SDK 6.0.300 (or higher) for building projects

# Smart Enums

Install: `Install-Package Thinktecture.Runtime.Extensions`

Documentation: [Smart Enums](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Smart-Enums)

Features:
* Roslyn Analyzers and CodeFixes help the developers to implement the Smart Enums correctly
* [Allows iteration over all items](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Smart-Enums#what-is-implemented-for-you)
* [Allows custom properties and methods](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Smart-Enums#adding-behavior)
* [Switch-case](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Smart-Enums#switch-case)
* Provides appropriate constructor, based on the specified properties/fields
* Provides means for lookup, cast and type conversion from key-type to Smart Enum and vice versa
* Provides proper implementation of `Equals`, `GetHashCode`, `ToString` and equality comparison via `==` and `!=`
* Provides implementation of `IComparable`, `IComparable<T>`, `IFormattable`, `IParsable<T>` and comparison operators `<`, `<=`, `>`, `>=` (if applicable to the underlying type)
* [Choice between always-valid `IEnum<T>` and maybe-valid `IValidatableEnum<T>`](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Smart-Enums#ienumt-vs-ivalidatableenumt)
* [Makes use of abstract static members](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Smart-Enums#make-use-of-abstract-static-members)
* [Derived types can be generic](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Smart-Enums#generic-derived-types)
* [Allows custom validation of constructor arguments](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Smart-Enums#validation-of-the-constructor-arguments)
* [Allows changing the property name `Key`](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Smart-Enums#changing-the-key-property-name), which holds the underlying value - thanks to [Roslyn Source Generator](https://docs.microsoft.com/en-us/dotnet/csharp/roslyn-sdk/source-generators-overview)
* [Allows custom key comparer](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Smart-Enums#custom-key-comparer)
* [JSON support](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Smart-Enums#json-serialization) (`System.Text.Json` and `Newtonsoft.Json`)
* [Support for Minimal Web Api Parameter Binding and ASP.NET Core Model Binding](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Smart-Enums#support-for-minimal-web-api-parameter-binding-and-aspnet-core-model-binding)
* [Entity Framework Core support](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Smart-Enums#support-for-entity-framework-core) (`ValueConverter`)
* [MessagePack support](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Smart-Enums#messagepack-serialization) (`IMessagePackFormatter`)

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
* Allows custom validation of [constructor](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Value-Objects#validation-of-the-constructor-arguments) and [factory method](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Value-Objects#validation-of-the-factory-method-arguments) arguments
* Additional features for [simple Value Objects (1 "key"-property/field)](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Value-Objects#simple-value-objects) and [complex Value Objects (2 properties/fields or more)](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Value-Objects#complex-value-objects)
* Simple Value Objects: allows cast and type conversion from key-type to Value Object and vice versa
* Simple Value Objects: provides an implementation of `IComparable<T>` if the key-property/field is an `IComparable<T>` or has an `IComparer<T>`
* Simple Value Objects: provides an implementation of `IFormattable` if the key-property/field is an `IFormattable`
* Provides proper implementation of `Equals`, `GetHashCode`, `ToString` and equality comparison via `==` and `!=`
* [Allows custom equality comparison](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Value-Objects#custom-comparer)
* Handling of [null](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Value-Objects#null-in-factory-methods-yields-null) and [empty strings](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Value-Objects#empty-string-in-factory-methods-yields-null)
* [JSON support](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Value-Objects#json-serialization) (`System.Text.Json` and `Newtonsoft.Json`)
* [ASP.NET Core support](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Value-Objects#support-for-aspnet-core-model-binding) (model binding and model validation)
* [Entity Framework Core support](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Value-Objects#support-for-entity-framework-core) (`ValueConverter`)
* [MessagePack support](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Value-Objects#messagepack-serialization) (`IMessagePackFormatter`)

Definition of a value object with 1 custom property `Value`. All other features mentioned above are generated by the [Roslyn Source Generators](https://docs.microsoft.com/en-us/dotnet/csharp/roslyn-sdk/source-generators-overview) in the background.

```C#
[ValueObject]
public partial class ProductName
{
   public string Value { get; }

   // The member can be a private readoly field as well
   //private readonly string _value;
}
```

Definition of a complex value object with 2 properties and a custom validation of the arguments.

```C#
[ValueObject]
public partial class Boundary
{
   public decimal Lower { get; }
   public decimal Upper { get; }

   static partial void ValidateFactoryArguments(ref ValidationResult? validationResult, ref decimal lower, ref decimal upper)
   {
      if (lower <= upper)
         return;

      validationResult = new ValidationResult($"Lower boundary '{lower}' must be less than upper boundary '{upper}'");
   }
}
```
