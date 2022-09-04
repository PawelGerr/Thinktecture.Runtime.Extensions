using System.Collections.Immutable;
using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Thinktecture.CodeAnalysis.ValueObjects;

[Generator]
public sealed class ValueObjectSourceGenerator : ThinktectureSourceGeneratorBase<ValueObjectSourceGeneratorState>, IIncrementalGenerator
{
   public ValueObjectSourceGenerator()
      : base(10_000)
   {
   }

   public void Initialize(IncrementalGeneratorInitializationContext context)
   {
      var candidates = context.SyntaxProvider.CreateSyntaxProvider(IsCandidate, GetValueObjectStateOrNull)
                              .Where(static state => state.HasValue)
                              .Select(static (state, _) => state!.Value)
                              .Collect()
                              .SelectMany(static (states, _) => states.Distinct());

      var generators = context.GetMetadataReferencesProvider()
                              .SelectMany(static (reference, _) => GetCodeGeneratorFactories(reference))
                              .Collect()
                              .WithComparer(new SetComparer<ICodeGeneratorFactory<ValueObjectSourceGeneratorState>>());

      context.RegisterSourceOutput(candidates.Combine(generators), GenerateCode);
   }

   private static ImmutableArray<ICodeGeneratorFactory<ValueObjectSourceGeneratorState>> GetCodeGeneratorFactories(MetadataReference reference)
   {
      var factories = ImmutableArray<ICodeGeneratorFactory<ValueObjectSourceGeneratorState>>.Empty;

      foreach (var module in reference.GetModules())
      {
         switch (module.Name)
         {
            case THINKTECTURE_RUNTIME_EXTENSIONS:
               factories = factories.Add(new ValueObjectCodeGeneratorFactory());
               break;

            case THINKTECTURE_RUNTIME_EXTENSIONS_JSON:
               factories = factories.Add(new JsonValueObjectCodeGeneratorFactory());
               break;

            case THINKTECTURE_RUNTIME_EXTENSIONS_NEWTONSOFT_JSON:
               factories = factories.Add(new NewtonsoftJsonValueObjectCodeGeneratorFactory());
               break;

            case THINKTECTURE_RUNTIME_EXTENSIONS_MESSAGEPACK:
               factories = factories.Add(new MessagePackValueObjectCodeGeneratorFactory());
               break;
         }
      }

      return factories;
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

         if (!type.HasValueObjectAttribute(out var valueObjectAttribute))
            return null;

         if (type.ContainingType is not null)
            return null;

         return new SourceGenState<ValueObjectSourceGeneratorState>(new ValueObjectSourceGeneratorState(type, valueObjectAttribute, cancellationToken), null);
      }
      catch (Exception ex)
      {
         return new SourceGenState<ValueObjectSourceGeneratorState>(null, ex);
      }
   }
}
