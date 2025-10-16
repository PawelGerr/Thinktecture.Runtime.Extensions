using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Thinktecture.CodeAnalysis.AdHocUnions;

[Generator]
public class AdHocUnionSourceGenerator : ThinktectureSourceGeneratorBase, IIncrementalGenerator
{
   public AdHocUnionSourceGenerator()
      : base(20_000)
   {
   }

   public void Initialize(IncrementalGeneratorInitializationContext context)
   {
      var options = GetGeneratorOptions(context);

      SetupLogger(context, options);

      InitializeUnionSourceGen(context, options);
   }

   private void InitializeUnionSourceGen(
      IncrementalGeneratorInitializationContext context,
      IncrementalValueProvider<GeneratorOptions> options)
   {
      InitializeGenericUnionSourceGen(context, options, Constants.Attributes.Union.FULL_NAME_2_TYPES);
      InitializeGenericUnionSourceGen(context, options, Constants.Attributes.Union.FULL_NAME_3_TYPES);
      InitializeGenericUnionSourceGen(context, options, Constants.Attributes.Union.FULL_NAME_4_TYPES);
      InitializeGenericUnionSourceGen(context, options, Constants.Attributes.Union.FULL_NAME_5_TYPES);
      InitializeNonGenericUnionSourceGen(context, options, Constants.Attributes.Union.FULL_NAME_AD_HOCH);
   }

   private void InitializeGenericUnionSourceGen(
      IncrementalGeneratorInitializationContext context,
      IncrementalValueProvider<GeneratorOptions> options,
      string fullyQualifiedMetadataName)
   {
      InitializeUnionSourceGen(context, options, fullyQualifiedMetadataName, IsGenericCandidate, GetSourceGenContextOrNullForGeneric);
   }

   private void InitializeNonGenericUnionSourceGen(
      IncrementalGeneratorInitializationContext context,
      IncrementalValueProvider<GeneratorOptions> options,
      string fullyQualifiedMetadataName)
   {
      InitializeUnionSourceGen(context, options, fullyQualifiedMetadataName, IsNonGenericCandidate, GetSourceGenContextOrNullForNonGeneric);
   }

   private void InitializeUnionSourceGen(
      IncrementalGeneratorInitializationContext context,
      IncrementalValueProvider<GeneratorOptions> options,
      string fullyQualifiedMetadataName,
      Func<SyntaxNode, CancellationToken, bool> isCandate,
      Func<GeneratorAttributeSyntaxContext, CancellationToken, SourceGenContext?> getSourceGenContextOrNull)
   {
      var unionTypeOrError = context.SyntaxProvider
                                    .ForAttributeWithMetadataName(fullyQualifiedMetadataName,
                                                                  isCandate,
                                                                  getSourceGenContextOrNull)
                                    .SelectMany(static (state, _) => state.HasValue
                                                                        ? [state.Value]
                                                                        : ImmutableArray<SourceGenContext>.Empty);

      var validStates = unionTypeOrError.SelectMany(static (state, _) => state.ValidState is not null
                                                                            ? [state.ValidState]
                                                                            : ImmutableArray<AdHocUnionSourceGenState>.Empty);

      InitializeUnionTypeGeneration(context, validStates, options);

      InitializeErrorReporting(context, unionTypeOrError);
      InitializeExceptionReporting(context, unionTypeOrError);
   }

   private bool IsGenericCandidate(SyntaxNode syntaxNode, CancellationToken cancellationToken)
   {
      try
      {
         return syntaxNode switch
         {
            ClassDeclarationSyntax classDeclaration when IsUnionCandidate(classDeclaration) => true,
            StructDeclarationSyntax structDeclaration when IsUnionCandidate(structDeclaration) => true,
            _ => false
         };
      }
      catch (Exception ex)
      {
         Logger.LogError("Error during checking whether a syntax node is a discriminated union candidate", exception: ex);
         return false;
      }
   }

   private bool IsNonGenericCandidate(SyntaxNode syntaxNode, CancellationToken cancellationToken)
   {
      try
      {
         return syntaxNode switch
         {
            ClassDeclarationSyntax => true,
            StructDeclarationSyntax => true,
            _ => false
         };
      }
      catch (Exception ex)
      {
         Logger.LogError("Error during checking whether a syntax node is a discriminated union candidate", exception: ex);
         return false;
      }
   }

   private bool IsUnionCandidate(TypeDeclarationSyntax typeDeclaration)
   {
      var isCandidate = !typeDeclaration.IsGeneric();

      if (isCandidate)
      {
         Logger.LogDebug("The type declaration is a discriminated union candidate", typeDeclaration);
      }
      else
      {
         Logger.LogTrace("The type declaration is not a discriminated union candidate", typeDeclaration);
      }

      return isCandidate;
   }

   private SourceGenContext? GetSourceGenContextOrNullForGeneric(GeneratorAttributeSyntaxContext context, CancellationToken cancellationToken)
   {
      return GetSourceGenContextOrNull(
         context,
         (tds, data) =>
         {
            var attributeType = data.AttributeClass;

            if (attributeType is null)
            {
               Logger.LogDebug("The attribute type is null", tds);
               return null;
            }

            if (attributeType.TypeKind == TypeKind.Error)
            {
               Logger.LogDebug("The attribute type is erroneous", tds);
               return null;
            }

            if (attributeType.TypeArguments.IsDefaultOrEmpty)
               return null;

            return attributeType.TypeArguments;
         },
         cancellationToken);
   }

   private SourceGenContext? GetSourceGenContextOrNullForNonGeneric(GeneratorAttributeSyntaxContext context, CancellationToken cancellationToken)
   {
      return GetSourceGenContextOrNull(
         context,
         (tds, data) =>
         {
            var attributeType = data.AttributeClass;

            if (attributeType is null)
            {
               Logger.LogDebug("The attribute type is null", tds);
               return null;
            }

            if (attributeType.TypeKind == TypeKind.Error)
            {
               Logger.LogDebug("The attribute type is erroneous", tds);
               return null;
            }

            if(data.ConstructorArguments.IsDefaultOrEmpty)
               return null;

            var types = new List<ITypeSymbol>();
            var foundNull = false;

            for (var i = 0; i < data.ConstructorArguments.Length; i++)
            {
               var argument = data.ConstructorArguments[i];

               if (argument.IsNull)
               {
                  foundNull = true;
                  continue;
               }

               if (foundNull || argument.Value is not ITypeSymbol type || type.TypeKind == TypeKind.Error)
                  return null;

               types.Add(type);
            }

            return types;
         },
         cancellationToken);
   }

   private SourceGenContext? GetSourceGenContextOrNull(
      GeneratorAttributeSyntaxContext context,
      Func<TypeDeclarationSyntax, AttributeData, IReadOnlyList<ITypeSymbol>?> getMemberTypes,
      CancellationToken cancellationToken)
   {
      var tds = (TypeDeclarationSyntax)context.TargetNode;

      try
      {
         var type = (INamedTypeSymbol)context.TargetSymbol;

         if (type.TypeKind == TypeKind.Error)
         {
            Logger.LogDebug("Type from semantic model is erroneous", tds);
            return null;
         }

         if (context.Attributes.IsDefaultOrEmpty)
            return null;

         if (context.Attributes.Length > 1)
         {
            Logger.LogDebug($"Type has more than 1 '{Constants.Attributes.Union.NAME}'", tds);
            return null;
         }

         var attributeData = context.Attributes[0];
         var memberTypeSymbols = getMemberTypes(tds, attributeData);

         if (memberTypeSymbols is null)
         {
            return null;
         }

         if (memberTypeSymbols.Count < 2)
         {
            Logger.LogDebug($"Expected the union to have at least 2 member types but found {memberTypeSymbols.Count}", tds);
            return null;
         }

         var errorMessage = AttributeInfo.TryCreate(type, out var attributeInfo);

         if (errorMessage is not null)
         {
            Logger.LogDebug(errorMessage, tds);
            return null;
         }

         var factory = TypedMemberStateFactoryProvider.GetFactoryOrNull(context.SemanticModel.Compilation, Logger);

         if (factory is null)
            return new SourceGenContext(new SourceGenError("Could not fetch type information for code generation of a discriminated union", tds));

         var settings = new AdHocUnionSettings(context.Attributes[0],
                                               memberTypeSymbols.Count);
         var memberTypeStates = new AdHocUnionMemberTypeState[memberTypeSymbols.Count];

         for (var i = 0; i < memberTypeSymbols.Count; i++)
         {
            var memberType = memberTypeSymbols[i];

            if (memberType.TypeKind == TypeKind.Error)
            {
               Logger.LogDebug("Type of the member is erroneous", tds);
               return null;
            }

            var memberTypeSettings = settings.MemberTypeSettings[i];
            memberType = memberType.IsReferenceType && memberTypeSettings.IsNullableReferenceType ? memberType.WithNullableAnnotation(NullableAnnotation.Annotated) : memberType;
            var typeState = factory.Create(memberType);

            var typeDuplicateCounter = 0;

            for (var j = 0; j < memberTypeSymbols.Count; j++)
            {
               if (j == i)
               {
                  if (typeDuplicateCounter != 0)
                     ++typeDuplicateCounter;

                  continue;
               }

               if (!SymbolEqualityComparer.Default.Equals(memberType, memberTypeSymbols[j]))
                  continue;

               if (j > i && typeDuplicateCounter != 0)
                  break;

               ++typeDuplicateCounter;
            }

            if (!memberType.TryBuildMemberName(out var defaultName))
            {
               Logger.LogError("Type of the member must be a named type or array type", tds);
               return null;
            }

            var name = memberTypeSettings.Name ??
                       (typeDuplicateCounter == 0 ? defaultName : defaultName + typeDuplicateCounter);

            memberTypeStates[i] = new AdHocUnionMemberTypeState(name,
                                                                defaultName,
                                                                typeDuplicateCounter,
                                                                typeState,
                                                                memberTypeSettings);
         }

         var unionState = new AdHocUnionSourceGenState(type,
                                                       memberTypeStates,
                                                       settings,
                                                       attributeInfo);

         Logger.LogDebug("The type declaration is a valid union", null, unionState);

         return new SourceGenContext(unionState);
      }
      catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
      {
         throw;
      }
      catch (Exception ex)
      {
         Logger.LogError("Error during extraction of relevant information out of semantic model for generation of a discriminated union", tds, ex);

         return new SourceGenContext(new SourceGenException(ex, tds));
      }
   }

   private void InitializeUnionTypeGeneration(
      IncrementalGeneratorInitializationContext context,
      IncrementalValuesProvider<AdHocUnionSourceGenState> validStates,
      IncrementalValueProvider<GeneratorOptions> options)
   {
      var unionTypes = validStates
                       .Collect()
                       .Select(static (states, _) => states.IsDefaultOrEmpty
                                                        ? ImmutableArray<AdHocUnionSourceGenState>.Empty
                                                        : states.Distinct(TypeOnlyComparer.Instance))
                       .WithComparer(new SetComparer<AdHocUnionSourceGenState>())
                       .SelectMany((states, _) => states);

      context.RegisterSourceOutput(unionTypes.Combine(options), (ctx, tuple) => GenerateCode(ctx, tuple.Left, tuple.Right, AdHocUnionCodeGeneratorFactory.Instance));
   }

   private void InitializeErrorReporting(
      IncrementalGeneratorInitializationContext context,
      IncrementalValuesProvider<SourceGenContext> unionTypeOrException)
   {
      var exceptions = unionTypeOrException.SelectMany(static (state, _) => state.Error is not null
                                                                               ? [state.Error.Value]
                                                                               : ImmutableArray<SourceGenError>.Empty);
      context.RegisterSourceOutput(exceptions, ReportError);
   }

   private void InitializeExceptionReporting(
      IncrementalGeneratorInitializationContext context,
      IncrementalValuesProvider<SourceGenContext> unionTypeOrException)
   {
      var exceptions = unionTypeOrException.SelectMany(static (state, _) => state.Exception is not null
                                                                               ? [state.Exception.Value]
                                                                               : ImmutableArray<SourceGenException>.Empty);
      context.RegisterSourceOutput(exceptions, ReportException);
   }

   private readonly record struct SourceGenContext(AdHocUnionSourceGenState? ValidState, SourceGenException? Exception, SourceGenError? Error)
   {
      public SourceGenContext(AdHocUnionSourceGenState validState)
         : this(validState, null, null)
      {
      }

      public SourceGenContext(SourceGenException exception)
         : this(null, exception, null)
      {
      }

      public SourceGenContext(SourceGenError errorMessage)
         : this(null, null, errorMessage)
      {
      }
   }
}
