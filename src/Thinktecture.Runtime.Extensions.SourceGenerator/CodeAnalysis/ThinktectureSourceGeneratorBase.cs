using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Text;
using Microsoft.CodeAnalysis;

namespace Thinktecture.CodeAnalysis;

public abstract class ThinktectureSourceGeneratorBase
{
   internal const string THINKTECTURE_RUNTIME_EXTENSIONS_JSON = "Thinktecture.Runtime.Extensions.Json.dll";
   internal const string THINKTECTURE_RUNTIME_EXTENSIONS_NEWTONSOFT_JSON = "Thinktecture.Runtime.Extensions.Newtonsoft.Json.dll";
   internal const string THINKTECTURE_RUNTIME_EXTENSIONS_MESSAGEPACK = "Thinktecture.Runtime.Extensions.MessagePack.dll";

   private readonly int _stringBuilderInitialSize;
   private readonly int _maxPooledStringBuilderSize;
   private readonly ConcurrentQueue<StringBuilder> _stringBuilderPool;

   protected ThinktectureSourceGeneratorBase(int stringBuilderInitialSize)
   {
      _stringBuilderInitialSize = stringBuilderInitialSize;
      _maxPooledStringBuilderSize = stringBuilderInitialSize * 2;

      _stringBuilderPool = new ConcurrentQueue<StringBuilder>();
      _stringBuilderPool.Enqueue(new StringBuilder(stringBuilderInitialSize));
   }

   protected void ReportError(
      SourceProductionContext context,
      SourceGenError error)
   {
      try
      {
         context.ReportError(error.Node, error.Message);
      }
      catch (Exception ex)
      {
         Debug.Write(ex);
      }
   }

   protected void ReportException(
      SourceProductionContext context,
      SourceGenException exception)
   {
      try
      {
         context.ReportError(exception.Node, exception.Exception.ToString());
      }
      catch (Exception ex)
      {
         Debug.Write(ex);
      }
   }

   protected void InitializeFormattableCodeGenerator(IncrementalGeneratorInitializationContext context, IncrementalValuesProvider<FormattableGeneratorState> formattables)
   {
      formattables = formattables
                     .Where(state => state is { SkipIFormattable: false, IsKeyMemberFormattable: true })
                     .Collect()
                     .Select(static (states, _) => states.IsDefaultOrEmpty
                                                      ? ImmutableArray<FormattableGeneratorState>.Empty
                                                      : states.Distinct(TypeOnlyComparer.Instance).ToImmutableArray())
                     .WithComparer(new SetComparer<FormattableGeneratorState>())
                     .SelectMany((states, _) => states);

      context.RegisterSourceOutput(formattables, (ctx, state) => GenerateCode(ctx, state.Type.Namespace, state.Type.Name, (state.Type, state.KeyMember), InterfaceCodeGeneratorFactory.Formattable));
   }

   protected void InitializeComparableCodeGenerator(IncrementalGeneratorInitializationContext context, IncrementalValuesProvider<ComparableGeneratorState> comparables)
   {
      comparables = comparables
                    .Where(state => !state.SkipIComparable && (state.IsKeyMemberComparable || state.ComparerAccessor is not null))
                    .Collect()
                    .Select(static (states, _) => states.IsDefaultOrEmpty
                                                     ? ImmutableArray<ComparableGeneratorState>.Empty
                                                     : states.Distinct(TypeOnlyComparer.Instance).ToImmutableArray())
                    .WithComparer(new SetComparer<ComparableGeneratorState>())
                    .SelectMany((states, _) => states);

      context.RegisterSourceOutput(comparables, (ctx, state) => GenerateCode(ctx, state.Type.Namespace, state.Type.Name, (state.Type, state.KeyMember), InterfaceCodeGeneratorFactory.Comparable(state.ComparerAccessor)));
   }

   protected void InitializeParsableCodeGenerator(IncrementalGeneratorInitializationContext context, IncrementalValuesProvider<ParsableGeneratorState> parsables)
   {
      parsables = parsables
                  .Where(state => !state.SkipIParsable && (state.KeyMember.IsString() || state.IsKeyMemberParsable))
                  .Collect()
                  .Select(static (states, _) => states.IsDefaultOrEmpty
                                                   ? ImmutableArray<ParsableGeneratorState>.Empty
                                                   : states.Distinct(TypeOnlyComparer.Instance).ToImmutableArray())
                  .WithComparer(new SetComparer<ParsableGeneratorState>())
                  .SelectMany((states, _) => states);

      context.RegisterSourceOutput(parsables, (ctx, state) => GenerateCode(ctx, state.Type.Namespace, state.Type.Name, (state.Type, state.KeyMember), InterfaceCodeGeneratorFactory.Parsable(state.IsValidatableEnum)));
   }

   protected void InitializeComparisonOperatorsCodeGenerator(IncrementalGeneratorInitializationContext context, IncrementalValuesProvider<ComparisonOperatorsGeneratorState> comparables)
   {
      var operators = comparables
                      .Where(state => state.HasKeyMemberOperators && state.OperatorsGeneration != OperatorsGeneration.None)
                      .Collect()
                      .Select(static (states, _) => states.IsDefaultOrEmpty
                                                       ? ImmutableArray<ComparisonOperatorsGeneratorState>.Empty
                                                       : states.Distinct(TypeOnlyComparer.Instance).ToImmutableArray())
                      .WithComparer(new SetComparer<ComparisonOperatorsGeneratorState>())
                      .SelectMany((states, _) => states)
                      .SelectMany((state, _) =>
                                  {
                                     if (ComparisonOperatorsCodeGenerator.TryGet(state.OperatorsGeneration, state.ComparerAccessor, out var codeGenerator))
                                        return ImmutableArray.Create((State: state, CodeGenerator: codeGenerator));

                                     return ImmutableArray<(ComparisonOperatorsGeneratorState State, IInterfaceCodeGenerator CodeGenerator)>.Empty;
                                  });

      context.RegisterSourceOutput(operators, (ctx, tuple) =>
                                              {
                                                 var state = tuple.State;
                                                 var generator = tuple.CodeGenerator;

                                                 GenerateCode(ctx, state.Type.Namespace, state.Type.Name, (state.Type, state.KeyMember), InterfaceCodeGeneratorFactory.Create(generator));
                                              });
   }

   protected void InitializeOperatorsCodeGenerator(IncrementalGeneratorInitializationContext context, IncrementalValuesProvider<OperatorsGeneratorState> operators)
   {
      var operatorsWithGenerator = operators
                                   .Where(state => state.HasKeyMemberOperators && state.OperatorsGeneration != OperatorsGeneration.None)
                                   .Collect()
                                   .Select(static (states, _) => states.IsDefaultOrEmpty
                                                                    ? ImmutableArray<OperatorsGeneratorState>.Empty
                                                                    : states.Distinct(TypeOnlyComparer.Instance).ToImmutableArray())
                                   .WithComparer(new SetComparer<OperatorsGeneratorState>())
                                   .SelectMany((states, _) => states)
                                   .SelectMany((state, _) =>
                                               {
                                                  if (state.GeneratorProvider.TryGet(state.OperatorsGeneration, out var codeGenerator))
                                                     return ImmutableArray.Create((State: state, CodeGenerator: codeGenerator));

                                                  return ImmutableArray<(OperatorsGeneratorState State, IInterfaceCodeGenerator CodeGenerator)>.Empty;
                                               });

      context.RegisterSourceOutput(operatorsWithGenerator, (ctx, tuple) =>
                                                           {
                                                              var state = tuple.State;
                                                              var generator = tuple.CodeGenerator;

                                                              GenerateCode(ctx, state.Type.Namespace, state.Type.Name, (state.Type, state.KeyMember), InterfaceCodeGeneratorFactory.Create(generator));
                                                           });
   }

   protected void GenerateCode<TState>(
      SourceProductionContext context,
      TState state,
      ICodeGeneratorFactory<TState> generatorFactory)
      where TState : INamespaceAndName, IEquatable<TState>
   {
      var stringBuilder = LeaseStringBuilder();

      try
      {
         GenerateCode(context, state, generatorFactory, stringBuilder);
      }
      finally
      {
         Return(stringBuilder);
      }
   }

   protected void GenerateCode<TState>(
      SourceProductionContext context,
      (TState, ImmutableArray<ICodeGeneratorFactory<TState>>) tuple)
      where TState : INamespaceAndName, IEquatable<TState>
   {
      var stringBuilder = LeaseStringBuilder();

      try
      {
         var (state, generatorFactories) = tuple;

         for (var i = 0; i < generatorFactories.Length; i++)
         {
            context.CancellationToken.ThrowIfCancellationRequested();
            stringBuilder.Clear();

            GenerateCode(context, state, generatorFactories[i], stringBuilder);
         }
      }
      finally
      {
         Return(stringBuilder);
      }
   }

   private static void GenerateCode<TState>(
      SourceProductionContext context,
      TState state,
      ICodeGeneratorFactory<TState> generatorFactory,
      StringBuilder stringBuilder)
      where TState : INamespaceAndName, IEquatable<TState>
   {
      GenerateCode(context, state.Namespace, state.Name, state, generatorFactory, stringBuilder);
   }

   private void GenerateCode<TState>(
      SourceProductionContext context,
      string? ns,
      string name,
      TState state,
      ICodeGeneratorFactory<TState> generatorFactory)
      where TState : IEquatable<TState>
   {
      var stringBuilder = LeaseStringBuilder();

      try
      {
         GenerateCode(context, ns, name, state, generatorFactory, stringBuilder);
      }
      finally
      {
         Return(stringBuilder);
      }
   }

   private static void GenerateCode<TState>(
      SourceProductionContext context,
      string? ns,
      string name,
      TState state,
      ICodeGeneratorFactory<TState> generatorFactory,
      StringBuilder stringBuilder)
      where TState : IEquatable<TState>
   {
      try
      {
         var generator = generatorFactory.Create(state, stringBuilder);
         generator.Generate(context.CancellationToken);

         if (stringBuilder.Length <= 0)
            return;

         var generatedCode = stringBuilder.ToString();

         context.EmitFile(ns, name, generatedCode, generator.FileNameSuffix);
      }
      catch (OperationCanceledException) when (context.CancellationToken.IsCancellationRequested)
      {
         throw;
      }
      catch (Exception ex)
      {
         context.ReportDiagnostic(Diagnostic.Create(DiagnosticsDescriptors.ErrorDuringGeneration,
                                                    Location.None,
                                                    name, ex.Message));
      }
   }

   protected StringBuilder LeaseStringBuilder()
   {
      return _stringBuilderPool.TryDequeue(out var stringBuilder)
                ? stringBuilder
                : new StringBuilder(_stringBuilderInitialSize);
   }

   protected void Return(StringBuilder stringBuilder)
   {
      if (stringBuilder.Capacity >= _maxPooledStringBuilderSize || _stringBuilderPool.Count >= 3)
         return;

      stringBuilder.Clear();
      _stringBuilderPool.Enqueue(stringBuilder);
   }
}
