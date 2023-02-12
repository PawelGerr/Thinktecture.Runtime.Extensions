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
* [Choice between always-valid `IEnum<T>` and maybe-valid `IValidatableEnum<T>`](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Smart-Enums#ienumt-vs-ivalidatableenumt)
* [Allows custom validation of constructor arguments](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Smart-Enums#validation-of-the-constructor-arguments)
* [Allows changing the property name `Key`](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Smart-Enums#changing-the-key-property-name), which holds the underlying value - thanks to [Roslyn Source Generator](https://docs.microsoft.com/en-us/dotnet/csharp/roslyn-sdk/source-generators-overview)
* [Allows custom key comparer](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Smart-Enums#custom-key-comparer)
* [JSON support](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Smart-Enums#json-serialization) (`System.Text.Json` and `Newtonsoft.Json`)
* [ASP.NET Core support](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Smart-Enums#support-for-aspnet-core-model-binding) (model binding and model validation)
* [Entity Framework Core support](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Smart-Enums#support-for-entity-framework-core) (`ValueConverter`)
* [MessagePack support](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Smart-Enums#messagepack-serialization) (`IMessagePackFormatter`)

Definition of a new Smart Enum without any custom properties and methods. All other features mentioned above are generated by the [Roslyn Source Generators](https://docs.microsoft.com/en-us/dotnet/csharp/roslyn-sdk/source-generators-overview) in the background.

```C#
public partial class ProductType : IEnum<string>
{
   public static readonly ProductType Groceries = new("Groceries");
   public static readonly ProductType Housewares = new("Housewares");
}
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

Documentation: [Immutable Value Objects](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Immutable-Value-Objects)

Features:
* Roslyn Analyzers and CodeFixes help the developers to implement the Value Objects correctly
* Allows custom properties and methods
* Provides appropriate factory methods for creation of new value objects based on the specified properties/fields
* Allows custom validation of [constructor](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Immutable-Value-Objects#validation-of-the-constructor-arguments) and [factory method](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Immutable-Value-Objects#validation-of-the-factory-method-arguments) arguments
* Additional features for [simple Value Objects (1 "key"-property/field)](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Immutable-Value-Objects#simple-value-objects) and [complex Value Objects (2 properties/fields or more)](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Immutable-Value-Objects#complex-value-objects)
* Simple Value Objects: allows cast and type conversion from key-type to Value Object and vice versa
* Simple Value Objects: provides an implementation of `IComparable<T>` if the key-property/field is an `IComparable<T>` or has an `IComparer<T>`
* Simple Value Objects: provides an implementation of `IFormattable` if the key-property/field is an `IFormattable`
* Provides proper implementation of `Equals`, `GetHashCode`, `ToString` and equality comparison via `==` and `!=`
* [Allows custom equality comparison](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Immutable-Value-Objects#custom-comparer)
* Handling of [null](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Immutable-Value-Objects#null-in-factory-methods-yields-null) and [empty strings](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Immutable-Value-Objects#empty-string-in-factory-methods-yields-null)
* [JSON support](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Immutable-Value-Objects#json-serialization) (`System.Text.Json` and `Newtonsoft.Json`)
* [ASP.NET Core support](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Immutable-Value-Objects#support-for-aspnet-core-model-binding) (model binding and model validation)
* [Entity Framework Core support](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Immutable-Value-Objects#support-for-entity-framework-core) (`ValueConverter`)
* [MessagePack support](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Immutable-Value-Objects#messagepack-serialization) (`IMessagePackFormatter`)

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
