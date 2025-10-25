- Error: Interface declaration ends with a semicolon and has no body. In C#, interface declarations must have a body, even if empty. Offending declaration:
  public interface IKeyedSerializerGeneratorTypeInformation
     : ITypeFullyQualified, INamespaceAndName, ITypeKindInformation;
  Suggested fix:
  public interface IKeyedSerializerGeneratorTypeInformation : ITypeFullyQualified, INamespaceAndName, ITypeKindInformation { }
