using System.Collections.Immutable;
using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Thinktecture.CodeAnalysis;

/// <summary>
/// Base class for source generator for Value Objects.
/// </summary>
public abstract class ValueObjectSourceGeneratorBase : ThinktectureSourceGeneratorBase, IIncrementalGenerator
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
                              .Collect();

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

   private static SourceGenState<ValueObjectSourceGeneratorState>? GetValueObjectStateOrNull(GeneratorSyntaxContext context, CancellationToken cancellationToken)
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

         return new SourceGenState<ValueObjectSourceGeneratorState>(new ValueObjectSourceGeneratorState(type, valueObjectAttribute), null);
      }
      catch (Exception ex)
      {
         return new SourceGenState<ValueObjectSourceGeneratorState>(null, ex);
      }
   }

   private void GenerateCode(SourceProductionContext context, ImmutableArray<SourceGenState<ValueObjectSourceGeneratorState>> states)
   {
      if (states.IsDefaultOrEmpty)
         return;

      IReadOnlyList<ValueObjectSourceGeneratorState> valueObjectStates;

      try
      {
         valueObjectStates = states.GetDistinctInnerStates(context);
      }
      catch (Exception ex)
      {
         context.ReportException(ex);
         return;
      }

      var stringBuilderProvider = new StringBuilderProvider();

      foreach (var valueObjectState in valueObjectStates)
      {
         if (context.CancellationToken.IsCancellationRequested)
            return;

         var type = valueObjectState.Type;

         try
         {
            var generatedCode = GenerateValueObject(valueObjectState, stringBuilderProvider);

            EmitFile(context, valueObjectState.Namespace, type.Name, generatedCode);
         }
         catch (Exception ex)
         {
            context.ReportDiagnostic(Diagnostic.Create(DiagnosticsDescriptors.ErrorDuringGeneration,
                                                       type.GetLocationOrNullSafe(context),
                                                       type.Name, ex.Message));
         }
      }
   }

   protected virtual string? GenerateValueObject(ValueObjectSourceGeneratorState state, StringBuilderProvider stringBuilderProvider)
   {
      return null;
   }
}
