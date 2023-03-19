using System.Collections.Immutable;
using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Thinktecture.Logging;

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
      SetupLogger(context);

      var valueObjectOrException = context.SyntaxProvider.CreateSyntaxProvider(IsCandidate, GetValueObjectStateOrNull)
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
      var options = GetGeneratorOptions(context);

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
                                                      .Where(tuple => tuple.Factory.MustGenerateCode(tuple.State.AttributeInfo));

      var complexSerializerGeneratorStates = validStates.SelectMany((state, _) =>
                                                                    {
                                                                       if (state.State.HasKeyMember)
                                                                          return ImmutableArray<ComplexSerializerGeneratorState>.Empty;

                                                                       var serializerState = new ComplexSerializerGeneratorState(state.State, state.State.AssignableInstanceFieldsAndProperties, state.AttributeInfo);

                                                                       return ImmutableArray.Create(serializerState);
                                                                    })
                                                        .Combine(serializerGeneratorFactories)
                                                        .SelectMany((tuple, _) => ImmutableArray.CreateRange(tuple.Right, (factory, state) => (State: state, Factory: factory), tuple.Left))
                                                        .Where(tuple => tuple.Factory.MustGenerateCode(tuple.State.AttributeInfo));

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

   private static bool IsKeyMemberNullable(ValueObjectSourceGeneratorState state)
   {
      return state.HasKeyMember && state.KeyMember.Member.NullableAnnotation == NullableAnnotation.Annotated;
   }

   private ImmutableArray<IValueObjectSerializerCodeGeneratorFactory> GetSerializerCodeGeneratorFactories(MetadataReference reference)
   {
      var factories = ImmutableArray<IValueObjectSerializerCodeGeneratorFactory>.Empty;

      try
      {
         foreach (var module in reference.GetModules())
         {
            factories = module.Name switch
            {
               THINKTECTURE_RUNTIME_EXTENSIONS_JSON => factories.Add(JsonValueObjectCodeGeneratorFactory.Instance),
               THINKTECTURE_RUNTIME_EXTENSIONS_NEWTONSOFT_JSON => factories.Add(NewtonsoftJsonValueObjectCodeGeneratorFactory.Instance),
               THINKTECTURE_RUNTIME_EXTENSIONS_MESSAGEPACK => factories.Add(MessagePackValueObjectCodeGeneratorFactory.Instance),
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
            ClassDeclarationSyntax classDeclaration when IsValueObjectCandidate(classDeclaration) => true,
            StructDeclarationSyntax structDeclaration when IsValueObjectCandidate(structDeclaration) => true,
            _ => false
         };
      }
      catch (Exception ex)
      {
         Logger.LogError("Error during checking whether a syntax node is a value object candidate", ex);
         return false;
      }
   }

   private bool IsValueObjectCandidate(TypeDeclarationSyntax typeDeclaration)
   {
      var isCandidate = typeDeclaration.IsPartial()
                        && !typeDeclaration.IsGeneric()
                        && typeDeclaration.IsValueObjectCandidate();

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

   private SourceGenContext? GetValueObjectStateOrNull(GeneratorSyntaxContext context, CancellationToken cancellationToken)
   {
      var tds = (TypeDeclarationSyntax)context.Node;

      try
      {
         var type = context.SemanticModel.GetDeclaredSymbol(tds);

         if (!type.HasValueObjectAttribute(out var valueObjectAttribute))
            return null;

         if (type.ContainingType is not null)
            return null;

         var factory = TypedMemberStateFactoryProvider.GetFactoryOrNull(context.SemanticModel.Compilation);

         if (factory is null)
            return new SourceGenContext(new SourceGenError("Could not fetch type information for code generation of a smart enum", tds));

         var settings = new AllValueObjectSettings(valueObjectAttribute);
         var state = new ValueObjectSourceGeneratorState(factory, type, new ValueObjectSettings(settings), cancellationToken);

         if (IsKeyMemberNullable(state))
            return null;

         var attributeInfo = new AttributeInfo(type);

         return new SourceGenContext(new ValidSourceGenState(state, settings, attributeInfo));
      }
      catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
      {
         throw;
      }
      catch (Exception ex)
      {
         Logger.LogError("Error during extraction of relevant information out of semantic model for generation of a value object", ex);

         return new SourceGenContext(new SourceGenException(ex, tds));
      }
   }

   private record struct ValidSourceGenState(
      ValueObjectSourceGeneratorState State,
      AllValueObjectSettings Settings,
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

   private record struct KeyedValueObjectState(ITypeInformation Type, EqualityInstanceMemberInfo KeyMember, AllValueObjectSettings Settings);
}
