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
      private const string _MAKE_STRUCT_READONLY = "Make the enumeration read-only";
      private const string _MAKE_FIELD_READONLY = "Make the enumeration item read-only";

      /// <inheritdoc />
      public override ImmutableArray<string> FixableDiagnosticIds { get; } = ImmutableArray.Create(DiagnosticsDescriptors.TypeMustBePartial.Id,
                                                                                                   DiagnosticsDescriptors.StructMustBeReadOnly.Id,
                                                                                                   DiagnosticsDescriptors.FieldMustBeReadOnly.Id);

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
            var enumDeclaration = GetDeclaration<TypeDeclarationSyntax>(context, root);

            if (enumDeclaration is not null)
            {
               var makePartialDiagnostic = FindDiagnostics(context, DiagnosticsDescriptors.TypeMustBePartial);

               if (makePartialDiagnostic is not null)
                  context.RegisterCodeFix(CodeAction.Create(_MAKE_PARTIAL, _ => AddTypeModifierAsync(context.Document, root, enumDeclaration, SyntaxKind.PartialKeyword), _MAKE_PARTIAL), makePartialDiagnostic);

               var makeStructReadOnlyDiagnostic = FindDiagnostics(context, DiagnosticsDescriptors.StructMustBeReadOnly);

               if (makeStructReadOnlyDiagnostic is not null)
                  context.RegisterCodeFix(CodeAction.Create(_MAKE_STRUCT_READONLY, _ => AddTypeModifierAsync(context.Document, root, enumDeclaration, SyntaxKind.ReadOnlyKeyword), _MAKE_STRUCT_READONLY), makeStructReadOnlyDiagnostic);

               var makeFieldReadOnlyDiagnostic = FindDiagnostics(context, DiagnosticsDescriptors.FieldMustBeReadOnly);

               if (makeFieldReadOnlyDiagnostic is not null)
               {
                  var fieldDeclaration = GetDeclaration<FieldDeclarationSyntax>(context, root);

                  if (fieldDeclaration is not null)
                     context.RegisterCodeFix(CodeAction.Create(_MAKE_FIELD_READONLY, _ => AddTypeModifierAsync(context.Document, root, fieldDeclaration, SyntaxKind.ReadOnlyKeyword), _MAKE_FIELD_READONLY), makeFieldReadOnlyDiagnostic);
               }
            }
         }
      }

      private static Diagnostic? FindDiagnostics(CodeFixContext context, DiagnosticDescriptor diagnostic)
      {
         return context.Diagnostics.FirstOrDefault(d => d.Id == diagnostic.Id);
      }

      private static T? GetDeclaration<T>(CodeFixContext context, SyntaxNode root)
         where T : MemberDeclarationSyntax
      {
         var diagnosticSpan = context.Diagnostics.First().Location.SourceSpan;
         return root.FindToken(diagnosticSpan.Start).Parent?.AncestorsAndSelf().OfType<T>().First();
      }

      private static Task<Document> AddTypeModifierAsync(
         Document document,
         SyntaxNode root,
         MemberDeclarationSyntax declaration,
         SyntaxKind modifier)
      {
         var newModifier = SyntaxFactory.Token(modifier);
         var indexOfPartialKeyword = declaration.Modifiers.IndexOf(SyntaxKind.PartialKeyword);
         var newDeclaration = indexOfPartialKeyword < 0 ? declaration.AddModifiers(newModifier) : declaration.WithModifiers(declaration.Modifiers.Insert(indexOfPartialKeyword, newModifier));
         var newRoot = root.ReplaceNode(declaration, newDeclaration);
         var newDoc = document.WithSyntaxRoot(newRoot);

         return Task.FromResult(newDoc);
      }
   }
}
