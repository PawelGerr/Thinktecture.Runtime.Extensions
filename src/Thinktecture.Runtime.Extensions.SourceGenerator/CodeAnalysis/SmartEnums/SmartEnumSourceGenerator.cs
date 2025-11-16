using Microsoft.CodeAnalysis.CSharp.Syntax;
using Thinktecture.Logging;

namespace Thinktecture.CodeAnalysis.SmartEnums;

[Generator]
public sealed class SmartEnumSourceGenerator()
   : ThinktectureSourceGeneratorBase(30_000), IIncrementalGenerator
{
   public void Initialize(IncrementalGeneratorInitializationContext context)
   {
      var options = GetGeneratorOptions(context);

      SetupLogger(context, options);

      InitializeKeyedSmartEnum(context, options);
      InitializeKeylessSmartEnum(context, options);
   }

   private void InitializeKeyedSmartEnum(IncrementalGeneratorInitializationContext context, IncrementalValueProvider<GeneratorOptions> options)
   {
      var validStates = InitializeSmartEnum(context, options, Constants.Attributes.SmartEnum.KEYED_FULL_NAME, true, IsSmartEnumCandidate);

      InitializeSerializerGenerators(context, validStates, options);
      InitializeFormattableCodeGenerator(context, validStates, options);
      InitializeComparableCodeGenerator(context, validStates, options);
      InitializeParsableCodeGenerator(context, validStates, options);
      InitializeComparisonOperatorsCodeGenerator(context, validStates, options);
   }

   private void InitializeKeylessSmartEnum(IncrementalGeneratorInitializationContext context, IncrementalValueProvider<GeneratorOptions> options)
   {
      InitializeSmartEnum(context, options, Constants.Attributes.SmartEnum.KEYLESS_FULL_NAME, false, IsSmartEnumCandidate);
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
                                                                       ? [state.Value]
                                                                       : ImmutableArray<SourceGenContext>.Empty);

      var validStates = enumTypeOrError.SelectMany(static (state, _) => state.ValidState is not null
                                                                           ? [state.ValidState.Value]
                                                                           : ImmutableArray<ValidSourceGenState>.Empty);

      InitializeEnumTypeGeneration(context, validStates, options);
      InitializeEqualityComparisonOperatorsCodeGenerator(context, validStates, options);

      InitializeExceptionReporting(context, enumTypeOrError);
      InitializeDiagnosticReporting(context, enumTypeOrError);

      return validStates;
   }

   private void InitializeComparisonOperatorsCodeGenerator(IncrementalGeneratorInitializationContext context, IncrementalValuesProvider<ValidSourceGenState> validStates, IncrementalValueProvider<GeneratorOptions> options)
   {
      var comparables = validStates
         .SelectMany((state, _) =>
         {
            if (state.KeyMember is null)
               return ImmutableArray<ComparisonOperatorsGeneratorState>.Empty;

            return
            [
               new ComparisonOperatorsGeneratorState(
                  state.State,
                  state.KeyMember,
                  Constants.Methods.GET,
                  state.Settings.ComparisonOperators,
                  state.KeyMember.ComparisonOperators,
                  state.AttributeInfo.KeyMemberComparerAccessor,
                  state.State.GenericParameters)
            ];
         });

      InitializeComparisonOperatorsCodeGenerator(context, comparables, options);
   }

   private void InitializeEqualityComparisonOperatorsCodeGenerator(IncrementalGeneratorInitializationContext context, IncrementalValuesProvider<ValidSourceGenState> validStates, IncrementalValueProvider<GeneratorOptions> options)
   {
      var comparables = validStates
         .Select((state, _) => new EqualityComparisonOperatorsGeneratorState(
                    state.State,
                    state.KeyMember,
                    state.Settings.EqualityComparisonOperators,
                    state.AttributeInfo.KeyMemberEqualityComparerAccessor is null ? null : new ComparerInfo(state.AttributeInfo.KeyMemberEqualityComparerAccessor, true),
                    state.State.GenericParameters));

      InitializeEqualityComparisonOperatorsCodeGenerator(context, comparables, options);
   }

   private void InitializeParsableCodeGenerator(IncrementalGeneratorInitializationContext context, IncrementalValuesProvider<ValidSourceGenState> validStates, IncrementalValueProvider<GeneratorOptions> options)
   {
      var parsables = validStates
         .SelectMany((state, _) =>
         {
            if (state.KeyMember is null)
               return ImmutableArray<ParsableGeneratorState>.Empty;

            return
            [
               new ParsableGeneratorState(
                  state.State,
                  state.KeyMember,
                  state.State.ValidationError,
                  state.Settings.SkipIParsable,
                  state.KeyMember.IsParsable && !state.AttributeInfo.ObjectFactories.Any(t => t.SpecialType == SpecialType.System_String),
                  true,
                  false,
                  state.State.GenericParameters)
            ];
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

            return
            [
               new ComparableGeneratorState(
                  state.State,
                  state.KeyMember,
                  Constants.Methods.GET,
                  state.Settings.SkipIComparable,
                  state.KeyMember.IsComparable,
                  state.AttributeInfo.KeyMemberComparerAccessor,
                  state.State.GenericParameters)
            ];
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

            return
            [
               new FormattableGeneratorState(
                  state.State,
                  state.KeyMember,
                  Constants.Methods.GET,
                  state.Settings.SkipIFormattable,
                  state.KeyMember.IsFormattable,
                  state.State.GenericParameters)
            ];
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
                                                       ? ImmutableArray<SmartEnumSourceGeneratorState>.Empty
                                                       : states.Distinct(TypeOnlyComparer.Instance))
                      .WithComparer(SetComparer<SmartEnumSourceGeneratorState>.Instance)
                      .SelectMany((states, _) => states);

      context.RegisterSourceOutput(enumTypes.Combine(options), (ctx, tuple) => GenerateCode(ctx, tuple.Left, tuple.Right, SmartEnumCodeGeneratorFactory.Instance));
   }

   private void InitializeSerializerGenerators(IncrementalGeneratorInitializationContext context, IncrementalValuesProvider<ValidSourceGenState> validStates, IncrementalValueProvider<GeneratorOptions> options)
   {
      var factoriesOrError = context.MetadataReferencesProvider
                                    .Select((reference, _) => GetSerializerCodeGeneratorFactories(reference));

      var serializerGeneratorFactories = factoriesOrError
                                         .Where(f => f.Factories is not null)
                                         .SelectMany<SerializerCodeGeneratorFactoriesContext, IKeyedSerializerCodeGeneratorFactory>((f, _) => f.Factories!)
                                         .Collect()
                                         .Select(static (states, _) => states.IsDefaultOrEmpty
                                                                          ? ImmutableArray<IKeyedSerializerCodeGeneratorFactory>.Empty
                                                                          : states.Distinct())
                                         .WithComparer(SetComparer<IKeyedSerializerCodeGeneratorFactory>.Instance);

      var serializerGeneratorStates = validStates.Select((state, _) => new KeyedSerializerGeneratorState(
                                                            state.State,
                                                            state.KeyMember,
                                                            state.AttributeInfo,
                                                            state.Settings.SerializationFrameworks,
                                                            state.State.GenericParameters))
                                                 .Combine(serializerGeneratorFactories)
                                                 .SelectMany((tuple, _) => ImmutableArray.CreateRange(tuple.Right, (factory, state) => (State: state, Factory: factory), tuple.Left))
                                                 .Where(tuple => tuple.Factory.MustGenerateCode(tuple.State));

      context.RegisterImplementationSourceOutput(serializerGeneratorStates.Combine(options), (ctx, tuple) => GenerateCode(ctx, tuple.Left.State, tuple.Right, tuple.Left.Factory));
      context.RegisterImplementationSourceOutput(factoriesOrError.Where(f => f.Exception is not null), (ctx, factoryOrError) =>
      {
         var exception = factoryOrError.Exception!;
         ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticsDescriptors.ErrorDuringModulesAnalysis,
                                                Location.None,
                                                exception.ToString()));
      });
   }

   private void InitializeExceptionReporting(IncrementalGeneratorInitializationContext context, IncrementalValuesProvider<SourceGenContext> enumTypeOrException)
   {
      var exceptions = enumTypeOrException.SelectMany(static (state, _) => state.Exception is not null
                                                                              ? [state.Exception.Value]
                                                                              : ImmutableArray<SourceGenException>.Empty);
      context.RegisterSourceOutput(exceptions, ReportException);
   }

   private void InitializeDiagnosticReporting(IncrementalGeneratorInitializationContext context, IncrementalValuesProvider<SourceGenContext> enumTypeOrException)
   {
      var exceptions = enumTypeOrException.SelectMany(static (state, _) => state.Diagnostic is not null
                                                                              ? [state.Diagnostic.Value]
                                                                              : ImmutableArray<SourceGenDiagnostic>.Empty);

      context.RegisterSourceOutput(exceptions, ReportDiagnostic);
   }

   private SerializerCodeGeneratorFactoriesContext GetSerializerCodeGeneratorFactories(MetadataReference reference)
   {
      try
      {
         var factories = new List<IKeyedSerializerCodeGeneratorFactory>(3);

         foreach (var module in reference.GetModules())
         {
            switch (module.Name)
            {
               case Constants.Modules.THINKTECTURE_RUNTIME_EXTENSIONS_JSON:
                  Logger.Log(LogLevel.Information, "Code generator for System.Text.Json will participate in code generation");
                  factories.Add(JsonSmartEnumCodeGeneratorFactory.Instance);
                  break;
               case Constants.Modules.THINKTECTURE_RUNTIME_EXTENSIONS_NEWTONSOFT_JSON:
                  Logger.Log(LogLevel.Information, "Code generator for Newtonsoft.Json will participate in code generation");
                  factories.Add(NewtonsoftJsonSmartEnumCodeGeneratorFactory.Instance);
                  break;
               case Constants.Modules.THINKTECTURE_RUNTIME_EXTENSIONS_MESSAGEPACK:
                  Logger.Log(LogLevel.Information, "Code generator for MessagePack will participate in code generation");
                  factories.Add(MessagePackSmartEnumCodeGeneratorFactory.Instance);
                  break;
            }
         }

         return new(factories);
      }
      catch (Exception ex)
      {
         return new(ex);
      }
   }

   private static bool IsSmartEnumCandidate(SyntaxNode syntaxNode, CancellationToken cancellationToken)
   {
      return syntaxNode is ClassDeclarationSyntax;
   }

   private static SourceGenContext? GetSourceGenContextOrNull(GeneratorAttributeSyntaxContext context, bool isKeyed, CancellationToken cancellationToken)
   {
      var tds = (TypeDeclarationSyntax)context.TargetNode;

      try
      {
         var type = (INamedTypeSymbol)context.TargetSymbol;

         if (type.TypeKind == TypeKind.Error)
            return null;

         if (context.Attributes.IsDefaultOrEmpty)
            return null;

         if (context.Attributes.Length > 1)
            return new SourceGenDiagnostic(tds, DiagnosticsDescriptors.TypeMustNotHaveMoveThanOneSmartEnumAttribute, [type.ToMinimallyQualifiedDisplayString()]);

         if (type.IsNestedInGenericClass())
            return null; // Analyzer emits DiagnosticsDescriptors.TypeMustNotBeInsideGenericType

         ITypeSymbol? keyMemberType = null;

         if (isKeyed)
         {
            var attributeType = context.Attributes[0].AttributeClass;

            if (attributeType is null)
               return new SourceGenDiagnostic(tds, DiagnosticsDescriptors.ErrorDuringCodeAnalysis, [type.ToMinimallyQualifiedDisplayString(), "Could not resolve Smart enum attribute type"]);

            if (attributeType.TypeKind == TypeKind.Error)
               return new SourceGenDiagnostic(tds, DiagnosticsDescriptors.ErrorDuringCodeAnalysis, [type.ToMinimallyQualifiedDisplayString(), "SmartEnumAttribute type has TypeKind=Error"]);

            if (attributeType.Arity != 1)
               return new SourceGenDiagnostic(tds, DiagnosticsDescriptors.ErrorDuringCodeAnalysis, [type.ToMinimallyQualifiedDisplayString(), "SmartEnumAttribute must have exactly one type argument"]);

            keyMemberType = attributeType.TypeArguments[0];

            if (keyMemberType.TypeKind == TypeKind.Error)
               return new SourceGenDiagnostic(tds, DiagnosticsDescriptors.ErrorDuringCodeAnalysis, [type.ToMinimallyQualifiedDisplayString(), "Smart enum key member type could not be resolved"]);

            if (keyMemberType.NullableAnnotation == NullableAnnotation.Annotated || keyMemberType.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T)
               return null; // Analyzer emits DiagnosticsDescriptors.SmartEnumKeyShouldNotBeNullable
         }

         var factory = TypedMemberStateFactoryProvider.GetFactoryOrNull(context.SemanticModel.Compilation);

         if (factory is null)
            return new SourceGenDiagnostic(tds, DiagnosticsDescriptors.ErrorDuringCodeAnalysis, [type.ToMinimallyQualifiedDisplayString(), "Could not fetch type information for code generation of a smart enum"]);

         var diagnostic = AttributeInfo.TryCreate(type, out var attributeInfo);

         if (diagnostic is not null)
            return new SourceGenDiagnostic(tds, diagnostic.Value.Descriptor, diagnostic.Value.Args);

         var settings = new AllEnumSettings(context.Attributes[0]);
         KeyMemberState? keyMember = null;

         if (keyMemberType is not null)
         {
            var keyTypedMemberState = factory.Create(keyMemberType);
            keyMember = settings.CreateKeyMember(keyTypedMemberState);
         }

         var enumState = new SmartEnumSourceGeneratorState(factory,
                                                           type,
                                                           keyMember,
                                                           attributeInfo.ValidationError,
                                                           new SmartEnumSettings(settings, attributeInfo),
                                                           type.FindDerivedInnerTypes().Count > 0,
                                                           type.GetGenericTypeParameters(),
                                                           cancellationToken);

         return new ValidSourceGenState(enumState, settings, keyMember, attributeInfo);
      }
      catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
      {
         throw;
      }
      catch (Exception ex)
      {
         return new SourceGenException("Error during extraction of relevant information out of semantic model for generation of a smart enum", ex, tds);
      }
   }

   private readonly record struct ValidSourceGenState(
      SmartEnumSourceGeneratorState State,
      AllEnumSettings Settings,
      KeyMemberState? KeyMember,
      AttributeInfo AttributeInfo);

   private readonly record struct SourceGenContext(
      ValidSourceGenState? ValidState,
      SourceGenException? Exception,
      SourceGenDiagnostic? Diagnostic)
   {
      public static implicit operator SourceGenContext(ValidSourceGenState state)
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

   private readonly record struct SerializerCodeGeneratorFactoriesContext(
      IReadOnlyList<IKeyedSerializerCodeGeneratorFactory>? Factories,
      Exception? Exception = null)
   {
      public SerializerCodeGeneratorFactoriesContext(IReadOnlyList<IKeyedSerializerCodeGeneratorFactory> factories)
         : this(factories, null)
      {
      }

      public SerializerCodeGeneratorFactoriesContext(Exception exception)
         : this(null, exception)
      {
      }
   }
}
