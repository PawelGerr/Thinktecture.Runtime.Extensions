using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Thinktecture.CodeAnalysis.SmartEnums;

[Generator]
public sealed class SmartEnumSourceGenerator : ThinktectureSourceGeneratorBase<EnumSourceGeneratorState>, IIncrementalGenerator
{
   public SmartEnumSourceGenerator()
      : base(12_000)
   {
   }

   public void Initialize(IncrementalGeneratorInitializationContext context)
   {
      var candidates = context.SyntaxProvider.CreateSyntaxProvider(IsCandidate, GetEnumStateOrNull)
                              .Where(static state => state.HasValue)
                              .Select(static (state, _) => state!.Value)
                              .Collect()
                              .SelectMany(static (states, _) => states.Distinct());

      var generators = context.GetMetadataReferencesProvider()
                              .SelectMany(static (reference, _) => TryGetCodeGeneratorFactory(reference, out var factory)
                                                                      ? ImmutableArray.Create(factory)
                                                                      : ImmutableArray<ICodeGeneratorFactory<EnumSourceGeneratorState>>.Empty)
                              .Collect()
                              .WithComparer(new SetComparer<ICodeGeneratorFactory<EnumSourceGeneratorState>>());

      context.RegisterSourceOutput(candidates.Combine(generators), GenerateCode);
   }

   private static bool TryGetCodeGeneratorFactory(
      MetadataReference reference,
      [MaybeNullWhen(false)] out ICodeGeneratorFactory<EnumSourceGeneratorState> factory)
   {
      foreach (var module in reference.GetModules())
      {
         switch (module.Name)
         {
            case THINKTECTURE_RUNTIME_EXTENSIONS:
               factory = new SmartEnumCodeGeneratorFactory();
               return true;

            case THINKTECTURE_RUNTIME_EXTENSIONS_JSON:
               factory = new JsonSmartEnumCodeGeneratorFactory();
               return true;

            case THINKTECTURE_RUNTIME_EXTENSIONS_NEWTONSOFT_JSON:
               factory = new NewtonsoftJsonSmartEnumCodeGeneratorFactory();
               return true;

            case THINKTECTURE_RUNTIME_EXTENSIONS_MESSAGEPACK:
               factory = new MessagePackSmartEnumCodeGeneratorFactory();
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

   private static SourceGenState<EnumSourceGeneratorState>? GetEnumStateOrNull(GeneratorSyntaxContext context, CancellationToken cancellationToken)
   {
      try
      {
         var tds = (TypeDeclarationSyntax)context.Node;
         var type = context.SemanticModel.GetDeclaredSymbol(tds);

         if (!type.IsEnum(out var enumInterfaces))
            return null;

         if (type.ContainingType is not null)
            return null;

         var enumInterface = enumInterfaces.GetValidEnumInterface(type);

         if (enumInterface is null)
            return null;

         return new SourceGenState<EnumSourceGeneratorState>(new EnumSourceGeneratorState(type, enumInterface), null);
      }
      catch (Exception ex)
      {
         return new SourceGenState<EnumSourceGeneratorState>(null, ex);
      }
   }
}
