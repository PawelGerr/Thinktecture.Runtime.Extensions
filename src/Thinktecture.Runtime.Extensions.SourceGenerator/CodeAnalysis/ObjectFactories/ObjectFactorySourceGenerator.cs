using Microsoft.CodeAnalysis.CSharp.Syntax;
using Thinktecture.Logging;

namespace Thinktecture.CodeAnalysis.ObjectFactories;

[Generator]
public sealed class ObjectFactorySourceGenerator : ThinktectureSourceGeneratorBase, IIncrementalGenerator
{
   public ObjectFactorySourceGenerator()
      : base(5_000)
   {
   }

   public void Initialize(IncrementalGeneratorInitializationContext context)
   {
      var options = GetGeneratorOptions(context);

      SetupLogger(context, options);

      InitializeObjectFactorySourceGen(context, options, Constants.Attributes.ObjectFactory.FULL_NAME);
   }

   private void InitializeObjectFactorySourceGen(
      IncrementalGeneratorInitializationContext context,
      IncrementalValueProvider<GeneratorOptions> options,
      string fullyQualifiedMetadataName)
   {
      var typeOrError = context.SyntaxProvider
                               .ForAttributeWithMetadataName(fullyQualifiedMetadataName,
                                                             IsCandidate,
                                                             GetSourceGenContextOrNull)
                               .SelectMany(static (state, _) => state.HasValue
                                                                   ? [state.Value]
                                                                   : ImmutableArray<SourceGenContext>.Empty);

      var validStates = typeOrError.SelectMany(static (state, _) => state.ValidState is not null
                                                                       ? [state.ValidState]
                                                                       : ImmutableArray<ObjectFactorySourceGeneratorState>.Empty);

      InitializeFactoryGeneration(context, validStates, options);
      InitializeParsableCodeGenerator(context, validStates, options);
      InitializeSerializerGenerators(context, validStates, options);

      InitializeErrorReporting(context, typeOrError);
      InitializeExceptionReporting(context, typeOrError);
   }

   private bool IsCandidate(SyntaxNode syntaxNode, CancellationToken cancellationToken)
   {
      try
      {
         return syntaxNode switch
         {
            ClassDeclarationSyntax => true,
            StructDeclarationSyntax => true,
            RecordDeclarationSyntax => true,
            _ => false
         };
      }
      catch (Exception ex)
      {
         Logger.LogError("Error during checking whether a syntax node is a candidate for an ObjectFactory", exception: ex);
         return false;
      }
   }

   private SourceGenContext? GetSourceGenContextOrNull(GeneratorAttributeSyntaxContext context, CancellationToken cancellationToken)
   {
      var tds = (TypeDeclarationSyntax)context.TargetNode;

      try
      {
         var type = (INamedTypeSymbol)context.TargetSymbol;

         if (type.TypeKind == TypeKind.Error)
            return null;

         if (context.Attributes.IsDefaultOrEmpty)
            return null;

         var errorMessage = AttributeInfo.TryCreate(type, out var attributeInfo, out var thinktectureComponentAttribute);

         if (errorMessage is not null)
            return new SourceGenContext(new SourceGenError(errorMessage, tds));

         if (attributeInfo.ObjectFactories.IsDefaultOrEmpty)
            return null;

         var factory = TypedMemberStateFactoryProvider.GetFactoryOrNull(context.SemanticModel.Compilation);

         if (factory is null)
            return new SourceGenContext(new SourceGenError("Could not fetch type information for code generation of an ObjectFactory", tds));

         var state = new ObjectFactorySourceGeneratorState(type, attributeInfo, thinktectureComponentAttribute);

         return new SourceGenContext(state);
      }
      catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
      {
         throw;
      }
      catch (Exception ex)
      {
         Logger.LogError("Error during extraction of relevant information out of semantic model for generation of an ObjectFactory", tds, ex);

         return new SourceGenContext(new SourceGenException(ex, tds));
      }
   }

   private void InitializeFactoryGeneration(
      IncrementalGeneratorInitializationContext context,
      IncrementalValuesProvider<ObjectFactorySourceGeneratorState> validStates,
      IncrementalValueProvider<GeneratorOptions> options)
   {
      var types = validStates
                  .Collect()
                  .Select(static (states, _) => states.IsDefaultOrEmpty
                                                   ? ImmutableArray<ObjectFactorySourceGeneratorState>.Empty
                                                   : states.Distinct(TypeOnlyComparer.Instance))
                  .WithComparer(SetComparer<ObjectFactorySourceGeneratorState>.Instance)
                  .SelectMany((states, _) => states);

      context.RegisterSourceOutput(types.Combine(options), (ctx, tuple) => GenerateCode(ctx, tuple.Left, tuple.Right, ObjectFactoryCodeGeneratorFactory.Instance));
   }

   private void InitializeParsableCodeGenerator(
      IncrementalGeneratorInitializationContext context,
      IncrementalValuesProvider<ObjectFactorySourceGeneratorState> validStates,
      IncrementalValueProvider<GeneratorOptions> options)
   {
      var parsables = validStates
         .Select((state, _) =>
         {
            return new ParsableGeneratorState(state,
                                              null,
                                              state.AttributeInfo.ValidationError,
                                              state.SkipIParsable,
                                              false,
                                              false,
                                              state.AttributeInfo.ObjectFactories.Any(t => t.SpecialType == SpecialType.System_String));
         });
      base.InitializeParsableCodeGenerator(context, parsables, options);
   }

   private void InitializeSerializerGenerators(IncrementalGeneratorInitializationContext context, IncrementalValuesProvider<ObjectFactorySourceGeneratorState> validStates, IncrementalValueProvider<GeneratorOptions> options)
   {
      var factoriesOrError = context.MetadataReferencesProvider
                                    .Select((reference, _) => GetSerializerCodeGeneratorFactories(reference));

      var serializerGeneratorFactories = factoriesOrError
                                         .Where(f => f.Factories is not null)
                                         .SelectMany<SerializerCodeGeneratorFactoriesContext, IKeyedSerializerCodeGeneratorFactory>((f, _) => f.Factories!)
                                         .Collect()
                                         .Select(static (states, _) => states.IsDefaultOrEmpty
                                                                          ? ImmutableArray<IKeyedSerializerCodeGeneratorFactory>.Empty
                                                                          : states.Distinct())
                                         .WithComparer(SetComparer<IKeyedSerializerCodeGeneratorFactory>.Instance);

      var serializerGeneratorStates = validStates
                                      .Select((state, _) => new KeyedSerializerGeneratorState(
                                                 state,
                                                 null,
                                                 state.AttributeInfo,
                                                 SerializationFrameworks.All))
                                      .Combine(serializerGeneratorFactories)
                                      .SelectMany((tuple, _) => ImmutableArray.CreateRange(tuple.Right, (factory, state) => (State: state, Factory: factory), tuple.Left))
                                      .Where(tuple => tuple.Factory.MustGenerateCode(tuple.State));

      context.RegisterImplementationSourceOutput(serializerGeneratorStates.Combine(options), (ctx, tuple) => GenerateCode(ctx, tuple.Left.State, tuple.Right, tuple.Left.Factory));
      context.RegisterImplementationSourceOutput(factoriesOrError.Where(f => f.Exception is not null), (ctx, factoryOrError) =>
      {
         var exception = factoryOrError.Exception!;
         ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticsDescriptors.ErrorDuringModulesAnalysis,
                                                Location.None,
                                                exception.ToString()));
      });
   }

   private SerializerCodeGeneratorFactoriesContext GetSerializerCodeGeneratorFactories(MetadataReference reference)
   {
      try
      {
         var factories = new List<IKeyedSerializerCodeGeneratorFactory>(3);

         foreach (var module in reference.GetModules())
         {
            switch (module.Name)
            {
               case Constants.Modules.THINKTECTURE_RUNTIME_EXTENSIONS_JSON:
                  Logger.Log(LogLevel.Information, "Code generator for System.Text.Json will participate in code generation");
                  factories.Add(JsonObjectFactoryCodeGeneratorFactory.Instance);
                  break;
               case Constants.Modules.THINKTECTURE_RUNTIME_EXTENSIONS_NEWTONSOFT_JSON:
                  Logger.Log(LogLevel.Information, "Code generator for Newtonsoft.Json will participate in code generation");
                  factories.Add(NewtonsoftJsonObjectFactoryCodeGeneratorFactory.Instance);
                  break;
               case Constants.Modules.THINKTECTURE_RUNTIME_EXTENSIONS_MESSAGEPACK:
                  Logger.Log(LogLevel.Information, "Code generator for MessagePack will participate in code generation");
                  factories.Add(MessagePackObjectFactoryCodeGeneratorFactory.Instance);
                  break;
            }
         }

         return new(factories);
      }
      catch (Exception ex)
      {
         return new(ex);
      }
   }

   private void InitializeErrorReporting(
      IncrementalGeneratorInitializationContext context,
      IncrementalValuesProvider<SourceGenContext> typeOrException)
   {
      var exceptions = typeOrException.SelectMany(static (state, _) => state.Error is not null
                                                                          ? [state.Error.Value]
                                                                          : ImmutableArray<SourceGenError>.Empty);
      context.RegisterSourceOutput(exceptions, ReportError);
   }

   private void InitializeExceptionReporting(
      IncrementalGeneratorInitializationContext context,
      IncrementalValuesProvider<SourceGenContext> typeOrException)
   {
      var exceptions = typeOrException.SelectMany(static (state, _) => state.Exception is not null
                                                                          ? [state.Exception.Value]
                                                                          : ImmutableArray<SourceGenException>.Empty);
      context.RegisterSourceOutput(exceptions, ReportException);
   }

   private readonly record struct SourceGenContext(
      ObjectFactorySourceGeneratorState? ValidState,
      SourceGenException? Exception = null,
      SourceGenError? Error = null)
   {
      public SourceGenContext(SourceGenException exception)
         : this(null, exception)
      {
      }

      public SourceGenContext(SourceGenError errorMessage)
         : this(null, null, errorMessage)
      {
      }
   }

   private readonly record struct SerializerCodeGeneratorFactoriesContext(
      IReadOnlyList<IKeyedSerializerCodeGeneratorFactory>? Factories,
      Exception? Exception = null)
   {
      public SerializerCodeGeneratorFactoriesContext(IReadOnlyList<IKeyedSerializerCodeGeneratorFactory> factories)
         : this(factories, null)
      {
      }

      public SerializerCodeGeneratorFactoriesContext(Exception exception)
         : this(null, exception)
      {
      }
   }
}
