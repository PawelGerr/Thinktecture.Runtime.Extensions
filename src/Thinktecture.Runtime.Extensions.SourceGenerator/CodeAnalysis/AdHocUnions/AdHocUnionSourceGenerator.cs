using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Thinktecture.CodeAnalysis.AdHocUnions;

[Generator]
public sealed class AdHocUnionSourceGenerator() : ThinktectureSourceGeneratorBase(20_000), IIncrementalGenerator
{
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
      InitializeUnionSourceGen(context, options, fullyQualifiedMetadataName, GetSourceGenContextOrNullForGeneric);
   }

   private void InitializeNonGenericUnionSourceGen(
      IncrementalGeneratorInitializationContext context,
      IncrementalValueProvider<GeneratorOptions> options,
      string fullyQualifiedMetadataName)
   {
      InitializeUnionSourceGen(context, options, fullyQualifiedMetadataName, GetSourceGenContextOrNullForNonGeneric);
   }

   private void InitializeUnionSourceGen(
      IncrementalGeneratorInitializationContext context,
      IncrementalValueProvider<GeneratorOptions> options,
      string fullyQualifiedMetadataName,
      Func<GeneratorAttributeSyntaxContext, CancellationToken, SourceGenContext?> getSourceGenContextOrNull)
   {
      var unionTypeOrError = context.SyntaxProvider
                                    .ForAttributeWithMetadataName(fullyQualifiedMetadataName,
                                                                  IsCandidate,
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

   private static bool IsCandidate(SyntaxNode syntaxNode, CancellationToken cancellationToken)
   {
      return syntaxNode switch
      {
         ClassDeclarationSyntax classDeclaration => !classDeclaration.IsGeneric(),
         StructDeclarationSyntax structDeclaration => !structDeclaration.IsGeneric(),
         _ => false
      };
   }

   private SourceGenContext? GetSourceGenContextOrNullForGeneric(GeneratorAttributeSyntaxContext context, CancellationToken cancellationToken)
   {
      return GetSourceGenContextOrNull(
         context,
         static (attributeClass, _) => attributeClass.TypeArguments.IsDefaultOrEmpty ? null : attributeClass.TypeArguments,
         cancellationToken);
   }

   private SourceGenContext? GetSourceGenContextOrNullForNonGeneric(GeneratorAttributeSyntaxContext context, CancellationToken cancellationToken)
   {
      return GetSourceGenContextOrNull(
         context,
         static (_, constructorArguments) =>
         {
            if (constructorArguments.IsDefaultOrEmpty)
               return null;

            var types = new List<ITypeSymbol>(constructorArguments.Length);
            var foundNull = false;

            for (var i = 0; i < constructorArguments.Length; i++)
            {
               var argument = constructorArguments[i];

               if (argument.IsNull)
               {
                  foundNull = true;
                  continue;
               }

               if (foundNull || argument.Value is not ITypeSymbol type || type.TypeKind == TypeKind.Error)
                  return null;

               types.Add(type);
            }

            return types.Count > 0 ? types : null;
         },
         cancellationToken);
   }

   private SourceGenContext? GetSourceGenContextOrNull(
      GeneratorAttributeSyntaxContext context,
      Func<INamedTypeSymbol, ImmutableArray<TypedConstant>, IReadOnlyList<ITypeSymbol>?> getMemberTypes,
      CancellationToken cancellationToken)
   {
      var tds = (TypeDeclarationSyntax)context.TargetNode;

      try
      {
         var type = (INamedTypeSymbol)context.TargetSymbol;

         if (type.TypeKind == TypeKind.Error
             || context.Attributes.IsDefaultOrEmpty
             || context.Attributes.Length != 1)
         {
            return null;
         }

         var attributeData = context.Attributes[0];

         if (attributeData.AttributeClass is null
             || attributeData.AttributeClass.TypeKind == TypeKind.Error)
         {
            return null;
         }

         var memberTypeSymbols = getMemberTypes(attributeData.AttributeClass, attributeData.ConstructorArguments);

         if (memberTypeSymbols is null
             || memberTypeSymbols.Count < 2)
         {
            return null;
         }

         var errorMessage = AttributeInfo.TryCreate(type, out var attributeInfo);

         if (errorMessage is not null)
            return new SourceGenContext(new SourceGenError(errorMessage, tds));

         var factory = TypedMemberStateFactoryProvider.GetFactoryOrNull(context.SemanticModel.Compilation);

         if (factory is null)
            return new SourceGenContext(new SourceGenError("Could not fetch type information for code generation of a discriminated union", tds));

         var settings = new AdHocUnionSettings(context.Attributes[0],
                                               memberTypeSymbols.Count);
         var memberTypeStates = new AdHocUnionMemberTypeState[memberTypeSymbols.Count];

         for (var i = 0; i < memberTypeSymbols.Count; i++)
         {
            var memberType = memberTypeSymbols[i];

            if (memberType.TypeKind == TypeKind.Error)
               return null;

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
               return new SourceGenContext(new SourceGenError("Type of the member must be a named type or array type", tds));
            }

            var name = memberTypeSettings.Name ??
                       (typeDuplicateCounter == 0 ? defaultName : defaultName + typeDuplicateCounter);

            if (String.IsNullOrWhiteSpace(name))
               return null;

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
