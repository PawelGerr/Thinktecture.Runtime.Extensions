using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Thinktecture
{
   [DiagnosticAnalyzer(LanguageNames.CSharp)]
   public class EnumDiagnosticAnalyzer : DiagnosticAnalyzer
   {
      /// <inheritdoc />
      public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(DiagnosticsDescriptors.TypeMustBePartial);

      /// <inheritdoc />
      public override void Initialize(AnalysisContext context)
      {
         context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
         context.EnableConcurrentExecution();

         context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.NamedType);
      }

      private static void AnalyzeSymbol(SymbolAnalysisContext context)
      {
         var namedTypeSymbol = (INamedTypeSymbol)context.Symbol;

         foreach (var syntaxReference in namedTypeSymbol.DeclaringSyntaxReferences)
         {
            var node = syntaxReference.GetSyntax();

            if (node is TypeDeclarationSyntax tds)
            {
               if (tds.IsEnum() && !tds.IsPartial())
                  context.ReportDiagnostic(Diagnostic.Create(DiagnosticsDescriptors.TypeMustBePartial, tds.Identifier.GetLocation(), tds.Identifier));
            }
         }
      }
   }
}
