# Instructions for GitHub Copilot

## About this repository

This repository contains the source code for Thinktecture.Runtime.Extensions, a library that provides extensions for runtime-related types in .NET. It includes features like Smart Enums, Value Objects, Discriminated Unions, and more.

## Target Audience

These instructions are for GitHub Copilot, acting as an assistant to a developer **contributing** to this repository. The goal is to help with tasks like adding new features, fixing bugs, and maintaining the codebase.

These instructions are **not** for end-users of the library. End-user documentation can be found in the `docs/` directory.

## How to contribute

- Follow the coding style defined in the `.editorconfig` file.
- Write unit tests for new features in the `test/` directory.
- Make sure all tests pass before submitting a pull request.
- Update the documentation in the `docs/` directory if you make any changes to the public API.

## Important files

- `Thinktecture.Runtime.Extensions.slnx`: The main solution file.
- `src/`: Contains the source code for the library.
- `test/`: Contains the unit tests.
- `docs/`: Contains the documentation.
- `README.md`: Provides an overview of the project.

## Project Structure

The repository is organized as follows:

- **`src/`**: Contains the source code for the library and its integrations.
    - `Thinktecture.Runtime.Extensions`: The core library with the runtime types.
    - `Thinktecture.Runtime.Extensions.SourceGenerator`: A source generator and analyzer that provides compile-time generation and diagnostics for features like Smart Enums, Value Objects, and Discriminated Unions. When working with these types, be aware that they are often `partial` classes/structs. The source generator creates a lot of the boilerplate code (constructors, equality members, factory methods, etc.).
    - `Thinktecture.Runtime.Extensions.Json`: Provides a `System.Text.Json` converter for the runtime types.
    - `Thinktecture.Runtime.Extensions.Newtonsoft.Json`: Provides a `Newtonsoft.Json` converter for the runtime types.
    - `Thinktecture.Runtime.Extensions.MessagePack`: Provides a `MessagePack` formatter for the runtime types.
    - `Thinktecture.Runtime.Extensions.EntityFrameworkCore7`: Provides integration with Entity Framework Core 7.
    - `Thinktecture.Runtime.Extensions.EntityFrameworkCore8`: Provides integration with Entity Framework Core 8.
    - `Thinktecture.Runtime.Extensions.EntityFrameworkCore9`: Provides integration with Entity Framework Core 9.
    - `Thinktecture.Runtime.Extensions.AspNetCore`: Provides extensions for ASP.NET Core, primarily for model binding of Smart Enums, Value Objects, and Discriminated Unions.
    - `Thinktecture.Runtime.Extensions.Swashbuckle`: Provides extensions for Swashbuckle (OpenAPI).
- **`test/`**: Contains unit tests, with a structure that mirrors the `src/` directory.
- **`samples/`**: Contains sample projects demonstrating how to use the library.
- **`docs/`**: Contains the documentation.
- **`Directory.Packages.props`**: Manages NuGet package versions for the entire solution. Please manage package versions here.

## Technology Stack

- **.NET**: The project is built on .NET, targeting multiple versions. The specific SDK version is defined in `global.json`.
- **C#**: The primary programming language.
- **Source Generators**: Used extensively to reduce boilerplate for types like Smart Enums, Value Objects, and Discriminated Unions.
- **Entity Framework Core**: Support for versions 7, 8, and 9.
- **ASP.NET Core**: For web-related extensions.
- **Testing**: The project uses xUnit for unit testing, along with AwesomeAssertions for more readable assertions and Verify.Xunit for snapshot testing.
- **Serialization**: The library provides integrations for the following serialization libraries:
    - `System.Text.Json`
    - `Newtonsoft.Json`
    - `MessagePack`
- **API Documentation**: Swashbuckle is used for generating OpenAPI specifications for the library's custom types.

## Core Feature Details

### Value Objects (`[ValueObject]`, `[ComplexValueObject]`)
- **Simple vs. Complex**: The library distinguishes between simple (keyed) value objects that wrap a single value, and complex value objects that compose multiple properties.
- **Validation**: Prefer implementing the static partial method `ValidateFactoryArguments` for validation logic. This allows for returning a `ValidationError` and is used by factory methods (`Create`, `TryCreate`, `Validate`). Avoid `ValidateConstructorArguments` as it can only throw exceptions, which integrates poorly with frameworks.
- **Customization**: Pay attention to attributes like `[KeyMemberEqualityComparer]` for custom equality logic (especially for strings) and `[ObjectFactory]` for custom serialization/parsing behavior.

### Smart Enums (`[SmartEnum]`)
- **Keyed vs. Keyless**: Smart Enums can have a key (e.g., `[SmartEnum<string>]`) or be keyless (`[SmartEnum]`).
- **Rich Behavior**: They are not just constants; they can have properties, methods, and item-specific behavior (often implemented via inheritance or delegates using `[UseDelegateFromConstructor]`).
- **Pattern Matching**: Use the generated `Switch` and `Map` methods for exhaustive, type-safe pattern matching.

### Discriminated Unions (`[Union]`)
- **Ad-hoc vs. Regular**: The library supports simple, ad-hoc unions (`[Union<T1, T2>]`) for combining a few types, and regular, inheritance-based unions for modeling more complex domain hierarchies.
- **Serialization**: Ad-hoc unions do not serialize to polymorphic JSON by default. For custom serialization (e.g., to a string), use the `[ObjectFactory<T>]` attribute. Regular unions can be persisted in EF Core using TPH and serialized to polymorphic JSON using `[JsonDerivedType]`.
- **Pattern Matching**: Like Smart Enums, use the generated `Switch` and `Map` methods for exhaustive matching.

## Getting Started

To get started with the project, you'll need to have the .NET 9.0 SDK installed.

1. **Restore NuGet packages**:
   ```bash
   dotnet restore
   ```
2. **Build the solution**:
   ```bash
   dotnet build
   ```
3. **Run the tests**:
   ```bash
   dotnet test
   ```

## Code Style

The project uses an `.editorconfig` file to define and maintain a consistent code style. Please ensure your editor is configured to respect these settings.

Publicly visible types and members must be documented with XML comments, unless they are in a source generator, test, or sample project.

To format the entire solution, you can run the following command:

```bash
dotnet format
```

## Documentation

The documentation is located in the `docs/` directory and is written in Markdown.

When making changes, please ensure that:

- The documentation is clear, concise, and easy to understand.
- Any new pages are added to the `_Sidebar.md` file to make them accessible.
- You preview your changes to ensure they render correctly before submitting a pull request.

## Framework Integration Notes
- **JSON/MessagePack Serialization**: There are two ways to enable serialization for the library types:
    1.  **Project Reference**: Add a reference to the corresponding integration package (e.g., `Thinktecture.Runtime.Extensions.Json`) in the project where the type is defined. This automatically adds the necessary converter attributes. This is the preferred approach.
    2.  **Manual Registration**: Register the converter factory (e.g., `ThinktectureJsonConverterFactory`) in your application's `Startup.cs` or `Program.cs`.
- **Entity Framework Core**: Use the `.UseThinktectureValueConverters()` extension method on `DbContextOptionsBuilder` to automatically configure value converters for all supported types in your model. For regular Discriminated Unions, you may need to configure the discriminator manually.
- **ASP.NET Core Model Binding**: For types to be bindable from the request path, query, or body, they often rely on the `IParsable<T>` interface, which the source generator implements. For string-based keys or custom parsing logic, the `[ObjectFactory<string>]` attribute is essential.

## Workflow for Common Tasks

### Adding a new Integration

When adding support for a new library (e.g., a new serializer, ORM, or web framework), follow these steps:

1.  **Create a new source project**: Add a new `.csproj` file under the `src/` directory (e.g., `src/Thinktecture.Runtime.Extensions.NewIntegration`).
2.  **Create a new test project**: Add a corresponding test project under the `test/` directory (e.g., `test/Thinktecture.Runtime.Extensions.NewIntegration.Tests`).
3.  **Add projects to the solution**: Use `dotnet sln add` to include both the new source and test projects in the main solution file.
4.  **Add dependencies**:
    - The new source project should reference the core `Thinktecture.Runtime.Extensions` project.
    - Add a package reference for the library you are integrating with. Manage the version in the `Directory.Packages.props` file.
5.  **Implement the integration logic**: Write the necessary code, such as custom converters, providers, or extension methods.
6.  **Write comprehensive tests**: Add unit tests in the test project to ensure the integration works correctly and handles edge cases.
7.  **Update the documentation**: Modify the existing documentation for Value Objects (`Value-Objects.md`), Smart Enums (`Smart-Enums.md`), and/or Discriminated Unions (`Discriminated-Unions.md`) in the `docs/` directory to include information about the new integration. Avoid creating a new documentation file for the integration itself.

### Common Use Cases
When implementing new features, consider these common patterns from the documentation:

- **Value Objects**:
    - **ISBN**: A string-based value object with validation and normalization.
    - **Amount**: A decimal-based value object ensuring the value is always positive and rounded.
    - **DateRange**: A complex value object with two `DateOnly` properties and validation to ensure the end date is not before the start date.
- **Smart Enums**:
    - **ShippingMethod**: An enum with additional properties like price, estimated days, and methods to calculate costs.
    - **ProductGroup**: An enum with custom properties that reference other enums (`ProductCategory`).
    - **JSON Discriminator**: Using a Smart Enum to act as a type discriminator in a custom JSON converter.
- **Discriminated Unions**:
    - **PartiallyKnownDate**: A regular union to represent a date that might only have a year, or a year and a month.
    - **Jurisdiction**: A combination of a regular union and value objects to represent different kinds of administrative areas (Country, State, etc.).
    - **Result<T>**: A generic regular union for handling success (`Success(T Value)`) and failure (`Failure(string Error)`) cases without exceptions.
