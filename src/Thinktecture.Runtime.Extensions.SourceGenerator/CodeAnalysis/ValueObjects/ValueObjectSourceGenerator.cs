using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
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
                              .SelectMany(static (reference, _) => TryGetCodeGeneratorFactory(reference, out var factory)
                                                                      ? ImmutableArray.Create(factory)
                                                                      : ImmutableArray<ICodeGeneratorFactory<ValueObjectSourceGeneratorState>>.Empty)
                              .Collect()
                              .WithComparer(new SetComparer<ICodeGeneratorFactory<ValueObjectSourceGeneratorState>>());

      context.RegisterSourceOutput(candidates.Combine(generators), GenerateCode);
   }

   private static bool TryGetCodeGeneratorFactory(
      MetadataReference reference,
      [MaybeNullWhen(false)] out ICodeGeneratorFactory<ValueObjectSourceGeneratorState> factory)
   {
      foreach (var module in reference.GetModules())
      {
         switch (module.Name)
         {
            case THINKTECTURE_RUNTIME_EXTENSIONS:
               factory = new ValueObjectCodeGeneratorFactory();
               return true;

            case THINKTECTURE_RUNTIME_EXTENSIONS_JSON:
               factory = new JsonValueObjectCodeGeneratorFactory();
               return true;

            case THINKTECTURE_RUNTIME_EXTENSIONS_NEWTONSOFT_JSON:
               factory = new NewtonsoftJsonValueObjectCodeGeneratorFactory();
               return true;

            case THINKTECTURE_RUNTIME_EXTENSIONS_MESSAGEPACK:
               factory = new MessagePackValueObjectCodeGeneratorFactory();
               return true;
         }
      }

      factory = null;
      return false;
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

         return new SourceGenState<ValueObjectSourceGeneratorState>(new ValueObjectSourceGeneratorState(type, valueObjectAttribute), null);
      }
      catch (Exception ex)
      {
         return new SourceGenState<ValueObjectSourceGeneratorState>(null, ex);
      }
   }
}
