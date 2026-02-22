![Build](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/workflows/CI/badge.svg) ![TestResults](https://gist.githubusercontent.com/PawelGerr/043909cfb348b36187d02222da1f372e/raw/badge.svg) ![NuGet Downloads](https://img.shields.io/nuget/dt/Thinktecture.Runtime.Extensions)

# Thinktecture.Runtime.Extensions

A .NET library that uses Roslyn Source Generators, Analyzers, and CodeFixes to give you **Smart Enums**, **Value Objects**, and **Discriminated Unions** with built-in validation, exhaustive pattern matching, and first-class framework integration -- so you write the declaration and the generator handles the boilerplate.

## Quick Start

```
Install-Package Thinktecture.Runtime.Extensions
```

## Smart Enums

Type-safe enumerations that go beyond plain `enum`. Each item can carry its own data and behavior, the generator produces equality, parsing, `Switch`/`Map`, and serializer integration automatically.

```csharp
[SmartEnum<string>]
public partial class ShippingMethod
{
   public static readonly ShippingMethod Standard = new("STANDARD", basePrice: 5.99m, estimatedDays: 5);
   public static readonly ShippingMethod Express = new("EXPRESS", basePrice: 15.99m, estimatedDays: 2);

   public decimal CalculatePrice(decimal weight) => _basePrice + weight;
}
```

[Full documentation](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Smart-Enums) -- customization, real-world examples, performance tips, framework integration.

## Value Objects

Immutable domain primitives that eliminate primitive obsession. Wrap a single value or multiple properties, add validation, and get factory methods, equality, conversion operators, and serialization for free.

**Simple value object**

```csharp
[ValueObject<decimal>]
public partial struct Amount
{
    static partial void ValidateFactoryArguments(ref ValidationError? validationError, ref decimal value)
    {
        if (value < 0)
            validationError = new ValidationError("Amount cannot be negative");
    }
}
```

**Complex value object**

```csharp
[ComplexValueObject]
public partial class Boundary
{
    public decimal Lower { get; }
    public decimal Upper { get; }

    static partial void ValidateFactoryArguments(ref ValidationError? validationError, ref decimal lower, ref decimal upper)
    {
        if (lower > upper)
            validationError = new ValidationError("Lower must be less than or equal to Upper");
    }
}
```

[Full documentation](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Value-Objects) -- simple & complex value objects, customization, framework integration.

## Discriminated Unions

Model "one of" types with full type safety. Choose ad-hoc unions for quick combinations (`Union<T1, T2>`) or regular unions (`Union`) for rich domain modeling with exhaustive `Switch`/`Map`.

**Ad-hoc union**

```csharp
[Union<string, int>]
public partial struct TextOrNumber;
```

**Regular union**

```csharp
[Union]
public partial record Result<T>
{
    public sealed record Success(T Value) : Result<T>;
    public sealed record Failure(string Error) : Result<T>;
}
```

[Full documentation](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Discriminated-Unions) -- ad-hoc unions, regular unions, customization, framework integration.

## Framework Integration

All generated types integrate with the .NET ecosystem out of the box:

- **System.Text.Json** -- zero-allocation span-based serialization on .NET 9+
- **Entity Framework Core** -- value converters for EF Core 8, 9, and 10
- **ASP.NET Core** -- model binding and Minimal API parameter binding via `IParsable<T>`
- **MessagePack** -- binary serialization support
- **Newtonsoft.Json** -- `JsonConverter` support
- **Swashbuckle / OpenAPI** -- schema and operation filters

## Packages

| Package | NuGet |
|---------|-------|
| Thinktecture.Runtime.Extensions | [![NuGet](https://img.shields.io/nuget/v/Thinktecture.Runtime.Extensions.svg?maxAge=60)](https://www.nuget.org/packages/Thinktecture.Runtime.Extensions/) |
| Thinktecture.Runtime.Extensions.Json | [![NuGet](https://img.shields.io/nuget/v/Thinktecture.Runtime.Extensions.Json.svg?maxAge=60)](https://www.nuget.org/packages/Thinktecture.Runtime.Extensions.Json/) |
| Thinktecture.Runtime.Extensions.Newtonsoft.Json | [![NuGet](https://img.shields.io/nuget/v/Thinktecture.Runtime.Extensions.Newtonsoft.Json.svg?maxAge=60)](https://www.nuget.org/packages/Thinktecture.Runtime.Extensions.Newtonsoft.Json/) |
| Thinktecture.Runtime.Extensions.MessagePack | [![NuGet](https://img.shields.io/nuget/v/Thinktecture.Runtime.Extensions.MessagePack.svg?maxAge=60)](https://www.nuget.org/packages/Thinktecture.Runtime.Extensions.MessagePack/) |
| Thinktecture.Runtime.Extensions.EntityFrameworkCore8 | [![NuGet](https://img.shields.io/nuget/v/Thinktecture.Runtime.Extensions.EntityFrameworkCore8.svg?maxAge=60)](https://www.nuget.org/packages/Thinktecture.Runtime.Extensions.EntityFrameworkCore8/) |
| Thinktecture.Runtime.Extensions.EntityFrameworkCore9 | [![NuGet](https://img.shields.io/nuget/v/Thinktecture.Runtime.Extensions.EntityFrameworkCore9.svg?maxAge=60)](https://www.nuget.org/packages/Thinktecture.Runtime.Extensions.EntityFrameworkCore9/) |
| Thinktecture.Runtime.Extensions.EntityFrameworkCore10 | [![NuGet](https://img.shields.io/nuget/v/Thinktecture.Runtime.Extensions.EntityFrameworkCore10.svg?maxAge=60)](https://www.nuget.org/packages/Thinktecture.Runtime.Extensions.EntityFrameworkCore10/) |
| Thinktecture.Runtime.Extensions.AspNetCore | [![NuGet](https://img.shields.io/nuget/v/Thinktecture.Runtime.Extensions.AspNetCore.svg?maxAge=60)](https://www.nuget.org/packages/Thinktecture.Runtime.Extensions.AspNetCore/) |
| Thinktecture.Runtime.Extensions.Swashbuckle | [![NuGet](https://img.shields.io/nuget/v/Thinktecture.Runtime.Extensions.Swashbuckle.svg?maxAge=60)](https://www.nuget.org/packages/Thinktecture.Runtime.Extensions.Swashbuckle/) |

## Requirements

- C# 11 (or higher) for generated code
- SDK 8.0.416 (or higher) for building projects

## Documentation

- [Smart Enums](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Smart-Enums) -- overview, real-world examples, design patterns
  - [Customization](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Smart-Enums-Customization) -- attribute settings, equality, comparison, parsable interfaces
  - [Framework Integration](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Smart-Enums-Framework-Integration) -- JSON, EF Core, ASP.NET Core, MessagePack
  - [Performance](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Smart-Enums-Performance) -- span-based JSON, zero-allocation parsing, benchmarks
- [Value Objects](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Value-Objects) -- simple & complex value objects, validation
  - [Customization](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Value-Objects-Customization) -- attribute settings, equality comparers, factory methods, operators
  - [Framework Integration](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Value-Objects-Framework-Integration) -- JSON, EF Core, ASP.NET Core, MessagePack
- [Discriminated Unions](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Discriminated-Unions) -- ad-hoc and regular unions, pattern matching
  - [Customization](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Discriminated-Unions-Customization) -- backing fields, stateless types, constructors
  - [Framework Integration](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Discriminated-Unions-Framework-Integration) -- JSON, EF Core, MessagePack
- [Object Factories](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Object-Factories) -- custom creation logic for advanced parsing and deserialization
- [Analyzer Diagnostics](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Analyzer-Diagnostics) -- reference for all `TTRESG` diagnostic rules
- [Source Generator Configuration](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Source-Generator-Configuration) -- MSBuild properties for controlling generator behavior

## Articles

**Smart Enums**:
- [Smart Enums: Beyond Traditional Enumerations in .NET](https://www.thinktecture.com/en/net/smart-enums-beyond-traditional-enumerations-in-dotnet/)
- [Smart Enums: Adding Domain Logic to Enumerations in .NET](https://www.thinktecture.com/en/net/smart-enums-adding-domain-logic-to-enumerations-in-dotnet/)
- [Smart Enums in .NET: Integration with Frameworks and Libraries](https://www.thinktecture.com/en/net/smart-enums-in-net-integration-with-frameworks-and-libraries/)

**Value Objects**:
- [Value Objects: Solving Primitive Obsession in .NET](https://www.thinktecture.com/en/net/value-objects-solving-primitive-obsession-in-net/)
- [Handling Complexity: Introducing Complex Value Objects in .NET](https://www.thinktecture.com/en/net/handling-complexity-introducing-complex-value-objects-in-dotnet/)
- [Value Objects in .NET: Integration with Frameworks and Libraries](https://www.thinktecture.com/en/net/value-objects-in-net-integration-with-frameworks-and-libraries/)
- [Value Objects in .NET: Enhancing Business Semantics](https://www.thinktecture.com/en/net/value-objects-in-dotnet-enhancing-business-semantics/)
- [Advanced Value Object Patterns in .NET](https://www.thinktecture.com/en/net/advanced-value-object-patterns-in-net/)

**Discriminated Unions**:
- [Discriminated Unions: Representation of Alternative Types in .NET](https://www.thinktecture.com/en/net/discriminated-unions-representation-of-alternative-types-in-dotnet/)
- [Pattern Matching with Discriminated Unions in .NET](https://www.thinktecture.com/en/net/pattern-matching-with-discriminated-unions-in-net/)
- [Discriminated Unions in .NET: Modeling States and Variants](https://www.thinktecture.com/en/net/discriminated-unions-in-net-modeling-states-and-variants/)
- [Discriminated Unions in .NET: Integration with Frameworks and Libraries](https://www.thinktecture.com/en/net/discriminated-unions-in-net-integration-with-frameworks-and-libraries/)

## Migrations

- [Migration from v9 to v10](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Migration-from-v9-to-v10)
- [Migration from v8 to v9](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Migration-from-v8-to-v9)
