using Microsoft.CodeAnalysis;
using Thinktecture.CodeAnalysis.Diagnostics;

namespace Thinktecture.CodeAnalysis;

internal static class DiagnosticsDescriptors
{
   public static readonly DiagnosticDescriptor FieldMustBeReadOnly = new("TTRESG001", "Field must be read-only", "The field '{0}' of the type '{1}' must be read-only", nameof(ThinktectureRuntimeExtensionsAnalyzer), DiagnosticSeverity.Error, true);
   public static readonly DiagnosticDescriptor EnumItemMustBePublic = new("TTRESG002", "Enumeration item must be public", "The enumeration item '{0}' of the type '{1}' must be public", nameof(ThinktectureRuntimeExtensionsAnalyzer), DiagnosticSeverity.Error, true);
   public static readonly DiagnosticDescriptor PropertyMustBeReadOnly = new("TTRESG003", "Property must be read-only", "The property '{0}' of the type '{1}' must be read-only", nameof(ThinktectureRuntimeExtensionsAnalyzer), DiagnosticSeverity.Error, true);
   public static readonly DiagnosticDescriptor TypeMustBeClassOrStruct = new("TTRESG004", "The type must be a class or a struct", "The type '{0}' must be a class or a struct", nameof(ThinktectureRuntimeExtensionsAnalyzer), DiagnosticSeverity.Error, true);
   public static readonly DiagnosticDescriptor MultipleIncompatibleEnumInterfaces = new("TTRESG005", "Multiple interfaces with different key types", "The type '{0}' implements multiple interfaces with different key types", nameof(ThinktectureRuntimeExtensionsAnalyzer), DiagnosticSeverity.Error, true);
   public static readonly DiagnosticDescriptor TypeMustBePartial = new("TTRESG006", "Type must be partial", "The type '{0}' must be partial", nameof(ThinktectureRuntimeExtensionsAnalyzer), DiagnosticSeverity.Error, true);
   public static readonly DiagnosticDescriptor InvalidSignatureOfCreateInvalidItem = new("TTRESG007", $"Incorrect signature of the method '{Constants.Methods.CREATE_INVALID_ITEM}'", $"The signature of the method '{Constants.Methods.CREATE_INVALID_ITEM}' must be 'private static {{0}} {Constants.Methods.CREATE_INVALID_ITEM}({{1}} key)'", nameof(ThinktectureRuntimeExtensionsAnalyzer), DiagnosticSeverity.Error, true);
   public static readonly DiagnosticDescriptor AbstractEnumNeedsCreateInvalidItemImplementation = new("TTRESG008", $"An abstract class needs an implementation of the method '{Constants.Methods.CREATE_INVALID_ITEM}'", $"An abstract class needs an implementation of the method '{Constants.Methods.CREATE_INVALID_ITEM}', the signature should be 'private static {{0}} {Constants.Methods.CREATE_INVALID_ITEM}({{1}} key)'", nameof(ThinktectureRuntimeExtensionsAnalyzer), DiagnosticSeverity.Error, true);
   public static readonly DiagnosticDescriptor EnumConstructorsMustBePrivate = new("TTRESG009", "An enumeration must have private constructors only", "All constructors of the enumeration '{0}' must be private", nameof(ThinktectureRuntimeExtensionsAnalyzer), DiagnosticSeverity.Error, true);
   public static readonly DiagnosticDescriptor NonValidatableEnumsMustBeClass = new("TTRESG010", "An non-validatable enumeration must be a class", "A non-validatable enum must be a class", nameof(ThinktectureRuntimeExtensionsAnalyzer), DiagnosticSeverity.Error, true);
   public static readonly DiagnosticDescriptor StructMustBeReadOnly = new("TTRESG011", "A struct must be readonly", "A struct must be readonly", nameof(ThinktectureRuntimeExtensionsAnalyzer), DiagnosticSeverity.Error, true);
   public static readonly DiagnosticDescriptor EnumKeyPropertyNameNotAllowed = new("TTRESG012", "Provided KeyPropertyName is not allowed", "Provided KeyPropertyName '{0}' is not allowed", nameof(ThinktectureRuntimeExtensionsAnalyzer), DiagnosticSeverity.Error, true);
   public static readonly DiagnosticDescriptor DerivedTypeMustNotImplementEnumInterfaces = new("TTRESG013", "Derived types must not implement IEnum<T> nor IValidatableEnum<T>", "Derived type '{0}' must not implement IEnum<T> nor IValidatableEnum<T> if a base type implements one of this interfaces already", nameof(ThinktectureRuntimeExtensionsAnalyzer), DiagnosticSeverity.Error, true);
   public static readonly DiagnosticDescriptor InnerEnumOnFirstLevelMustBePrivate = new("TTRESG014", "First-level inner enumerations must be private", "Derived inner enumeration '{0}' on first-level must be private", nameof(ThinktectureRuntimeExtensionsAnalyzer), DiagnosticSeverity.Error, true);
   public static readonly DiagnosticDescriptor InnerEnumOnNonFirstLevelMustBePublic = new("TTRESG015", "Non-first-level inner enumerations must be public", "Derived inner enumeration '{0}' on non-first-level must be public", nameof(ThinktectureRuntimeExtensionsAnalyzer), DiagnosticSeverity.Error, true);
   public static readonly DiagnosticDescriptor TypeCannotBeNestedClass = new("TTRESG016", "The type cannot be a nested class", "The type '{0}' cannot be a nested class", nameof(ThinktectureRuntimeExtensionsAnalyzer), DiagnosticSeverity.Error, true);
   public static readonly DiagnosticDescriptor KeyMemberShouldNotBeNullable = new("TTRESG017", "The key member must not be nullable", "The key member '{0}' must not be nullable", nameof(ThinktectureRuntimeExtensionsAnalyzer), DiagnosticSeverity.Error, true);
   public static readonly DiagnosticDescriptor ComparerApplicableOnKeyMemberOnly = new("TTRESG031", "A comparer is applicable on a key member only", "A comparer like '{0}' is applicable on a key member only", nameof(ThinktectureRuntimeExtensionsAnalyzer), DiagnosticSeverity.Error, true);
   public static readonly DiagnosticDescriptor EnumsAndValueObjectsMustNotBeGeneric = new("TTRESG033", "Enumerations and value objects must not be generic", "{0} '{1}' must not be generic", nameof(ThinktectureRuntimeExtensionsAnalyzer), DiagnosticSeverity.Error, true);
   public static readonly DiagnosticDescriptor BaseClassFieldMustBeReadOnly = new("TTRESG034", "Field of the base class must be read-only", "The field '{0}' of the base class '{1}' must be read-only", nameof(ThinktectureRuntimeExtensionsAnalyzer), DiagnosticSeverity.Error, true);
   public static readonly DiagnosticDescriptor BaseClassPropertyMustBeReadOnly = new("TTRESG035", "Property of the base class must be read-only", "The property '{0}' of the base class '{1}' must be read-only", nameof(ThinktectureRuntimeExtensionsAnalyzer), DiagnosticSeverity.Error, true);
   public static readonly DiagnosticDescriptor EnumKeyShouldNotBeNullable = new("TTRESG036", "The key must not be nullable", "The generic type T of IEnum<T> and IValidatableEnum<T> must not be nullable", nameof(ThinktectureRuntimeExtensionsAnalyzer), DiagnosticSeverity.Error, true);
   public static readonly DiagnosticDescriptor EnumWithoutDerivedTypesMustBeSealed = new("TTRESG037", "Enumeration without derived types must be sealed", "Enumeration '{0}' without derived types must be sealed", nameof(ThinktectureRuntimeExtensionsAnalyzer), DiagnosticSeverity.Error, true);
   public static readonly DiagnosticDescriptor ValueObjectMustBeSealed = new("TTRESG038", "Value objects must be sealed", "Value object '{0}' must be sealed", nameof(ThinktectureRuntimeExtensionsAnalyzer), DiagnosticSeverity.Error, true);
   public static readonly DiagnosticDescriptor SwitchMustCoverAllItems = new("TTRESG039", "Switch must cover all items", "The switch-case on the enumeration '{0}' does not cover all items. Following items are not covered: {1}.", nameof(ThinktectureRuntimeExtensionsAnalyzer), DiagnosticSeverity.Error, true);
   public static readonly DiagnosticDescriptor DontImplementEnumInterfaceWithTwoGenerics = new("TTRESG040", "Don't implement \"IEnum<TKey, T>\", implement \"IEnum<TKey>\" only", "The interface \"IEnum<{0}, {1}>\" will be implemented by Source Generator, implement \"IEnum<{0}>\" only", nameof(ThinktectureRuntimeExtensionsAnalyzer), DiagnosticSeverity.Error, true);
   public static readonly DiagnosticDescriptor ComparerTypeMustMatchMemberType = new("TTRESG041", "The type of the comparer doesn't match the type of the member", "A comparer '{0}' doesn't match the type of the member", nameof(ThinktectureRuntimeExtensionsAnalyzer), DiagnosticSeverity.Error, true);

   public static readonly DiagnosticDescriptor ErrorDuringCodeAnalysis = new("TTRESG098", "Error during code analysis", "Error during code analysis of '{0}': '{1}'", nameof(ThinktectureRuntimeExtensionsAnalyzer), DiagnosticSeverity.Warning, true);
   public static readonly DiagnosticDescriptor ErrorDuringGeneration = new("TTRESG099", "Error during code generation", "Error during code generation for '{0}': '{1}'", nameof(ThinktectureRuntimeExtensionsAnalyzer), DiagnosticSeverity.Error, true);

   public static readonly DiagnosticDescriptor EnumerationHasNoItems = new("TTRESG100", "The enumeration has no items", "The enumeration '{0}' has no items", nameof(ThinktectureRuntimeExtensionsAnalyzer), DiagnosticSeverity.Warning, true);
   public static readonly DiagnosticDescriptor StaticPropertiesAreNotConsideredItems = new("TTRESG101", "Static properties are not considered enumeration items, use a field instead", "The static property '{0}' is not considered a enumeration item, use a field instead", nameof(ThinktectureRuntimeExtensionsAnalyzer), DiagnosticSeverity.Warning, true);
}
