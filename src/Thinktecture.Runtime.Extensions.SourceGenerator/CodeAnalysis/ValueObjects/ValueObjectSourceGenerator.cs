using Microsoft.CodeAnalysis.CSharp.Syntax;
using Thinktecture.Logging;

namespace Thinktecture.CodeAnalysis.ValueObjects;

[Generator]
public sealed class ValueObjectSourceGenerator()
   : ThinktectureSourceGeneratorBase(18_000), IIncrementalGenerator
{
   public void Initialize(IncrementalGeneratorInitializationContext context)
   {
      var options = GetGeneratorOptions(context);

      SetupLogger(context, options);

      var serializerGeneratorFactories = GetSerializerGeneratorFactories(context);

      InitializeKeyedSourceGen(context, options, serializerGeneratorFactories);
      InitializeComplexCodeGen(context, options, serializerGeneratorFactories);
   }

   private IncrementalValueProvider<ImmutableArray<IValueObjectSerializerCodeGeneratorFactory>> GetSerializerGeneratorFactories(IncrementalGeneratorInitializationContext context)
   {
      var factoriesOrError = context.MetadataReferencesProvider
                                    .Select((reference, _) => GetSerializerCodeGeneratorFactories(reference));

      var serializerGeneratorFactories = factoriesOrError
                                         .Where(f => f.Factories is not null)
                                         .SelectMany<SerializerCodeGeneratorFactoriesContext, IValueObjectSerializerCodeGeneratorFactory>((f, _) => f.Factories!)
                                         .Collect()
                                         .Select(static (states, _) => states.IsDefaultOrEmpty
                                                                          ? ImmutableArray<IValueObjectSerializerCodeGeneratorFactory>.Empty
                                                                          : states.Distinct())
                                         .WithComparer(SetComparer<IValueObjectSerializerCodeGeneratorFactory>.Instance);

      context.RegisterImplementationSourceOutput(factoriesOrError.Where(f => f.Exception is not null), (ctx, factoryOrError) =>
      {
         var exception = factoryOrError.Exception!;
         ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticsDescriptors.ErrorDuringModulesAnalysis,
                                                Location.None,
                                                exception.ToString()));
      });

      return serializerGeneratorFactories;
   }

   private void InitializeKeyedSourceGen(
      IncrementalGeneratorInitializationContext context,
      IncrementalValueProvider<GeneratorOptions> options,
      IncrementalValueProvider<ImmutableArray<IValueObjectSerializerCodeGeneratorFactory>> serializerGeneratorFactories)
   {
      var states = InitializeSourceGen<KeyedValidSourceGenState, KeyedValueObjectSourceGeneratorState>(context,
                                                                                                       options,
                                                                                                       Constants.Attributes.ValueObject.KEYED_FULL_NAME,
                                                                                                       GetKeyedSourceGenContextOrNull,
                                                                                                       TypeOnlyComparer.Instance,
                                                                                                       ValueObjectCodeGeneratorFactory.Instance);

      InitializeSerializerGenerators(context, states, options, serializerGeneratorFactories);
      InitializeParsableCodeGenerator(context, states, options);
      InitializeSpanParsableCodeGenerator(context, states, options);

      InitializeFormattableCodeGenerator(context, states, options);
      InitializeComparableCodeGenerator(context, states, options);
      InitializeComparisonOperatorsCodeGenerator(context, states, options);
      InitializeEqualityComparisonOperatorsCodeGenerator(context, states, options);
      InitializeAdditionOperatorsCodeGenerator(context, states, options);
      InitializeSubtractionOperatorsCodeGenerator(context, states, options);
      InitializeMultiplyOperatorsCodeGenerator(context, states, options);
      InitializeDivisionOperatorsCodeGenerator(context, states, options);
   }

   private void InitializeComplexCodeGen(
      IncrementalGeneratorInitializationContext context,
      IncrementalValueProvider<GeneratorOptions> options,
      IncrementalValueProvider<ImmutableArray<IValueObjectSerializerCodeGeneratorFactory>> serializerGeneratorFactories)
   {
      var validComplexValueObjects = InitializeSourceGen<ComplexValidSourceGenState, ComplexValueObjectSourceGeneratorState>(context,
                                                                                                                             options,
                                                                                                                             Constants.Attributes.ValueObject.COMPLEX_FULL_NAME,
                                                                                                                             GetComplexSourceGenContextOrNull,
                                                                                                                             TypeOnlyComparer.Instance,
                                                                                                                             ValueObjectCodeGeneratorFactory.Instance);

      InitializeSerializerGenerators(context, validComplexValueObjects, options, serializerGeneratorFactories);
   }

   private IncrementalValuesProvider<TState> InitializeSourceGen<TState, TGenState>(
      IncrementalGeneratorInitializationContext context,
      IncrementalValueProvider<GeneratorOptions> options,
      string fullyQualifiedMetadataName,
      Func<GeneratorAttributeSyntaxContext, CancellationToken, SourceGenContext<TState>?> transform,
      IEqualityComparer<TGenState> typeOnlyComparer,
      ICodeGeneratorFactory<TGenState> codeGeneratorFactory)
      where TState : struct, ISourceGenState<TGenState>
      where TGenState : class, IEquatable<TGenState>, INamespaceAndName
   {
      var valueObjectOrException = context.SyntaxProvider
                                          .ForAttributeWithMetadataName(fullyQualifiedMetadataName,
                                                                        IsCandidate,
                                                                        transform)
                                          .SelectMany(static (state, _) => state.HasValue
                                                                              ? [state.Value]
                                                                              : ImmutableArray<SourceGenContext<TState>>.Empty);

      var validValueObjects = valueObjectOrException.SelectMany(static (state, _) => state.ValidState is not null
                                                                                        ? [state.ValidState.Value]
                                                                                        : ImmutableArray<TState>.Empty);

      var sourceGenStates = validValueObjects
                            .Select(static (state, _) => state.State)
                            .Collect()
                            .Select((states, _) => states.IsDefaultOrEmpty
                                                      ? ImmutableArray<TGenState>.Empty
                                                      : states.Distinct(typeOnlyComparer))
                            .WithComparer(SetComparer<TGenState>.Instance)
                            .SelectMany(static (states, _) => states);

      context.RegisterSourceOutput(sourceGenStates.Combine(options), (ctx, state) => GenerateCode(ctx, state.Left, state.Right, codeGeneratorFactory));

      InitializeDiagnosticReporting(context, valueObjectOrException);
      InitializeExceptionReporting(context, valueObjectOrException);

      return validValueObjects;
   }

   private void InitializeSerializerGenerators(
      IncrementalGeneratorInitializationContext context,
      IncrementalValuesProvider<KeyedValidSourceGenState> validStates,
      IncrementalValueProvider<GeneratorOptions> options,
      IncrementalValueProvider<ImmutableArray<IValueObjectSerializerCodeGeneratorFactory>> serializerGeneratorFactories)
   {
      validStates = validStates.Where(state => !state.Settings.SkipFactoryMethods || state.AttributeInfo.ObjectFactories.Any(f => f.UseForSerialization != SerializationFrameworks.None));

      var keyedSerializerGeneratorStates = validStates.SelectMany((state, _) =>
                                                      {
                                                         var serializerState = new KeyedSerializerGeneratorState(
                                                            state.State,
                                                            state.State.KeyMember,
                                                            state.AttributeInfo,
                                                            state.Settings.SerializationFrameworks,
                                                            state.State.GenericParameters);

                                                         return ImmutableArray.Create(serializerState);
                                                      })
                                                      .Combine(serializerGeneratorFactories)
                                                      .SelectMany((tuple, _) => ImmutableArray.CreateRange(tuple.Right, (factory, state) => (State: state, Factory: factory), tuple.Left))
                                                      .Where(tuple => tuple.Factory.MustGenerateCode(tuple.State));

      context.RegisterImplementationSourceOutput(keyedSerializerGeneratorStates.Combine(options), (ctx, tuple) => GenerateCode(ctx, tuple.Left.State, tuple.Right, tuple.Left.Factory));
   }

   private void InitializeSerializerGenerators(
      IncrementalGeneratorInitializationContext context,
      IncrementalValuesProvider<ComplexValidSourceGenState> validStates,
      IncrementalValueProvider<GeneratorOptions> options,
      IncrementalValueProvider<ImmutableArray<IValueObjectSerializerCodeGeneratorFactory>> serializerGeneratorFactories)
   {
      validStates = validStates.Where(state => !state.Settings.SkipFactoryMethods || state.AttributeInfo.ObjectFactories.Any(f => f.UseForSerialization != SerializationFrameworks.None));

      var keyedSerializerGeneratorStates = validStates.SelectMany((state, _) =>
                                                      {
                                                         if (state.AttributeInfo.ObjectFactories.All(f => f.UseForSerialization == SerializationFrameworks.None))
                                                            return ImmutableArray<KeyedSerializerGeneratorState>.Empty;

                                                         var serializerState = new KeyedSerializerGeneratorState(
                                                            state.State,
                                                            null,
                                                            state.AttributeInfo,
                                                            state.Settings.SerializationFrameworks,
                                                            state.State.GenericParameters);

                                                         return [serializerState];
                                                      })
                                                      .Combine(serializerGeneratorFactories)
                                                      .SelectMany((tuple, _) => ImmutableArray.CreateRange(tuple.Right, (factory, state) => (State: state, Factory: factory), tuple.Left))
                                                      .Where(tuple => tuple.Factory.MustGenerateCode(tuple.State));

      var complexSerializerGeneratorStates = validStates.SelectMany((state, _) =>
                                                        {
                                                           var serializerState = new ComplexSerializerGeneratorState<ComplexValueObjectSourceGeneratorState>(
                                                              state.State,
                                                              state.State.AssignableInstanceFieldsAndProperties,
                                                              state.AttributeInfo,
                                                              state.Settings.SerializationFrameworks);

                                                           return ImmutableArray.Create(serializerState);
                                                        })
                                                        .Combine(serializerGeneratorFactories)
                                                        .SelectMany((tuple, _) => ImmutableArray.CreateRange(tuple.Right, (factory, state) => (State: state, Factory: factory), tuple.Left))
                                                        .Where(tuple => tuple.Factory.MustGenerateCode(tuple.State));

      context.RegisterImplementationSourceOutput(keyedSerializerGeneratorStates.Combine(options), (ctx, tuple) => GenerateCode(ctx, tuple.Left.State, tuple.Right, tuple.Left.Factory));
      context.RegisterImplementationSourceOutput(complexSerializerGeneratorStates.Combine(options), (ctx, tuple) => GenerateCode(ctx, tuple.Left.State, tuple.Right, tuple.Left.Factory));
   }

   private void InitializeFormattableCodeGenerator(IncrementalGeneratorInitializationContext context, IncrementalValuesProvider<KeyedValidSourceGenState> validStates, IncrementalValueProvider<GeneratorOptions> options)
   {
      var formattables = validStates
         .Select((state, _) => new FormattableGeneratorState(
                    state.State,
                    state.State.KeyMember,
                    state.Settings.CreateFactoryMethodName,
                    state.Settings.SkipIFormattable,
                    state.State.KeyMember.IsFormattable,
                    state.State.GenericParameters));

      InitializeFormattableCodeGenerator(context, formattables, options);
   }

   private void InitializeComparableCodeGenerator(IncrementalGeneratorInitializationContext context, IncrementalValuesProvider<KeyedValidSourceGenState> validStates, IncrementalValueProvider<GeneratorOptions> options)
   {
      var comparables = validStates
         .Select((state, _) => new ComparableGeneratorState(
                    state.State,
                    state.State.KeyMember,
                    state.Settings.CreateFactoryMethodName,
                    state.Settings.SkipIComparable,
                    state.State.KeyMember.IsComparable,
                    state.AttributeInfo.KeyMemberComparerAccessor,
                    state.State.GenericParameters));

      InitializeComparableCodeGenerator(context, comparables, options);
   }

   private void InitializeParsableCodeGenerator(IncrementalGeneratorInitializationContext context, IncrementalValuesProvider<KeyedValidSourceGenState> validStates, IncrementalValueProvider<GeneratorOptions> options)
   {
      var parsables = validStates
                      // Object factory will generate IParsable implementation
                      .Where(state => !state.AttributeInfo.ObjectFactories.Any(t => t.TypeFullyQualified == state.State.KeyMember.TypeFullyQualified
                                                                                    || t.SpecialType == SpecialType.System_String))
                      .Select((state, _) => new ParsableGeneratorState(
                                 state.State,
                                 state.State.KeyMember,
                                 state.State.ValidationError,
                                 state.Settings.SkipIParsable,
                                 hasStringBasedValidateMethod: false,
                                 state.State.GenericParameters));

      InitializeParsableCodeGenerator(context, parsables, options);
   }

   private void InitializeSpanParsableCodeGenerator(IncrementalGeneratorInitializationContext context, IncrementalValuesProvider<KeyedValidSourceGenState> validStates, IncrementalValueProvider<GeneratorOptions> options)
   {
      var parsables = validStates
                      // Object factory will generate ISpanParsable implementation
                      .Where(state => !state.AttributeInfo.ObjectFactories.Any(t => t.IsReadOnlySpanOfChar))
                      .Select((state, _) => new SpanParsableGeneratorState(
                                 state.State,
                                 state.State.KeyMember,
                                 state.State.ValidationError,
                                 state.Settings.SkipIParsable,
                                 state.Settings.SkipISpanParsable,
                                 isEnum: false,
                                 hasStringBasedValidateMethod: false,
                                 hasReadOnlySpanOfCharBasedValidateMethod: false,
                                 state.State.GenericParameters));

      InitializeSpanParsableCodeGenerator(context, parsables, options);
   }

   private void InitializeComparisonOperatorsCodeGenerator(IncrementalGeneratorInitializationContext context, IncrementalValuesProvider<KeyedValidSourceGenState> validStates, IncrementalValueProvider<GeneratorOptions> options)
   {
      var comparables = validStates
         .Select((state, _) => new ComparisonOperatorsGeneratorState(
                    state.State,
                    state.State.KeyMember,
                    state.Settings.CreateFactoryMethodName,
                    state.Settings.ComparisonOperators,
                    state.State.KeyMember.ComparisonOperators,
                    state.AttributeInfo.KeyMemberComparerAccessor,
                    state.State.GenericParameters));

      InitializeComparisonOperatorsCodeGenerator(context, comparables, options);
   }

   private void InitializeEqualityComparisonOperatorsCodeGenerator(IncrementalGeneratorInitializationContext context, IncrementalValuesProvider<KeyedValidSourceGenState> validStates, IncrementalValueProvider<GeneratorOptions> options)
   {
      var comparables = validStates
         .Select((state, _) => new EqualityComparisonOperatorsGeneratorState(
                    state.State,
                    state.State.KeyMember,
                    state.Settings.EqualityComparisonOperators,
                    state.AttributeInfo.KeyMemberEqualityComparerAccessor is null ? null : new ComparerInfo(state.AttributeInfo.KeyMemberEqualityComparerAccessor, true),
                    state.State.GenericParameters));

      InitializeEqualityComparisonOperatorsCodeGenerator(context, comparables, options);
   }

   private void InitializeAdditionOperatorsCodeGenerator(IncrementalGeneratorInitializationContext context, IncrementalValuesProvider<KeyedValidSourceGenState> validStates, IncrementalValueProvider<GeneratorOptions> options)
   {
      var operators = validStates
         .Select((state, _) => new OperatorsGeneratorState(
                    state.State,
                    state.State.KeyMember,
                    state.Settings.CreateFactoryMethodName,
                    state.Settings.AdditionOperators,
                    state.State.KeyMember.AdditionOperators,
                    AdditionOperatorsCodeGeneratorProvider.Instance,
                    state.State.GenericParameters));

      InitializeOperatorsCodeGenerator(context, operators, options);
   }

   private void InitializeSubtractionOperatorsCodeGenerator(IncrementalGeneratorInitializationContext context, IncrementalValuesProvider<KeyedValidSourceGenState> validStates, IncrementalValueProvider<GeneratorOptions> options)
   {
      var operators = validStates
         .Select((state, _) => new OperatorsGeneratorState(
                    state.State,
                    state.State.KeyMember,
                    state.Settings.CreateFactoryMethodName,
                    state.Settings.SubtractionOperators,
                    state.State.KeyMember.SubtractionOperators,
                    SubtractionOperatorsCodeGeneratorProvider.Instance,
                    state.State.GenericParameters));

      InitializeOperatorsCodeGenerator(context, operators, options);
   }

   private void InitializeMultiplyOperatorsCodeGenerator(IncrementalGeneratorInitializationContext context, IncrementalValuesProvider<KeyedValidSourceGenState> validStates, IncrementalValueProvider<GeneratorOptions> options)
   {
      var operators = validStates
         .Select((state, _) => new OperatorsGeneratorState(
                    state.State,
                    state.State.KeyMember,
                    state.Settings.CreateFactoryMethodName,
                    state.Settings.MultiplyOperators,
                    state.State.KeyMember.MultiplyOperators,
                    MultiplyOperatorsCodeGeneratorProvider.Instance,
                    state.State.GenericParameters));

      InitializeOperatorsCodeGenerator(context, operators, options);
   }

   private void InitializeDivisionOperatorsCodeGenerator(IncrementalGeneratorInitializationContext context, IncrementalValuesProvider<KeyedValidSourceGenState> validStates, IncrementalValueProvider<GeneratorOptions> options)
   {
      var operators = validStates
         .Select((state, _) => new OperatorsGeneratorState(
                    state.State,
                    state.State.KeyMember,
                    state.Settings.CreateFactoryMethodName,
                    state.Settings.DivisionOperators,
                    state.State.KeyMember.DivisionOperators,
                    DivisionOperatorsCodeGeneratorProvider.Instance,
                    state.State.GenericParameters));

      InitializeOperatorsCodeGenerator(context, operators, options);
   }

   private void InitializeExceptionReporting<T>(IncrementalGeneratorInitializationContext context, IncrementalValuesProvider<T> valueObjectOrException)
      where T : struct, ISourceGenContext
   {
      var exceptions = valueObjectOrException.SelectMany(static (state, _) => state.Exception is not null
                                                                                 ? [state.Exception.Value]
                                                                                 : ImmutableArray<SourceGenException>.Empty);
      context.RegisterSourceOutput(exceptions, ReportException);
   }

   private void InitializeDiagnosticReporting<T>(IncrementalGeneratorInitializationContext context, IncrementalValuesProvider<T> valueObjectOrException)
      where T : struct, ISourceGenContext
   {
      var exceptions = valueObjectOrException.SelectMany(static (state, _) => state.Diagnostic is not null
                                                                                 ? [state.Diagnostic.Value]
                                                                                 : ImmutableArray<SourceGenDiagnostic>.Empty);
      context.RegisterSourceOutput(exceptions, ReportDiagnostic);
   }

   private SerializerCodeGeneratorFactoriesContext GetSerializerCodeGeneratorFactories(MetadataReference reference)
   {
      try
      {
         var factories = new List<IValueObjectSerializerCodeGeneratorFactory>(3);

         foreach (var module in reference.GetModules())
         {
            switch (module.Name)
            {
               case Constants.Modules.THINKTECTURE_RUNTIME_EXTENSIONS_JSON:
                  Logger.Log(LogLevel.Information, "Code generator for System.Text.Json will participate in code generation");
                  factories.Add(JsonValueObjectCodeGeneratorFactory.Instance);
                  break;
               case Constants.Modules.THINKTECTURE_RUNTIME_EXTENSIONS_NEWTONSOFT_JSON:
                  Logger.Log(LogLevel.Information, "Code generator for Newtonsoft.Json will participate in code generation");
                  factories.Add(NewtonsoftJsonValueObjectCodeGeneratorFactory.Instance);
                  break;
               case Constants.Modules.THINKTECTURE_RUNTIME_EXTENSIONS_MESSAGEPACK:
                  Logger.Log(LogLevel.Information, "Code generator for MessagePack will participate in code generation");
                  factories.Add(MessagePackValueObjectCodeGeneratorFactory.Instance);
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

   private bool IsCandidate(SyntaxNode syntaxNode, CancellationToken cancellationToken)
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
         Logger.LogError("Error during checking whether a syntax node is a value object candidate", exception: ex);
         return false;
      }
   }

   private SourceGenContext<KeyedValidSourceGenState>? GetKeyedSourceGenContextOrNull(GeneratorAttributeSyntaxContext context, CancellationToken cancellationToken)
   {
      var tds = (TypeDeclarationSyntax)context.TargetNode;

      try
      {
         if (!TryGetType(context, out var type))
            return null;

         if (context.Attributes.IsDefaultOrEmpty)
            return null;

         if (context.Attributes.Length > 1)
            return new SourceGenDiagnostic(tds, DiagnosticsDescriptors.TypeMustNotHaveMoveThanOneValueObjectAttribute, [type.ToMinimallyQualifiedDisplayString()]);

         var attributeType = context.Attributes[0].AttributeClass;

         if (attributeType is null)
            return new SourceGenDiagnostic(tds, DiagnosticsDescriptors.ErrorDuringCodeAnalysis, [type.ToMinimallyQualifiedDisplayString(), "Could not resolve ValueObjectAttribute type"]);

         if (attributeType.TypeKind == TypeKind.Error)
            return new SourceGenDiagnostic(tds, DiagnosticsDescriptors.ErrorDuringCodeAnalysis, [type.ToMinimallyQualifiedDisplayString(), "ValueObjectAttribute type has TypeKind=Error"]);

         if (attributeType.Arity != 1)
            return new SourceGenDiagnostic(tds, DiagnosticsDescriptors.ErrorDuringCodeAnalysis, [type.ToMinimallyQualifiedDisplayString(), "ValueObjectAttribute must have exactly one type argument"]);

         var keyMemberType = attributeType.TypeArguments[0];

         if (keyMemberType.TypeKind == TypeKind.Error)
            return new SourceGenDiagnostic(tds, DiagnosticsDescriptors.ErrorDuringCodeAnalysis, [type.ToMinimallyQualifiedDisplayString(), "Key member type could not be resolved"]);

         if (keyMemberType.NullableAnnotation == NullableAnnotation.Annotated || keyMemberType.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T)
            return null; // Analyzer emits DiagnosticsDescriptors.KeyMemberShouldNotBeNullable

         if (type.IsNestedInGenericClass())
            return null; // Analyzer emits DiagnosticsDescriptors.TypeMustNotBeInsideGenericType

         var factory = TypedMemberStateFactoryProvider.GetFactoryOrNull(context.SemanticModel.Compilation);

         if (factory is null)
            return new SourceGenDiagnostic(tds, DiagnosticsDescriptors.ErrorDuringCodeAnalysis, [type.ToMinimallyQualifiedDisplayString(), "Could not fetch type information for code generation of a value object"]);

         var diagnostic = AttributeInfo.TryCreate(type, out var attributeInfo);

         if (diagnostic is not null)
            return new SourceGenDiagnostic(tds, diagnostic.Value.Descriptor, diagnostic.Value.Args);

         var settings = new AllValueObjectSettings(context.Attributes[0], keyMemberType.SpecialType == SpecialType.System_String);

         var keyTypedMemberState = factory.Create(keyMemberType);
         var keyMember = settings.CreateKeyMember(keyTypedMemberState);

         var state = new KeyedValueObjectSourceGeneratorState(type, keyMember, attributeInfo.ValidationError, new ValueObjectSettings(settings, attributeInfo));

         return new KeyedValidSourceGenState(state, settings, attributeInfo);
      }
      catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
      {
         throw;
      }
      catch (Exception ex)
      {
         return new SourceGenException("Error during extraction of relevant information out of semantic model for generation of a value object", ex, tds);
      }
   }

   private SourceGenContext<ComplexValidSourceGenState>? GetComplexSourceGenContextOrNull(GeneratorAttributeSyntaxContext context, CancellationToken cancellationToken)
   {
      var tds = (TypeDeclarationSyntax)context.TargetNode;

      try
      {
         if (!TryGetType(context, out var type))
            return null;

         if (type.IsNestedInGenericClass())
            return null; // Analyzer emits DiagnosticsDescriptors.TypeMustNotBeInsideGenericType

         var factory = TypedMemberStateFactoryProvider.GetFactoryOrNull(context.SemanticModel.Compilation);

         if (factory is null)
            return new SourceGenDiagnostic(tds, DiagnosticsDescriptors.ErrorDuringCodeAnalysis, [type.ToMinimallyQualifiedDisplayString(), "Could not fetch type information for code generation of a value object"]);

         var diagnostic = AttributeInfo.TryCreate(type, out var attributeInfo);

         if (diagnostic is not null)
            return new SourceGenDiagnostic(tds, diagnostic.Value.Descriptor, diagnostic.Value.Args);

         var settings = new AllValueObjectSettings(context.Attributes[0], false);
         var state = new ComplexValueObjectSourceGeneratorState(factory, type, attributeInfo.ValidationError, new ValueObjectSettings(settings, attributeInfo), cancellationToken);

         return new ComplexValidSourceGenState(state, settings, attributeInfo);
      }
      catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
      {
         throw;
      }
      catch (Exception ex)
      {
         return new SourceGenException("Error during extraction of relevant information out of semantic model for generation of a value object", ex, tds);
      }
   }

   private static bool TryGetType(GeneratorAttributeSyntaxContext context, out INamedTypeSymbol type)
   {
      type = (INamedTypeSymbol)context.TargetSymbol;

      return type.TypeKind != TypeKind.Error;
   }

   private readonly record struct KeyedValidSourceGenState(
      KeyedValueObjectSourceGeneratorState State,
      AllValueObjectSettings Settings,
      AttributeInfo AttributeInfo) : ISourceGenState<KeyedValueObjectSourceGeneratorState>;

   private readonly record struct ComplexValidSourceGenState(
      ComplexValueObjectSourceGeneratorState State,
      AllValueObjectSettings Settings,
      AttributeInfo AttributeInfo) : ISourceGenState<ComplexValueObjectSourceGeneratorState>;

   private readonly record struct SourceGenContext<TState>(
      TState? ValidState,
      SourceGenException? Exception,
      SourceGenDiagnostic? Diagnostic)
      : ISourceGenContext
      where TState : struct
   {
      public static implicit operator SourceGenContext<TState>(TState state)
      {
         return new SourceGenContext<TState>(state, null, null);
      }

      public static implicit operator SourceGenContext<TState>(SourceGenException exception)
      {
         return new SourceGenContext<TState>(null, exception, null);
      }

      public static implicit operator SourceGenContext<TState>(SourceGenDiagnostic diagnostic)
      {
         return new SourceGenContext<TState>(null, null, diagnostic);
      }
   }

   private interface ISourceGenContext
   {
      SourceGenException? Exception { get; }
      SourceGenDiagnostic? Diagnostic { get; }
   }

   private interface ISourceGenState<out TState>
   {
      TState State { get; }
   }

   private readonly record struct SerializerCodeGeneratorFactoriesContext(
      IReadOnlyList<IValueObjectSerializerCodeGeneratorFactory>? Factories,
      Exception? Exception = null)
   {
      public SerializerCodeGeneratorFactoriesContext(Exception exception)
         : this(null, exception)
      {
      }
   }
}
