using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Thinktecture.CodeAnalysis.SmartEnums;

[Generator]
public sealed class SmartEnumSourceGenerator : ThinktectureSourceGeneratorBase, IIncrementalGenerator
{
   public SmartEnumSourceGenerator()
      : base(25_000)
   {
   }

   public void Initialize(IncrementalGeneratorInitializationContext context)
   {
      var options = GetGeneratorOptions(context);

      SetupLogger(context, options);

      InitializeKeyedSmartEnum(context, options);
      InitializeKeylessSmartEnum(context, options);
   }

   private void InitializeKeyedSmartEnum(IncrementalGeneratorInitializationContext context, IncrementalValueProvider<GeneratorOptions> options)
   {
      var validStates = InitializeSmartEnum(context, options, Constants.Attributes.SmartEnum.KEYED_FULL_NAME, true, IsKeyedCandidate);

      InitializeSerializerGenerators(context, validStates, options);
      InitializeDerivedTypesGeneration(context, validStates, options);
      InitializeFormattableCodeGenerator(context, validStates, options);
      InitializeComparableCodeGenerator(context, validStates, options);
      InitializeParsableCodeGenerator(context, validStates, options);
      InitializeComparisonOperatorsCodeGenerator(context, validStates, options);
   }

   private void InitializeKeylessSmartEnum(IncrementalGeneratorInitializationContext context, IncrementalValueProvider<GeneratorOptions> options)
   {
      InitializeSmartEnum(context, options, Constants.Attributes.SmartEnum.KEYLESS_FULL_NAME, false, IsKeylessCandidate);
   }

   private IncrementalValuesProvider<ValidSourceGenState> InitializeSmartEnum(
      IncrementalGeneratorInitializationContext context,
      IncrementalValueProvider<GeneratorOptions> options,
      string fullyQualifiedMetadataName,
      bool isKeyed,
      Func<SyntaxNode, CancellationToken, bool> predicate)
   {
      var enumTypeOrError = context.SyntaxProvider
                                   .ForAttributeWithMetadataName(fullyQualifiedMetadataName,
                                                                 predicate,
                                                                 (ctx, token) => GetSourceGenContextOrNull(ctx, isKeyed, token))
                                   .SelectMany(static (state, _) => state.HasValue
                                                                       ? ImmutableArray.Create(state.Value)
                                                                       : ImmutableArray<SourceGenContext>.Empty);

      var validStates = enumTypeOrError.SelectMany(static (state, _) => state.ValidState is not null
                                                                           ? ImmutableArray.Create(state.ValidState.Value)
                                                                           : ImmutableArray<ValidSourceGenState>.Empty);

      InitializeEnumTypeGeneration(context, validStates, options);
      InitializeEqualityComparisonOperatorsCodeGenerator(context, validStates, options);

      InitializeErrorReporting(context, enumTypeOrError);
      InitializeExceptionReporting(context, enumTypeOrError);

      return validStates;
   }

   private void InitializeComparisonOperatorsCodeGenerator(IncrementalGeneratorInitializationContext context, IncrementalValuesProvider<ValidSourceGenState> validStates, IncrementalValueProvider<GeneratorOptions> options)
   {
      var comparables = validStates
         .SelectMany((state, _) =>
                     {
                        if (state.KeyMember is null)
                           return ImmutableArray<ComparisonOperatorsGeneratorState>.Empty;

                        return ImmutableArray.Create(new ComparisonOperatorsGeneratorState(state.State,
                                                                                           state.KeyMember,
                                                                                           Constants.Methods.GET,
                                                                                           state.Settings.ComparisonOperators,
                                                                                           state.KeyMember.ComparisonOperators,
                                                                                           state.AttributeInfo.KeyMemberComparerAccessor));
                     });

      InitializeComparisonOperatorsCodeGenerator(context, comparables, options);
   }

   private void InitializeEqualityComparisonOperatorsCodeGenerator(IncrementalGeneratorInitializationContext context, IncrementalValuesProvider<ValidSourceGenState> validStates, IncrementalValueProvider<GeneratorOptions> options)
   {
      var comparables = validStates
         .Select((state, _) => new EqualityComparisonOperatorsGeneratorState(state.State,
                                                                             state.KeyMember,
                                                                             state.Settings.EqualityComparisonOperators,
                                                                             state.AttributeInfo.KeyMemberEqualityComparerAccessor is null ? null : new ComparerInfo(state.AttributeInfo.KeyMemberEqualityComparerAccessor, true)));

      InitializeEqualityComparisonOperatorsCodeGenerator(context, comparables, options);
   }

   private void InitializeParsableCodeGenerator(IncrementalGeneratorInitializationContext context, IncrementalValuesProvider<ValidSourceGenState> validStates, IncrementalValueProvider<GeneratorOptions> options)
   {
      var parsables = validStates
         .SelectMany((state, _) =>
                     {
                        if (state.KeyMember is null)
                           return ImmutableArray<ParsableGeneratorState>.Empty;

                        return ImmutableArray.Create(new ParsableGeneratorState(state.State,
                                                                                state.KeyMember,
                                                                                state.State.ValidationError,
                                                                                state.Settings.SkipIParsable,
                                                                                state.KeyMember.IsParsable,
                                                                                true,
                                                                                state.State.Settings.IsValidatable,
                                                                                state.AttributeInfo.DesiredFactories.Any(t => t.SpecialType == SpecialType.System_String)));
                     });
      InitializeParsableCodeGenerator(context, parsables, options);
   }

   private void InitializeComparableCodeGenerator(IncrementalGeneratorInitializationContext context, IncrementalValuesProvider<ValidSourceGenState> validStates, IncrementalValueProvider<GeneratorOptions> options)
   {
      var comparables = validStates
         .SelectMany((state, _) =>
                     {
                        if (state.KeyMember is null)
                           return ImmutableArray<ComparableGeneratorState>.Empty;

                        return ImmutableArray.Create(new ComparableGeneratorState(state.State,
                                                                                  state.KeyMember,
                                                                                  Constants.Methods.GET,
                                                                                  state.Settings.SkipIComparable,
                                                                                  state.KeyMember.IsComparable,
                                                                                  state.AttributeInfo.KeyMemberComparerAccessor));
                     });

      InitializeComparableCodeGenerator(context, comparables, options);
   }

   private void InitializeFormattableCodeGenerator(IncrementalGeneratorInitializationContext context, IncrementalValuesProvider<ValidSourceGenState> validStates, IncrementalValueProvider<GeneratorOptions> options)
   {
      var formattables = validStates
         .SelectMany((state, _) =>
                     {
                        if (state.KeyMember is null)
                           return ImmutableArray<FormattableGeneratorState>.Empty;

                        return ImmutableArray.Create(new FormattableGeneratorState(state.State,
                                                                                   state.KeyMember,
                                                                                   Constants.Methods.GET,
                                                                                   state.Settings.SkipIFormattable,
                                                                                   state.KeyMember.IsFormattable));
                     });

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
                                                       : states.Distinct(TypeOnlyComparer.Instance))
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
                                                                                 : states.Distinct())
                                                .WithComparer(new SetComparer<IKeyedSerializerCodeGeneratorFactory>());

      var serializerGeneratorStates = validStates.Select((state, _) => new KeyedSerializerGeneratorState(state.State, state.KeyMember, state.AttributeInfo))
                                                 .Combine(serializerGeneratorFactories)
                                                 .SelectMany((tuple, _) => ImmutableArray.CreateRange(tuple.Right, (factory, state) => (State: state, Factory: factory), tuple.Left))
                                                 .Where(tuple =>
                                                        {
                                                           if (tuple.Factory.MustGenerateCode(tuple.State))
                                                           {
                                                              Logger.LogDebug("Code generator must generate code.", null, tuple.State, factory: tuple.Factory);
                                                              return true;
                                                           }

                                                           Logger.LogInformation("Code generator must not generate code.", null, tuple.State, factory: tuple.Factory);
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
                                                          : states.Distinct(TypeOnlyComparer.Instance))
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

   private bool IsKeyedCandidate(SyntaxNode syntaxNode, CancellationToken cancellationToken)
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

   private bool IsKeylessCandidate(SyntaxNode syntaxNode, CancellationToken cancellationToken)
   {
      try
      {
         return syntaxNode switch
         {
            ClassDeclarationSyntax classDeclaration when IsEnumCandidate(classDeclaration) => true,
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
      var isCandidate = !typeDeclaration.IsGeneric();

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

   private SourceGenContext? GetSourceGenContextOrNull(GeneratorAttributeSyntaxContext context, bool isKeyed, CancellationToken cancellationToken)
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
            Logger.LogDebug($"Type has more than 1 '{Constants.Attributes.SmartEnum.NAME}'", tds);
            return null;
         }

         ITypeSymbol? keyMemberType = null;

         if (isKeyed)
         {
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

            if (attributetype.TypeArguments.Length != 1)
            {
               Logger.LogDebug($"Expected the attribute type to have 1 type argument but found {attributetype.TypeArguments.Length.ToString()}", tds);
               return null;
            }

            keyMemberType = attributetype.TypeArguments[0];

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
         }

         var factory = TypedMemberStateFactoryProvider.GetFactoryOrNull(context.SemanticModel.Compilation, Logger);

         if (factory is null)
            return new SourceGenContext(new SourceGenError("Could not fetch type information for code generation of a smart enum", tds));

         var errorMessage = AttributeInfo.TryCreate(type, out var attributeInfo);

         if (errorMessage is not null)
         {
            Logger.LogDebug(errorMessage, tds);
            return null;
         }

         var settings = new AllEnumSettings(context.Attributes[0]);
         KeyMemberState? keyMember = null;

         if (keyMemberType is not null)
         {
            var keyTypedMemberState = factory.Create(keyMemberType);
            keyMember = settings.CreateKeyMember(keyTypedMemberState);
         }

         var nonIgnoredMembers = type.GetNonIgnoredMembers();
         var hasCreateInvalidItemImplementation = keyMemberType is not null && settings.IsValidatable && type.HasCreateInvalidItemImplementation(nonIgnoredMembers, keyMemberType, cancellationToken);

         var derivedTypeNames = FindDerivedTypes(type);
         var enumState = new EnumSourceGeneratorState(factory,
                                                      type,
                                                      keyMember,
                                                      attributeInfo.ValidationError,
                                                      nonIgnoredMembers,
                                                      new EnumSettings(settings, attributeInfo),
                                                      hasCreateInvalidItemImplementation,
                                                      derivedTypeNames.Count > 0,
                                                      cancellationToken);
         var derivedTypes = new SmartEnumDerivedTypes(enumState.Namespace,
                                                      enumState.Name,
                                                      enumState.TypeFullyQualified,
                                                      enumState.IsReferenceType,
                                                      enumState.ContainingTypes,
                                                      derivedTypeNames);

         Logger.LogDebug("The type declaration is a valid smart enum", null, enumState);

         return new SourceGenContext(new ValidSourceGenState(enumState, derivedTypes, settings, keyMember, attributeInfo));
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
      var derivedTypes = type.FindDerivedInnerTypes();

      if (derivedTypes.Count == 0)
         return Array.Empty<string>();

      return derivedTypes
             .Select(t => t.Type.ToFullyQualifiedDisplayString())
             .Distinct()
             .ToList();
   }

   private readonly record struct ValidSourceGenState(
      EnumSourceGeneratorState State,
      SmartEnumDerivedTypes DerivedTypes,
      AllEnumSettings Settings,
      KeyMemberState? KeyMember,
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
