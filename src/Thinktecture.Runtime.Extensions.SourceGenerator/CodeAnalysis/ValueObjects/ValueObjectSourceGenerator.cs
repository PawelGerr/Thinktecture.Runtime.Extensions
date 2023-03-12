using System.Collections.Immutable;
using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
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

      InitializeValueObjectsGeneration(context, validStates);
      InitializeFormattableCodeGenerator(context, keyedValueObjects);
      InitializeComparableCodeGenerator(context, keyedValueObjects);
      InitializeParsableCodeGenerator(context, keyedValueObjects);
      InitializeComparisonOperatorsCodeGenerator(context, keyedValueObjects);
      InitializeAdditionOperatorsCodeGenerator(context, keyedValueObjects);
      InitializeSubtractionOperatorsCodeGenerator(context, keyedValueObjects);
      InitializeMultiplyOperatorsCodeGenerator(context, keyedValueObjects);
      InitializeDivisionOperatorsCodeGenerator(context, keyedValueObjects);

      InitializeErrorReporting(context, valueObjectOrException);
      InitializeExceptionReporting(context, valueObjectOrException);
   }

   private void InitializeValueObjectsGeneration(IncrementalGeneratorInitializationContext context, IncrementalValuesProvider<ValidSourceGenState> validStates)
   {
      var valueObjects = validStates
                         .Select((state, _) => state.State)
                         .Collect()
                         .Select(static (states, _) => states.IsDefaultOrEmpty
                                                          ? ImmutableArray<ValueObjectSourceGeneratorState>.Empty
                                                          : states.Distinct(TypeOnlyComparer<ValueObjectSourceGeneratorState>.Instance).ToImmutableArray())
                         .WithComparer(new SetComparer<ValueObjectSourceGeneratorState>())
                         .SelectMany((states, _) => states);

      var additionalGenerators = context.MetadataReferencesProvider
                                        .SelectMany(static (reference, _) => GetCodeGeneratorFactories(reference))
                                        .Collect()
                                        .Select(static (states, _) => states.IsDefaultOrEmpty
                                                                         ? ImmutableArray<ICodeGeneratorFactory<ValueObjectSourceGeneratorState>>.Empty
                                                                         : states.Distinct().ToImmutableArray())
                                        .WithComparer(new SetComparer<ICodeGeneratorFactory<ValueObjectSourceGeneratorState>>());

      context.RegisterSourceOutput(valueObjects, (ctx, state) => GenerateCode(ctx, state, ValueObjectCodeGeneratorFactory.Instance));
      context.RegisterImplementationSourceOutput(valueObjects.Combine(additionalGenerators), GenerateCode);
   }

   private void InitializeFormattableCodeGenerator(IncrementalGeneratorInitializationContext context, IncrementalValuesProvider<KeyedValueObjectState> validStates)
   {
      var formattables = validStates
         .Select((state, _) => new FormattableGeneratorState(state.Type,
                                                             state.KeyMember.Member,
                                                             state.Settings.SkipIFormattable,
                                                             state.KeyMember.Member.IsFormattable));

      InitializeFormattableCodeGenerator(context, formattables);
   }

   private void InitializeComparableCodeGenerator(IncrementalGeneratorInitializationContext context, IncrementalValuesProvider<KeyedValueObjectState> validStates)
   {
      var comparables = validStates
         .Select((state, _) => new ComparableGeneratorState(state.Type,
                                                            state.KeyMember.Member,
                                                            state.Settings.SkipIComparable,
                                                            state.KeyMember.Member.IsComparable,
                                                            state.KeyMember.ComparerAccessor));

      InitializeComparableCodeGenerator(context, comparables);
   }

   private void InitializeParsableCodeGenerator(IncrementalGeneratorInitializationContext context, IncrementalValuesProvider<KeyedValueObjectState> validStates)
   {
      var parsables = validStates
         .Select((state, _) => new ParsableGeneratorState(state.Type,
                                                          state.KeyMember.Member,
                                                          state.Settings.SkipIParsable,
                                                          state.KeyMember.Member.IsParsable,
                                                          false));

      InitializeParsableCodeGenerator(context, parsables);
   }

   private void InitializeComparisonOperatorsCodeGenerator(IncrementalGeneratorInitializationContext context, IncrementalValuesProvider<KeyedValueObjectState> validStates)
   {
      var comparables = validStates
         .Select((state, _) => new ComparisonOperatorsGeneratorState(state.Type,
                                                                     state.KeyMember.Member,
                                                                     state.Settings.ComparisonOperators,
                                                                     state.KeyMember.Member.HasComparisonOperators,
                                                                     state.KeyMember.ComparerAccessor));

      InitializeComparisonOperatorsCodeGenerator(context, comparables);
   }

   private void InitializeAdditionOperatorsCodeGenerator(IncrementalGeneratorInitializationContext context, IncrementalValuesProvider<KeyedValueObjectState> validStates)
   {
      var operators = validStates
         .Select((state, _) => new OperatorsGeneratorState(state.Type,
                                                           state.KeyMember.Member,
                                                           state.Settings.AdditionOperators,
                                                           state.KeyMember.Member.HasAdditionOperators,
                                                           AdditionOperatorsCodeGeneratorProvider.Instance));

      InitializeOperatorsCodeGenerator(context, operators);
   }

   private void InitializeSubtractionOperatorsCodeGenerator(IncrementalGeneratorInitializationContext context, IncrementalValuesProvider<KeyedValueObjectState> validStates)
   {
      var operators = validStates
         .Select((state, _) => new OperatorsGeneratorState(state.Type,
                                                           state.KeyMember.Member,
                                                           state.Settings.SubtractionOperators,
                                                           state.KeyMember.Member.HasSubtractionOperators,
                                                           SubtractionOperatorsCodeGeneratorProvider.Instance));

      InitializeOperatorsCodeGenerator(context, operators);
   }

   private void InitializeMultiplyOperatorsCodeGenerator(IncrementalGeneratorInitializationContext context, IncrementalValuesProvider<KeyedValueObjectState> validStates)
   {
      var operators = validStates
         .Select((state, _) => new OperatorsGeneratorState(state.Type,
                                                           state.KeyMember.Member,
                                                           state.Settings.MultiplyOperators,
                                                           state.KeyMember.Member.HasMultiplyOperators,
                                                           MultiplyOperatorsCodeGeneratorProvider.Instance));

      InitializeOperatorsCodeGenerator(context, operators);
   }

   private void InitializeDivisionOperatorsCodeGenerator(IncrementalGeneratorInitializationContext context, IncrementalValuesProvider<KeyedValueObjectState> validStates)
   {
      var operators = validStates
         .Select((state, _) => new OperatorsGeneratorState(state.Type,
                                                           state.KeyMember.Member,
                                                           state.Settings.DivisionOperators,
                                                           state.KeyMember.Member.HasDivisionOperators,
                                                           DivisionOperatorsCodeGeneratorProvider.Instance));

      InitializeOperatorsCodeGenerator(context, operators);
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

   private static ImmutableArray<ICodeGeneratorFactory<ValueObjectSourceGeneratorState>> GetCodeGeneratorFactories(MetadataReference reference)
   {
      var factories = ImmutableArray<ICodeGeneratorFactory<ValueObjectSourceGeneratorState>>.Empty;

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
         Debug.Write(ex);
      }

      return factories;
   }

   private static bool IsCandidate(SyntaxNode syntaxNode, CancellationToken cancellationToken)
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
         Debug.Write(ex);
         return false;
      }
   }

   private static bool IsValueObjectCandidate(TypeDeclarationSyntax typeDeclaration)
   {
      return typeDeclaration.IsPartial()
             && !typeDeclaration.IsGeneric()
             && typeDeclaration.IsValueObjectCandidate();
   }

   private static SourceGenContext? GetValueObjectStateOrNull(GeneratorSyntaxContext context, CancellationToken cancellationToken)
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

         return new SourceGenContext(new ValidSourceGenState(state, settings));
      }
      catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
      {
         throw;
      }
      catch (Exception ex)
      {
         return new SourceGenContext(new SourceGenException(ex, tds));
      }
   }

   private record struct ValidSourceGenState(
      ValueObjectSourceGeneratorState State,
      AllValueObjectSettings Settings);

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
