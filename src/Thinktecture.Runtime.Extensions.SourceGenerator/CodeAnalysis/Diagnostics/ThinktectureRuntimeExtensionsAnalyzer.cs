using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Thinktecture.CodeAnalysis.Diagnostics
{
   [DiagnosticAnalyzer(LanguageNames.CSharp)]
   public class ThinktectureRuntimeExtensionsAnalyzer : DiagnosticAnalyzer
   {
      /// <inheritdoc />
      public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(DiagnosticsDescriptors.TypeMustBePartial,
                                                                                                                 DiagnosticsDescriptors.StructMustBeReadOnly,
                                                                                                                 DiagnosticsDescriptors.TypeMustBeClassOrStruct,
                                                                                                                 DiagnosticsDescriptors.NonValidatableEnumsMustBeClass,
                                                                                                                 DiagnosticsDescriptors.ConstructorsMustBePrivate,
                                                                                                                 DiagnosticsDescriptors.NoItemsWarning,
                                                                                                                 DiagnosticsDescriptors.FieldMustBePublic,
                                                                                                                 DiagnosticsDescriptors.FieldMustBeReadOnly,
                                                                                                                 DiagnosticsDescriptors.PropertyMustBeReadOnly,
                                                                                                                 DiagnosticsDescriptors.AbstractEnumNeedsCreateInvalidItemImplementation,
                                                                                                                 DiagnosticsDescriptors.InvalidSignatureOfCreateInvalidItem,
                                                                                                                 DiagnosticsDescriptors.KeyPropertyNameNotAllowed,
                                                                                                                 DiagnosticsDescriptors.MultipleIncompatibleEnumInterfaces,
                                                                                                                 DiagnosticsDescriptors.DerivedTypeMustNotImplementEnumInterfaces,
                                                                                                                 DiagnosticsDescriptors.FirstLevelInnerTypeMustBePrivate,
                                                                                                                 DiagnosticsDescriptors.NonFirstLevelInnerTypeMustBePublic,
                                                                                                                 DiagnosticsDescriptors.TypeCannotBeNestedClass);

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
         var declaration = type.DeclaringSyntaxReferences.First().GetSyntax();

         if (declaration is not TypeDeclarationSyntax tds)
            return;

         if (type.IsEnum(out var enumInterfaces))
            ValidateEnum(context, declaration, tds, type, enumInterfaces);
      }

      private static void ValidateEnum(SymbolAnalysisContext context, SyntaxNode declaration, TypeDeclarationSyntax tds, INamedTypeSymbol enumType, IReadOnlyList<INamedTypeSymbol> enumInterfaces)
      {
         if (!declaration.IsKind(SyntaxKind.ClassDeclaration) && !declaration.IsKind(SyntaxKind.StructDeclaration))
         {
            context.ReportDiagnostic(Diagnostic.Create(DiagnosticsDescriptors.TypeMustBeClassOrStruct, tds.Identifier.GetLocation(), tds.Identifier));
            return;
         }

         if (enumType.ContainingType is not null) // is nested class
         {
            if (!enumType.BaseType.IsSelfOrBaseTypesAnEnum()) // base class is not an enum because in this case "DiagnosticsDescriptors.DerivedTypeMustNotImplementEnumInterfaces" kicks in
            {
               context.ReportDiagnostic(Diagnostic.Create(DiagnosticsDescriptors.TypeCannotBeNestedClass, tds.Identifier.GetLocation(), tds.Identifier));
               return;
            }
         }

         TypeMustBePartial(context, tds);

         var validEnumInterface = enumInterfaces.GetValidEnumInterface(enumType, context.ReportDiagnostic);

         if (validEnumInterface is null)
            return;

         var isValidatable = validEnumInterface.IsValidatableEnumInterface();

         StructMustBeReadOnly(context, tds, enumType);

         if (enumType.IsValueType && !isValidatable)
            context.ReportDiagnostic(Diagnostic.Create(DiagnosticsDescriptors.NonValidatableEnumsMustBeClass, tds.Identifier.GetLocation(), tds.Identifier));

         ConstructorsMustBePrivate(context, tds, enumType);

         var items = enumType.GetEnumItems();

         if (items.Count == 0)
            context.ReportDiagnostic(Diagnostic.Create(DiagnosticsDescriptors.NoItemsWarning, tds.Identifier.GetLocation(), tds.Identifier));

         FieldsMustBePublic(context, enumType, items);

         if (isValidatable)
         {
            var keyType = validEnumInterface.TypeArguments[0];
            var hasCreateInvalidImplementation = enumType.HasCreateInvalidImplementation(keyType, context.ReportDiagnostic);

            if (enumType.IsAbstract && !hasCreateInvalidImplementation)
            {
               context.ReportDiagnostic(Diagnostic.Create(DiagnosticsDescriptors.AbstractEnumNeedsCreateInvalidItemImplementation,
                                                          tds.Identifier.GetLocation(),
                                                          enumType.Name,
                                                          keyType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)));
            }
         }

         enumType.GetAssignableFieldsAndPropertiesAndCheckForReadOnly(false, context.ReportDiagnostic);

         var enumSettingsAttr = enumType.FindEnumGenerationAttribute();

         if (enumSettingsAttr is not null)
            EnumKeyPropertyNameMustNotBeItem(context, tds, enumSettingsAttr);

         var derivedTypes = enumType.FindDerivedInnerTypes();

         foreach (var derivedType in derivedTypes)
         {
            if (derivedType.Type.IsEnum(out _))
            {
               context.ReportDiagnostic(Diagnostic.Create(DiagnosticsDescriptors.DerivedTypeMustNotImplementEnumInterfaces,
                                                          ((TypeDeclarationSyntax)derivedType.Type.DeclaringSyntaxReferences.First().GetSyntax()).Identifier.GetLocation(),
                                                          derivedType.Type.Name));
            }

            if (derivedType.Level == 1)
            {
               if (derivedType.Type.DeclaredAccessibility != Accessibility.Private)
               {
                  context.ReportDiagnostic(Diagnostic.Create(DiagnosticsDescriptors.FirstLevelInnerTypeMustBePrivate,
                                                             ((TypeDeclarationSyntax)derivedType.Type.DeclaringSyntaxReferences.First().GetSyntax()).Identifier.GetLocation(),
                                                             derivedType.Type.Name));
               }
            }
            else if (derivedType.Type.DeclaredAccessibility != Accessibility.Public)
            {
               context.ReportDiagnostic(Diagnostic.Create(DiagnosticsDescriptors.NonFirstLevelInnerTypeMustBePublic,
                                                          ((TypeDeclarationSyntax)derivedType.Type.DeclaringSyntaxReferences.First().GetSyntax()).Identifier.GetLocation(),
                                                          derivedType.Type.Name));
            }
         }
      }

      private static void EnumKeyPropertyNameMustNotBeItem(SymbolAnalysisContext context, TypeDeclarationSyntax tds, AttributeData enumSettingsAttr)
      {
         var keyPropName = enumSettingsAttr.FindKeyPropertyName();

         if (StringComparer.OrdinalIgnoreCase.Equals(keyPropName, "Item"))
         {
            var attributeSyntax = (AttributeSyntax?)enumSettingsAttr.ApplicationSyntaxReference?.GetSyntax();

            context.ReportDiagnostic(Diagnostic.Create(DiagnosticsDescriptors.KeyPropertyNameNotAllowed,
                                                       attributeSyntax?.ArgumentList?.GetLocation() ?? tds.Identifier.GetLocation(),
                                                       keyPropName));
         }
      }

      private static void FieldsMustBePublic(SymbolAnalysisContext context, INamedTypeSymbol type, IReadOnlyList<IFieldSymbol> items)
      {
         foreach (var item in items)
         {
            if (item.DeclaredAccessibility != Accessibility.Public)
            {
               context.ReportDiagnostic(Diagnostic.Create(DiagnosticsDescriptors.FieldMustBePublic,
                                                          item.GetLocation(),
                                                          item.Name, type.Name));
            }
         }
      }

      private static void TypeMustBePartial(SymbolAnalysisContext context, TypeDeclarationSyntax tds)
      {
         if (!tds.IsPartial())
            context.ReportDiagnostic(Diagnostic.Create(DiagnosticsDescriptors.TypeMustBePartial, tds.Identifier.GetLocation(), tds.Identifier));
      }

      private static void StructMustBeReadOnly(SymbolAnalysisContext context, TypeDeclarationSyntax tds, INamedTypeSymbol type)
      {
         if (type.IsValueType && !type.IsReadOnly)
            context.ReportDiagnostic(Diagnostic.Create(DiagnosticsDescriptors.StructMustBeReadOnly, tds.Identifier.GetLocation(), tds.Identifier));
      }

      private static void ConstructorsMustBePrivate(SymbolAnalysisContext context, TypeDeclarationSyntax tds, INamedTypeSymbol type)
      {
         foreach (var ctor in type.Constructors)
         {
            if (!ctor.IsImplicitlyDeclared && ctor.DeclaredAccessibility != Accessibility.Private)
            {
               var location = ((ConstructorDeclarationSyntax)ctor.DeclaringSyntaxReferences.First().GetSyntax()).Identifier.GetLocation();
               context.ReportDiagnostic(Diagnostic.Create(DiagnosticsDescriptors.ConstructorsMustBePrivate, location, tds.Identifier));
            }
         }
      }
   }
}
