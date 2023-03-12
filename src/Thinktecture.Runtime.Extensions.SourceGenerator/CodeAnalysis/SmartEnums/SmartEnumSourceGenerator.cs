using System.Collections.Immutable;
using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Thinktecture.CodeAnalysis.SmartEnums;

[Generator]
public sealed class SmartEnumSourceGenerator : ThinktectureSourceGeneratorBase, IIncrementalGenerator
{
   public SmartEnumSourceGenerator()
      : base(24_000)
   {
   }

   public void Initialize(IncrementalGeneratorInitializationContext context)
   {
      var enumTypeOrError = context.SyntaxProvider
                                   .CreateSyntaxProvider(IsCandidate, GetSourceGenContext)
                                   .SelectMany(static (state, _) => state.HasValue
                                                                       ? ImmutableArray.Create(state.Value)
                                                                       : ImmutableArray<SourceGenContext>.Empty);

      var validStates = enumTypeOrError.SelectMany(static (state, _) => state.ValidState is not null
                                                                           ? ImmutableArray.Create(state.ValidState.Value)
                                                                           : ImmutableArray<ValidSourceGenState>.Empty);

      InitializeEnumTypeGeneration(context, validStates);
      InitializeDerivedTypesGeneration(context, validStates);
      InitializeFormattableCodeGenerator(context, validStates);
      InitializeComparableCodeGenerator(context, validStates);
      InitializeParsableCodeGenerator(context, validStates);
      InitializeComparisonOperatorsCodeGenerator(context, validStates);

      InitializeErrorReporting(context, enumTypeOrError);
      InitializeExceptionReporting(context, enumTypeOrError);
   }

   private void InitializeComparisonOperatorsCodeGenerator(IncrementalGeneratorInitializationContext context, IncrementalValuesProvider<ValidSourceGenState> validStates)
   {
      var comparables = validStates
                        .Select((state, _) => new ComparisonOperatorsGeneratorState(state.State,
                                                                                    state.KeyMember,
                                                                                    state.Settings.ComparisonOperators,
                                                                                    state.KeyMember.HasComparisonOperators))
                        .Where(state => state.HasKeyMemberComparisonOperators && state.ComparisonOperators != OperatorsGeneration.None)
                        .Collect()
                        .Select(static (states, _) => states.IsDefaultOrEmpty
                                                         ? ImmutableArray<ComparisonOperatorsGeneratorState>.Empty
                                                         : states.Distinct(EnumTypeOnlyComparer.Instance).ToImmutableArray())
                        .WithComparer(new SetComparer<ComparisonOperatorsGeneratorState>())
                        .SelectMany((states, _) => states)
                        .SelectMany((state, _) =>
                                    {
                                       if (ComparisonOperatorsCodeGenerator.TryGet(state.ComparisonOperators, null, out var codeGenerator))
                                          return ImmutableArray.Create((State: state, CodeGenerator: codeGenerator));

                                       return ImmutableArray<(ComparisonOperatorsGeneratorState State, IInterfaceCodeGenerator CodeGenerator)>.Empty;
                                    });

      context.RegisterSourceOutput(comparables, (ctx, tuple) => GenerateCode(ctx, tuple.State, tuple.CodeGenerator));
   }

   private void InitializeParsableCodeGenerator(IncrementalGeneratorInitializationContext context, IncrementalValuesProvider<ValidSourceGenState> validStates)
   {
      var parsables = validStates
                      .Select((state, _) => new ParsableGeneratorState(state.State,
                                                                       state.KeyMember,
                                                                       state.Settings.SkipIParsable,
                                                                       state.KeyMember.IsParsable,
                                                                       state.State.IsValidatable))
                      .Where(state => !state.SkipIParsable && (state.KeyMember.IsString() || state.IsKeyMemberParsable))
                      .Collect()
                      .Select(static (states, _) => states.IsDefaultOrEmpty
                                                       ? ImmutableArray<ParsableGeneratorState>.Empty
                                                       : states.Distinct(EnumTypeOnlyComparer.Instance).ToImmutableArray())
                      .WithComparer(new SetComparer<ParsableGeneratorState>())
                      .SelectMany((states, _) => states);

      context.RegisterSourceOutput(parsables, GenerateCode);
   }

   private void InitializeComparableCodeGenerator(IncrementalGeneratorInitializationContext context, IncrementalValuesProvider<ValidSourceGenState> validStates)
   {
      var comparables = validStates
                        .Select((state, _) => new ComparableGeneratorState(state.State,
                                                                           state.KeyMember,
                                                                           state.Settings.SkipIComparable,
                                                                           state.KeyMember.IsComparable,
                                                                           null))
                        .Where(state => state is { SkipIComparable: false, IsKeyMemberComparable: true })
                        .Collect()
                        .Select(static (states, _) => states.IsDefaultOrEmpty
                                                         ? ImmutableArray<ComparableGeneratorState>.Empty
                                                         : states.Distinct(EnumTypeOnlyComparer.Instance).ToImmutableArray())
                        .WithComparer(new SetComparer<ComparableGeneratorState>())
                        .SelectMany((states, _) => states);

      context.RegisterSourceOutput(comparables, GenerateCode);
   }

   private void InitializeFormattableCodeGenerator(IncrementalGeneratorInitializationContext context, IncrementalValuesProvider<ValidSourceGenState> validStates)
   {
      var formattables = validStates
                         .Select((state, _) => new FormattableGeneratorState(state.State,
                                                                             state.KeyMember,
                                                                             state.Settings.SkipIFormattable,
                                                                             state.KeyMember.IsFormattable))
                         .Where(state => state is { SkipIFormattable: false, IsKeyMemberFormattable: true })
                         .Collect()
                         .Select(static (states, _) => states.IsDefaultOrEmpty
                                                          ? ImmutableArray<FormattableGeneratorState>.Empty
                                                          : states.Distinct(EnumTypeOnlyComparer.Instance).ToImmutableArray())
                         .WithComparer(new SetComparer<FormattableGeneratorState>())
                         .SelectMany((states, _) => states);

      context.RegisterSourceOutput(formattables, GenerateCode);
   }

   private void InitializeEnumTypeGeneration(
      IncrementalGeneratorInitializationContext context,
      IncrementalValuesProvider<ValidSourceGenState> validStates)
   {
      var enumTypes = validStates
                      .Select((state, _) => state.State)
                      .Collect()
                      .Select(static (states, _) => states.IsDefaultOrEmpty
                                                       ? ImmutableArray<EnumSourceGeneratorState>.Empty
                                                       : states.Distinct(EnumTypeOnlyComparer<EnumSourceGeneratorState>.Instance).ToImmutableArray())
                      .WithComparer(new SetComparer<EnumSourceGeneratorState>())
                      .SelectMany((states, _) => states);

      var additionalGenerators = context.MetadataReferencesProvider
                                        .SelectMany(static (reference, _) => GetCodeGeneratorFactories(reference))
                                        .Collect()
                                        .Select(static (states, _) => states.IsDefaultOrEmpty
                                                                         ? ImmutableArray<ICodeGeneratorFactory<EnumSourceGeneratorState>>.Empty
                                                                         : states.Distinct().ToImmutableArray())
                                        .WithComparer(new SetComparer<ICodeGeneratorFactory<EnumSourceGeneratorState>>());

      context.RegisterSourceOutput(enumTypes, (ctx, state) => GenerateCode(ctx, state, SmartEnumCodeGeneratorFactory.Instance));
      context.RegisterImplementationSourceOutput(enumTypes.Combine(additionalGenerators), GenerateCode);
   }

   private void InitializeDerivedTypesGeneration(IncrementalGeneratorInitializationContext context, IncrementalValuesProvider<ValidSourceGenState> validStates)
   {
      var derivedTypes = validStates
                         .Select(static (state, _) => state.DerivedTypes)
                         .Where(static derivedTypes => derivedTypes.DerivedTypesFullyQualified.Count > 0)
                         .Collect()
                         .Select(static (states, _) => states.IsDefaultOrEmpty
                                                          ? ImmutableArray<SmartEnumDerivedTypes>.Empty
                                                          : states.Distinct(EnumTypeOnlyComparer<SmartEnumDerivedTypes>.Instance).ToImmutableArray())
                         .WithComparer(new SetComparer<SmartEnumDerivedTypes>())
                         .SelectMany((states, _) => states);

      context.RegisterImplementationSourceOutput(derivedTypes, (ctx, types) => GenerateCode(ctx, types, DerivedTypesCodeGeneratorFactory.Instance));
   }

   private void InitializeErrorReporting(IncrementalGeneratorInitializationContext context, IncrementalValuesProvider<SourceGenContext> enumTypeOrException)
   {
      var exceptions = enumTypeOrException.SelectMany(static (state, _) => state.Error is not null
                                                                              ? ImmutableArray.Create(state.Error.Value)
                                                                              : ImmutableArray<SourceGenError>.Empty);
      context.RegisterSourceOutput(exceptions, ReportError);
   }

   private void InitializeExceptionReporting(IncrementalGeneratorInitializationContext context, IncrementalValuesProvider<SourceGenContext> enumTypeOrException)
   {
      var exceptions = enumTypeOrException.SelectMany(static (state, _) => state.Exception is not null
                                                                              ? ImmutableArray.Create(state.Exception.Value)
                                                                              : ImmutableArray<SourceGenException>.Empty);
      context.RegisterSourceOutput(exceptions, ReportException);
   }

   private static ImmutableArray<ICodeGeneratorFactory<EnumSourceGeneratorState>> GetCodeGeneratorFactories(MetadataReference reference)
   {
      var factories = ImmutableArray<ICodeGeneratorFactory<EnumSourceGeneratorState>>.Empty;

      try
      {
         foreach (var module in reference.GetModules())
         {
            factories = module.Name switch
            {
               THINKTECTURE_RUNTIME_EXTENSIONS_JSON => factories.Add(JsonSmartEnumCodeGeneratorFactory.Instance),
               THINKTECTURE_RUNTIME_EXTENSIONS_NEWTONSOFT_JSON => factories.Add(NewtonsoftJsonSmartEnumCodeGeneratorFactory.Instance),
               THINKTECTURE_RUNTIME_EXTENSIONS_MESSAGEPACK => factories.Add(MessagePackSmartEnumCodeGeneratorFactory.Instance),
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
            ClassDeclarationSyntax classDeclaration when IsEnumCandidate(classDeclaration) => true,
            StructDeclarationSyntax structDeclaration when IsEnumCandidate(structDeclaration) => true,
            _ => false
         };
      }
      catch (Exception ex)
      {
         Debug.Write(ex);
         return false;
      }
   }

   private static bool IsEnumCandidate(TypeDeclarationSyntax typeDeclaration)
   {
      return typeDeclaration.IsPartial()
             && !typeDeclaration.IsGeneric()
             && typeDeclaration.IsEnumCandidate();
   }

   private static SourceGenContext? GetSourceGenContext(GeneratorSyntaxContext context, CancellationToken cancellationToken)
   {
      var tds = (TypeDeclarationSyntax)context.Node;

      try
      {
         var type = context.SemanticModel.GetDeclaredSymbol(tds, cancellationToken);

         if (type is null || type.TypeKind == TypeKind.Error || type.ContainingType is not null)
            return null;

         if (!type.IsEnum(out var enumInterfaces))
            return null;

         var enumInterface = enumInterfaces.GetValidEnumInterface(type);

         if (enumInterface is null || enumInterface.TypeKind == TypeKind.Error)
            return null;

         var keyType = enumInterface.TypeArguments[0];

         if (keyType.TypeKind == TypeKind.Error)
            return null;

         if (keyType.NullableAnnotation == NullableAnnotation.Annotated)
            return null;

         var factory = TypedMemberStateFactoryProvider.GetFactoryOrNull(context.SemanticModel.Compilation);

         if (factory is null)
            return new SourceGenContext(new SourceGenError("Could not fetch type information for code generation of a smart enum", tds));

         var settings = new EnumSettings(type.FindEnumGenerationAttribute());
         var keyProperty = settings.CreateKeyProperty(factory, keyType);
         var isValidatable = enumInterface.IsValidatableEnumInterface();
         var hasCreateInvalidItemImplementation = isValidatable && type.HasCreateInvalidItemImplementation(keyType, cancellationToken);

         var enumState = new EnumSourceGeneratorState(factory, type, keyProperty, settings.SkipToString, isValidatable, hasCreateInvalidItemImplementation, cancellationToken);
         var derivedTypes = new SmartEnumDerivedTypes(enumState.Namespace, enumState.Name, enumState.TypeFullyQualified, enumState.IsReferenceType, FindDerivedTypes(type));

         return new SourceGenContext(new ValidSourceGenState(enumState, derivedTypes, settings, keyProperty));
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

   private static IReadOnlyList<string> FindDerivedTypes(INamedTypeSymbol type)
   {
      var derivedTypes = type.FindDerivedInnerEnums();

      if (derivedTypes.Count == 0)
         return Array.Empty<string>();

      return derivedTypes
             .Select(t => t.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat))
             .Distinct()
             .ToList();
   }

   private record struct ValidSourceGenState(
      EnumSourceGeneratorState State,
      SmartEnumDerivedTypes DerivedTypes,
      EnumSettings Settings,
      IMemberState KeyMember);

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
}
