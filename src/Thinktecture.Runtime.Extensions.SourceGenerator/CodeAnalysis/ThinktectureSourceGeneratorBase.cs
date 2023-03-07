using System.Collections.Concurrent;
using System.Collections.Immutable;
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
   private readonly int _maxPooledStringBuilderInitialSize;
   private readonly ConcurrentQueue<StringBuilder> _stringBuilderPool;

   protected ThinktectureSourceGeneratorBase(int stringBuilderInitialSize)
   {
      _stringBuilderInitialSize = stringBuilderInitialSize;
      _maxPooledStringBuilderInitialSize = stringBuilderInitialSize * 2;

      _stringBuilderPool = new ConcurrentQueue<StringBuilder>();
      _stringBuilderPool.Enqueue(new StringBuilder(stringBuilderInitialSize));
   }

   protected void GenerateCode(
      SourceProductionContext context,
      (SourceGenState<TState>, ImmutableArray<ICodeGeneratorFactory<TState>>) tuple)
   {
      var ((state, exception), generatorFactories) = tuple;

      if (exception is not null)
      {
         context.ReportException(exception);
         return;
      }

      if (state is null || generatorFactories.IsDefaultOrEmpty)
         return;

      StringBuilder? stringBuilder = null;

      try
      {
         if (!_stringBuilderPool.TryDequeue(out stringBuilder))
            stringBuilder = new StringBuilder(_stringBuilderInitialSize);

         foreach (var generatorFactory in generatorFactories.Distinct())
         {
            context.CancellationToken.ThrowIfCancellationRequested();

            stringBuilder.Clear();

            var generator = generatorFactory.Create(state, stringBuilder);
            generator.Generate();

            if (stringBuilder.Length <= 0)
               continue;

            var generatedCode = stringBuilder.ToString();

            context.EmitFile(state.Namespace, state.Name, generatedCode, generator.FileNameSuffix);
         }
      }
      catch (OperationCanceledException)
      {
         throw;
      }
      catch (Exception ex)
      {
         context.ReportDiagnostic(Diagnostic.Create(DiagnosticsDescriptors.ErrorDuringGeneration,
                                                    state.GetLocationOrNullSafe(context),
                                                    state.Name, ex.Message));
      }
      finally
      {
         if (stringBuilder is not null
             && stringBuilder.Capacity < _maxPooledStringBuilderInitialSize
             && _stringBuilderPool.Count < 3)
         {
            stringBuilder.Clear();
            _stringBuilderPool.Enqueue(stringBuilder);
         }
      }
   }
}
