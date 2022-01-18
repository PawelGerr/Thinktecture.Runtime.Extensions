using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Thinktecture.CodeAnalysis;

/// <summary>
/// Base class for source generator for value objects.
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
                              .Where(static state => state is not null)!
                              .Collect<ValueObjectSourceGeneratorState>();

      context.RegisterSourceOutput(candidates, GenerateCode);
   }

   private static bool IsCandidate(SyntaxNode syntaxNode, CancellationToken cancellationToken)
   {
      return syntaxNode switch
      {
         ClassDeclarationSyntax classDeclaration when classDeclaration.IsPartial() && classDeclaration.IsValueObjectCandidate() => true,
         StructDeclarationSyntax structDeclaration when structDeclaration.IsPartial() && structDeclaration.IsValueObjectCandidate() => true,
         _ => false
      };
   }

   private static ValueObjectSourceGeneratorState? GetValueObjectStateOrNull(GeneratorSyntaxContext context, CancellationToken cancellationToken)
   {
      var tds = (TypeDeclarationSyntax)context.Node;
      var type = context.SemanticModel.GetDeclaredSymbol(tds);

      if (type is null)
         return null;

      if (!type.HasValueObjectAttribute(out var valueObjectAttribute))
         return null;

      if (type.ContainingType is not null)
         return null;

      return new ValueObjectSourceGeneratorState(type, valueObjectAttribute);
   }

   private void GenerateCode(SourceProductionContext context, ImmutableArray<ValueObjectSourceGeneratorState> valueObjectStates)
   {
      if (valueObjectStates.IsDefaultOrEmpty)
         return;

      foreach (var valueObjectState in valueObjectStates.Distinct())
      {
         var type = valueObjectState.Type;

         try
         {
            var generatedCode = GenerateValueObject(valueObjectState);

            EmitFile(context, valueObjectState.Namespace, type.Name, generatedCode);
         }
         catch (Exception ex)
         {
            context.ReportDiagnostic(Diagnostic.Create(DiagnosticsDescriptors.ErrorDuringGeneration,
                                                       type.DeclaringSyntaxReferences.First().GetSyntax().GetLocation(), // pick one location as the representative,
                                                       type.Name, ex.Message));
         }
      }
   }

   protected virtual string? GenerateValueObject(ValueObjectSourceGeneratorState state)
   {
      return null;
   }
}
