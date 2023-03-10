using System.Collections.Immutable;
using System.Diagnostics;
using System.Text;
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

      var genericTypes = context.SyntaxProvider.CreateSyntaxProvider(IsInstanceCreation, GetDerivedGenericEnums)
                                .Where(static tuple => tuple is not null)
                                .Select(static (tuple, _) => tuple)!
                                .Collect<GenericEnumInfo>()
                                .Select(static (tuple, _) => tuple.Distinct().ToImmutableArray())
                                .WithComparer(new SetComparer<GenericEnumInfo>());

      context.RegisterImplementationSourceOutput(genericTypes, GenerateModuleInitializerCode);
   }

   private void GenerateModuleInitializerCode(
      SourceProductionContext context,
      ImmutableArray<GenericEnumInfo> genericEnumInfos)
   {
      var stringBuilder = LeaseStringBuilder();

      try
      {
         foreach (var group in genericEnumInfos.GroupBy(i => i.EnumTypeFullyQualified))
         {
            stringBuilder.Clear();

            var candidate = group.First();
            GenerateModuleInitializer(stringBuilder, candidate, group);

            if (stringBuilder.Length <= 0)
               return;

            var generatedCode = stringBuilder.ToString();

            context.EmitFile(candidate.Namespace, candidate.EnumTypeName, generatedCode, ".Generics");
         }
      }
      finally
      {
         Return(stringBuilder);
      }
   }

   private static void GenerateModuleInitializer(StringBuilder sb, GenericEnumInfo candidate, IEnumerable<GenericEnumInfo> genericEnums)
   {
      sb.Append(CodeGeneratorBase.GENERATED_CODE_PREFIX).Append(@"
");

      if (candidate.Namespace is not null)
      {
         sb.Append(@"
namespace ").Append(candidate.Namespace).Append(@";
");
      }

      sb.Append(@"
partial class ").Append(candidate.EnumTypeName).Append(@"
{
   [global::System.Runtime.CompilerServices.ModuleInitializer]
   internal static void GenericsModuleInit()
   {
      var enumType = typeof(").Append(candidate.EnumTypeFullyQualified).Append(@");");

      foreach (var genericEnum in genericEnums)
      {
         sb.Append(@"
      global::Thinktecture.Internal.KeyedValueObjectMetadataLookup.AddDerivedType(enumType, typeof(").Append(genericEnum.GenericEnumTypeFullyQualified).Append(@"));");
      }

      sb.Append(@"
   }
}
");
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
