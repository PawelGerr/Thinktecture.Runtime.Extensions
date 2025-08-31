# Task Completion Checklist

## After Implementing a New Feature or Fix

### 1. Code Quality
- [ ] **Build Success**: Ensure `dotnet build` completes without errors
- [ ] **Code Formatting**: Run `dotnet format` to apply consistent formatting
- [ ] **Analyzer Warnings**: Address any new compiler or analyzer warnings
- [ ] **XML Documentation**: Add/update documentation for public APIs

### 2. Testing
- [ ] **Unit Tests**: Write comprehensive unit tests for new functionality
- [ ] **Test Coverage**: Ensure all code paths are tested
- [ ] **Test Execution**: Run `dotnet test` and verify all tests pass
- [ ] **Integration Tests**: Test framework integrations (JSON, EF Core, etc.) if applicable

### 3. Documentation
- [ ] **Wiki Updates**: Update relevant documentation in `docs/` directory
- [ ] **README**: Update README.md if adding new features
- [ ] **Migration Guide**: Update migration documentation if breaking changes
- [ ] **Code Examples**: Add usage examples for new features

### 4. Source Generator Considerations
- [ ] **Generated Code**: Verify generated code compiles and behaves correctly
- [ ] **Analyzer Tests**: Test any new analyzers or code fixes
- [ ] **Performance**: Ensure source generators don't significantly impact compile time

### 5. Framework Integrations
- [ ] **Serialization**: Test JSON, MessagePack, ProtoBuf serialization if applicable
- [ ] **Entity Framework**: Test EF Core integration across supported versions
- [ ] **ASP.NET Core**: Test model binding and API integration
- [ ] **OpenAPI**: Verify Swashbuckle integration generates correct schemas

### 6. Package Management
- [ ] **Dependencies**: Update `Directory.Packages.props` for any new package references
- [ ] **Version Compatibility**: Ensure compatibility with supported framework versions
- [ ] **Package Structure**: Maintain proper separation between core and integration packages

### 7. Before Submitting Pull Request
- [ ] **Release Build**: Run `dotnet build --configuration Release`
- [ ] **All Tests**: Run full test suite with `dotnet test --verbosity normal`
- [ ] **Documentation Review**: Ensure all documentation is clear and accurate
- [ ] **Breaking Changes**: Document any breaking changes in migration guide

## Common Validation Commands
```powershell
# Full validation sequence
dotnet restore
dotnet build --configuration Release
dotnet test --verbosity normal
dotnet format --verify-no-changes

# Quick check during development
dotnet build && dotnet test
```

## Quality Gates
- All tests must pass
- No build warnings or errors
- Code must be properly formatted
- Public APIs must be documented
- Integration tests must validate framework compatibility