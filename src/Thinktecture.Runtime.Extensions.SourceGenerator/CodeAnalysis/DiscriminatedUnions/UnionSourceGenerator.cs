using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Thinktecture.CodeAnalysis.DiscriminatedUnions;

[Generator]
public class UnionSourceGenerator : ThinktectureSourceGeneratorBase, IIncrementalGenerator
{
   public UnionSourceGenerator()
      : base(15_000)
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
      InitializeUnionSourceGen(context, options, Constants.Attributes.Union.FULL_NAME_2_TYPES);
      InitializeUnionSourceGen(context, options, Constants.Attributes.Union.FULL_NAME_3_TYPES);
      InitializeUnionSourceGen(context, options, Constants.Attributes.Union.FULL_NAME_4_TYPES);
      InitializeUnionSourceGen(context, options, Constants.Attributes.Union.FULL_NAME_5_TYPES);
   }

   private void InitializeUnionSourceGen(
      IncrementalGeneratorInitializationContext context,
      IncrementalValueProvider<GeneratorOptions> options,
      string fullyQualifiedMetadataName)
   {
      var unionTypeOrError = context.SyntaxProvider
                                    .ForAttributeWithMetadataName(fullyQualifiedMetadataName,
                                                                  IsCandidate,
                                                                  GetSourceGenContextOrNull)
                                    .SelectMany(static (state, _) => state.HasValue
                                                                        ? [state.Value]
                                                                        : ImmutableArray<SourceGenContext>.Empty);

      var validStates = unionTypeOrError.SelectMany(static (state, _) => state.ValidState is not null
                                                                            ? [state.ValidState]
                                                                            : ImmutableArray<UnionSourceGenState>.Empty);

      InitializeUnionTypeGeneration(context, validStates, options);

      InitializeErrorReporting(context, unionTypeOrError);
      InitializeExceptionReporting(context, unionTypeOrError);
   }

   private bool IsCandidate(SyntaxNode syntaxNode, CancellationToken cancellationToken)
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

   private SourceGenContext? GetSourceGenContextOrNull(GeneratorAttributeSyntaxContext context, CancellationToken cancellationToken)
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

         if (context.Attributes.Length > 1)
         {
            Logger.LogDebug($"Type has more than 1 '{Constants.Attributes.Union.NAME}'", tds);
            return null;
         }

         var attributetype = context.Attributes[0].AttributeClass;

         if (attributetype is null)
         {
            Logger.LogDebug("The attribute type is null", tds);
            return null;
         }

         if (attributetype.TypeKind == TypeKind.Error)
         {
            Logger.LogDebug("The attribute type is erroneous", tds);
            return null;
         }

         if (attributetype.TypeArguments.Length < 2)
         {
            Logger.LogDebug($"Expected the attribute type to have at least 2 type arguments but found {attributetype.TypeArguments.Length.ToString()}", tds);
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

         var settings = new AllUnionSettings(context.Attributes[0], attributetype.TypeArguments.Length);
         var memberTypeStates = new MemberTypeState[attributetype.TypeArguments.Length];

         for (var i = 0; i < attributetype.TypeArguments.Length; i++)
         {
            var memberType = attributetype.TypeArguments[i];

            if (memberType.TypeKind == TypeKind.Error)
            {
               Logger.LogDebug("Type of the member is erroneous", tds);
               return null;
            }

            var memberTypeSettings = settings.MemberTypeSettings[i];
            memberType = memberType.IsReferenceType && memberTypeSettings.IsNullableReferenceType ? memberType.WithNullableAnnotation(NullableAnnotation.Annotated) : memberType;
            var typeState = factory.Create(memberType);

            var typeDuplicateIndex = 0;

            for (var j = 0; j < attributetype.TypeArguments.Length; j++)
            {
               if (j == i)
               {
                  if (typeDuplicateIndex != 0)
                     ++typeDuplicateIndex;

                  continue;
               }

               if (!SymbolEqualityComparer.Default.Equals(memberType, attributetype.TypeArguments[j]))
                  continue;

               if (j > i && typeDuplicateIndex != 0)
                  break;

               ++typeDuplicateIndex;
            }

            (string Name, string DefaultName) memberTypeName;
            var duplicateIndex = typeDuplicateIndex == 0 ? (int?)null : typeDuplicateIndex;

            switch (memberType)
            {
               case INamedTypeSymbol namedTypeSymbol:
                  memberTypeName = MemberTypeState.GetMemberName(memberTypeSettings,
                                                                 duplicateIndex,
                                                                 namedTypeSymbol,
                                                                 typeState);
                  break;
               case IArrayTypeSymbol arrayTypeSymbol:
                  memberTypeName = MemberTypeState.GetMemberName(memberTypeSettings,
                                                                 duplicateIndex,
                                                                 arrayTypeSymbol);
                  break;
               default:
                  Logger.LogError("Type of the member must be a named type or array type", tds);
                  return null;
            }

            memberTypeStates[i] = new MemberTypeState(memberTypeName.Name,
                                                      memberTypeName.DefaultName,
                                                      duplicateIndex,
                                                      typeState,
                                                      memberTypeSettings);
         }

         var unionState = new UnionSourceGenState(type,
                                                  memberTypeStates,
                                                  new UnionSettings(settings, attributeInfo));

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
      IncrementalValuesProvider<UnionSourceGenState> validStates,
      IncrementalValueProvider<GeneratorOptions> options)
   {
      var unionTypes = validStates
                       .Collect()
                       .Select(static (states, _) => states.IsDefaultOrEmpty
                                                        ? ImmutableArray<UnionSourceGenState>.Empty
                                                        : states.Distinct(TypeOnlyComparer.Instance))
                       .WithComparer(new SetComparer<UnionSourceGenState>())
                       .SelectMany((states, _) => states);

      context.RegisterSourceOutput(unionTypes.Combine(options), (ctx, tuple) => GenerateCode(ctx, tuple.Left, tuple.Right, UnionCodeGeneratorFactory.Instance));
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

   private readonly record struct SourceGenContext(UnionSourceGenState? ValidState, SourceGenException? Exception, SourceGenError? Error)
   {
      public SourceGenContext(UnionSourceGenState validState)
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
