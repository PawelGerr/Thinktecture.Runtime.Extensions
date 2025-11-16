using System.Collections.Concurrent;
using System.Globalization;
using System.Text;
using Microsoft.CodeAnalysis.Diagnostics;
using Thinktecture.Logging;

namespace Thinktecture.CodeAnalysis;

public abstract class ThinktectureSourceGeneratorBase
{
   private static long _counter;

   private readonly int _stringBuilderInitialSize;
   private readonly int _maxPooledStringBuilderSize;
   private readonly ConcurrentQueue<StringBuilder> _stringBuilderPool;
   private readonly LoggerFactory _loggerFactory;

   protected ILogger Logger { get; private set; }

   protected ThinktectureSourceGeneratorBase(int stringBuilderInitialSize)
   {
      _stringBuilderInitialSize = stringBuilderInitialSize;
      _maxPooledStringBuilderSize = stringBuilderInitialSize * 2;

      _stringBuilderPool = new ConcurrentQueue<StringBuilder>();
      _stringBuilderPool.Enqueue(new StringBuilder(stringBuilderInitialSize));
      _loggerFactory = new LoggerFactory(this);
      Logger = new SelfLogErrorLogger(GetType().Name);
   }

   protected static IncrementalValueProvider<GeneratorOptions> GetGeneratorOptions(IncrementalGeneratorInitializationContext context)
   {
      return context.AnalyzerConfigOptionsProvider.Select((options, _) =>
      {
         var counterEnabled = options.GlobalOptions.TryGetValue(Constants.Configuration.COUNTER, out var counterEnabledValue)
                              && IsFeatureEnabled(counterEnabledValue);

         var generateJetbrainsAnnotations = !options.GlobalOptions.TryGetValue(Constants.Configuration.GENERATE_JETBRAINS_ANNOTATIONS, out var enabledValue)
                                            || !IsFeatureDisabled(enabledValue);

         var loggingOptions = GetLoggingOptions(options);

         return new GeneratorOptions(counterEnabled, generateJetbrainsAnnotations, loggingOptions);
      });
   }

   protected void SetupLogger(
      IncrementalGeneratorInitializationContext context,
      IncrementalValueProvider<GeneratorOptions> optionsProvider)
   {
      var logging = optionsProvider
                    .Select((options, _) => options.Logging)
                    .Select((options, _) =>
                    {
                       var logger = Logger;
                       var source = GetType().Name;

                       Logger = options is null
                                   ? new SelfLogErrorLogger(source)
                                   : _loggerFactory.CreateLogger(options.Value.Level, options.Value.FilePath, options.Value.FilePathMustBeUnique, options.Value.InitialBufferSize, source);

                       logger.Dispose();

                       return options;
                    })
                    .SelectMany((_, _) => ImmutableArray<int>.Empty); // don't emit anything

      context.RegisterSourceOutput(logging, static (_, _) =>
      {
      });
   }

   private static LoggingOptions? GetLoggingOptions(AnalyzerConfigOptionsProvider options)
   {
      if (!options.GlobalOptions.TryGetValue(Constants.Configuration.LOG_FILE_PATH, out var logFilePath))
         return null;

      if (String.IsNullOrWhiteSpace(logFilePath))
         return null;

      logFilePath = logFilePath.Trim();

      if (!options.GlobalOptions.TryGetValue(Constants.Configuration.LOG_FILE_PATH_UNIQUE, out var mustBeUnique)
          || !Boolean.TryParse(mustBeUnique, out var isLogFileUnique))
      {
         isLogFileUnique = true;
      }

      if (!options.GlobalOptions.TryGetValue(Constants.Configuration.LOG_LEVEL, out var logLevelValue)
          || !Enum.TryParse(logLevelValue, true, out LogLevel logLevel))
      {
         logLevel = LogLevel.Warning;
      }

      if (!options.GlobalOptions.TryGetValue(Constants.Configuration.LOG_INITIAL_BUFFER_SIZE, out var initialBufferSizeValue)
          || !Int32.TryParse(initialBufferSizeValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out var initialBufferSize)
          || initialBufferSize < 100)
      {
         initialBufferSize = 100;
      }

      return new LoggingOptions(logFilePath, isLogFileUnique, logLevel, initialBufferSize);
   }

   private static bool IsFeatureEnabled(string booleanValue)
   {
      if (String.IsNullOrWhiteSpace(booleanValue))
         return false;

      booleanValue = booleanValue.Trim();

      return StringComparer.OrdinalIgnoreCase.Equals("enable", booleanValue)
             || StringComparer.OrdinalIgnoreCase.Equals("enabled", booleanValue)
             || StringComparer.OrdinalIgnoreCase.Equals("true", booleanValue)
             || StringComparer.OrdinalIgnoreCase.Equals("1", booleanValue);
   }

   private static bool IsFeatureDisabled(string booleanValue)
   {
      if (String.IsNullOrWhiteSpace(booleanValue))
         return false;

      booleanValue = booleanValue.Trim();

      return StringComparer.OrdinalIgnoreCase.Equals("disable", booleanValue)
             || StringComparer.OrdinalIgnoreCase.Equals("disabled", booleanValue)
             || StringComparer.OrdinalIgnoreCase.Equals("false", booleanValue)
             || StringComparer.OrdinalIgnoreCase.Equals("0", booleanValue);
   }

   protected void ReportDiagnostic(
      SourceProductionContext context,
      SourceGenDiagnostic diagnostic)
   {
      try
      {
         context.ReportDiagnostic(Diagnostic.Create(diagnostic.Descriptor,
                                                    diagnostic.IdentifierLocation,
                                                    diagnostic.Args));
      }
      catch (Exception ex)
      {
         Logger.LogError("Error during reporting an error to Roslyn", null, ex);
      }
   }

   protected void ReportException(
      SourceProductionContext context,
      SourceGenException exception)
   {
      var node = exception.Node;

      try
      {
         Logger.LogError(exception.Message, node, exception.Exception);

         context.ReportError(node.GetLocation(), node.Identifier.Text, exception.Exception.ToString());
      }
      catch (Exception ex)
      {
         Logger.LogError("Error during reporting an error to Roslyn", node, ex);
      }
   }

   protected void InitializeFormattableCodeGenerator(
      IncrementalGeneratorInitializationContext context,
      IncrementalValuesProvider<FormattableGeneratorState> formattables,
      IncrementalValueProvider<GeneratorOptions> options)
   {
      formattables = formattables
                     .Where(state => state is { SkipIFormattable: false, IsKeyMemberFormattable: true })
                     .Collect()
                     .Select(static (states, _) => states.IsDefaultOrEmpty
                                                      ? ImmutableArray<FormattableGeneratorState>.Empty
                                                      : states.Distinct(TypeOnlyComparer.Instance))
                     .WithComparer(SetComparer<FormattableGeneratorState>.Instance)
                     .SelectMany((states, _) => states);

      context.RegisterSourceOutput(formattables.Combine(options), (ctx, state) => GenerateCode(ctx,
                                                                                               state.Left.Type.Namespace,
                                                                                               state.Left.Type.ContainingTypes,
                                                                                               state.Left.Type.Name,
                                                                                               state.Left.Type.NumberOfGenerics,
                                                                                               new InterfaceCodeGeneratorState(
                                                                                                  state.Left.Type,
                                                                                                  state.Left.KeyMember,
                                                                                                  state.Left.CreateFactoryMethodName,
                                                                                                  state.Left.GenericParameters),
                                                                                               state.Right,
                                                                                               InterfaceCodeGeneratorFactory.Formattable));
   }

   protected void InitializeComparableCodeGenerator(
      IncrementalGeneratorInitializationContext context,
      IncrementalValuesProvider<ComparableGeneratorState> comparables,
      IncrementalValueProvider<GeneratorOptions> options)
   {
      comparables = comparables
                    .Where(state => !state.SkipIComparable && (state.IsKeyMemberComparable || state.ComparerAccessor is not null))
                    .Collect()
                    .Select(static (states, _) => states.IsDefaultOrEmpty
                                                     ? ImmutableArray<ComparableGeneratorState>.Empty
                                                     : states.Distinct(TypeOnlyComparer.Instance))
                    .WithComparer(SetComparer<ComparableGeneratorState>.Instance)
                    .SelectMany((states, _) => states);

      context.RegisterSourceOutput(comparables.Combine(options), (ctx, state) => GenerateCode(ctx,
                                                                                              state.Left.Type.Namespace,
                                                                                              state.Left.Type.ContainingTypes,
                                                                                              state.Left.Type.Name,
                                                                                              state.Left.Type.NumberOfGenerics,
                                                                                              new InterfaceCodeGeneratorState(
                                                                                                 state.Left.Type,
                                                                                                 state.Left.KeyMember,
                                                                                                 state.Left.CreateFactoryMethodName,
                                                                                                 state.Left.GenericParameters),
                                                                                              state.Right,
                                                                                              InterfaceCodeGeneratorFactory.Comparable(state.Left.ComparerAccessor)));
   }

   protected void InitializeParsableCodeGenerator(
      IncrementalGeneratorInitializationContext context,
      IncrementalValuesProvider<ParsableGeneratorState> parsables,
      IncrementalValueProvider<GeneratorOptions> options)
   {
      parsables = parsables
                  .Where(state => !state.SkipIParsable && (state.KeyMember?.IsString() == true || state.IsKeyMemberParsable || state.HasStringBasedValidateMethod))
                  .Collect()
                  .Select(static (states, _) => states.IsDefaultOrEmpty
                                                   ? ImmutableArray<ParsableGeneratorState>.Empty
                                                   : states.Distinct(TypeOnlyComparer.Instance))
                  .WithComparer(SetComparer<ParsableGeneratorState>.Instance)
                  .SelectMany((states, _) => states);

      context.RegisterSourceOutput(parsables.Combine(options), (ctx, state) => GenerateCode(ctx,
                                                                                            state.Left.Type.Namespace,
                                                                                            state.Left.Type.ContainingTypes,
                                                                                            state.Left.Type.Name,
                                                                                            state.Left.Type.NumberOfGenerics,
                                                                                            state.Left,
                                                                                            state.Right,
                                                                                            InterfaceCodeGeneratorFactory.Parsable(state.Left.IsEnum)));
   }

   protected void InitializeComparisonOperatorsCodeGenerator(
      IncrementalGeneratorInitializationContext context,
      IncrementalValuesProvider<ComparisonOperatorsGeneratorState> comparables,
      IncrementalValueProvider<GeneratorOptions> options)
   {
      var operators = comparables
                      .Where(state => state.OperatorsGeneration != OperatorsGeneration.None
                                      && (state.KeyMemberOperators != ImplementedComparisonOperators.None || state.ComparerAccessor is not null || state.KeyMember.IsString()))
                      .Collect()
                      .Select(static (states, _) => states.IsDefaultOrEmpty
                                                       ? ImmutableArray<ComparisonOperatorsGeneratorState>.Empty
                                                       : states.Distinct(TypeOnlyComparer.Instance))
                      .WithComparer(SetComparer<ComparisonOperatorsGeneratorState>.Instance)
                      .SelectMany((states, _) => states)
                      .SelectMany((state, _) =>
                      {
                         if (ComparisonOperatorsCodeGenerator.TryGet(state.KeyMemberOperators, state.OperatorsGeneration, state.ComparerAccessor, out var codeGenerator))
                            return [(State: state, CodeGenerator: codeGenerator)];

                         return ImmutableArray<(ComparisonOperatorsGeneratorState State, IInterfaceCodeGenerator CodeGenerator)>.Empty;
                      });

      context.RegisterSourceOutput(operators.Combine(options), (ctx, tuple) =>
      {
         var state = tuple.Left.State;
         var generator = tuple.Left.CodeGenerator;

         GenerateCode(ctx,
                      state.Type.Namespace,
                      state.Type.ContainingTypes,
                      state.Type.Name,
                      state.Type.NumberOfGenerics,
                      new InterfaceCodeGeneratorState(
                         state.Type,
                         state.KeyMember,
                         state.CreateFactoryMethodName,
                         state.GenericParameters),
                      tuple.Right,
                      InterfaceCodeGeneratorFactory.Create(generator));
      });
   }

   protected void InitializeEqualityComparisonOperatorsCodeGenerator(
      IncrementalGeneratorInitializationContext context,
      IncrementalValuesProvider<EqualityComparisonOperatorsGeneratorState> comparables,
      IncrementalValueProvider<GeneratorOptions> options)
   {
      var operators = comparables
                      .Where(state => state.OperatorsGeneration != OperatorsGeneration.None)
                      .Collect()
                      .Select(static (states, _) => states.IsDefaultOrEmpty
                                                       ? ImmutableArray<EqualityComparisonOperatorsGeneratorState>.Empty
                                                       : states.Distinct(TypeOnlyComparer.Instance))
                      .WithComparer(SetComparer<EqualityComparisonOperatorsGeneratorState>.Instance)
                      .SelectMany((states, _) => states);

      context.RegisterSourceOutput(operators.Combine(options), (ctx, tuple) =>
      {
         if (InterfaceCodeGeneratorFactory.EqualityComparison(tuple.Left.OperatorsGeneration, tuple.Left.EqualityComparer, out var generator))
            GenerateCode(ctx,
                         tuple.Left.Type.Namespace,
                         tuple.Left.Type.ContainingTypes,
                         tuple.Left.Type.Name,
                         tuple.Left.Type.NumberOfGenerics,
                         tuple.Left,
                         tuple.Right,
                         generator);
      });
   }

   protected void InitializeOperatorsCodeGenerator(
      IncrementalGeneratorInitializationContext context,
      IncrementalValuesProvider<OperatorsGeneratorState> operators,
      IncrementalValueProvider<GeneratorOptions> options)
   {
      var operatorsWithGenerator = operators
                                   .Where(state => state.KeyMemberOperators != ImplementedOperators.None && state.OperatorsGeneration != OperatorsGeneration.None)
                                   .Collect()
                                   .Select(static (states, _) => states.IsDefaultOrEmpty
                                                                    ? ImmutableArray<OperatorsGeneratorState>.Empty
                                                                    : states.Distinct(TypeOnlyComparer.Instance))
                                   .WithComparer(SetComparer<OperatorsGeneratorState>.Instance)
                                   .SelectMany((states, _) => states)
                                   .SelectMany((state, _) =>
                                   {
                                      if (state.GeneratorProvider.TryGet(state.KeyMemberOperators, state.OperatorsGeneration, out var codeGenerator))
                                         return [(State: state, CodeGenerator: codeGenerator)];

                                      return ImmutableArray<(OperatorsGeneratorState State, IInterfaceCodeGenerator CodeGenerator)>.Empty;
                                   });

      context.RegisterSourceOutput(operatorsWithGenerator.Combine(options),
                                   (ctx, tuple) =>
                                   {
                                      var state = tuple.Left.State;
                                      var generator = tuple.Left.CodeGenerator;

                                      GenerateCode(ctx,
                                                   state.Type.Namespace,
                                                   state.Type.ContainingTypes,
                                                   state.Type.Name,
                                                   state.Type.NumberOfGenerics,
                                                   new InterfaceCodeGeneratorState(
                                                      state.Type,
                                                      state.KeyMember,
                                                      state.CreateFactoryMethodName,
                                                      state.GenericParameters),
                                                   tuple.Right,
                                                   InterfaceCodeGeneratorFactory.Create(generator));
                                   });
   }

   protected void GenerateCode<TState>(
      SourceProductionContext context,
      TState state,
      GeneratorOptions options,
      ICodeGeneratorFactory<TState> generatorFactory)
      where TState : INamespaceAndName, IEquatable<TState>
   {
      var stringBuilder = LeaseStringBuilder();

      try
      {
         GenerateCode(context, state, options, generatorFactory, stringBuilder);
      }
      finally
      {
         Return(stringBuilder);
      }
   }

   private void GenerateCode<TState>(
      SourceProductionContext context,
      TState state,
      GeneratorOptions options,
      ICodeGeneratorFactory<TState> generatorFactory,
      StringBuilder stringBuilder)
      where TState : INamespaceAndName, IEquatable<TState>
   {
      GenerateCode(context, state.Namespace, state.ContainingTypes, state.Name, state.NumberOfGenerics, state, options, generatorFactory, stringBuilder);
   }

   private void GenerateCode<TState>(
      SourceProductionContext context,
      string? ns,
      ImmutableArray<ContainingTypeState> containingTypes,
      string name,
      int numberOfGenerics,
      TState state,
      GeneratorOptions options,
      ICodeGeneratorFactory<TState> generatorFactory)
      where TState : IEquatable<TState>
   {
      var stringBuilder = LeaseStringBuilder();

      try
      {
         GenerateCode(context, ns, containingTypes, name, numberOfGenerics, state, options, generatorFactory, stringBuilder);
      }
      finally
      {
         Return(stringBuilder);
      }
   }

   private void GenerateCode<TState>(
      SourceProductionContext context,
      string? ns,
      ImmutableArray<ContainingTypeState> containingTypes,
      string name,
      int numberOfGenerics,
      TState state,
      GeneratorOptions options,
      ICodeGeneratorFactory<TState> generatorFactory,
      StringBuilder stringBuilder)
      where TState : IEquatable<TState>
   {
      try
      {
         var sbLengthBeforeActualContent = 0;

         if (options.CounterEnabled)
         {
            var counter = Interlocked.Increment(ref _counter);
            stringBuilder.Append("// COUNTER: ").AppendLine(counter.ToString(CultureInfo.InvariantCulture).PadLeft(8, ' ')).AppendLine();
            sbLengthBeforeActualContent = stringBuilder.Length;
         }

         var generator = generatorFactory.Create(state, stringBuilder);
         generator.Generate(context.CancellationToken);

         if (stringBuilder.Length <= sbLengthBeforeActualContent)
         {
            if (Logger.IsEnabled(LogLevel.Warning))
               Logger.Log(LogLevel.Warning, $"Code generator '{generator.CodeGeneratorName}' didn't emit any code for '{ns}.{name}'.");

            return;
         }

         var generatedCode = stringBuilder.ToString();

         context.EmitFile(ns, containingTypes, name, numberOfGenerics, generatedCode, generator.FileNameSuffix);
      }
      catch (OperationCanceledException) when (context.CancellationToken.IsCancellationRequested)
      {
         throw;
      }
      catch (Exception ex)
      {
         try
         {
            Logger.LogError("Error during code generation", exception: ex);

            context.ReportError(Location.None, name, ex.ToString());
         }
         catch (Exception innerEx)
         {
            Logger.LogError("Error during reporting an error to Roslyn", exception: innerEx);
         }
      }
   }

   private StringBuilder LeaseStringBuilder()
   {
      return _stringBuilderPool.TryDequeue(out var stringBuilder)
                ? stringBuilder
                : new StringBuilder(_stringBuilderInitialSize);
   }

   private void Return(StringBuilder stringBuilder)
   {
      if (stringBuilder.Capacity >= _maxPooledStringBuilderSize || _stringBuilderPool.Count >= 3)
         return;

      stringBuilder.Clear();
      _stringBuilderPool.Enqueue(stringBuilder);
   }
}
