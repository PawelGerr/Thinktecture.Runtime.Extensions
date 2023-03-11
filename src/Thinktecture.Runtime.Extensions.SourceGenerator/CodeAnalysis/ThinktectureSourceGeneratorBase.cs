using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Text;
using Microsoft.CodeAnalysis;

namespace Thinktecture.CodeAnalysis;

public abstract class ThinktectureSourceGeneratorBase<TState>
   where TState : ISourceGeneratorState, IEquatable<TState>
{
   internal const string THINKTECTURE_RUNTIME_EXTENSIONS = "Thinktecture.Runtime.Extensions.dll";
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

   protected void GenerateCode(
      SourceProductionContext context,
      TState state,
      ICodeGeneratorFactory<TState> generatorFactory)
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

   protected void GenerateCode(
      SourceProductionContext context,
      (TState, ImmutableArray<ICodeGeneratorFactory<TState>>) tuple)
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

   private static void GenerateCode(
      SourceProductionContext context,
      TState state,
      ICodeGeneratorFactory<TState> generatorFactory,
      StringBuilder stringBuilder)
   {
      try
      {
         var generator = generatorFactory.Create(state, stringBuilder);
         generator.Generate(context.CancellationToken);

         if (stringBuilder.Length <= 0)
            return;

         var generatedCode = stringBuilder.ToString();

         context.EmitFile(state.Namespace, state.Name, generatedCode, generator.FileNameSuffix);
      }
      catch (OperationCanceledException) when (context.CancellationToken.IsCancellationRequested)
      {
         throw;
      }
      catch (Exception ex)
      {
         context.ReportDiagnostic(Diagnostic.Create(DiagnosticsDescriptors.ErrorDuringGeneration,
                                                    Location.None,
                                                    state.Name, ex.Message));
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
