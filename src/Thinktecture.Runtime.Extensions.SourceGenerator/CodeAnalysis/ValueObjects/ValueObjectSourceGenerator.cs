using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Thinktecture.CodeAnalysis.ValueObjects;

[Generator]
public sealed class ValueObjectSourceGenerator : ThinktectureSourceGeneratorBase, IIncrementalGenerator
{
   public ValueObjectSourceGenerator()
      : base(12_000)
   {
   }

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
      return context.MetadataReferencesProvider
                    .SelectMany((reference, _) => GetSerializerCodeGeneratorFactories(reference))
                    .Collect()
                    .Select(static (states, _) => states.IsDefaultOrEmpty
                                                     ? ImmutableArray<IValueObjectSerializerCodeGeneratorFactory>.Empty
                                                     : states.Distinct())
                    .WithComparer(new SetComparer<IValueObjectSerializerCodeGeneratorFactory>());
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
      InitializeParsableCodeGenerator(context, validComplexValueObjects, options);
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
                                                                              ? ImmutableArray.Create(state.Value)
                                                                              : ImmutableArray<SourceGenContext<TState>>.Empty);

      var validValueObjects = valueObjectOrException.SelectMany(static (state, _) => state.ValidState is not null
                                                                                        ? ImmutableArray.Create(state.ValidState.Value)
                                                                                        : ImmutableArray<TState>.Empty);

      var sourceGenStates = validValueObjects
                            .Select(static (state, _) => state.State)
                            .Collect()
                            .Select((states, _) => states.IsDefaultOrEmpty
                                                      ? ImmutableArray<TGenState>.Empty
                                                      : states.Distinct(typeOnlyComparer))
                            .WithComparer(new SetComparer<TGenState>())
                            .SelectMany(static (states, _) => states);

      context.RegisterSourceOutput(sourceGenStates.Combine(options), (ctx, state) => GenerateCode(ctx, state.Left, state.Right, codeGeneratorFactory));

      InitializeErrorReporting(context, valueObjectOrException);
      InitializeExceptionReporting(context, valueObjectOrException);

      return validValueObjects;
   }

   private void InitializeSerializerGenerators(
      IncrementalGeneratorInitializationContext context,
      IncrementalValuesProvider<KeyedValidSourceGenState> validStates,
      IncrementalValueProvider<GeneratorOptions> options,
      IncrementalValueProvider<ImmutableArray<IValueObjectSerializerCodeGeneratorFactory>> serializerGeneratorFactories)
   {
      validStates = validStates.Where(state => !state.Settings.SkipFactoryMethods || state.AttributeInfo.DesiredFactories.Any(f => f.UseForSerialization != SerializationFrameworks.None));

      var keyedSerializerGeneratorStates = validStates.SelectMany((state, _) =>
                                                                  {
                                                                     var serializerState = new KeyedSerializerGeneratorState(state.State, state.State.KeyMember, state.AttributeInfo);

                                                                     return ImmutableArray.Create(serializerState);
                                                                  })
                                                      .Combine(serializerGeneratorFactories)
                                                      .SelectMany((tuple, _) => ImmutableArray.CreateRange(tuple.Right, (factory, state) => (State: state, Factory: factory), tuple.Left))
                                                      .Where(tuple =>
                                                             {
                                                                if (tuple.Factory.MustGenerateCode(tuple.State))
                                                                {
                                                                   Logger.LogDebug("Code generator must generate code.", null, tuple.State, tuple.Factory);
                                                                   return true;
                                                                }

                                                                Logger.LogInformation("Code generator must not generate code.", null, tuple.State, tuple.Factory);
                                                                return false;
                                                             });

      context.RegisterImplementationSourceOutput(keyedSerializerGeneratorStates.Combine(options), (ctx, tuple) => GenerateCode(ctx, tuple.Left.State, tuple.Right, tuple.Left.Factory));
   }

   private void InitializeSerializerGenerators(
      IncrementalGeneratorInitializationContext context,
      IncrementalValuesProvider<ComplexValidSourceGenState> validStates,
      IncrementalValueProvider<GeneratorOptions> options,
      IncrementalValueProvider<ImmutableArray<IValueObjectSerializerCodeGeneratorFactory>> serializerGeneratorFactories)
   {
      validStates = validStates.Where(state => !state.Settings.SkipFactoryMethods || state.AttributeInfo.DesiredFactories.Any(f => f.UseForSerialization != SerializationFrameworks.None));

      var keyedSerializerGeneratorStates = validStates.SelectMany((state, _) =>
                                                                  {
                                                                     if (state.AttributeInfo.DesiredFactories.All(f => f.UseForSerialization == SerializationFrameworks.None))
                                                                        return ImmutableArray<KeyedSerializerGeneratorState>.Empty;

                                                                     var serializerState = new KeyedSerializerGeneratorState(state.State, null, state.AttributeInfo);

                                                                     return ImmutableArray.Create(serializerState);
                                                                  })
                                                      .Combine(serializerGeneratorFactories)
                                                      .SelectMany((tuple, _) => ImmutableArray.CreateRange(tuple.Right, (factory, state) => (State: state, Factory: factory), tuple.Left))
                                                      .Where(tuple =>
                                                             {
                                                                if (tuple.Factory.MustGenerateCode(tuple.State))
                                                                {
                                                                   Logger.LogDebug("Code generator must generate code.", null, tuple.State, tuple.Factory);
                                                                   return true;
                                                                }

                                                                Logger.LogInformation("Code generator must not generate code.", null, tuple.State, tuple.Factory);
                                                                return false;
                                                             });

      var complexSerializerGeneratorStates = validStates.SelectMany((state, _) =>
                                                                    {
                                                                       var serializerState = new ComplexSerializerGeneratorState(state.State, state.State.AssignableInstanceFieldsAndProperties, state.AttributeInfo);

                                                                       return ImmutableArray.Create(serializerState);
                                                                    })
                                                        .Combine(serializerGeneratorFactories)
                                                        .SelectMany((tuple, _) => ImmutableArray.CreateRange(tuple.Right, (factory, state) => (State: state, Factory: factory), tuple.Left))
                                                        .Where(tuple =>
                                                               {
                                                                  if (tuple.Factory.MustGenerateCode(tuple.State))
                                                                  {
                                                                     Logger.LogDebug("Code generator must generate code.", null, tuple.State, tuple.Factory);
                                                                     return true;
                                                                  }

                                                                  Logger.LogInformation("Code generator must not generate code.", null, tuple.State, tuple.Factory);
                                                                  return false;
                                                               });

      context.RegisterImplementationSourceOutput(keyedSerializerGeneratorStates.Combine(options), (ctx, tuple) => GenerateCode(ctx, tuple.Left.State, tuple.Right, tuple.Left.Factory));
      context.RegisterImplementationSourceOutput(complexSerializerGeneratorStates.Combine(options), (ctx, tuple) => GenerateCode(ctx, tuple.Left.State, tuple.Right, tuple.Left.Factory));
   }

   private void InitializeFormattableCodeGenerator(IncrementalGeneratorInitializationContext context, IncrementalValuesProvider<KeyedValidSourceGenState> validStates, IncrementalValueProvider<GeneratorOptions> options)
   {
      var formattables = validStates
         .Select((state, _) => new FormattableGeneratorState(state.State,
                                                             state.State.KeyMember,
                                                             state.Settings.CreateFactoryMethodName,
                                                             state.Settings.SkipIFormattable,
                                                             state.State.KeyMember.IsFormattable));

      InitializeFormattableCodeGenerator(context, formattables, options);
   }

   private void InitializeComparableCodeGenerator(IncrementalGeneratorInitializationContext context, IncrementalValuesProvider<KeyedValidSourceGenState> validStates, IncrementalValueProvider<GeneratorOptions> options)
   {
      var comparables = validStates
         .Select((state, _) => new ComparableGeneratorState(state.State,
                                                            state.State.KeyMember,
                                                            state.Settings.CreateFactoryMethodName,
                                                            state.Settings.SkipIComparable,
                                                            state.State.KeyMember.IsComparable,
                                                            state.AttributeInfo.KeyMemberComparerAccessor));

      InitializeComparableCodeGenerator(context, comparables, options);
   }

   private void InitializeParsableCodeGenerator(IncrementalGeneratorInitializationContext context, IncrementalValuesProvider<KeyedValidSourceGenState> validStates, IncrementalValueProvider<GeneratorOptions> options)
   {
      var parsables = validStates
         .Select((state, _) => new ParsableGeneratorState(state.State,
                                                          state.State.KeyMember,
                                                          state.State.ValidationError,
                                                          state.Settings.SkipIParsable,
                                                          state.State.KeyMember.IsParsable,
                                                          false,
                                                          false,
                                                          state.AttributeInfo.DesiredFactories.Any(t => t.SpecialType == SpecialType.System_String)));

      InitializeParsableCodeGenerator(context, parsables, options);
   }

   private void InitializeParsableCodeGenerator(IncrementalGeneratorInitializationContext context, IncrementalValuesProvider<ComplexValidSourceGenState> validStates, IncrementalValueProvider<GeneratorOptions> options)
   {
      var parsables = validStates
         .Select((state, _) => new ParsableGeneratorState(state.State,
                                                          null,
                                                          state.State.ValidationError,
                                                          state.Settings.SkipIParsable,
                                                          false,
                                                          false,
                                                          false,
                                                          state.AttributeInfo.DesiredFactories.Any(t => t.SpecialType == SpecialType.System_String)));

      InitializeParsableCodeGenerator(context, parsables, options);
   }

   private void InitializeComparisonOperatorsCodeGenerator(IncrementalGeneratorInitializationContext context, IncrementalValuesProvider<KeyedValidSourceGenState> validStates, IncrementalValueProvider<GeneratorOptions> options)
   {
      var comparables = validStates
         .Select((state, _) => new ComparisonOperatorsGeneratorState(state.State,
                                                                     state.State.KeyMember,
                                                                     state.Settings.CreateFactoryMethodName,
                                                                     state.Settings.ComparisonOperators,
                                                                     state.State.KeyMember.ComparisonOperators,
                                                                     state.AttributeInfo.KeyMemberComparerAccessor));

      InitializeComparisonOperatorsCodeGenerator(context, comparables, options);
   }

   private void InitializeEqualityComparisonOperatorsCodeGenerator(IncrementalGeneratorInitializationContext context, IncrementalValuesProvider<KeyedValidSourceGenState> validStates, IncrementalValueProvider<GeneratorOptions> options)
   {
      var comparables = validStates
         .Select((state, _) => new EqualityComparisonOperatorsGeneratorState(state.State,
                                                                             state.State.KeyMember,
                                                                             state.Settings.EqualityComparisonOperators,
                                                                             state.AttributeInfo.KeyMemberEqualityComparerAccessor is null ? null : new ComparerInfo(state.AttributeInfo.KeyMemberEqualityComparerAccessor, true)));

      InitializeEqualityComparisonOperatorsCodeGenerator(context, comparables, options);
   }

   private void InitializeAdditionOperatorsCodeGenerator(IncrementalGeneratorInitializationContext context, IncrementalValuesProvider<KeyedValidSourceGenState> validStates, IncrementalValueProvider<GeneratorOptions> options)
   {
      var operators = validStates
         .Select((state, _) => new OperatorsGeneratorState(state.State,
                                                           state.State.KeyMember,
                                                           state.Settings.CreateFactoryMethodName,
                                                           state.Settings.AdditionOperators,
                                                           state.State.KeyMember.AdditionOperators,
                                                           AdditionOperatorsCodeGeneratorProvider.Instance));

      InitializeOperatorsCodeGenerator(context, operators, options);
   }

   private void InitializeSubtractionOperatorsCodeGenerator(IncrementalGeneratorInitializationContext context, IncrementalValuesProvider<KeyedValidSourceGenState> validStates, IncrementalValueProvider<GeneratorOptions> options)
   {
      var operators = validStates
         .Select((state, _) => new OperatorsGeneratorState(state.State,
                                                           state.State.KeyMember,
                                                           state.Settings.CreateFactoryMethodName,
                                                           state.Settings.SubtractionOperators,
                                                           state.State.KeyMember.SubtractionOperators,
                                                           SubtractionOperatorsCodeGeneratorProvider.Instance));

      InitializeOperatorsCodeGenerator(context, operators, options);
   }

   private void InitializeMultiplyOperatorsCodeGenerator(IncrementalGeneratorInitializationContext context, IncrementalValuesProvider<KeyedValidSourceGenState> validStates, IncrementalValueProvider<GeneratorOptions> options)
   {
      var operators = validStates
         .Select((state, _) => new OperatorsGeneratorState(state.State,
                                                           state.State.KeyMember,
                                                           state.Settings.CreateFactoryMethodName,
                                                           state.Settings.MultiplyOperators,
                                                           state.State.KeyMember.MultiplyOperators,
                                                           MultiplyOperatorsCodeGeneratorProvider.Instance));

      InitializeOperatorsCodeGenerator(context, operators, options);
   }

   private void InitializeDivisionOperatorsCodeGenerator(IncrementalGeneratorInitializationContext context, IncrementalValuesProvider<KeyedValidSourceGenState> validStates, IncrementalValueProvider<GeneratorOptions> options)
   {
      var operators = validStates
         .Select((state, _) => new OperatorsGeneratorState(state.State,
                                                           state.State.KeyMember,
                                                           state.Settings.CreateFactoryMethodName,
                                                           state.Settings.DivisionOperators,
                                                           state.State.KeyMember.DivisionOperators,
                                                           DivisionOperatorsCodeGeneratorProvider.Instance));

      InitializeOperatorsCodeGenerator(context, operators, options);
   }

   private void InitializeErrorReporting<T>(IncrementalGeneratorInitializationContext context, IncrementalValuesProvider<T> valueObjectOrException)
      where T : struct, ISourceGenContext
   {
      var exceptions = valueObjectOrException.SelectMany(static (state, _) => state.Error is not null
                                                                                 ? ImmutableArray.Create(state.Error.Value)
                                                                                 : ImmutableArray<SourceGenError>.Empty);
      context.RegisterSourceOutput(exceptions, ReportError);
   }

   private void InitializeExceptionReporting<T>(IncrementalGeneratorInitializationContext context, IncrementalValuesProvider<T> valueObjectOrException)
      where T : struct, ISourceGenContext
   {
      var exceptions = valueObjectOrException.SelectMany(static (state, _) => state.Exception is not null
                                                                                 ? ImmutableArray.Create(state.Exception.Value)
                                                                                 : ImmutableArray<SourceGenException>.Empty);
      context.RegisterSourceOutput(exceptions, ReportException);
   }

   private ImmutableArray<IValueObjectSerializerCodeGeneratorFactory> GetSerializerCodeGeneratorFactories(MetadataReference reference)
   {
      var factories = ImmutableArray<IValueObjectSerializerCodeGeneratorFactory>.Empty;

      try
      {
         foreach (var module in reference.GetModules())
         {
            switch (module.Name)
            {
               case Constants.Modules.THINKTECTURE_RUNTIME_EXTENSIONS_JSON:
                  Logger.LogInformation("Code generator for System.Text.Json will participate in code generation");
                  factories = factories.Add(JsonValueObjectCodeGeneratorFactory.Instance);
                  break;
               case Constants.Modules.THINKTECTURE_RUNTIME_EXTENSIONS_NEWTONSOFT_JSON:
                  Logger.LogInformation("Code generator for Newtonsoft.Json will participate in code generation");
                  factories = factories.Add(NewtonsoftJsonValueObjectCodeGeneratorFactory.Instance);
                  break;
               case Constants.Modules.THINKTECTURE_RUNTIME_EXTENSIONS_MESSAGEPACK:
                  Logger.LogInformation("Code generator for MessagePack will participate in code generation");
                  factories = factories.Add(MessagePackValueObjectCodeGeneratorFactory.Instance);
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
            ClassDeclarationSyntax classDeclaration when IsValueObjectCandidate(classDeclaration) => true,
            StructDeclarationSyntax structDeclaration when IsValueObjectCandidate(structDeclaration) => true,
            _ => false
         };
      }
      catch (Exception ex)
      {
         Logger.LogError("Error during checking whether a syntax node is a value object candidate", exception: ex);
         return false;
      }
   }

   private bool IsValueObjectCandidate(TypeDeclarationSyntax typeDeclaration)
   {
      var isCandidate = !typeDeclaration.IsGeneric();

      if (isCandidate)
      {
         Logger.LogDebug("The type declaration is a value object candidate", typeDeclaration);
      }
      else
      {
         Logger.LogTrace("The type declaration is not a value object candidate", typeDeclaration);
      }

      return isCandidate;
   }

   private SourceGenContext<KeyedValidSourceGenState>? GetKeyedSourceGenContextOrNull(GeneratorAttributeSyntaxContext context, CancellationToken cancellationToken)
   {
      var tds = (TypeDeclarationSyntax)context.TargetNode;

      try
      {
         if (!TryGetType(context, tds, out var type))
            return null;

         var attributeType = context.Attributes[0].AttributeClass;

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

         if (attributeType.TypeArguments.Length != 1)
         {
            Logger.LogDebug($"Expected the attribute type to have 1 type argument but found {attributeType.TypeArguments.Length.ToString()}", tds);
            return null;
         }

         var keyMemberType = attributeType.TypeArguments[0];

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
            return new SourceGenError("Could not fetch type information for code generation of a smart enum", tds);

         var errorMessage = AttributeInfo.TryCreate(type, out var attributeInfo);

         if (errorMessage is not null)
         {
            Logger.LogDebug(errorMessage, tds);
            return null;
         }

         var settings = new AllValueObjectSettings(context.Attributes[0]);

         var keyTypedMemberState = factory.Create(keyMemberType);
         var keyMember = settings.CreateKeyMember(keyTypedMemberState);

         var state = new KeyedValueObjectSourceGeneratorState(type, keyMember, attributeInfo.ValidationError, new ValueObjectSettings(settings, attributeInfo));

         Logger.LogDebug("The type declaration is a valid simple/keyed value object", null, state);

         return new KeyedValidSourceGenState(state, settings, attributeInfo);
      }
      catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
      {
         throw;
      }
      catch (Exception ex)
      {
         Logger.LogError("Error during extraction of relevant information out of semantic model for generation of a value object", tds, ex);

         return new SourceGenException(ex, tds);
      }
   }

   private SourceGenContext<ComplexValidSourceGenState>? GetComplexSourceGenContextOrNull(GeneratorAttributeSyntaxContext context, CancellationToken cancellationToken)
   {
      var tds = (TypeDeclarationSyntax)context.TargetNode;

      try
      {
         if (!TryGetType(context, tds, out var type))
            return null;

         var factory = TypedMemberStateFactoryProvider.GetFactoryOrNull(context.SemanticModel.Compilation, Logger);

         if (factory is null)
            return new SourceGenError("Could not fetch type information for code generation of a smart enum", tds);

         var errorMessage = AttributeInfo.TryCreate(type, out var attributeInfo);

         if (errorMessage is not null)
         {
            Logger.LogDebug(errorMessage, tds);
            return null;
         }

         var settings = new AllValueObjectSettings(context.Attributes[0]);

         var state = new ComplexValueObjectSourceGeneratorState(factory, type, attributeInfo.ValidationError, new ValueObjectSettings(settings, attributeInfo), cancellationToken);

         Logger.LogDebug("The type declaration is a valid complex value object", null, state);

         return new ComplexValidSourceGenState(state, settings, attributeInfo);
      }
      catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
      {
         throw;
      }
      catch (Exception ex)
      {
         Logger.LogError("Error during extraction of relevant information out of semantic model for generation of a value object", tds, ex);

         return new SourceGenException(ex, tds);
      }
   }

   private bool TryGetType(GeneratorAttributeSyntaxContext context, TypeDeclarationSyntax tds, out INamedTypeSymbol type)
   {
      type = (INamedTypeSymbol)context.TargetSymbol;

      if (type.TypeKind == TypeKind.Error)
      {
         Logger.LogDebug("Type from semantic model is erroneous", tds);
         return false;
      }

      return true;
   }

   private readonly record struct KeyedValidSourceGenState(
      KeyedValueObjectSourceGeneratorState State,
      AllValueObjectSettings Settings,
      AttributeInfo AttributeInfo) : ISourceGenState<KeyedValueObjectSourceGeneratorState>;

   private readonly record struct ComplexValidSourceGenState(
      ComplexValueObjectSourceGeneratorState State,
      AllValueObjectSettings Settings,
      AttributeInfo AttributeInfo) : ISourceGenState<ComplexValueObjectSourceGeneratorState>;

   private readonly record struct SourceGenContext<TState>(TState? ValidState, SourceGenException? Exception, SourceGenError? Error)
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

      public static implicit operator SourceGenContext<TState>(SourceGenError error)
      {
         return new SourceGenContext<TState>(null, null, error);
      }
   }

   private interface ISourceGenContext
   {
      SourceGenException? Exception { get; }
      SourceGenError? Error { get; }
   }

   private interface ISourceGenState<out TState>
   {
      TState State { get; }
   }
}
