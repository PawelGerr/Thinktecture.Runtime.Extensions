using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Thinktecture.CodeAnalysis.SmartEnums;

[Generator]
public sealed class SmartEnumSourceGenerator : ThinktectureSourceGeneratorBase, IIncrementalGenerator
{
   public SmartEnumSourceGenerator()
      : base(17_000)
   {
   }

   public void Initialize(IncrementalGeneratorInitializationContext context)
   {
      SetupLogger(context);

      var enumTypeOrError = context.SyntaxProvider
                                   .CreateSyntaxProvider(IsCandidate, GetSourceGenContext)
                                   .SelectMany(static (state, _) => state.HasValue
                                                                       ? ImmutableArray.Create(state.Value)
                                                                       : ImmutableArray<SourceGenContext>.Empty);

      var validStates = enumTypeOrError.SelectMany(static (state, _) => state.ValidState is not null
                                                                           ? ImmutableArray.Create(state.ValidState.Value)
                                                                           : ImmutableArray<ValidSourceGenState>.Empty);

      var options = GetGeneratorOptions(context);

      InitializeEnumTypeGeneration(context, validStates, options);
      InitializeSerializerGenerators(context, validStates, options);
      InitializeDerivedTypesGeneration(context, validStates, options);
      InitializeFormattableCodeGenerator(context, validStates, options);
      InitializeComparableCodeGenerator(context, validStates, options);
      InitializeParsableCodeGenerator(context, validStates, options);
      InitializeComparisonOperatorsCodeGenerator(context, validStates, options);

      InitializeErrorReporting(context, enumTypeOrError);
      InitializeExceptionReporting(context, enumTypeOrError);
   }

   private void InitializeComparisonOperatorsCodeGenerator(IncrementalGeneratorInitializationContext context, IncrementalValuesProvider<ValidSourceGenState> validStates, IncrementalValueProvider<GeneratorOptions> options)
   {
      var comparables = validStates
         .Select((state, _) => new ComparisonOperatorsGeneratorState(state.State,
                                                                     state.KeyMember,
                                                                     state.Settings.ComparisonOperators,
                                                                     state.KeyMember.HasComparisonOperators,
                                                                     null));

      InitializeComparisonOperatorsCodeGenerator(context, comparables, options);
   }

   private void InitializeParsableCodeGenerator(IncrementalGeneratorInitializationContext context, IncrementalValuesProvider<ValidSourceGenState> validStates, IncrementalValueProvider<GeneratorOptions> options)
   {
      var parsables = validStates
         .Select((state, _) => new ParsableGeneratorState(state.State,
                                                          state.KeyMember,
                                                          state.Settings.SkipIParsable,
                                                          state.KeyMember.IsParsable,
                                                          state.State.IsValidatable));
      InitializeParsableCodeGenerator(context, parsables, options);
   }

   private void InitializeComparableCodeGenerator(IncrementalGeneratorInitializationContext context, IncrementalValuesProvider<ValidSourceGenState> validStates, IncrementalValueProvider<GeneratorOptions> options)
   {
      var comparables = validStates
         .Select((state, _) => new ComparableGeneratorState(state.State,
                                                            state.KeyMember,
                                                            state.Settings.SkipIComparable,
                                                            state.KeyMember.IsComparable,
                                                            null));

      InitializeComparableCodeGenerator(context, comparables, options);
   }

   private void InitializeFormattableCodeGenerator(IncrementalGeneratorInitializationContext context, IncrementalValuesProvider<ValidSourceGenState> validStates, IncrementalValueProvider<GeneratorOptions> options)
   {
      var formattables = validStates
         .Select((state, _) => new FormattableGeneratorState(state.State,
                                                             state.KeyMember,
                                                             state.Settings.SkipIFormattable,
                                                             state.KeyMember.IsFormattable));

      InitializeFormattableCodeGenerator(context, formattables, options);
   }

   private void InitializeEnumTypeGeneration(
      IncrementalGeneratorInitializationContext context,
      IncrementalValuesProvider<ValidSourceGenState> validStates,
      IncrementalValueProvider<GeneratorOptions> options)
   {
      var enumTypes = validStates
                      .Select((state, _) => state.State)
                      .Collect()
                      .Select(static (states, _) => states.IsDefaultOrEmpty
                                                       ? ImmutableArray<EnumSourceGeneratorState>.Empty
                                                       : states.Distinct(TypeOnlyComparer<EnumSourceGeneratorState>.Instance).ToImmutableArray())
                      .WithComparer(new SetComparer<EnumSourceGeneratorState>())
                      .SelectMany((states, _) => states);

      context.RegisterSourceOutput(enumTypes.Combine(options), (ctx, tuple) => GenerateCode(ctx, tuple.Left, tuple.Right, SmartEnumCodeGeneratorFactory.Instance));
   }

   private void InitializeSerializerGenerators(IncrementalGeneratorInitializationContext context, IncrementalValuesProvider<ValidSourceGenState> validStates, IncrementalValueProvider<GeneratorOptions> options)
   {
      var serializerGeneratorFactories = context.MetadataReferencesProvider
                                                .SelectMany((reference, _) => GetSerializerCodeGeneratorFactories(reference))
                                                .Collect()
                                                .Select(static (states, _) => states.IsDefaultOrEmpty
                                                                                 ? ImmutableArray<IKeyedSerializerCodeGeneratorFactory>.Empty
                                                                                 : states.Distinct().ToImmutableArray())
                                                .WithComparer(new SetComparer<IKeyedSerializerCodeGeneratorFactory>());

      var serializerGeneratorStates = validStates.Select((state, _) => new KeyedSerializerGeneratorState(state.State, state.KeyMember, state.AttributeInfo))
                                                 .Combine(serializerGeneratorFactories)
                                                 .SelectMany((tuple, _) => ImmutableArray.CreateRange(tuple.Right, (factory, state) => (State: state, Factory: factory), tuple.Left))
                                                 .Where(tuple => tuple.Factory.MustGenerateCode(tuple.State.AttributeInfo));

      context.RegisterImplementationSourceOutput(serializerGeneratorStates.Combine(options), (ctx, tuple) => GenerateCode(ctx, tuple.Left.State, tuple.Right, tuple.Left.Factory));
   }

   private void InitializeDerivedTypesGeneration(IncrementalGeneratorInitializationContext context, IncrementalValuesProvider<ValidSourceGenState> validStates, IncrementalValueProvider<GeneratorOptions> options)
   {
      var derivedTypes = validStates
                         .Select(static (state, _) => state.DerivedTypes)
                         .Where(static derivedTypes => derivedTypes.DerivedTypesFullyQualified.Count > 0)
                         .Collect()
                         .Select(static (states, _) => states.IsDefaultOrEmpty
                                                          ? ImmutableArray<SmartEnumDerivedTypes>.Empty
                                                          : states.Distinct(TypeOnlyComparer<SmartEnumDerivedTypes>.Instance).ToImmutableArray())
                         .WithComparer(new SetComparer<SmartEnumDerivedTypes>())
                         .SelectMany((states, _) => states);

      context.RegisterImplementationSourceOutput(derivedTypes.Combine(options), (ctx, types) => GenerateCode(ctx, types.Left, types.Right, DerivedTypesCodeGeneratorFactory.Instance));
   }

   private void InitializeErrorReporting(IncrementalGeneratorInitializationContext context, IncrementalValuesProvider<SourceGenContext> enumTypeOrException)
   {
      var exceptions = enumTypeOrException.SelectMany(static (state, _) => state.Error is not null
                                                                              ? ImmutableArray.Create(state.Error.Value)
                                                                              : ImmutableArray<SourceGenError>.Empty);
      context.RegisterSourceOutput(exceptions, ReportError);
   }

   private void InitializeExceptionReporting(IncrementalGeneratorInitializationContext context, IncrementalValuesProvider<SourceGenContext> enumTypeOrException)
   {
      var exceptions = enumTypeOrException.SelectMany(static (state, _) => state.Exception is not null
                                                                              ? ImmutableArray.Create(state.Exception.Value)
                                                                              : ImmutableArray<SourceGenException>.Empty);
      context.RegisterSourceOutput(exceptions, ReportException);
   }

   private ImmutableArray<IKeyedSerializerCodeGeneratorFactory> GetSerializerCodeGeneratorFactories(MetadataReference reference)
   {
      var factories = ImmutableArray<IKeyedSerializerCodeGeneratorFactory>.Empty;

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
         Logger.LogError("Error during checking referenced modules", ex);
      }

      return factories;
   }

   private bool IsCandidate(SyntaxNode syntaxNode, CancellationToken cancellationToken)
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
         Logger.LogError("Error during checking whether a syntax node is a smart enum candidate", ex);
         return false;
      }
   }

   private bool IsEnumCandidate(TypeDeclarationSyntax typeDeclaration)
   {
      var isCandidate = typeDeclaration.IsPartial()
                        && !typeDeclaration.IsGeneric()
                        && typeDeclaration.IsEnumCandidate();

      if (isCandidate)
      {
         Logger.LogDebug("The type declaration is a smart enum candidate", typeDeclaration);
      }
      else
      {
         Logger.LogTrace("The type declaration is not a smart enum candidate", typeDeclaration);
      }

      return isCandidate;
   }

   private SourceGenContext? GetSourceGenContext(GeneratorSyntaxContext context, CancellationToken cancellationToken)
   {
      var tds = (TypeDeclarationSyntax)context.Node;

      try
      {
         var type = context.SemanticModel.GetDeclaredSymbol(tds, cancellationToken);

         if (type is null || type.TypeKind == TypeKind.Error || type.ContainingType is not null)
            return null;

         if (!type.IsEnum(out var enumInterfaces))
            return null;

         var enumInterface = enumInterfaces.GetValidEnumInterface(type);

         if (enumInterface is null || enumInterface.TypeKind == TypeKind.Error)
            return null;

         var keyMemberType = enumInterface.TypeArguments[0];

         if (keyMemberType.TypeKind == TypeKind.Error)
            return null;

         if (keyMemberType.NullableAnnotation == NullableAnnotation.Annotated)
            return null;

         var factory = TypedMemberStateFactoryProvider.GetFactoryOrNull(context.SemanticModel.Compilation);

         if (factory is null)
            return new SourceGenContext(new SourceGenError("Could not fetch type information for code generation of a smart enum", tds));

         var settings = new EnumSettings(type.FindEnumGenerationAttribute());
         var keyTypedMemberState = factory.Create(keyMemberType);
         var keyProperty = settings.CreateKeyProperty(keyTypedMemberState);
         var isValidatable = enumInterface.IsValidatableEnumInterface();
         var hasCreateInvalidItemImplementation = isValidatable && type.HasCreateInvalidItemImplementation(keyMemberType, cancellationToken);

         var attributeInfo = new AttributeInfo(type);

         var enumState = new EnumSourceGeneratorState(factory, type, keyProperty, settings.SkipToString, isValidatable, hasCreateInvalidItemImplementation, attributeInfo.HasStructLayoutAttribute, cancellationToken);
         var derivedTypes = new SmartEnumDerivedTypes(enumState.Namespace, enumState.Name, enumState.TypeFullyQualified, enumState.IsReferenceType, FindDerivedTypes(type));

         return new SourceGenContext(new ValidSourceGenState(enumState, derivedTypes, settings, keyProperty, attributeInfo));
      }
      catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
      {
         throw;
      }
      catch (Exception ex)
      {
         Logger.LogError("Error during extraction of relevant information out of semantic model for generation of a smart enum", ex);

         return new SourceGenContext(new SourceGenException(ex, tds));
      }
   }

   private static IReadOnlyList<string> FindDerivedTypes(INamedTypeSymbol type)
   {
      var derivedTypes = type.FindDerivedInnerEnums();

      if (derivedTypes.Count == 0)
         return Array.Empty<string>();

      return derivedTypes
             .Select(t => t.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat))
             .Distinct()
             .ToList();
   }

   private record struct ValidSourceGenState(EnumSourceGeneratorState State,
                                             SmartEnumDerivedTypes DerivedTypes,
                                             EnumSettings Settings,
                                             IMemberState KeyMember,
                                             AttributeInfo AttributeInfo);

   private record struct SourceGenContext(ValidSourceGenState? ValidState, SourceGenException? Exception, SourceGenError? Error)
   {
      public SourceGenContext(ValidSourceGenState validState)
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
