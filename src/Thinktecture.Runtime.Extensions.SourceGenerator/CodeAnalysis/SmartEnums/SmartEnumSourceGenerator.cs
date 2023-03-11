using System.Collections.Immutable;
using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Thinktecture.CodeAnalysis.SmartEnums;

[Generator]
public sealed class SmartEnumSourceGenerator : ThinktectureSourceGeneratorBase<EnumSourceGeneratorState>, IIncrementalGenerator
{
   public SmartEnumSourceGenerator()
      : base(24_000)
   {
   }

   public void Initialize(IncrementalGeneratorInitializationContext context)
   {
      var enumTypes = context.SyntaxProvider
                             .CreateSyntaxProvider(IsCandidate, GetEnumType)
                             .Where(static state => state.HasValue && IsKeyNotNotNullable(state.Value))
                             .Select(static (state, _) => state!.Value)
                             .Collect()
                             .SelectMany(static (states, _) => states.Distinct());

      var generators = context.GetMetadataReferencesProvider()
                              .SelectMany(static (reference, _) => GetCodeGeneratorFactories(reference))
                              .Collect()
                              .WithComparer(new SetComparer<ICodeGeneratorFactory<EnumSourceGeneratorState>>());

      context.RegisterSourceOutput(enumTypes.Combine(generators), GenerateCode);
   }

   private static bool IsKeyNotNotNullable(SourceGenState<EnumSourceGeneratorState> state)
   {
      if (state.State is null)
         return true;

      return !state.State.KeyProperty.IsNullableStruct
             && state.State.KeyProperty.NullableAnnotation != NullableAnnotation.Annotated;
   }

   private static ImmutableArray<ICodeGeneratorFactory<EnumSourceGeneratorState>> GetCodeGeneratorFactories(MetadataReference reference)
   {
      var factories = ImmutableArray<ICodeGeneratorFactory<EnumSourceGeneratorState>>.Empty;

      foreach (var module in reference.GetModules())
      {
         factories = module.Name switch
         {
            THINKTECTURE_RUNTIME_EXTENSIONS => factories.Add(new SmartEnumCodeGeneratorFactory()),
            THINKTECTURE_RUNTIME_EXTENSIONS_JSON => factories.Add(new JsonSmartEnumCodeGeneratorFactory()),
            THINKTECTURE_RUNTIME_EXTENSIONS_NEWTONSOFT_JSON => factories.Add(new NewtonsoftJsonSmartEnumCodeGeneratorFactory()),
            THINKTECTURE_RUNTIME_EXTENSIONS_MESSAGEPACK => factories.Add(new MessagePackSmartEnumCodeGeneratorFactory()),
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
            ClassDeclarationSyntax classDeclaration when IsEnumCandidate(classDeclaration) => true,
            StructDeclarationSyntax structDeclaration when IsEnumCandidate(structDeclaration) => true,
            _ => false
         };
      }
      catch (Exception ex)
      {
         Debug.Write(ex);
         return false;
      }
   }

   private static bool IsEnumCandidate(TypeDeclarationSyntax typeDeclaration)
   {
      return typeDeclaration.IsPartial()
             && !typeDeclaration.IsGeneric()
             && typeDeclaration.IsEnumCandidate();
   }

   private static SourceGenState<EnumSourceGeneratorState>? GetEnumType(GeneratorSyntaxContext context, CancellationToken cancellationToken)
   {
      try
      {
         var tds = (TypeDeclarationSyntax)context.Node;
         var type = context.SemanticModel.GetDeclaredSymbol(tds, cancellationToken);

         if (type?.ContainingType is not null)
            return null;

         if (!type.IsEnum(out var enumInterfaces))
            return null;

         var enumInterface = enumInterfaces.GetValidEnumInterface(type);

         if (enumInterface is null)
            return null;

         return new SourceGenState<EnumSourceGeneratorState>(new EnumSourceGeneratorState(type, enumInterface, cancellationToken), null);
      }
      catch (OperationCanceledException)
      {
         throw;
      }
      catch (Exception ex)
      {
         return new SourceGenState<EnumSourceGeneratorState>(null, ex);
      }
   }
}
