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
      factory = null;

      if (reference is not PortableExecutableReference portable)
         return false;

      if (portable.GetMetadata() is not AssemblyMetadata assemblyMetadata)
         return false;

      var modules = assemblyMetadata.GetModules();

      foreach (var module in modules)
      {
         switch (module.Name)
         {
            case "Thinktecture.Runtime.Extensions.dll":
               factory = new ValueObjectCodeGeneratorFactory();
               return true;

            case "Thinktecture.Runtime.Extensions.Json.dll":
               factory = new JsonValueObjectCodeGeneratorFactory();
               return true;

            case "Thinktecture.Runtime.Extensions.Newtonsoft.Json.dll":
               factory = new NewtonsoftJsonValueObjectCodeGeneratorFactory();
               return true;

            case "Thinktecture.Runtime.Extensions.MessagePack.dll":
               factory = new MessagePackValueObjectCodeGeneratorFactory();
               return true;
         }
      }

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
