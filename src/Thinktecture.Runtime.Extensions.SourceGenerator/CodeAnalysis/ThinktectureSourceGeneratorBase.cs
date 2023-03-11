using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Text;
using Microsoft.CodeAnalysis;
using Thinktecture.CodeAnalysis.SmartEnums;

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

   protected void ReportException(
      SourceProductionContext context,
      Exception exception)
   {
      try
      {
         context.ReportException(exception);
      }
      catch (Exception ex)
      {
         Debug.Write(ex);
      }
   }

   protected void GenerateCode(SourceProductionContext context, FormattableGeneratorState state) => GenerateCode(context, state.Type.Namespace, state.Type.Name, (state.Type, state.KeyMember), InterfaceCodeGeneratorFactory.Formattable);
   protected void GenerateCode(SourceProductionContext context, ComparableGeneratorState state) => GenerateCode(context, state.Type.Namespace, state.Type.Name, (state.Type, state.KeyMember), InterfaceCodeGeneratorFactory.Comparable(state.ComparerAccessor));
   protected void GenerateCode(SourceProductionContext context, ParsableGeneratorState state) => GenerateCode(context, state.Type.Namespace, state.Type.Name, (state.Type, state.KeyMember), InterfaceCodeGeneratorFactory.Parsable(state.IsValidatableEnum));
   protected void GenerateCode(SourceProductionContext context, ComparisonOperatorsGeneratorState state, IInterfaceCodeGenerator codeGenerator) => GenerateCode(context, state.Type.Namespace, state.Type.Name, (state.Type, state.KeyMember), InterfaceCodeGeneratorFactory.ComparisonOperators(codeGenerator));

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
