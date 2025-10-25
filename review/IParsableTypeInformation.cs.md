- Error: Interface declaration ends with a semicolon and has no body. In C#, interface declarations must have a body, even if empty. Suggested fix:
  public interface IParsableTypeInformation : ITypeInformationWithNullability, INamespaceAndName { }
