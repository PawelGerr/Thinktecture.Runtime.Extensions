# Thinktecture.Runtime.Extensions Project Overview

## Purpose
This is a .NET library providing runtime extensions that help solve common software development problems:

1. **Smart Enums**: Type-safe, extensible alternatives to traditional C# enums with rich behavior and validation
2. **Value Objects**: Type-safe wrappers that prevent primitive obsession and provide built-in validation
3. **Discriminated Unions**: Type-safe representation of values that can be one of several different types

## Technology Stack
- **.NET 9.0 SDK** (defined in global.json)
- **C# 13.0** with nullable reference types enabled
- **Roslyn Source Generators** for compile-time code generation
- **Roslyn Analyzers** for development-time guidance
- **Central Package Management** via Directory.Packages.props

## Framework Integrations
The library provides integration packages for:
- **Serialization**: System.Text.Json, Newtonsoft.Json, MessagePack, ProtoBuf
- **Entity Framework Core**: Versions 7, 8, and 9
- **ASP.NET Core**: Model binding and API integration
- **Swashbuckle**: OpenAPI documentation support

## Testing Framework
- **xUnit** for unit testing
- **AwesomeAssertions** for readable test assertions
- **Verify.Xunit** for snapshot testing
- **NSubstitute** for mocking
- Test results stored in `test-results/$(TargetFramework)` directory

## Project Structure
- `src/`: Core library and integration packages
- `test/`: Unit tests mirroring the src structure
- `samples/`: Example projects demonstrating usage
- `docs/`: Markdown documentation
- All projects target **.NET 7.0** as baseline with LangVersion 13.0

## Key Features
- Compile-time code generation reduces boilerplate
- Comprehensive validation with descriptive error messages
- Exhaustive pattern matching for unions and smart enums
- Framework integrations for seamless usage in .NET applications
- Rich developer experience with analyzers and code fixes