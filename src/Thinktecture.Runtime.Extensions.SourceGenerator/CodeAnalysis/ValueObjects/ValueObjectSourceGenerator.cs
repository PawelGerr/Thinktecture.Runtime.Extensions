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
      : base(12_000)
   {
   }

   public void Initialize(IncrementalGeneratorInitializationContext context)
   {
      var valueObjectOrException = context.SyntaxProvider.CreateSyntaxProvider(IsCandidate, GetValueObjectStateOrNull)
                                          .SelectMany(static (state, _) => state.HasValue
                                                                              ? ImmutableArray.Create(state.Value)
                                                                              : ImmutableArray<SourceGenState<ValueObjectSourceGeneratorState>>.Empty);

      var valueObjects = valueObjectOrException.SelectMany(static (state, _) => state.State is not null && IsNotKeyedOrKeyNotNullable(state.State)
                                                                                   ? ImmutableArray.Create(state.State)
                                                                                   : ImmutableArray<ValueObjectSourceGeneratorState>.Empty)
                                               .Collect()
                                               .Select(static (states, _) => states.Distinct().ToImmutableArray())
                                               .WithComparer(new SetComparer<ValueObjectSourceGeneratorState>())
                                               .SelectMany((states, _) => states);

      var exceptions = valueObjectOrException.SelectMany(static (state, _) => state.Exception is not null
                                                                                 ? ImmutableArray.Create(state.Exception)
                                                                                 : ImmutableArray<Exception>.Empty);
      var generators = context.GetMetadataReferencesProvider()
                              .SelectMany(static (reference, _) => GetCodeGeneratorFactories(reference))
                              .Collect()
                              .WithComparer(new SetComparer<ICodeGeneratorFactory<ValueObjectSourceGeneratorState>>());

      context.RegisterSourceOutput(valueObjects.Combine(generators), GenerateCode);
      context.RegisterSourceOutput(exceptions, ReportException);
   }

   private static bool IsNotKeyedOrKeyNotNullable(ValueObjectSourceGeneratorState state)
   {
      if (!state.HasKeyMember)
         return true;

      return !state.KeyMember.Member.IsNullableStruct
             && state.KeyMember.Member.NullableAnnotation != NullableAnnotation.Annotated;
   }

   private static ImmutableArray<ICodeGeneratorFactory<ValueObjectSourceGeneratorState>> GetCodeGeneratorFactories(MetadataReference reference)
   {
      var factories = ImmutableArray<ICodeGeneratorFactory<ValueObjectSourceGeneratorState>>.Empty;

      foreach (var module in reference.GetModules())
      {
         factories = module.Name switch
         {
            THINKTECTURE_RUNTIME_EXTENSIONS => factories.Add(ValueObjectCodeGeneratorFactory.Instance),
            THINKTECTURE_RUNTIME_EXTENSIONS_JSON => factories.Add(JsonValueObjectCodeGeneratorFactory.Instance),
            THINKTECTURE_RUNTIME_EXTENSIONS_NEWTONSOFT_JSON => factories.Add(NewtonsoftJsonValueObjectCodeGeneratorFactory.Instance),
            THINKTECTURE_RUNTIME_EXTENSIONS_MESSAGEPACK => factories.Add(MessagePackValueObjectCodeGeneratorFactory.Instance),
            _ => factories
         };
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
      catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
      {
         throw;
      }
      catch (Exception ex)
      {
         return new SourceGenState<ValueObjectSourceGeneratorState>(null, ex);
      }
   }
}
