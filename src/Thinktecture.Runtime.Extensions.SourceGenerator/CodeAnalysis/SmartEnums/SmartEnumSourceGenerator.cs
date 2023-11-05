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
      var options = GetGeneratorOptions(context);

      SetupLogger(context, options);

      var enumTypeOrError = context.SyntaxProvider
                                   .CreateSyntaxProvider(IsCandidate, GetSourceGenContextOrNull)
                                   .SelectMany(static (state, _) => state.HasValue
                                                                       ? ImmutableArray.Create(state.Value)
                                                                       : ImmutableArray<SourceGenContext>.Empty);

      var validStates = enumTypeOrError.SelectMany(static (state, _) => state.ValidState is not null
                                                                           ? ImmutableArray.Create(state.ValidState.Value)
                                                                           : ImmutableArray<ValidSourceGenState>.Empty);

      InitializeEnumTypeGeneration(context, validStates, options);
      InitializeSerializerGenerators(context, validStates, options);
      InitializeDerivedTypesGeneration(context, validStates, options);
      InitializeFormattableCodeGenerator(context, validStates, options);
      InitializeComparableCodeGenerator(context, validStates, options);
      InitializeParsableCodeGenerator(context, validStates, options);
      InitializeComparisonOperatorsCodeGenerator(context, validStates, options);
      InitializeEqualityComparisonOperatorsCodeGenerator(context, validStates, options);

      InitializeErrorReporting(context, enumTypeOrError);
      InitializeExceptionReporting(context, enumTypeOrError);
   }

   private void InitializeComparisonOperatorsCodeGenerator(IncrementalGeneratorInitializationContext context, IncrementalValuesProvider<ValidSourceGenState> validStates, IncrementalValueProvider<GeneratorOptions> options)
   {
      var comparables = validStates
         .Select((state, _) => new ComparisonOperatorsGeneratorState(state.State,
                                                                     state.KeyMember,
                                                                     state.Settings.ComparisonOperators,
                                                                     state.KeyMember.ComparisonOperators,
                                                                     null));

      InitializeComparisonOperatorsCodeGenerator(context, comparables, options);
   }

   private void InitializeEqualityComparisonOperatorsCodeGenerator(IncrementalGeneratorInitializationContext context, IncrementalValuesProvider<ValidSourceGenState> validStates, IncrementalValueProvider<GeneratorOptions> options)
   {
      var comparables = validStates
         .Select((state, _) => new EqualityComparisonOperatorsGeneratorState(state.State,
                                                                             state.KeyMember,
                                                                             state.Settings.EqualityComparisonOperators,
                                                                             new ComparerInfo(Constants.KEY_EQUALITY_COMPARER_NAME, false)));

      InitializeEqualityComparisonOperatorsCodeGenerator(context, comparables, options);
   }

   private void InitializeParsableCodeGenerator(IncrementalGeneratorInitializationContext context, IncrementalValuesProvider<ValidSourceGenState> validStates, IncrementalValueProvider<GeneratorOptions> options)
   {
      var parsables = validStates
         .Select((state, _) => new ParsableGeneratorState(state.State,
                                                          state.KeyMember,
                                                          state.Settings.SkipIParsable,
                                                          state.KeyMember.IsParsable,
                                                          state.State.Settings.IsValidatable,
                                                          state.AttributeInfo.DesiredFactorySourceTypes.Any(t => t.SpecialType == SpecialType.System_String)));
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
                                                 .Where(tuple =>
                                                        {
                                                           if (tuple.Factory.MustGenerateCode(tuple.State.AttributeInfo))
                                                           {
                                                              Logger.LogDebug("Code generator must generate code.", namespaceAndName: tuple.State, factory: tuple.Factory);
                                                              return true;
                                                           }

                                                           Logger.LogInformation("Code generator must not generate code.", namespaceAndName: tuple.State, factory: tuple.Factory);
                                                           return false;
                                                        });

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
            switch (module.Name)
            {
               case Constants.Modules.THINKTECTURE_RUNTIME_EXTENSIONS_JSON:
                  Logger.LogInformation("Code generator for System.Text.Json will participate in code generation");
                  factories = factories.Add(JsonSmartEnumCodeGeneratorFactory.Instance);
                  break;
               case Constants.Modules.THINKTECTURE_RUNTIME_EXTENSIONS_NEWTONSOFT_JSON:
                  Logger.LogInformation("Code generator for Newtonsoft.Json will participate in code generation");
                  factories = factories.Add(NewtonsoftJsonSmartEnumCodeGeneratorFactory.Instance);
                  break;
               case Constants.Modules.THINKTECTURE_RUNTIME_EXTENSIONS_MESSAGEPACK:
                  Logger.LogInformation("Code generator for MessagePack will participate in code generation");
                  factories = factories.Add(MessagePackSmartEnumCodeGeneratorFactory.Instance);
                  break;
            }
         }
      }
      catch (Exception ex)
      {
         Logger.LogError("Error during checking referenced modules", exception: ex);
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
         Logger.LogError("Error during checking whether a syntax node is a smart enum candidate", exception: ex);
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

   private SourceGenContext? GetSourceGenContextOrNull(GeneratorSyntaxContext context, CancellationToken cancellationToken)
   {
      var tds = (TypeDeclarationSyntax)context.Node;

      try
      {
         var type = context.SemanticModel.GetDeclaredSymbol(tds, cancellationToken);

         if (type is null)
         {
            Logger.LogDebug("Type in semantic model not found", tds);
            return null;
         }

         if (type.TypeKind == TypeKind.Error)
         {
            Logger.LogDebug("Type from semantic model is erroneous", tds);
            return null;
         }

         if (type.ContainingType is not null)
         {
            Logger.LogDebug("Nested types are not supported", tds);
            return null;
         }

         if (!type.IsEnum(out var enumInterfaces))
         {
            Logger.LogDebug("Candidate isn't a Smart Enum", tds);
            return null;
         }

         var enumInterface = enumInterfaces.GetValidEnumInterface(type);

         if (enumInterface is null)
         {
            Logger.LogDebug("No valid Smart-Enum-interface found", tds);
            return null;
         }

         if (enumInterface.TypeKind == TypeKind.Error)
         {
            Logger.LogDebug("Type of the Smart-Enum-interface is erroneous", tds);
            return null;
         }

         var keyMemberType = enumInterface.TypeArguments[0];

         if (keyMemberType.TypeKind == TypeKind.Error)
         {
            Logger.LogDebug("Type of the key member is erroneous", tds);
            return null;
         }

         if (keyMemberType.NullableAnnotation == NullableAnnotation.Annotated)
         {
            Logger.LogDebug("Type of the key member must not be nullable", tds);
            return null;
         }

         var factory = TypedMemberStateFactoryProvider.GetFactoryOrNull(context.SemanticModel.Compilation, Logger);

         if (factory is null)
            return new SourceGenContext(new SourceGenError("Could not fetch type information for code generation of a smart enum", tds));

         var isValidatable = enumInterface.IsValidatableEnumInterface();
         var attributeInfo = new AttributeInfo(type);
         var settings = new AllEnumSettings(type.FindEnumGenerationAttribute(), isValidatable);
         var keyTypedMemberState = factory.Create(keyMemberType);
         var keyProperty = settings.CreateKeyProperty(keyTypedMemberState);
         var hasCreateInvalidItemImplementation = isValidatable && type.HasCreateInvalidItemImplementation(keyMemberType, cancellationToken);

         var enumState = new EnumSourceGeneratorState(factory, type, keyProperty, new EnumSettings(settings, attributeInfo), hasCreateInvalidItemImplementation, cancellationToken);
         var derivedTypes = new SmartEnumDerivedTypes(enumState.Namespace, enumState.Name, enumState.TypeFullyQualified, enumState.IsReferenceType, FindDerivedTypes(type));

         Logger.LogDebug("The type declaration is a valid smart enum", namespaceAndName: enumState);

         return new SourceGenContext(new ValidSourceGenState(enumState, derivedTypes, settings, keyProperty, attributeInfo));
      }
      catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
      {
         throw;
      }
      catch (Exception ex)
      {
         Logger.LogError("Error during extraction of relevant information out of semantic model for generation of a smart enum", tds, ex);

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

   private readonly record struct ValidSourceGenState(EnumSourceGeneratorState State,
                                                      SmartEnumDerivedTypes DerivedTypes,
                                                      AllEnumSettings Settings,
                                                      IMemberState KeyMember,
                                                      AttributeInfo AttributeInfo);

   private readonly record struct SourceGenContext(ValidSourceGenState? ValidState, SourceGenException? Exception, SourceGenError? Error)
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
