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
      : base(12_000)
   {
   }

   public void Initialize(IncrementalGeneratorInitializationContext context)
   {
      IncrementalValuesProvider<INamedTypeSymbol> candidates = context.SyntaxProvider.CreateSyntaxProvider(IsCandidate, GetEnumType)
                                                                      .Where(type => type is not null)!;

      var genericTypes = context.SyntaxProvider.CreateSyntaxProvider(IsInstanceCreation, GetDerivedGenericEnums)
                                .Where(static tuple => tuple is not null)
                                .Select(static (tuple, _) => tuple!.Value)
                                .Collect()
                                .Select(static (tuple, _) => tuple.Distinct());

      var enumTypes = candidates.Combine(genericTypes)
                                .Select(GetEnumStateOrNull)
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

   private static bool IsInstanceCreation(SyntaxNode node, CancellationToken _)
   {
      return node is BaseObjectCreationExpressionSyntax;
   }

   private static GenericEnumInfo? GetDerivedGenericEnums(
      GeneratorSyntaxContext context,
      CancellationToken cancellationToken)
   {
      var typeInfo = context.SemanticModel.GetTypeInfo(context.Node, cancellationToken);

      // search for generic inner enums
      if (typeInfo.Type is INamedTypeSymbol { IsGenericType: true, IsUnboundGenericType: false, ContainingSymbol: INamedTypeSymbol enumType } type)
      {
         // the enum is always the most outer class
         while (enumType.ContainingSymbol is INamedTypeSymbol namedTypeSymbol)
         {
            enumType = namedTypeSymbol;
         }

         if (enumType.IsEnum())
            return new(enumType, type);
      }

      return null;
   }

   private static ImmutableArray<ICodeGeneratorFactory<EnumSourceGeneratorState>> GetCodeGeneratorFactories(MetadataReference reference)
   {
      var factories = ImmutableArray<ICodeGeneratorFactory<EnumSourceGeneratorState>>.Empty;

      foreach (var module in reference.GetModules())
      {
         switch (module.Name)
         {
            case THINKTECTURE_RUNTIME_EXTENSIONS:
               factories = factories.Add(new SmartEnumCodeGeneratorFactory());
               break;

            case THINKTECTURE_RUNTIME_EXTENSIONS_JSON:
               factories = factories.Add(new JsonSmartEnumCodeGeneratorFactory());
               break;

            case THINKTECTURE_RUNTIME_EXTENSIONS_NEWTONSOFT_JSON:
               factories = factories.Add(new NewtonsoftJsonSmartEnumCodeGeneratorFactory());
               break;

            case THINKTECTURE_RUNTIME_EXTENSIONS_MESSAGEPACK:
               factories = factories.Add(new MessagePackSmartEnumCodeGeneratorFactory());
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

   private static INamedTypeSymbol? GetEnumType(GeneratorSyntaxContext context, CancellationToken cancellationToken)
   {
      var tds = (TypeDeclarationSyntax)context.Node;
      var type = context.SemanticModel.GetDeclaredSymbol(tds, cancellationToken);

      if (type?.ContainingType is not null)
         return null;

      return type;
   }

   private static SourceGenState<EnumSourceGeneratorState>? GetEnumStateOrNull(
      (INamedTypeSymbol, IEnumerable<GenericEnumInfo>) tuple,
      CancellationToken cancellationToken)
   {
      try
      {
         var (type, genericEnums) = tuple;
         var genericEnumTypes = ImmutableArray<INamedTypeSymbol>.Empty;

         foreach (var genericEnum in genericEnums)
         {
            if (SymbolEqualityComparer.Default.Equals(genericEnum.EnumType, type))
               genericEnumTypes = genericEnumTypes.Add(genericEnum.GenericEnumType);
         }

         if (!type.IsEnum(out var enumInterfaces))
            return null;

         var enumInterface = enumInterfaces.GetValidEnumInterface(type, cancellationToken);

         if (enumInterface is null)
            return null;

         return new SourceGenState<EnumSourceGeneratorState>(new EnumSourceGeneratorState(type, genericEnumTypes, enumInterface, cancellationToken), null);
      }
      catch (Exception ex)
      {
         return new SourceGenState<EnumSourceGeneratorState>(null, ex);
      }
   }

   private struct GenericEnumInfo : IEquatable<GenericEnumInfo>
   {
      public INamedTypeSymbol EnumType { get; }
      public INamedTypeSymbol GenericEnumType { get; }

      public GenericEnumInfo(INamedTypeSymbol enumType, INamedTypeSymbol genericEnumType)
      {
         EnumType = enumType;
         GenericEnumType = genericEnumType;
      }

      public bool Equals(GenericEnumInfo other)
      {
         var comparer = SymbolEqualityComparer.Default;

         return comparer.Equals(EnumType, other.EnumType)
                && comparer.Equals(GenericEnumType, other.GenericEnumType);
      }

      public override bool Equals(object? obj)
      {
         return obj is GenericEnumInfo other && Equals(other);
      }

      public override int GetHashCode()
      {
         var comparer = SymbolEqualityComparer.Default;

         unchecked
         {
            return (comparer.GetHashCode(EnumType) * 397) ^ comparer.GetHashCode(GenericEnumType);
         }
      }
   }
}
