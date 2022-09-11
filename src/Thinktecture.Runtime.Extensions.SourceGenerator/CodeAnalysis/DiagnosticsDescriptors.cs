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
   public static readonly DiagnosticDescriptor InvalidSignatureOfCreateInvalidItem = new("TTRESG007", "Incorrect signature of the method 'CreateInvalidItem'", "The signature of the method 'CreateInvalidItem' must be 'private static {0} CreateInvalidItem({1} key)'", nameof(ThinktectureRuntimeExtensionsAnalyzer), DiagnosticSeverity.Error, true);
   public static readonly DiagnosticDescriptor AbstractEnumNeedsCreateInvalidItemImplementation = new("TTRESG008", "An abstract class needs an implementation of the method 'CreateInvalidItem'", "An abstract class needs an implementation of the method 'CreateInvalidItem', the signature should be 'private static {0} CreateInvalidItem({1} key)'", nameof(ThinktectureRuntimeExtensionsAnalyzer), DiagnosticSeverity.Error, true);
   public static readonly DiagnosticDescriptor EnumConstructorsMustBePrivate = new("TTRESG009", "An enumeration must have private constructors only", "All constructors of the enumeration '{0}' must be private", nameof(ThinktectureRuntimeExtensionsAnalyzer), DiagnosticSeverity.Error, true);
   public static readonly DiagnosticDescriptor NonValidatableEnumsMustBeClass = new("TTRESG010", "An non-validatable enumeration must be a class", "A non-validatable enum must be a class", nameof(ThinktectureRuntimeExtensionsAnalyzer), DiagnosticSeverity.Error, true);
   public static readonly DiagnosticDescriptor StructMustBeReadOnly = new("TTRESG011", "A struct must be readonly", "A struct must be readonly", nameof(ThinktectureRuntimeExtensionsAnalyzer), DiagnosticSeverity.Error, true);
   public static readonly DiagnosticDescriptor EnumKeyPropertyNameNotAllowed = new("TTRESG012", "Provided KeyPropertyName is not allowed", "Provided KeyPropertyName '{0}' is not allowed", nameof(ThinktectureRuntimeExtensionsAnalyzer), DiagnosticSeverity.Error, true);
   public static readonly DiagnosticDescriptor DerivedTypeMustNotImplementEnumInterfaces = new("TTRESG013", "Derived types must not implement IEnum<T> nor IValidatableEnum<T>", "Derived type '{0}' must not implement IEnum<T> nor IValidatableEnum<T> if a base type implements one of this interfaces already", nameof(ThinktectureRuntimeExtensionsAnalyzer), DiagnosticSeverity.Error, true);
   public static readonly DiagnosticDescriptor InnerEnumOnFirstLevelMustBePrivate = new("TTRESG014", "First-level inner enumerations must be private", "Derived inner enumeration '{0}' on first-level must be private", nameof(ThinktectureRuntimeExtensionsAnalyzer), DiagnosticSeverity.Error, true);
   public static readonly DiagnosticDescriptor InnerEnumOnNonFirstLevelMustBePublic = new("TTRESG015", "Non-first-level inner enumerations must be public", "Derived inner enumeration '{0}' on non-first-level must be public", nameof(ThinktectureRuntimeExtensionsAnalyzer), DiagnosticSeverity.Error, true);
   public static readonly DiagnosticDescriptor TypeCannotBeNestedClass = new("TTRESG016", "The type cannot be a nested class", "The type '{0}' cannot be a nested class", nameof(ThinktectureRuntimeExtensionsAnalyzer), DiagnosticSeverity.Error, true);
   public static readonly DiagnosticDescriptor KeyMemberShouldNotBeNullable = new("TTRESG017", "The key member should not be nullable", "The key member '{0}' should not be nullable", nameof(ThinktectureRuntimeExtensionsAnalyzer), DiagnosticSeverity.Error, true);
   public static readonly DiagnosticDescriptor KeyComparerMustBeStaticFieldOrProperty = new("TTRESG030", "The key comparer must a static field or property", "The key comparer '{0}' must be a static field or property", nameof(ThinktectureRuntimeExtensionsAnalyzer), DiagnosticSeverity.Error, true);
   public static readonly DiagnosticDescriptor ComparerApplicableOnKeyMemberOnly = new("TTRESG031", "A comparer is applicable on a key member only", "A comparer like '{0}' is applicable on a key member only", nameof(ThinktectureRuntimeExtensionsAnalyzer), DiagnosticSeverity.Error, true);
   public static readonly DiagnosticDescriptor EnumsAndValueObjectsMustNotBeGeneric = new("TTRESG033", "Enumerations and value objects must not be generic", "{0} '{1}' must not be generic", nameof(ThinktectureRuntimeExtensionsAnalyzer), DiagnosticSeverity.Error, true);
   public static readonly DiagnosticDescriptor BaseClassFieldMustBeReadOnly = new("TTRESG034", "Field of the base class must be read-only", "The field '{0}' of the base class '{1}' must be read-only", nameof(ThinktectureRuntimeExtensionsAnalyzer), DiagnosticSeverity.Error, true);
   public static readonly DiagnosticDescriptor BaseClassPropertyMustBeReadOnly = new("TTRESG035", "Property of the base class must be read-only", "The property '{0}' of the base class '{1}' must be read-only", nameof(ThinktectureRuntimeExtensionsAnalyzer), DiagnosticSeverity.Error, true);
   public static readonly DiagnosticDescriptor EnumKeyShouldNotBeNullable = new("TTRESG036", "The key should not be nullable", "The generic type T of IEnum<T> and IValidatableEnum<T> should not be nullable", nameof(ThinktectureRuntimeExtensionsAnalyzer), DiagnosticSeverity.Error, true);

   public static readonly DiagnosticDescriptor ErrorDuringGeneration = new("TTRESG099", "Error during code generation", "Error during code generation for '{0}': '{1}'", nameof(ThinktectureRuntimeExtensionsAnalyzer), DiagnosticSeverity.Error, true);

   public static readonly DiagnosticDescriptor EnumerationHasNoItems = new("TTRESG100", "The enumeration has no items", "The enumeration '{0}' has no items", nameof(ThinktectureRuntimeExtensionsAnalyzer), DiagnosticSeverity.Warning, true);
   public static readonly DiagnosticDescriptor StaticPropertiesAreNotConsideredItems = new("TTRESG101", "Static properties are not considered enumeration items, use a field instead", "The static property '{0}' is not considered a enumeration item, use a field instead", nameof(ThinktectureRuntimeExtensionsAnalyzer), DiagnosticSeverity.Warning, true);
}
