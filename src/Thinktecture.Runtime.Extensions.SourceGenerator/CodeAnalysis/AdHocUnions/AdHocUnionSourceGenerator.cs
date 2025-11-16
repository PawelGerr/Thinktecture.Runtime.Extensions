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
      InitializeNonGenericUnionSourceGen(context, options, Constants.Attributes.Union.FULL_NAME_AD_HOC);
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

      InitializeDiagnosticReporting(context, unionTypeOrError);
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
         static (attributeClass, _) => attributeClass.TypeArguments.IsDefaultOrEmpty ? [] : attributeClass.TypeArguments,
         cancellationToken);
   }

   private SourceGenContext? GetSourceGenContextOrNullForNonGeneric(GeneratorAttributeSyntaxContext context, CancellationToken cancellationToken)
   {
      return GetSourceGenContextOrNull(
         context,
         static (_, constructorArguments) =>
         {
            if (constructorArguments.IsDefaultOrEmpty)
               return [];

            ImmutableArray<ITypeSymbol>.Builder? types = null;
            var foundNull = false;

            for (var i = 0; i < constructorArguments.Length; i++)
            {
               var argument = constructorArguments[i];

               if (argument.IsNull)
               {
                  foundNull = true;
                  continue;
               }

               if (foundNull || argument.Value is not ITypeSymbol type)
                  return [];

               (types ??= ImmutableArray.CreateBuilder<ITypeSymbol>(constructorArguments.Length)).Add(type);
            }

            return types?.Count > 0 ? types.DrainToImmutable() : [];
         },
         cancellationToken);
   }

   private static SourceGenContext? GetSourceGenContextOrNull(
      GeneratorAttributeSyntaxContext context,
      Func<INamedTypeSymbol, ImmutableArray<TypedConstant>, ImmutableArray<ITypeSymbol>> getMemberTypes,
      CancellationToken cancellationToken)
   {
      var tds = (TypeDeclarationSyntax)context.TargetNode;

      try
      {
         var type = (INamedTypeSymbol)context.TargetSymbol;

         if (type.TypeKind == TypeKind.Error)
            return null;

         if (type.Arity > 0)
            return null; // Analyzer emits DiagnosticsDescriptors.AdHocUnionsMustNotBeGeneric

         if (context.Attributes.IsDefaultOrEmpty)
            return null;

         if (context.Attributes.Length > 1)
            return new SourceGenDiagnostic(tds, DiagnosticsDescriptors.TypeMustNotHaveMoveThanOneDiscriminatedUnionAttribute, [type.ToMinimallyQualifiedDisplayString()]);

         var attributeData = context.Attributes[0];

         if (attributeData.AttributeClass is null)
            return new SourceGenDiagnostic(tds, DiagnosticsDescriptors.ErrorDuringCodeAnalysis, [type.ToMinimallyQualifiedDisplayString(), "Could not resolve discriminated union attribute type"]);

         if (attributeData.AttributeClass.TypeKind == TypeKind.Error)
            return new SourceGenDiagnostic(tds, DiagnosticsDescriptors.ErrorDuringCodeAnalysis, [type.ToMinimallyQualifiedDisplayString(), "Discriminated union attribute has TypeKind=Error"]);

         var memberTypeSymbols = getMemberTypes(attributeData.AttributeClass, attributeData.ConstructorArguments);

         if (memberTypeSymbols.Length < 2)
            return new SourceGenDiagnostic(tds, DiagnosticsDescriptors.AdHocUnionMustHaveAtLeastTwoMemberTypes, [type.ToMinimallyQualifiedDisplayString()]);

         var diagnostic = AttributeInfo.TryCreate(type, out var attributeInfo);

         if (diagnostic is not null)
            return new SourceGenDiagnostic(tds, diagnostic.Value.Descriptor, diagnostic.Value.Args);

         var factory = TypedMemberStateFactoryProvider.GetFactoryOrNull(context.SemanticModel.Compilation);

         if (factory is null)
            return new SourceGenDiagnostic(tds, DiagnosticsDescriptors.ErrorDuringCodeAnalysis, [type.ToMinimallyQualifiedDisplayString(), "Could not fetch type information for code generation of a discriminated union"]);

         var settings = new AdHocUnionSettings(context.Attributes[0],
                                               memberTypeSymbols.Length);
         var memberTypeStates = ImmutableArray.CreateBuilder<AdHocUnionMemberTypeState>(memberTypeSymbols.Length);

         for (var i = 0; i < memberTypeSymbols.Length; i++)
         {
            var memberType = memberTypeSymbols[i];

            if (memberType.TypeKind == TypeKind.Error)
               return new SourceGenDiagnostic(tds, DiagnosticsDescriptors.ErrorDuringCodeAnalysis, [type.ToMinimallyQualifiedDisplayString(), $"The member type '{memberType.Name}' could not be resolved"]);

            var memberTypeSettings = settings.MemberTypeSettings[i];
            memberType = memberType.IsReferenceType && memberTypeSettings.IsNullableReferenceType ? memberType.WithNullableAnnotation(NullableAnnotation.Annotated) : memberType;
            var typeState = factory.Create(memberType);

            var typeDuplicateCounter = 0;

            for (var j = 0; j < memberTypeSymbols.Length; j++)
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
               return new SourceGenDiagnostic(tds, DiagnosticsDescriptors.ErrorDuringGeneration, [type.ToMinimallyQualifiedDisplayString(), $"Could not build name for type '{memberType.Name}'. The type must be a named type, an array or a parameter but found '{memberType.GetType().FullName}'."]);

            var name = memberTypeSettings.Name ??
                       (typeDuplicateCounter == 0 ? defaultName : defaultName + typeDuplicateCounter);

            if (String.IsNullOrWhiteSpace(name))
               return new SourceGenDiagnostic(tds, DiagnosticsDescriptors.ErrorDuringGeneration, [type.ToMinimallyQualifiedDisplayString(), $"The name for type '{memberType.Name}' must not be null nor empty."]);

            memberTypeStates.Add(new AdHocUnionMemberTypeState(name,
                                                               defaultName,
                                                               typeDuplicateCounter,
                                                               typeState,
                                                               memberTypeSettings));
         }

         return new AdHocUnionSourceGenState(type,
                                             memberTypeStates.DrainToImmutable(),
                                             settings,
                                             attributeInfo);
      }
      catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
      {
         throw;
      }
      catch (Exception ex)
      {
         return new SourceGenException("Error during extraction of relevant information out of semantic model for generation of a discriminated union", ex, tds);
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
                       .WithComparer(SetComparer<AdHocUnionSourceGenState>.Instance)
                       .SelectMany((states, _) => states);

      context.RegisterSourceOutput(unionTypes.Combine(options), (ctx, tuple) => GenerateCode(ctx, tuple.Left, tuple.Right, AdHocUnionCodeGeneratorFactory.Instance));
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

   private void InitializeDiagnosticReporting(
      IncrementalGeneratorInitializationContext context,
      IncrementalValuesProvider<SourceGenContext> unionTypeOrException)
   {
      var exceptions = unionTypeOrException.SelectMany(static (state, _) => state.Diagnostic is not null
                                                                               ? [state.Diagnostic.Value]
                                                                               : ImmutableArray<SourceGenDiagnostic>.Empty);
      context.RegisterSourceOutput(exceptions, ReportDiagnostic);
   }

   private readonly record struct SourceGenContext(
      AdHocUnionSourceGenState? ValidState,
      SourceGenException? Exception,
      SourceGenDiagnostic? Diagnostic)
   {
      public static implicit operator SourceGenContext(AdHocUnionSourceGenState state)
      {
         return new SourceGenContext(state, null, null);
      }

      public static implicit operator SourceGenContext(SourceGenException exception)
      {
         return new SourceGenContext(null, exception, null);
      }

      public static implicit operator SourceGenContext(SourceGenDiagnostic diagnostic)
      {
         return new SourceGenContext(null, null, diagnostic);
      }
   }
}
