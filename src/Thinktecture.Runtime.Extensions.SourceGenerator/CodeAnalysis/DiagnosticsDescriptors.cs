using Thinktecture.CodeAnalysis.Diagnostics;

namespace Thinktecture.CodeAnalysis;

internal static class DiagnosticsDescriptors
{
   public static readonly DiagnosticDescriptor FieldMustBeReadOnly = new("TTRESG001", "Field must be read-only", "The field '{0}' of the type '{1}' must be read-only", nameof(ThinktectureRuntimeExtensionsAnalyzer), DiagnosticSeverity.Error, true);
   public static readonly DiagnosticDescriptor EnumItemMustBePublic = new("TTRESG002", "Enumeration item must be public", "The enumeration item '{0}' of the type '{1}' must be public", nameof(ThinktectureRuntimeExtensionsAnalyzer), DiagnosticSeverity.Error, true);
   public static readonly DiagnosticDescriptor PropertyMustBeReadOnly = new("TTRESG003", "Property must be read-only", "The property '{0}' of the type '{1}' must be read-only", nameof(ThinktectureRuntimeExtensionsAnalyzer), DiagnosticSeverity.Error, true);
   public static readonly DiagnosticDescriptor TypeMustBeClassOrStruct = new("TTRESG004", "The type must be a class or a struct", "The type '{0}' must be a class or a struct", nameof(ThinktectureRuntimeExtensionsAnalyzer), DiagnosticSeverity.Error, true);
   public static readonly DiagnosticDescriptor TypeMustBePartial = new("TTRESG006", "Type must be partial", "The type '{0}' must be partial", nameof(ThinktectureRuntimeExtensionsAnalyzer), DiagnosticSeverity.Error, true);
   public static readonly DiagnosticDescriptor InvalidSignatureOfCreateInvalidItem = new("TTRESG007", $"Incorrect signature of the method '{Constants.Methods.CREATE_INVALID_ITEM}'", $"The signature of the method '{Constants.Methods.CREATE_INVALID_ITEM}' must be 'private static {{0}} {Constants.Methods.CREATE_INVALID_ITEM}({{1}} key)'", nameof(ThinktectureRuntimeExtensionsAnalyzer), DiagnosticSeverity.Error, true);
   public static readonly DiagnosticDescriptor AbstractEnumNeedsCreateInvalidItemImplementation = new("TTRESG008", $"An abstract class needs an implementation of the method '{Constants.Methods.CREATE_INVALID_ITEM}'", $"An abstract class needs an implementation of the method '{Constants.Methods.CREATE_INVALID_ITEM}', the signature should be 'private static {{0}} {Constants.Methods.CREATE_INVALID_ITEM}({{1}} key)'", nameof(ThinktectureRuntimeExtensionsAnalyzer), DiagnosticSeverity.Error, true);
   public static readonly DiagnosticDescriptor ConstructorsMustBePrivate = new("TTRESG009", "The constructors must be private", "All constructors of type '{0}' must be private", nameof(ThinktectureRuntimeExtensionsAnalyzer), DiagnosticSeverity.Error, true);
   public static readonly DiagnosticDescriptor NonValidatableEnumsMustBeClass = new("TTRESG010", "An non-validatable enumeration must be a class", "A non-validatable enum must be a class", nameof(ThinktectureRuntimeExtensionsAnalyzer), DiagnosticSeverity.Error, true);
   public static readonly DiagnosticDescriptor EnumKeyMemberNameNotAllowed = new("TTRESG012", "Provided key member name is not allowed", "Provided key member name '{0}' is not allowed", nameof(ThinktectureRuntimeExtensionsAnalyzer), DiagnosticSeverity.Error, true);
   public static readonly DiagnosticDescriptor InnerEnumOnFirstLevelMustBePrivate = new("TTRESG014", "First-level inner enumerations must be private", "Derived inner enumeration '{0}' on first-level must be private", nameof(ThinktectureRuntimeExtensionsAnalyzer), DiagnosticSeverity.Error, true);
   public static readonly DiagnosticDescriptor InnerEnumOnNonFirstLevelMustBePublic = new("TTRESG015", "Non-first-level inner enumerations must be public", "Derived inner enumeration '{0}' on non-first-level must be public", nameof(ThinktectureRuntimeExtensionsAnalyzer), DiagnosticSeverity.Error, true);
   public static readonly DiagnosticDescriptor KeyMemberShouldNotBeNullable = new("TTRESG017", "The key member must not be nullable", "A key member must not be nullable", nameof(ThinktectureRuntimeExtensionsAnalyzer), DiagnosticSeverity.Error, true);
   public static readonly DiagnosticDescriptor EnumsValueObjectsAndAdHocUnionsMustNotBeGeneric = new("TTRESG033", "Enumerations, value objects and ad hoc unions must not be generic", "Type '{0}' must not be generic", nameof(ThinktectureRuntimeExtensionsAnalyzer), DiagnosticSeverity.Error, true);
   public static readonly DiagnosticDescriptor BaseClassFieldMustBeReadOnly = new("TTRESG034", "Field of the base class must be read-only", "The field '{0}' of the base class '{1}' must be read-only", nameof(ThinktectureRuntimeExtensionsAnalyzer), DiagnosticSeverity.Error, true);
   public static readonly DiagnosticDescriptor BaseClassPropertyMustBeReadOnly = new("TTRESG035", "Property of the base class must be read-only", "The property '{0}' of the base class '{1}' must be read-only", nameof(ThinktectureRuntimeExtensionsAnalyzer), DiagnosticSeverity.Error, true);
   public static readonly DiagnosticDescriptor EnumKeyShouldNotBeNullable = new("TTRESG036", "The key must not be nullable", "The generic type T of SmartEnumAttribute<T> must not be nullable", nameof(ThinktectureRuntimeExtensionsAnalyzer), DiagnosticSeverity.Error, true);
   public static readonly DiagnosticDescriptor EnumWithoutDerivedTypesMustBeSealed = new("TTRESG037", "Enumeration without derived types must be sealed", "Enumeration '{0}' without derived types must be sealed", nameof(ThinktectureRuntimeExtensionsAnalyzer), DiagnosticSeverity.Error, true);
   public static readonly DiagnosticDescriptor ComparerTypeMustMatchMemberType = new("TTRESG041", "The type of the comparer doesn't match the type of the member", "The comparer '{0}' doesn't match the member type '{1}'", nameof(ThinktectureRuntimeExtensionsAnalyzer), DiagnosticSeverity.Error, true);
   public static readonly DiagnosticDescriptor InitAccessorMustBePrivate = new("TTRESG042", "Property 'init' accessor must be private", "The 'init' accessor of the property '{0}' of the type '{1}' must be private", nameof(ThinktectureRuntimeExtensionsAnalyzer), DiagnosticSeverity.Error, true);
   public static readonly DiagnosticDescriptor PrimaryConstructorNotAllowed = new("TTRESG043", "Primary constructor is not allowed", "Primary constructor is not allowed in object of type '{0}' because it cannot be marked as 'private'", nameof(ThinktectureRuntimeExtensionsAnalyzer), DiagnosticSeverity.Error, true);
   public static readonly DiagnosticDescriptor CustomKeyMemberImplementationNotFound = new("TTRESG044", "Custom implementation of the key member not found", $"Provide a custom implementation of the key member. Implement a field or property '{{0}}'. Use '{Constants.Attributes.Properties.KEY_MEMBER_NAME}' to change the name.", nameof(ThinktectureRuntimeExtensionsAnalyzer), DiagnosticSeverity.Error, true);
   public static readonly DiagnosticDescriptor CustomKeyMemberImplementationTypeMismatch = new("TTRESG045", "Key member type mismatch", "The type of the key member '{0}' must be '{2}' instead of '{1}'", nameof(ThinktectureRuntimeExtensionsAnalyzer), DiagnosticSeverity.Error, true);
   public static readonly DiagnosticDescriptor IndexBasedSwitchAndMapMustUseNamedParameters = new("TTRESG046", "The arguments of 'Switch' and 'Map' must named", "Not all arguments of 'Switch/Map' on type '{0}' are named", nameof(ThinktectureRuntimeExtensionsAnalyzer), DiagnosticSeverity.Error, true);
   public static readonly DiagnosticDescriptor VariableMustBeInitializedWithNonDefaultValue = new("TTRESG047", "Variable must be initialed with non-default value", "Variable of type '{0}' must be initialized with non default value", nameof(ThinktectureRuntimeExtensionsAnalyzer), DiagnosticSeverity.Error, true);
   public static readonly DiagnosticDescriptor StringBasedValueObjectNeedsEqualityComparer = new("TTRESG048", "String-based Value Object needs equality comparer", "String-based Value Object needs equality comparer. Use ValueObjectKeyMemberEqualityComparerAttribute<TAccessor, TKey> to defined the equality comparer. Example: [ValueObjectKeyMemberEqualityComparer<ComparerAccessors.StringOrdinalIgnoreCase, string>].", nameof(ThinktectureRuntimeExtensionsAnalyzer), DiagnosticSeverity.Warning, true);
   public static readonly DiagnosticDescriptor ComplexValueObjectWithStringMembersNeedsDefaultEqualityComparer = new("TTRESG049", "Complex Value Object with string members needs equality comparer", "Complex Value Object with string members needs equality comparer. Use ValueObjectKeyMemberEqualityComparerAttribute<TAccessor, TKey> to defined the equality comparer. Example: [ValueObjectKeyMemberEqualityComparer<ComparerAccessors.StringOrdinalIgnoreCase, string>].", nameof(ThinktectureRuntimeExtensionsAnalyzer), DiagnosticSeverity.Warning, true);
   public static readonly DiagnosticDescriptor MethodWithUseDelegateFromConstructorMustBePartial = new("TTRESG050", $"Method with '{Constants.Attributes.UseDelegateFromConstructorAttribute.NAME}' must be partial", $"The method '{{0}}' with '{Constants.Attributes.UseDelegateFromConstructorAttribute.NAME}' must be marked as partial", nameof(ThinktectureRuntimeExtensionsAnalyzer), DiagnosticSeverity.Error, true);
   public static readonly DiagnosticDescriptor MethodWithUseDelegateFromConstructorMustNotHaveGenerics = new("TTRESG051", $"Method with '{Constants.Attributes.UseDelegateFromConstructorAttribute.NAME}' must not have generics", $"The method '{{0}}' with '{Constants.Attributes.UseDelegateFromConstructorAttribute.NAME}' must not have generic type parameters. Use inheritance approach instead.", nameof(ThinktectureRuntimeExtensionsAnalyzer), DiagnosticSeverity.Error, true);
   public static readonly DiagnosticDescriptor TypeMustNotBeInsideGenericType = new("TTRESG052", "The type must not be inside generic type", "Type '{0}' must not be inside a generic type", nameof(ThinktectureRuntimeExtensionsAnalyzer), DiagnosticSeverity.Error, true);
   public static readonly DiagnosticDescriptor NonAbstractUnionDerivedTypesMustNotBeGeneric = new("TTRESG053", "Non-abstract derived type of a union must not be generic", "Non-abstract derived type '{0}' of a union must not be generic", nameof(ThinktectureRuntimeExtensionsAnalyzer), DiagnosticSeverity.Error, true);

   public static readonly DiagnosticDescriptor ErrorDuringCodeAnalysis = new("TTRESG098", "Error during code analysis", "Error during code analysis of '{0}': '{1}'", nameof(ThinktectureRuntimeExtensionsAnalyzer), DiagnosticSeverity.Warning, true);
   public static readonly DiagnosticDescriptor ErrorDuringGeneration = new("TTRESG099", "Error during code generation", "Error during code generation for '{0}': '{1}'", nameof(ThinktectureRuntimeExtensionsAnalyzer), DiagnosticSeverity.Error, true);

   public static readonly DiagnosticDescriptor EnumerationHasNoItems = new("TTRESG100", "The enumeration has no items", "The enumeration '{0}' has no items", nameof(ThinktectureRuntimeExtensionsAnalyzer), DiagnosticSeverity.Warning, true);
   public static readonly DiagnosticDescriptor StaticPropertiesAreNotConsideredItems = new("TTRESG101", "Static properties are not considered enumeration items, use a field instead", "The static property '{0}' is not considered a enumeration item, use a field instead", nameof(ThinktectureRuntimeExtensionsAnalyzer), DiagnosticSeverity.Warning, true);
   public static readonly DiagnosticDescriptor ExplicitComparerWithoutEqualityComparer = new("TTRESG102", "The type has a comparer defined but no equality comparer", "The type '{0}' has a comparer defined but no equality comparer", nameof(ThinktectureRuntimeExtensionsAnalyzer), DiagnosticSeverity.Warning, true);
   public static readonly DiagnosticDescriptor ExplicitEqualityComparerWithoutComparer = new("TTRESG103", "The type has an equality comparer defined but no comparer", "The type '{0}' has an equality comparer defined but no comparer", nameof(ThinktectureRuntimeExtensionsAnalyzer), DiagnosticSeverity.Warning, true);
}
