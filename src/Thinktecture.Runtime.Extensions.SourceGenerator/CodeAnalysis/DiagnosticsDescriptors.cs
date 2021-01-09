using Microsoft.CodeAnalysis;
using Thinktecture.CodeAnalysis.Diagnostics;

namespace Thinktecture.CodeAnalysis
{
   internal static class DiagnosticsDescriptors
   {
      public static readonly DiagnosticDescriptor FieldMustBeReadOnly = new("TTRESG001", "Field must be read-only", "The field '{0}' of the type '{1}' must be read-only", nameof(ThinktectureRuntimeExtensionsAnalyzer), DiagnosticSeverity.Error, true);
      public static readonly DiagnosticDescriptor FieldMustBePublic = new("TTRESG002", "Field must be public", "The field '{0}' of the type '{1}' must be public", nameof(ThinktectureRuntimeExtensionsAnalyzer), DiagnosticSeverity.Error, true);
      public static readonly DiagnosticDescriptor PropertyMustBeReadOnly = new("TTRESG003", "Property must be read-only", "The Property '{0}' of the type '{1}' must be read-only", nameof(ThinktectureRuntimeExtensionsAnalyzer), DiagnosticSeverity.Error, true);
      public static readonly DiagnosticDescriptor TypeMustBeClassOrStruct = new("TTRESG004", "The type must be a class or a struct", "The type '{0}' must be a class or a struct", nameof(ThinktectureRuntimeExtensionsAnalyzer), DiagnosticSeverity.Error, true);
      public static readonly DiagnosticDescriptor MultipleIncompatibleEnumInterfaces = new("TTRESG005", "Multiple interfaces with different key types", "The type '{0}' implements multiple interfaces with different key types", nameof(ThinktectureRuntimeExtensionsAnalyzer), DiagnosticSeverity.Error, true);
      public static readonly DiagnosticDescriptor TypeMustBePartial = new("TTRESG006", "Type must be partial", "The type '{0}' must be partial", nameof(ThinktectureRuntimeExtensionsAnalyzer), DiagnosticSeverity.Error, true);
      public static readonly DiagnosticDescriptor InvalidSignatureOfCreateInvalidItem = new("TTRESG007", "Incorrect signature of the method 'CreateInvalidItem'", "The signature of the method 'CreateInvalidItem' must be 'private static {0} CreateInvalidItem({1} key)'", nameof(ThinktectureRuntimeExtensionsAnalyzer), DiagnosticSeverity.Error, true);
      public static readonly DiagnosticDescriptor AbstractEnumNeedsCreateInvalidItemImplementation = new("TTRESG008", "An abstract class needs an implementation of the method 'CreateInvalidItem'", "An abstract class needs an implementation of the method 'CreateInvalidItem', the signature should be 'private static {0} CreateInvalidItem({1} key)'", nameof(ThinktectureRuntimeExtensionsAnalyzer), DiagnosticSeverity.Error, true);
      public static readonly DiagnosticDescriptor ConstructorsMustBePrivate = new("TTRESG009", "An enumeration must have private constructors only", "All constructors of the enumeration '{0}' must be private", nameof(ThinktectureRuntimeExtensionsAnalyzer), DiagnosticSeverity.Error, true);
      public static readonly DiagnosticDescriptor NonValidatableEnumsMustBeClass = new("TTRESG010", "An non-validatable enumeration must be a class", "A non-validatable enum must be a class", nameof(ThinktectureRuntimeExtensionsAnalyzer), DiagnosticSeverity.Error, true);
      public static readonly DiagnosticDescriptor StructMustBeReadOnly = new("TTRESG011", "A struct must be readonly", "A struct must be readonly", nameof(ThinktectureRuntimeExtensionsAnalyzer), DiagnosticSeverity.Error, true);
      public static readonly DiagnosticDescriptor KeyPropertyNameNotAllowed = new("TTRESG012", "Provided KeyPropertyName is not allowed", "Provided KeyPropertyName '{0}' is not allowed", nameof(ThinktectureRuntimeExtensionsAnalyzer), DiagnosticSeverity.Error, true);
      public static readonly DiagnosticDescriptor DerivedTypeMustNotImplementEnumInterfaces = new("TTRESG013", "Derived types must not implement IEnum<T> or IValidatableEnum<T>", "Derived type '{0}' must not implement IEnum<T> or IValidatableEnum<T> if a base type implement one of this interfaces already", nameof(ThinktectureRuntimeExtensionsAnalyzer), DiagnosticSeverity.Error, true);
      public static readonly DiagnosticDescriptor FirstLevelInnerTypeMustBePrivate = new("TTRESG014", "First-level inner types must be private", "Derived type '{0}' must be private", nameof(ThinktectureRuntimeExtensionsAnalyzer), DiagnosticSeverity.Error, true);
      public static readonly DiagnosticDescriptor NonFirstLevelInnerTypeMustBePublic = new("TTRESG015", "Non-first-level inner types must be public", "Derived type '{0}' must be public", nameof(ThinktectureRuntimeExtensionsAnalyzer), DiagnosticSeverity.Error, true);
      public static readonly DiagnosticDescriptor TypeCannotBeNestedClass = new("TTRESG016", "The type cannot be a nested class", "The type '{0}' cannot be a nested class", nameof(ThinktectureRuntimeExtensionsAnalyzer), DiagnosticSeverity.Error, true);
      public static readonly DiagnosticDescriptor KeyMemberShouldNotBeNullable = new("TTRESG017", "The key member should not be nullable", "The key member '{0}' should not be nullable", nameof(ThinktectureRuntimeExtensionsAnalyzer), DiagnosticSeverity.Error, true);

      public static readonly DiagnosticDescriptor NoItemsWarning = new("TTRESG100", "The enumeration has no items", "The enumeration '{0}' has no items", nameof(ThinktectureRuntimeExtensionsAnalyzer), DiagnosticSeverity.Warning, true);
   }
}
