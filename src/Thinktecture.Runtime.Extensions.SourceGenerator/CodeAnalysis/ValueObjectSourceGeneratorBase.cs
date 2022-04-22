using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Thinktecture.CodeAnalysis;

/// <summary>
/// Base class for source generator for Value Objects.
/// </summary>
public abstract class ValueObjectSourceGeneratorBase<T> : ThinktectureSourceGeneratorBase, IIncrementalGenerator
   where T : ValueObjectSourceGeneratorState, IEquatable<T>
{
   protected ValueObjectSourceGeneratorBase(string? generatedFileSuffix)
      : base(generatedFileSuffix)
   {
   }

   /// <inheritdoc />
   public void Initialize(IncrementalGeneratorInitializationContext context)
   {
      var candidates = context.SyntaxProvider.CreateSyntaxProvider(IsCandidate, GetValueObjectStateOrNull)
                              .Where(static state => state.HasValue)
                              .Select((state, _) => state!.Value)
                              .Collect()
                              .SelectMany((states, _) => states.Distinct());

      context.RegisterSourceOutput(candidates, GenerateCode);
   }

   private static bool IsCandidate(SyntaxNode syntaxNode, CancellationToken cancellationToken)
   {
      try
      {
         return syntaxNode switch
         {
            ClassDeclarationSyntax classDeclaration when IsValueObjectCandidate(classDeclaration) => true,
            StructDeclarationSyntax structDeclaration when IsValueObjectCandidate(structDeclaration) => true,
            _ => false
         };
      }
      catch (Exception ex)
      {
         Debug.Write(ex);
         return false;
      }
   }

   private static bool IsValueObjectCandidate(TypeDeclarationSyntax typeDeclaration)
   {
      return typeDeclaration.IsPartial()
             && !typeDeclaration.IsGeneric()
             && typeDeclaration.IsValueObjectCandidate();
   }

   private SourceGenState<T>? GetValueObjectStateOrNull(GeneratorSyntaxContext context, CancellationToken cancellationToken)
   {
      try
      {
         var tds = (TypeDeclarationSyntax)context.Node;
         var type = context.SemanticModel.GetDeclaredSymbol(tds);

         if (type is null)
            return null;

         if (!type.HasValueObjectAttribute(out var valueObjectAttribute))
            return null;

         if (type.ContainingType is not null)
            return null;

         return new SourceGenState<T>(CreateState(type, valueObjectAttribute), null);
      }
      catch (Exception ex)
      {
         return new SourceGenState<T>(null, ex);
      }
   }

   protected abstract T CreateState(INamedTypeSymbol type, AttributeData valueObjectAttribute);

   private void GenerateCode(SourceProductionContext context, SourceGenState<T> state)
   {
      if (state.Exception is not null)
      {
         context.ReportException(state.Exception);
         return;
      }

      var valueObjectState = state.State;

      if (valueObjectState is null)
         return;

      var stringBuilderProvider = new StringBuilderProvider();

      try
      {
         var generatedCode = GenerateValueObject(valueObjectState, stringBuilderProvider);

         EmitFile(context, valueObjectState.Namespace, valueObjectState.Name, generatedCode);
      }
      catch (Exception ex)
      {
         context.ReportDiagnostic(Diagnostic.Create(DiagnosticsDescriptors.ErrorDuringGeneration,
                                                    valueObjectState.GetLocationOrNullSafe(context),
                                                    valueObjectState.Name, ex.Message));
      }
   }

   protected virtual string? GenerateValueObject(T state, StringBuilderProvider stringBuilderProvider)
   {
      return null;
   }
}
