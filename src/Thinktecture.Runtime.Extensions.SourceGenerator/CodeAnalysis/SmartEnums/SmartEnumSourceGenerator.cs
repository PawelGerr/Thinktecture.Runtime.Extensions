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
      var enumTypeOrException = context.SyntaxProvider
                                       .CreateSyntaxProvider(IsCandidate, GetEnumType)
                                       .SelectMany(static (state, _) => state.HasValue
                                                                           ? ImmutableArray.Create(state.Value)
                                                                           : ImmutableArray<SourceGenState<EnumSourceGeneratorState>>.Empty);

      var enumTypes = enumTypeOrException
                      .SelectMany(static (state, _) => state.State is not null && IsKeyNotNotNullable(state.State)
                                                          ? ImmutableArray.Create(state.State)
                                                          : ImmutableArray<EnumSourceGeneratorState>.Empty)
                      .Collect()
                      .Select(static (states, _) => states.IsDefaultOrEmpty
                                                       ? ImmutableArray<EnumSourceGeneratorState>.Empty
                                                       : states.Distinct().ToImmutableArray())
                      .WithComparer(new SetComparer<EnumSourceGeneratorState>())
                      .SelectMany((states, _) => states);

      var exceptions = enumTypeOrException.SelectMany(static (state, _) => state.Exception is not null
                                                                              ? ImmutableArray.Create(state.Exception)
                                                                              : ImmutableArray<Exception>.Empty);

      var additionalGenerators = context.MetadataReferencesProvider
                                        .SelectMany(static (reference, _) => GetCodeGeneratorFactories(reference))
                                        .Collect()
                                        .Select(static (states, _) => states.IsDefaultOrEmpty
                                                                         ? ImmutableArray<ICodeGeneratorFactory<EnumSourceGeneratorState>>.Empty
                                                                         : states.Distinct().ToImmutableArray())
                                        .WithComparer(new SetComparer<ICodeGeneratorFactory<EnumSourceGeneratorState>>());

      context.RegisterSourceOutput(enumTypes, (ctx, state) => GenerateCode(ctx, state, SmartEnumCodeGeneratorFactory.Instance));
      context.RegisterImplementationSourceOutput(enumTypes.Combine(additionalGenerators), GenerateCode);
      context.RegisterSourceOutput(exceptions, ReportException);
   }

   private static bool IsKeyNotNotNullable(EnumSourceGeneratorState state)
   {
      return !state.KeyProperty.IsNullableStruct
             && state.KeyProperty.NullableAnnotation != NullableAnnotation.Annotated;
   }

   private static ImmutableArray<ICodeGeneratorFactory<EnumSourceGeneratorState>> GetCodeGeneratorFactories(MetadataReference reference)
   {
      var factories = ImmutableArray<ICodeGeneratorFactory<EnumSourceGeneratorState>>.Empty;

      try
      {
         foreach (var module in reference.GetModules())
         {
            factories = module.Name switch
            {
               THINKTECTURE_RUNTIME_EXTENSIONS_JSON => factories.Add(JsonSmartEnumCodeGeneratorFactory.Instance),
               THINKTECTURE_RUNTIME_EXTENSIONS_NEWTONSOFT_JSON => factories.Add(NewtonsoftJsonSmartEnumCodeGeneratorFactory.Instance),
               THINKTECTURE_RUNTIME_EXTENSIONS_MESSAGEPACK => factories.Add(MessagePackSmartEnumCodeGeneratorFactory.Instance),
               _ => factories
            };
         }
      }
      catch (Exception ex)
      {
         Debug.Write(ex);
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
      catch (OperationCanceledException)when (cancellationToken.IsCancellationRequested)
      {
         throw;
      }
      catch (Exception ex)
      {
         return new SourceGenState<EnumSourceGeneratorState>(null, ex);
      }
   }
}
