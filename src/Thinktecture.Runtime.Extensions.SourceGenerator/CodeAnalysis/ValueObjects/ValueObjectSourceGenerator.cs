using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
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

      var valueObjectOrException = context.SyntaxProvider
                                          .ForAttributeWithMetadataName(Constants.Attributes.VALUE_OBJECT,
                                                                        IsCandidate,
                                                                        GetSourceGenContextOrNull)
                                          .SelectMany(static (state, _) => state.HasValue
                                                                              ? ImmutableArray.Create(state.Value)
                                                                              : ImmutableArray<SourceGenContext>.Empty);

      var validStates = valueObjectOrException.SelectMany(static (state, _) => state.ValidState is not null
                                                                                  ? ImmutableArray.Create(state.ValidState.Value)
                                                                                  : ImmutableArray<ValidSourceGenState>.Empty);

      var keyedValueObjects = validStates.SelectMany((state, _) =>
                                                     {
                                                        if (!state.State.HasKeyMember)
                                                           return ImmutableArray<KeyedValueObjectState>.Empty;

                                                        var keyedValueObject = new KeyedValueObjectState(state.State, state.State.KeyMember, state.Settings);

                                                        return ImmutableArray.Create(keyedValueObject);
                                                     });

      InitializeValueObjectsGeneration(context, validStates, options);
      InitializeSerializerGenerators(context, validStates, options);
      InitializeFormattableCodeGenerator(context, keyedValueObjects, options);
      InitializeComparableCodeGenerator(context, keyedValueObjects, options);
      InitializeParsableCodeGenerator(context, keyedValueObjects, options);
      InitializeComparisonOperatorsCodeGenerator(context, keyedValueObjects, options);
      InitializeAdditionOperatorsCodeGenerator(context, keyedValueObjects, options);
      InitializeSubtractionOperatorsCodeGenerator(context, keyedValueObjects, options);
      InitializeMultiplyOperatorsCodeGenerator(context, keyedValueObjects, options);
      InitializeDivisionOperatorsCodeGenerator(context, keyedValueObjects, options);

      InitializeErrorReporting(context, valueObjectOrException);
      InitializeExceptionReporting(context, valueObjectOrException);
   }

   private void InitializeValueObjectsGeneration(IncrementalGeneratorInitializationContext context, IncrementalValuesProvider<ValidSourceGenState> validStates, IncrementalValueProvider<GeneratorOptions> options)
   {
      var valueObjects = validStates
                         .Select((state, _) => state.State)
                         .Collect()
                         .Select(static (states, _) => states.IsDefaultOrEmpty
                                                          ? ImmutableArray<ValueObjectSourceGeneratorState>.Empty
                                                          : states.Distinct(TypeOnlyComparer<ValueObjectSourceGeneratorState>.Instance).ToImmutableArray())
                         .WithComparer(new SetComparer<ValueObjectSourceGeneratorState>())
                         .SelectMany((states, _) => states);

      context.RegisterSourceOutput(valueObjects.Combine(options), (ctx, state) => GenerateCode(ctx, state.Left, state.Right, ValueObjectCodeGeneratorFactory.Instance));
   }

   private void InitializeSerializerGenerators(IncrementalGeneratorInitializationContext context, IncrementalValuesProvider<ValidSourceGenState> validStates, IncrementalValueProvider<GeneratorOptions> options)
   {
      var serializerGeneratorFactories = context.MetadataReferencesProvider
                                                .SelectMany((reference, _) => GetSerializerCodeGeneratorFactories(reference))
                                                .Collect()
                                                .Select(static (states, _) => states.IsDefaultOrEmpty
                                                                                 ? ImmutableArray<IValueObjectSerializerCodeGeneratorFactory>.Empty
                                                                                 : states.Distinct().ToImmutableArray())
                                                .WithComparer(new SetComparer<IValueObjectSerializerCodeGeneratorFactory>());

      validStates = validStates.Where(state => !state.Settings.SkipFactoryMethods);

      var keyedSerializerGeneratorStates = validStates.SelectMany((state, _) =>
                                                                  {
                                                                     if (!state.State.HasKeyMember)
                                                                        return ImmutableArray<KeyedSerializerGeneratorState>.Empty;

                                                                     var serializerState = new KeyedSerializerGeneratorState(state.State, state.State.KeyMember.Member, state.AttributeInfo);

                                                                     return ImmutableArray.Create(serializerState);
                                                                  })
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

      var complexSerializerGeneratorStates = validStates.SelectMany((state, _) =>
                                                                    {
                                                                       if (state.State.HasKeyMember)
                                                                          return ImmutableArray<ComplexSerializerGeneratorState>.Empty;

                                                                       var serializerState = new ComplexSerializerGeneratorState(state.State, state.State.AssignableInstanceFieldsAndProperties, state.AttributeInfo);

                                                                       return ImmutableArray.Create(serializerState);
                                                                    })
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

      context.RegisterImplementationSourceOutput(keyedSerializerGeneratorStates.Combine(options), (ctx, tuple) => GenerateCode(ctx, tuple.Left.State, tuple.Right, tuple.Left.Factory));
      context.RegisterImplementationSourceOutput(complexSerializerGeneratorStates.Combine(options), (ctx, tuple) => GenerateCode(ctx, tuple.Left.State, tuple.Right, tuple.Left.Factory));
   }

   private void InitializeFormattableCodeGenerator(IncrementalGeneratorInitializationContext context, IncrementalValuesProvider<KeyedValueObjectState> validStates, IncrementalValueProvider<GeneratorOptions> options)
   {
      var formattables = validStates
         .Select((state, _) => new FormattableGeneratorState(state.Type,
                                                             state.KeyMember.Member,
                                                             state.Settings.SkipIFormattable,
                                                             state.KeyMember.Member.IsFormattable));

      InitializeFormattableCodeGenerator(context, formattables, options);
   }

   private void InitializeComparableCodeGenerator(IncrementalGeneratorInitializationContext context, IncrementalValuesProvider<KeyedValueObjectState> validStates, IncrementalValueProvider<GeneratorOptions> options)
   {
      var comparables = validStates
         .Select((state, _) => new ComparableGeneratorState(state.Type,
                                                            state.KeyMember.Member,
                                                            state.Settings.SkipIComparable,
                                                            state.KeyMember.Member.IsComparable,
                                                            state.KeyMember.ComparerAccessor));

      InitializeComparableCodeGenerator(context, comparables, options);
   }

   private void InitializeParsableCodeGenerator(IncrementalGeneratorInitializationContext context, IncrementalValuesProvider<KeyedValueObjectState> validStates, IncrementalValueProvider<GeneratorOptions> options)
   {
      var parsables = validStates
         .Select((state, _) => new ParsableGeneratorState(state.Type,
                                                          state.KeyMember.Member,
                                                          state.Settings.SkipIParsable,
                                                          state.KeyMember.Member.IsParsable,
                                                          false));

      InitializeParsableCodeGenerator(context, parsables, options);
   }

   private void InitializeComparisonOperatorsCodeGenerator(IncrementalGeneratorInitializationContext context, IncrementalValuesProvider<KeyedValueObjectState> validStates, IncrementalValueProvider<GeneratorOptions> options)
   {
      var comparables = validStates
         .Select((state, _) => new ComparisonOperatorsGeneratorState(state.Type,
                                                                     state.KeyMember.Member,
                                                                     state.Settings.ComparisonOperators,
                                                                     state.KeyMember.Member.HasComparisonOperators,
                                                                     state.KeyMember.ComparerAccessor));

      InitializeComparisonOperatorsCodeGenerator(context, comparables, options);
   }

   private void InitializeAdditionOperatorsCodeGenerator(IncrementalGeneratorInitializationContext context, IncrementalValuesProvider<KeyedValueObjectState> validStates, IncrementalValueProvider<GeneratorOptions> options)
   {
      var operators = validStates
         .Select((state, _) => new OperatorsGeneratorState(state.Type,
                                                           state.KeyMember.Member,
                                                           state.Settings.AdditionOperators,
                                                           state.KeyMember.Member.HasAdditionOperators,
                                                           AdditionOperatorsCodeGeneratorProvider.Instance));

      InitializeOperatorsCodeGenerator(context, operators, options);
   }

   private void InitializeSubtractionOperatorsCodeGenerator(IncrementalGeneratorInitializationContext context, IncrementalValuesProvider<KeyedValueObjectState> validStates, IncrementalValueProvider<GeneratorOptions> options)
   {
      var operators = validStates
         .Select((state, _) => new OperatorsGeneratorState(state.Type,
                                                           state.KeyMember.Member,
                                                           state.Settings.SubtractionOperators,
                                                           state.KeyMember.Member.HasSubtractionOperators,
                                                           SubtractionOperatorsCodeGeneratorProvider.Instance));

      InitializeOperatorsCodeGenerator(context, operators, options);
   }

   private void InitializeMultiplyOperatorsCodeGenerator(IncrementalGeneratorInitializationContext context, IncrementalValuesProvider<KeyedValueObjectState> validStates, IncrementalValueProvider<GeneratorOptions> options)
   {
      var operators = validStates
         .Select((state, _) => new OperatorsGeneratorState(state.Type,
                                                           state.KeyMember.Member,
                                                           state.Settings.MultiplyOperators,
                                                           state.KeyMember.Member.HasMultiplyOperators,
                                                           MultiplyOperatorsCodeGeneratorProvider.Instance));

      InitializeOperatorsCodeGenerator(context, operators, options);
   }

   private void InitializeDivisionOperatorsCodeGenerator(IncrementalGeneratorInitializationContext context, IncrementalValuesProvider<KeyedValueObjectState> validStates, IncrementalValueProvider<GeneratorOptions> options)
   {
      var operators = validStates
         .Select((state, _) => new OperatorsGeneratorState(state.Type,
                                                           state.KeyMember.Member,
                                                           state.Settings.DivisionOperators,
                                                           state.KeyMember.Member.HasDivisionOperators,
                                                           DivisionOperatorsCodeGeneratorProvider.Instance));

      InitializeOperatorsCodeGenerator(context, operators, options);
   }

   private void InitializeErrorReporting(IncrementalGeneratorInitializationContext context, IncrementalValuesProvider<SourceGenContext> valueObjectOrException)
   {
      var exceptions = valueObjectOrException.SelectMany(static (state, _) => state.Error is not null
                                                                                 ? ImmutableArray.Create(state.Error.Value)
                                                                                 : ImmutableArray<SourceGenError>.Empty);
      context.RegisterSourceOutput(exceptions, ReportError);
   }

   private void InitializeExceptionReporting(IncrementalGeneratorInitializationContext context, IncrementalValuesProvider<SourceGenContext> valueObjectOrException)
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
      var isCandidate = typeDeclaration.IsPartial()
                        && !typeDeclaration.IsGeneric();

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

         if (type.ContainingType is not null)
         {
            Logger.LogDebug("Nested types are not supported", tds);
            return null;
         }

         if (context.Attributes.Length > 1)
         {
            Logger.LogDebug("Type has more than 1 ValueObjectAttribute", tds);
            return null;
         }

         var factory = TypedMemberStateFactoryProvider.GetFactoryOrNull(context.SemanticModel.Compilation, Logger);

         if (factory is null)
            return new SourceGenContext(new SourceGenError("Could not fetch type information for code generation of a smart enum", tds));

         var settings = new AllValueObjectSettings(context.Attributes[0]);
         var state = new ValueObjectSourceGeneratorState(factory, type, new ValueObjectSettings(settings), cancellationToken);

         if (state.HasKeyMember)
         {
            if (state.KeyMember.Member.IsErroneous)
            {
               Logger.LogDebug("Type of the key member is erroneous", tds);
               return null;
            }

            if (state.KeyMember.Member.NullableAnnotation == NullableAnnotation.Annotated)
            {
               Logger.LogDebug("Type of the key member must not be nullable", tds);
               return null;
            }

            Logger.LogDebug("The type declaration is a valid simple/keyed value object", namespaceAndName: state);
         }
         else
         {
            Logger.LogDebug("The type declaration is a valid complex value object", namespaceAndName: state);
         }

         var attributeInfo = new AttributeInfo(type);

         return new SourceGenContext(new ValidSourceGenState(state, settings, attributeInfo));
      }
      catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
      {
         throw;
      }
      catch (Exception ex)
      {
         Logger.LogError("Error during extraction of relevant information out of semantic model for generation of a value object", tds, ex);

         return new SourceGenContext(new SourceGenException(ex, tds));
      }
   }

   private readonly record struct ValidSourceGenState(
      ValueObjectSourceGeneratorState State,
      AllValueObjectSettings Settings,
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

   private readonly record struct KeyedValueObjectState(ITypeInformation Type, EqualityInstanceMemberInfo KeyMember, AllValueObjectSettings Settings);
}
