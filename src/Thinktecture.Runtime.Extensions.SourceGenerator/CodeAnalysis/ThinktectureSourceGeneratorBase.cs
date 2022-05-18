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

   protected ThinktectureSourceGeneratorBase(int stringBuilderInitialSize)
   {
      _stringBuilderInitialSize = stringBuilderInitialSize;
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

      try
      {
         var stringBuilder = new StringBuilder(_stringBuilderInitialSize);

         foreach (var generatorFactory in generatorFactories.Distinct())
         {
            stringBuilder.Clear();

            var generator = generatorFactory.Create(state, stringBuilder);
            var generatedCode = generator.Generate();

            context.EmitFile(state.Namespace, state.Name, generatedCode, generator.FileNameSuffix);
         }
      }
      catch (Exception ex)
      {
         context.ReportDiagnostic(Diagnostic.Create(DiagnosticsDescriptors.ErrorDuringGeneration,
                                                    state.GetLocationOrNullSafe(context),
                                                    state.Name, ex.Message));
      }
   }
}
