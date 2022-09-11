using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Thinktecture.CodeAnalysis.Diagnostics;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class ThinktectureRuntimeExtensionsAnalyzer : DiagnosticAnalyzer
{
   /// <inheritdoc />
   public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(DiagnosticsDescriptors.TypeMustBePartial,
                                                                                                              DiagnosticsDescriptors.StructMustBeReadOnly,
                                                                                                              DiagnosticsDescriptors.TypeMustBeClassOrStruct,
                                                                                                              DiagnosticsDescriptors.NonValidatableEnumsMustBeClass,
                                                                                                              DiagnosticsDescriptors.EnumConstructorsMustBePrivate,
                                                                                                              DiagnosticsDescriptors.EnumerationHasNoItems,
                                                                                                              DiagnosticsDescriptors.EnumItemMustBePublic,
                                                                                                              DiagnosticsDescriptors.FieldMustBeReadOnly,
                                                                                                              DiagnosticsDescriptors.PropertyMustBeReadOnly,
                                                                                                              DiagnosticsDescriptors.AbstractEnumNeedsCreateInvalidItemImplementation,
                                                                                                              DiagnosticsDescriptors.InvalidSignatureOfCreateInvalidItem,
                                                                                                              DiagnosticsDescriptors.EnumKeyPropertyNameNotAllowed,
                                                                                                              DiagnosticsDescriptors.MultipleIncompatibleEnumInterfaces,
                                                                                                              DiagnosticsDescriptors.DerivedTypeMustNotImplementEnumInterfaces,
                                                                                                              DiagnosticsDescriptors.InnerEnumOnFirstLevelMustBePrivate,
                                                                                                              DiagnosticsDescriptors.InnerEnumOnNonFirstLevelMustBePublic,
                                                                                                              DiagnosticsDescriptors.TypeCannotBeNestedClass,
                                                                                                              DiagnosticsDescriptors.KeyMemberShouldNotBeNullable,
                                                                                                              DiagnosticsDescriptors.StaticPropertiesAreNotConsideredItems,
                                                                                                              DiagnosticsDescriptors.KeyComparerMustBeStaticFieldOrProperty,
                                                                                                              DiagnosticsDescriptors.ComparerApplicableOnKeyMemberOnly,
                                                                                                              DiagnosticsDescriptors.EnumsAndValueObjectsMustNotBeGeneric,
                                                                                                              DiagnosticsDescriptors.BaseClassFieldMustBeReadOnly,
                                                                                                              DiagnosticsDescriptors.BaseClassPropertyMustBeReadOnly,
                                                                                                              DiagnosticsDescriptors.EnumKeyShouldNotBeNullable);

   /// <inheritdoc />
   public override void Initialize(AnalysisContext context)
   {
      context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
      context.EnableConcurrentExecution();

      context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.NamedType);
   }

   private static void AnalyzeSymbol(SymbolAnalysisContext context)
   {
      var type = (INamedTypeSymbol)context.Symbol;

      if (type.DeclaringSyntaxReferences.Length == 0)
         return;

      var declarations = new TypeDeclarationSyntax[type.DeclaringSyntaxReferences.Length];

      for (var i = 0; i < type.DeclaringSyntaxReferences.Length; i++)
      {
         var syntaxRef = type.DeclaringSyntaxReferences[i];

         if (syntaxRef.GetSyntax(context.CancellationToken) is not TypeDeclarationSyntax tds)
            return;

         declarations[i] = tds;
      }

      if (type.IsEnum(out var enumInterfaces))
         ValidateEnum(context, declarations, type, enumInterfaces);

      if (type.HasValueObjectAttribute(out _))
         ValidateValueObject(context, declarations, type);
   }

   private static void ValidateValueObject(
      SymbolAnalysisContext context,
      IReadOnlyList<TypeDeclarationSyntax> declarations,
      INamedTypeSymbol type)
   {
      var locationOfFirstDeclaration = declarations[0].Identifier.GetLocation(); // a representative for all

      if (type.IsRecord ||
          (type.TypeKind != TypeKind.Class && type.TypeKind != TypeKind.Struct))
      {
         ReportDiagnostic(context, DiagnosticsDescriptors.TypeMustBeClassOrStruct, locationOfFirstDeclaration, type);
         return;
      }

      if (type.ContainingType is not null) // is nested class
      {
         ReportDiagnostic(context, DiagnosticsDescriptors.TypeCannotBeNestedClass, locationOfFirstDeclaration, type);
         return;
      }

      TypeMustBePartial(context, type, declarations);
      TypeMustNotBeGeneric(context, type, locationOfFirstDeclaration, "Value Object");
      StructMustBeReadOnly(context, type, locationOfFirstDeclaration);

      var assignableMembers = type.GetAssignableFieldsAndPropertiesAndCheckForReadOnly(false, context.CancellationToken, context.ReportDiagnostic)
                                  .Where(m => !m.IsStatic)
                                  .ToList();

      var baseClass = type.BaseType;

      while (!baseClass.IsNullOrObject())
      {
         baseClass.IterateAssignableFieldsAndPropertiesAndCheckForReadOnly(false, context.CancellationToken, locationOfFirstDeclaration, context.ReportDiagnostic).Enumerate();

         baseClass = baseClass.BaseType;
      }

      if (assignableMembers.Count == 1)
      {
         var keyMember = assignableMembers[0];

         if (keyMember.NullableAnnotation == NullableAnnotation.Annotated || keyMember.IsNullableStruct)
            ReportDiagnostic(context, DiagnosticsDescriptors.KeyMemberShouldNotBeNullable, keyMember.GetIdentifierLocation(), keyMember.Name);
      }
      else
      {
         CheckAssignableMembers(context, assignableMembers);
      }
   }

   private static void CheckAssignableMembers(SymbolAnalysisContext context, IReadOnlyList<InstanceMemberInfo> assignableMembers)
   {
      foreach (var assignableMember in assignableMembers)
      {
         var comparer = assignableMember.ValueObjectMemberSettings.Comparer;

         if (comparer is not null)
         {
            ReportDiagnostic(context,
                             DiagnosticsDescriptors.ComparerApplicableOnKeyMemberOnly,
                             assignableMember.ValueObjectMemberSettings.GetAttributeLocationOrNull(context.CancellationToken) ?? assignableMember.GetIdentifierLocation(),
                             comparer);
         }
      }
   }

   private static void ValidateEnum(
      SymbolAnalysisContext context,
      IReadOnlyList<TypeDeclarationSyntax> declarations,
      INamedTypeSymbol enumType,
      IReadOnlyList<INamedTypeSymbol> enumInterfaces)
   {
      var locationOfFirstDeclaration = declarations[0].Identifier.GetLocation(); // a representative for all

      if (enumType.IsRecord
          || (enumType.TypeKind != TypeKind.Class && enumType.TypeKind != TypeKind.Struct))
      {
         ReportDiagnostic(context, DiagnosticsDescriptors.TypeMustBeClassOrStruct, locationOfFirstDeclaration, enumType);
         return;
      }

      if (enumType.ContainingType is not null) // is nested class
      {
         if (!enumType.BaseType.IsSelfOrBaseTypesAnEnum()) // base class is not an enum because in this case "DiagnosticsDescriptors.DerivedTypeMustNotImplementEnumInterfaces" kicks in
         {
            ReportDiagnostic(context, DiagnosticsDescriptors.TypeCannotBeNestedClass, locationOfFirstDeclaration, enumType);
            return;
         }
      }

      ConstructorsMustBePrivate(context, enumType);
      TypeMustBePartial(context, enumType, declarations);
      TypeMustNotBeGeneric(context, enumType, locationOfFirstDeclaration, "Enumeration");

      var validEnumInterface = enumInterfaces.GetValidEnumInterface(enumType, context.CancellationToken, locationOfFirstDeclaration, context.ReportDiagnostic);

      if (validEnumInterface is null)
         return;

      var keyType = validEnumInterface.TypeArguments[0];

      if (keyType.NullableAnnotation == NullableAnnotation.Annotated || keyType.SpecialType == SpecialType.System_Nullable_T)
         ReportDiagnostic(context, DiagnosticsDescriptors.EnumKeyShouldNotBeNullable, locationOfFirstDeclaration);

      var isValidatable = validEnumInterface.IsValidatableEnumInterface();

      StructMustBeReadOnly(context, enumType, locationOfFirstDeclaration);

      if (enumType.IsValueType && !isValidatable)
         ReportDiagnostic(context, DiagnosticsDescriptors.NonValidatableEnumsMustBeClass, locationOfFirstDeclaration, enumType);

      var items = enumType.EnumerateEnumItems().ToList();

      if (items.Count == 0)
         ReportDiagnostic(context, DiagnosticsDescriptors.EnumerationHasNoItems, locationOfFirstDeclaration, enumType);

      Check_ItemLike_StaticProperties(context, enumType);
      EnumItemsMustBePublic(context, enumType, items);

      if (isValidatable)
         ValidateCreateInvalidItem(context, enumType, keyType, locationOfFirstDeclaration);

      enumType.GetAssignableFieldsAndPropertiesAndCheckForReadOnly(false, context.CancellationToken, context.ReportDiagnostic).Enumerate();
      var baseClass = enumType.BaseType;

      while (!baseClass.IsNullOrObject())
      {
         baseClass.IterateAssignableFieldsAndPropertiesAndCheckForReadOnly(false, context.CancellationToken, locationOfFirstDeclaration, context.ReportDiagnostic).Enumerate();

         baseClass = baseClass.BaseType;
      }

      var enumAttr = enumType.FindEnumGenerationAttribute();

      if (enumAttr is not null)
      {
         EnumKeyPropertyNameMustNotBeItem(context, enumAttr, locationOfFirstDeclaration);

         var comparer = enumAttr.FindKeyComparer();
         var comparerMembers = comparer is null ? Array.Empty<ISymbol>() : enumType.GetNonIgnoredMembers(comparer);

         CheckKeyComparer(context, comparerMembers);
      }

      ValidateDerivedTypes(context, enumType);
   }

   private static void TypeMustNotBeGeneric(SymbolAnalysisContext context, INamedTypeSymbol type, Location locationOfFirstDeclaration, string typeKind)
   {
      if (type.TypeParameters.Length > 0)
         ReportDiagnostic(context, DiagnosticsDescriptors.EnumsAndValueObjectsMustNotBeGeneric, locationOfFirstDeclaration, typeKind, BuildTypeName(type));
   }

   private static void CheckKeyComparer(SymbolAnalysisContext context, IEnumerable<ISymbol> comparerMembers)
   {
      foreach (var comparerMember in comparerMembers)
      {
         switch (comparerMember)
         {
            case IFieldSymbol field:

               if (!field.IsStatic)
                  ReportDiagnostic(context, DiagnosticsDescriptors.KeyComparerMustBeStaticFieldOrProperty, field.GetIdentifier(context.CancellationToken).GetLocation(), field.Name);

               break;

            case IPropertySymbol property:

               if (!property.IsStatic)
                  ReportDiagnostic(context, DiagnosticsDescriptors.KeyComparerMustBeStaticFieldOrProperty, property.GetIdentifier(context.CancellationToken).GetLocation(), property.Name);

               break;

            default:
               var location = comparerMember.DeclaringSyntaxReferences.Single().GetSyntax(context.CancellationToken).GetLocation();
               ReportDiagnostic(context, DiagnosticsDescriptors.KeyComparerMustBeStaticFieldOrProperty, location, comparerMember.Name);
               break;
         }
      }
   }

   private static void Check_ItemLike_StaticProperties(SymbolAnalysisContext context, INamedTypeSymbol enumType)
   {
      foreach (var member in enumType.GetNonIgnoredMembers())
      {
         if (member.IsStatic && member is IPropertySymbol property && SymbolEqualityComparer.Default.Equals(property.Type, enumType))
         {
            ReportDiagnostic(context, DiagnosticsDescriptors.StaticPropertiesAreNotConsideredItems, property.GetIdentifier(context.CancellationToken).GetLocation(), property.Name);
         }
      }
   }

   private static void ValidateDerivedTypes(SymbolAnalysisContext context, INamedTypeSymbol enumType)
   {
      var derivedTypes = enumType.FindDerivedInnerEnums();

      foreach (var derivedType in derivedTypes)
      {
         if (derivedType.Type.IsEnum())
         {
            ReportDiagnostic(context, DiagnosticsDescriptors.DerivedTypeMustNotImplementEnumInterfaces,
                             ((TypeDeclarationSyntax)derivedType.Type.DeclaringSyntaxReferences.First().GetSyntax(context.CancellationToken)).Identifier.GetLocation(), derivedType.Type);
         }

         if (derivedType.Level == 1)
         {
            if (derivedType.Type.DeclaredAccessibility != Accessibility.Private)
            {
               ReportDiagnostic(context, DiagnosticsDescriptors.InnerEnumOnFirstLevelMustBePrivate,
                                ((TypeDeclarationSyntax)derivedType.Type.DeclaringSyntaxReferences.First().GetSyntax(context.CancellationToken)).Identifier.GetLocation(), derivedType.Type);
            }
         }
         else if (derivedType.Type.DeclaredAccessibility != Accessibility.Public)
         {
            ReportDiagnostic(context, DiagnosticsDescriptors.InnerEnumOnNonFirstLevelMustBePublic,
                             ((TypeDeclarationSyntax)derivedType.Type.DeclaringSyntaxReferences.First().GetSyntax(context.CancellationToken)).Identifier.GetLocation(), derivedType.Type);
         }
      }
   }

   private static void ValidateCreateInvalidItem(SymbolAnalysisContext context, INamedTypeSymbol enumType, ITypeSymbol keyType, Location location)
   {
      var hasCreateInvalidImplementation = enumType.HasCreateInvalidImplementation(keyType, context.CancellationToken, context.ReportDiagnostic);

      if (!hasCreateInvalidImplementation && enumType.IsAbstract)
      {
         ReportDiagnostic(context, DiagnosticsDescriptors.AbstractEnumNeedsCreateInvalidItemImplementation,
                          location,
                          enumType,
                          keyType);
      }
   }

   private static void EnumKeyPropertyNameMustNotBeItem(SymbolAnalysisContext context, AttributeData enumSettingsAttr, Location location)
   {
      var keyPropName = enumSettingsAttr.FindKeyPropertyName();

      if (!StringComparer.OrdinalIgnoreCase.Equals(keyPropName, "Item"))
         return;

      var attributeSyntax = (AttributeSyntax?)enumSettingsAttr.ApplicationSyntaxReference?.GetSyntax(context.CancellationToken);

      ReportDiagnostic(context, DiagnosticsDescriptors.EnumKeyPropertyNameNotAllowed,
                       attributeSyntax?.ArgumentList?.GetLocation() ?? location, keyPropName);
   }

   private static void EnumItemsMustBePublic(SymbolAnalysisContext context, INamedTypeSymbol type, IReadOnlyList<IFieldSymbol> items)
   {
      foreach (var item in items)
      {
         if (item.DeclaredAccessibility != Accessibility.Public)
         {
            ReportDiagnostic(context, DiagnosticsDescriptors.EnumItemMustBePublic,
                             item.GetIdentifier(context.CancellationToken).GetLocation(),
                             item.Name, BuildTypeName(type));
         }
      }
   }

   private static void TypeMustBePartial(SymbolAnalysisContext context, INamedTypeSymbol type, IReadOnlyList<TypeDeclarationSyntax> declarations)
   {
      foreach (var tds in declarations)
      {
         if (!tds.IsPartial())
            ReportDiagnostic(context, DiagnosticsDescriptors.TypeMustBePartial, tds.Identifier.GetLocation(), type);
      }
   }

   private static void StructMustBeReadOnly(SymbolAnalysisContext context, INamedTypeSymbol type, Location location)
   {
      if (type.IsValueType && !type.IsReadOnly)
         ReportDiagnostic(context, DiagnosticsDescriptors.StructMustBeReadOnly, location, type);
   }

   private static void ConstructorsMustBePrivate(SymbolAnalysisContext context, INamedTypeSymbol type)
   {
      foreach (var ctor in type.Constructors)
      {
         if (ctor.IsImplicitlyDeclared || ctor.DeclaredAccessibility == Accessibility.Private)
            continue;

         var location = ((ConstructorDeclarationSyntax)ctor.DeclaringSyntaxReferences.Single().GetSyntax(context.CancellationToken)).Identifier.GetLocation();
         ReportDiagnostic(context, DiagnosticsDescriptors.EnumConstructorsMustBePrivate, location, type);
      }
   }

   private static void ReportDiagnostic(SymbolAnalysisContext context, DiagnosticDescriptor descriptor, Location location, ITypeSymbol arg0)
   {
      ReportDiagnostic(context, descriptor, location, BuildTypeName(arg0));
   }

   private static void ReportDiagnostic(SymbolAnalysisContext context, DiagnosticDescriptor descriptor, Location location, ITypeSymbol arg0, ITypeSymbol arg1)
   {
      ReportDiagnostic(context, descriptor, location, BuildTypeName(arg0), BuildTypeName(arg1));
   }

   private static void ReportDiagnostic(SymbolAnalysisContext context, DiagnosticDescriptor descriptor, Location location)
   {
      context.ReportDiagnostic(Diagnostic.Create(descriptor, location));
   }

   private static void ReportDiagnostic(SymbolAnalysisContext context, DiagnosticDescriptor descriptor, Location location, string arg0)
   {
      context.ReportDiagnostic(Diagnostic.Create(descriptor, location, arg0));
   }

   private static void ReportDiagnostic(SymbolAnalysisContext context, DiagnosticDescriptor descriptor, Location location, string arg0, string arg1)
   {
      context.ReportDiagnostic(Diagnostic.Create(descriptor, location, arg0, arg1));
   }

   private static string BuildTypeName(ITypeSymbol type)
   {
      return type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
   }
}
