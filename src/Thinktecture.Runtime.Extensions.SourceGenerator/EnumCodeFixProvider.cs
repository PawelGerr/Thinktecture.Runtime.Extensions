using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Thinktecture
{
   [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(EnumCodeFixProvider)), Shared]
   public class EnumCodeFixProvider : CodeFixProvider
   {
      private const string _MAKE_PARTIAL = "Make the enumeration partial";

      /// <inheritdoc />
      public override ImmutableArray<string> FixableDiagnosticIds { get; } = ImmutableArray.Create(DiagnosticsDescriptors.TypeMustBePartial.Id);

      /// <inheritdoc />
      public override FixAllProvider GetFixAllProvider()
      {
         return WellKnownFixAllProviders.BatchFixer;
      }

      /// <inheritdoc />
      public override async Task RegisterCodeFixesAsync(CodeFixContext context)
      {
         var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

         if (root is not null)
         {
            var makePartialDiagnostic = context.Diagnostics.FirstOrDefault(d => d.Id == DiagnosticsDescriptors.TypeMustBePartial.Id);

            if (makePartialDiagnostic is not null)
            {
               var diagnosticSpan = makePartialDiagnostic.Location.SourceSpan;
               var declaration = root.FindToken(diagnosticSpan.Start).Parent?.AncestorsAndSelf().OfType<TypeDeclarationSyntax>().First();

               if (declaration is not null)
                  context.RegisterCodeFix(CodeAction.Create(_MAKE_PARTIAL, _ => MakePartialAsync(context.Document, root, declaration), _MAKE_PARTIAL), makePartialDiagnostic);
            }
         }
      }

      private static Task<Document> MakePartialAsync(
         Document document,
         SyntaxNode root,
         TypeDeclarationSyntax declaration)
      {
         var newDeclaration = declaration.AddModifiers(SyntaxFactory.Token(SyntaxKind.PartialKeyword));
         var newRoot = root.ReplaceNode(declaration, newDeclaration);
         var newDoc = document.WithSyntaxRoot(newRoot);

         return Task.FromResult(newDoc);
      }
   }
}
