using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Thinktecture
{
   [DiagnosticAnalyzer(LanguageNames.CSharp)]
   public class EnumDiagnosticAnalyzer : DiagnosticAnalyzer
   {
      /// <inheritdoc />
      public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(DiagnosticsDescriptors.TypeMustBePartial,
                                                                                                                 DiagnosticsDescriptors.StructMustBeReadOnly,
                                                                                                                 DiagnosticsDescriptors.EnumMustBeClassOrStruct,
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
                                                                                                                 DiagnosticsDescriptors.EnumCannotBeNestedClass);

      /// <inheritdoc />
      public override void Initialize(AnalysisContext context)
      {
         context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
         context.EnableConcurrentExecution();

         context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.NamedType);
      }

      private static void AnalyzeSymbol(SymbolAnalysisContext context)
      {
         var enumType = (INamedTypeSymbol)context.Symbol;
         var declaration = enumType.DeclaringSyntaxReferences.First().GetSyntax();

         if (declaration is not TypeDeclarationSyntax tds)
            return;

         if (!enumType.IsEnum(out var enumInterfaces))
            return;

         if (!declaration.IsKind(SyntaxKind.ClassDeclaration) && !declaration.IsKind(SyntaxKind.StructDeclaration))
         {
            context.ReportDiagnostic(Diagnostic.Create(DiagnosticsDescriptors.EnumMustBeClassOrStruct, tds.Identifier.GetLocation(), tds.Identifier));
            return;
         }

         if (enumType.ContainingType is not null) // is nested class
         {
            if (!enumType.BaseType.IsSelfOrBaseTypesAnEnum()) // base class is not an enum because in this case "DiagnosticsDescriptors.DerivedTypeMustNotImplementEnumInterfaces" kicks in
            {
               context.ReportDiagnostic(Diagnostic.Create(DiagnosticsDescriptors.EnumCannotBeNestedClass, tds.Identifier.GetLocation(), tds.Identifier));
               return;
            }
         }

         if (!tds.IsPartial())
            context.ReportDiagnostic(Diagnostic.Create(DiagnosticsDescriptors.TypeMustBePartial, tds.Identifier.GetLocation(), tds.Identifier));

         var validEnumInterface = enumInterfaces.GetValidEnumInterface(enumType, context.ReportDiagnostic);

         if (validEnumInterface is null)
            return;

         var isValidatable = validEnumInterface.IsValidatableEnumInterface();

         if (enumType.IsValueType)
         {
            if (!enumType.IsReadOnly)
               context.ReportDiagnostic(Diagnostic.Create(DiagnosticsDescriptors.StructMustBeReadOnly, tds.Identifier.GetLocation(), tds.Identifier));

            if (!isValidatable)
               context.ReportDiagnostic(Diagnostic.Create(DiagnosticsDescriptors.NonValidatableEnumsMustBeClass, tds.Identifier.GetLocation(), tds.Identifier));
         }

         foreach (var ctor in enumType.Constructors)
         {
            if (!ctor.IsImplicitlyDeclared)
            {
               if (ctor.DeclaredAccessibility != Accessibility.Private)
               {
                  var location = ((ConstructorDeclarationSyntax)ctor.DeclaringSyntaxReferences.First().GetSyntax()).Identifier.GetLocation();
                  context.ReportDiagnostic(Diagnostic.Create(DiagnosticsDescriptors.ConstructorsMustBePrivate, location, tds.Identifier));
               }
            }
         }

         var items = enumType.GetEnumItems(context.ReportDiagnostic);

         if (items.Count == 0)
            context.ReportDiagnostic(Diagnostic.Create(DiagnosticsDescriptors.NoItemsWarning, tds.Identifier.GetLocation(), tds.Identifier));

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

         enumType.GetAssignableInstanceFieldsAndProperties(context.ReportDiagnostic);

         var enumSettingsAttr = enumType.FindEnumGenerationAttribute();

         if (enumSettingsAttr is not null)
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
   }
}
