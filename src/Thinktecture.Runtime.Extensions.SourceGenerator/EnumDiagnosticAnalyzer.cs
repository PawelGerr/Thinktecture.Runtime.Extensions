using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
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
                                                                                                                 DiagnosticsDescriptors.NonValidatableEnumsMustBeClass,
                                                                                                                 DiagnosticsDescriptors.ConstructorsMustBePrivate,
                                                                                                                 DiagnosticsDescriptors.NoItemsWarning,
                                                                                                                 DiagnosticsDescriptors.FieldMustBePublic,
                                                                                                                 DiagnosticsDescriptors.FieldMustBeReadOnly,
                                                                                                                 DiagnosticsDescriptors.AbstractEnumNeedsCreateInvalidItemImplementation,
                                                                                                                 DiagnosticsDescriptors.InvalidImplementationOfCreateInvalidItem);

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

         if (!tds.IsPartial())
            context.ReportDiagnostic(Diagnostic.Create(DiagnosticsDescriptors.TypeMustBePartial, tds.Identifier.GetLocation(), tds.Identifier));

         var validEnumInterface = enumInterfaces.GetValidEnumInterface(enumType);

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
                  context.ReportDiagnostic(Diagnostic.Create(DiagnosticsDescriptors.ConstructorsMustBePrivate, ctor.DeclaringSyntaxReferences.First().GetSyntax().GetLocation(), tds.Identifier));
            }
         }

         var items = enumType.GetValidItems(context.ReportDiagnostic);

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
                                                          keyType.Name));
            }
         }
      }
   }
}
