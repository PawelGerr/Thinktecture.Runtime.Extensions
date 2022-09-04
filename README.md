![Build](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/workflows/CI/badge.svg?branch=master)


[![Thinktecture.Runtime.Extensions](https://img.shields.io/nuget/vpre/Thinktecture.Runtime.Extensions.svg?maxAge=60&label=Thinktecture.Runtime.Extensions)](https://www.nuget.org/packages/Thinktecture.Runtime.Extensions/)  
[![Thinktecture.Runtime.Extensions.EntityFrameworkCore](https://img.shields.io/nuget/v/Thinktecture.Runtime.Extensions.EntityFrameworkCore.svg?maxAge=60&label=Thinktecture.Runtime.Extensions.EntityFrameworkCore+(DEPRECATED+in+v5))](https://www.nuget.org/packages/Thinktecture.Runtime.Extensions.EntityFrameworkCore/)  
[![Thinktecture.Runtime.Extensions.EntityFrameworkCore5](https://img.shields.io/nuget/vpre/Thinktecture.Runtime.Extensions.EntityFrameworkCore5.svg?maxAge=60&label=Thinktecture.Runtime.Extensions.EntityFrameworkCore5)](https://www.nuget.org/packages/Thinktecture.Runtime.Extensions.EntityFrameworkCore5/)  
[![Thinktecture.Runtime.Extensions.EntityFrameworkCore6](https://img.shields.io/nuget/vpre/Thinktecture.Runtime.Extensions.EntityFrameworkCore6.svg?maxAge=60&label=Thinktecture.Runtime.Extensions.EntityFrameworkCore6)](https://www.nuget.org/packages/Thinktecture.Runtime.Extensions.EntityFrameworkCore6/)  
[![Thinktecture.Runtime.Extensions.Json](https://img.shields.io/nuget/vpre/Thinktecture.Runtime.Extensions.Json.svg?maxAge=60&label=Thinktecture.Runtime.Extensions.Json)](https://www.nuget.org/packages/Thinktecture.Runtime.Extensions.Newtonsoft.Json/)  
[![Thinktecture.Runtime.Extensions.Newtonsoft.Json](https://img.shields.io/nuget/vpre/Thinktecture.Runtime.Extensions.Newtonsoft.Json.svg?maxAge=60&label=Thinktecture.Runtime.Extensions.Newtonsoft.Json)](https://www.nuget.org/packages/Thinktecture.Runtime.Extensions.Newtonsoft.Json/)  
[![Thinktecture.Runtime.Extensions.MessagePack](https://img.shields.io/nuget/vpre/Thinktecture.Runtime.Extensions.MessagePack.svg?maxAge=60&label=Thinktecture.Runtime.Extensions.MessagePack)](https://www.nuget.org/packages/Thinktecture.Runtime.Extensions.MessagePack.Json/)  
[![Thinktecture.Runtime.Extensions.EntityFrameworkCore](https://img.shields.io/nuget/vpre/Thinktecture.Runtime.Extensions.EntityFrameworkCore.svg?maxAge=60&label=Thinktecture.Runtime.Extensions.EntityFrameworkCore)](https://www.nuget.org/packages/Thinktecture.Runtime.Extensions.EntityFrameworkCore/)  
[![Thinktecture.Runtime.Extensions.AspNetCore](https://img.shields.io/nuget/vpre/Thinktecture.Runtime.Extensions.AspNetCore.svg?maxAge=60&label=Thinktecture.Runtime.Extensions.AspNetCore)](https://www.nuget.org/packages/Thinktecture.Runtime.Extensions.AspNetCore/)

This library provides some interfaces, classes, [Roslyn Source Generators](https://docs.microsoft.com/en-us/dotnet/csharp/roslyn-sdk/source-generators-overview), Roslyn Analyzers and Roslyn CodeFixes for implementation of **Smart Enums** and **Value Objects**.

See [wiki](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki) for more documentation.

# Required SDK/Compiler Version
* Compiler version: 4.2.0

Verify the version by placing `#error version` into any of your cs-files and build the project/solution.  
The build output should display the compiler version:
```
MyFile.cs(15, 8): [CS1029] #error: 'version'
MyFile.cs(15, 8): [CS8304] Compiler version: '4.2.0-4.22220.2 (1e40aa11)'. Language version: 10.0.
```

## Update your IDE and SDK to newest version.  
Works/tested with:
* SDK: 6.0.300 / 6.0.301
* Visual Studio: 17.2.4
* JetBrains Rider: 2022.1.2

> Please note: For developers having both, JetBrains Rider and Visual Studio, please update Visual Studio as well, because Rider is using the SDK of Visual Studio by default.


# Smart Enums

Install: `Install-Package Thinktecture.Runtime.Extensions`

Documentation: [Smart Enums](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Smart-Enums)

Features:
* Roslyn Analyzers and CodeFixes help the developers to implement the Smart Enums correctly
* Allows iteration over all items
* Allows custom properties and methods
* Provides appropriate constructor based on the specified properties/fields
* Provides means for lookup, cast and type conversion from key-type to Smart Enum and vice versa
* Provides proper implementation of `Equals`, `GetHashCode`, `ToString` and equality comparison via `==` and `!=`
* Choice between always-valid `IEnum<T>` and maybe-valid `IValidatableEnum<T>`
* Allows custom validation of constructor arguments
* Allows changing the property name `Key`, which holds the underlying value - thanks to [Roslyn Source Generator](https://docs.microsoft.com/en-us/dotnet/csharp/roslyn-sdk/source-generators-overview)
* Allows custom key comparer
* JSON support (`System.Text.Json` and `Newtonsoft.Json`)
* ASP.NET Core support (model binding and model validation)
* Entity Framework Core support (`ValueConverter`)
* MessagePack support (`IMessagePackFormatter`)

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
* Allows custom validation of constructor and factory method arguments
* Additional features for simple Value Objects (1 "key"-property/field) and complex Value Objects (2 properties/fields or more)
* Simple Value Objects: allows cast and type conversion from key-type to Value Object and vice versa
* Simple Value Objects: provides an implementation of `IComparable<T>` if the key-property/field is an `IComparable<T>` or has an `IComparer<T>`
* Simple Value Objects: provides an implementation of `IFormattable` if the key-property/field is an `IFormattable`
* Provides proper implementation of `Equals`, `GetHashCode`, `ToString` and equality comparison via `==` and `!=`
* Allows custom equality comparison
* JSON support (`System.Text.Json` and `Newtonsoft.Json`)
* ASP.NET Core support (model binding and model validation)
* Entity Framework Core support (`ValueConverter`)
* MessagePack support (`IMessagePackFormatter`)

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
