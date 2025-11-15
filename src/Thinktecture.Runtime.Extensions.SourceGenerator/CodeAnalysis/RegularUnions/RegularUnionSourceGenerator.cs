using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Thinktecture.CodeAnalysis.RegularUnions;

[Generator]
public sealed class RegularUnionSourceGenerator() : ThinktectureSourceGeneratorBase(25_000), IIncrementalGenerator
{
   public void Initialize(IncrementalGeneratorInitializationContext context)
   {
      var options = GetGeneratorOptions(context);

      SetupLogger(context, options);

      InitializeUnionSourceGen(context, options, Constants.Attributes.Union.FULL_NAME);
   }

   private void InitializeUnionSourceGen(
      IncrementalGeneratorInitializationContext context,
      IncrementalValueProvider<GeneratorOptions> options,
      string fullyQualifiedMetadataName)
   {
      var unionTypeOrError = context.SyntaxProvider
                                    .ForAttributeWithMetadataName(fullyQualifiedMetadataName,
                                                                  IsCandidate,
                                                                  GetSourceGenContextOrNull)
                                    .SelectMany(static (state, _) => state.HasValue
                                                                        ? [state.Value]
                                                                        : ImmutableArray<SourceGenContext>.Empty);

      var validStates = unionTypeOrError.SelectMany(static (state, _) => state.ValidState is not null
                                                                            ? [state.ValidState]
                                                                            : ImmutableArray<RegularUnionSourceGenState>.Empty);

      InitializeUnionTypeGeneration(context, validStates, options);

      InitializeDiagnosticReporting(context, unionTypeOrError);
      InitializeExceptionReporting(context, unionTypeOrError);
   }

   private bool IsCandidate(SyntaxNode syntaxNode, CancellationToken cancellationToken)
   {
      try
      {
         return syntaxNode is ClassDeclarationSyntax or RecordDeclarationSyntax;
      }
      catch (Exception ex)
      {
         Logger.LogError("Error during checking whether a syntax node is a discriminated union candidate", exception: ex);
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

         if (context.Attributes.Length > 1)
            return new SourceGenDiagnostic(tds, DiagnosticsDescriptors.TypeMustNotHaveMoveThanOneDiscriminatedUnionAttribute, [type.ToMinimallyQualifiedDisplayString()]);

         var attributeType = context.Attributes[0].AttributeClass;

         if (attributeType is null)
            return new SourceGenDiagnostic(tds, DiagnosticsDescriptors.ErrorDuringCodeAnalysis, [type.ToMinimallyQualifiedDisplayString(), "Could not resolve discriminated union attribute type"]);

         if (attributeType.TypeKind == TypeKind.Error)
            return new SourceGenDiagnostic(tds, DiagnosticsDescriptors.ErrorDuringCodeAnalysis, [type.ToMinimallyQualifiedDisplayString(), "UnionAttribute type has TypeKind=Error"]);

         if (attributeType.Arity != 0)
            return new SourceGenDiagnostic(tds, DiagnosticsDescriptors.ErrorDuringCodeAnalysis, [type.ToMinimallyQualifiedDisplayString(), "UnionAttribute must not have generic arguments"]);

         var derivedTypeInfos = type.FindDerivedInnerTypes();

         if (derivedTypeInfos.Count == 0)
            return null;

         var factory = TypedMemberStateFactoryProvider.GetFactoryOrNull(context.SemanticModel.Compilation);

         if (factory is null)
            return new SourceGenDiagnostic(tds, DiagnosticsDescriptors.ErrorDuringCodeAnalysis, [type.ToMinimallyQualifiedDisplayString(), "Could not fetch type information for code generation of a discriminated union"]);

         var derivedTypes = ImmutableArray.CreateBuilder<RegularUnionTypeMemberState>(derivedTypeInfos.Count);
         var singleArgCtorsPerType = GetSingleArgumentConstructors(factory, derivedTypeInfos);

         for (var i = 0; i < derivedTypeInfos.Count; i++)
         {
            var derivedTypeInfo = derivedTypeInfos[i];

            if (derivedTypeInfo.Type.Arity != 0)
               return null; // Analyzer emits DiagnosticsDescriptors.UnionDerivedTypesMustNotBeGeneric

            derivedTypes.Add(new RegularUnionTypeMemberState(derivedTypeInfo.Type, derivedTypeInfo.TypeDef, singleArgCtorsPerType[i]));
         }

         var switchMapOverloads = GetSwitchMapOverloads(type);
         var settings = new RegularUnionSettings(context.Attributes[0], switchMapOverloads);

         return new RegularUnionSourceGenState(type,
                                               derivedTypes.DrainToImmutable(),
                                               settings);
      }
      catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
      {
         throw;
      }
      catch (Exception ex)
      {
         return new SourceGenException("Error during extraction of relevant information out of semantic model for generation of a discriminated union", ex, tds);
      }
   }

   private static List<ImmutableArray<DefaultMemberState>> GetSingleArgumentConstructors(
      TypedMemberStateFactory factory,
      IReadOnlyList<DerivedTypeInfo> derivedTypeInfos)
   {
      var (typeInfoCtors, foundArgTypes) = GetTypeInfoCtors(derivedTypeInfos);

      // Remove duplicates
      var statesPerTypeInfo = new List<ImmutableArray<DefaultMemberState>>(typeInfoCtors.Count);

      for (var i = 0; i < typeInfoCtors.Count; i++)
      {
         var paramPerCtors = typeInfoCtors[i];

         ImmutableArray<DefaultMemberState>.Builder? states = null;

         for (var j = 0; j < paramPerCtors.Length; j++)
         {
            var ctorParam = paramPerCtors[j];
            var foundParam = foundArgTypes.First(p => SymbolEqualityComparer.Default.Equals(p.Type, ctorParam.Type));

            if (foundParam.Counter != 1)
               continue;

            var parameterState = new DefaultMemberState(factory.Create(ctorParam.Type), ctorParam.Name, ArgumentName.Create(ctorParam.Name));

            (states ??= ImmutableArray.CreateBuilder<DefaultMemberState>()).Add(parameterState);
         }

         statesPerTypeInfo.Add(states?.DrainToImmutable() ?? []);
      }

      return statesPerTypeInfo;
   }

   private static (IReadOnlyList<ImmutableArray<IParameterSymbol>> TypeInfoCtors, IReadOnlyList<(ITypeSymbol Type, int Counter)> ArgTypes) GetTypeInfoCtors(
      IReadOnlyList<DerivedTypeInfo> derivedTypeInfos)
   {
      var typeInfoCtors = new List<ImmutableArray<IParameterSymbol>>(derivedTypeInfos.Count);
      var argTypes = new List<(ITypeSymbol Type, int Counter)>();

      for (var i = 0; i < derivedTypeInfos.Count; i++)
      {
         var parameters = GetSingleArgumentConstructors(derivedTypeInfos[i]);

         if (parameters.IsDefaultOrEmpty)
         {
            typeInfoCtors.Add([]);
            continue;
         }

         typeInfoCtors.Add(parameters);

         for (var j = 0; j < parameters.Length; j++)
         {
            var parameter = parameters[j];
            var foundParamIndex = argTypes.FindIndex(p => SymbolEqualityComparer.Default.Equals(p.Type, parameter.Type));

            if (foundParamIndex < 0)
            {
               argTypes.Add((parameter.Type, 1));
            }
            else
            {
               var foundParam = argTypes[foundParamIndex];
               argTypes[foundParamIndex] = (foundParam.Type, foundParam.Counter + 1);
            }
         }
      }

      return (typeInfoCtors, argTypes);
   }

   private static ImmutableArray<IParameterSymbol> GetSingleArgumentConstructors(
      DerivedTypeInfo derivedTypeInfo)
   {
      var ctors = derivedTypeInfo.Type.Constructors;

      if (ctors.IsDefaultOrEmpty)
         return ImmutableArray<IParameterSymbol>.Empty;

      IParameterSymbol? first = null;
      ImmutableArray<IParameterSymbol>.Builder? builder = null;

      for (var i = 0; i < ctors.Length; i++)
      {
         var ctor = ctors[i];

         if (ctor.DeclaredAccessibility != Accessibility.Public)
            continue;

         var parameters = ctor.Parameters;

         if (parameters.IsDefaultOrEmpty || parameters.Length != 1)
            continue;

         var parameterCandidate = parameters[0];

         // Ignore copy/base/derived-type constructors
         if (SymbolEqualityComparer.Default.Equals(parameterCandidate.Type, derivedTypeInfo.Type)
             || IsBaseTypeOf(parameterCandidate.Type, derivedTypeInfo.Type)
             || IsBaseTypeOf(derivedTypeInfo.Type, parameterCandidate.Type))
            continue;

         if (first is null)
         {
            // Fast path: record the first valid parameter without allocating
            first = parameterCandidate;
         }
         else
         {
            // Second item: allocate a builder once and add both
            builder ??= ImmutableArray.CreateBuilder<IParameterSymbol>(2);

            if (builder.Count == 0)
               builder.Add(first);

            builder.Add(parameterCandidate);
         }
      }

      if (builder is not null)
         return builder.DrainToImmutable();

      return first is not null
                ? [first] // Single-item, no builder allocation
                : ImmutableArray<IParameterSymbol>.Empty;
   }

   private static bool IsBaseTypeOf(ITypeSymbol type, ITypeSymbol potentialBaseType)
   {
      var baseType = type.BaseType;

      while (baseType is not null)
      {
         if (SymbolEqualityComparer.Default.Equals(baseType, potentialBaseType))
            return true;

         baseType = baseType.BaseType;
      }

      return false;
   }

   private static ImmutableArray<RegularUnionSwitchMapOverload> GetSwitchMapOverloads(INamedTypeSymbol type)
   {
      var allAttributes = type.GetAttributes();

      if (allAttributes.IsDefaultOrEmpty)
         return [];

      ImmutableArray<RegularUnionSwitchMapOverload>.Builder? switchMapOverloads = null;

      for (var i = 0; i < allAttributes.Length; i++)
      {
         var attribute = allAttributes[i];

         if (attribute.AttributeClass?.IsUnionSwitchMapOverloadAttribute() != true)
            continue;

         var stopAtTypes = attribute.FindUnionSwitchMapOverloadStopAtTypes();

         if (stopAtTypes.Count > 0)
            (switchMapOverloads ??= ImmutableArray.CreateBuilder<RegularUnionSwitchMapOverload>(allAttributes.Length)).Add(new RegularUnionSwitchMapOverload(stopAtTypes));
      }

      return switchMapOverloads?.DrainToImmutable() ?? [];
   }

   private void InitializeUnionTypeGeneration(
      IncrementalGeneratorInitializationContext context,
      IncrementalValuesProvider<RegularUnionSourceGenState> validStates,
      IncrementalValueProvider<GeneratorOptions> options)
   {
      var unionTypes = validStates
                       .Collect()
                       .Select(static (states, _) => states.IsDefaultOrEmpty
                                                        ? ImmutableArray<RegularUnionSourceGenState>.Empty
                                                        : states.Distinct(TypeOnlyComparer.Instance))
                       .WithComparer(SetComparer<RegularUnionSourceGenState>.Instance)
                       .SelectMany((states, _) => states);

      context.RegisterSourceOutput(unionTypes.Combine(options), (ctx, tuple) => GenerateCode(ctx, tuple.Left, tuple.Right, RegularUnionCodeGeneratorFactory.Instance));
   }

   private void InitializeExceptionReporting(
      IncrementalGeneratorInitializationContext context,
      IncrementalValuesProvider<SourceGenContext> unionTypeOrException)
   {
      var exceptions = unionTypeOrException.SelectMany(static (state, _) => state.Exception is not null
                                                                               ? [state.Exception.Value]
                                                                               : ImmutableArray<SourceGenException>.Empty);
      context.RegisterSourceOutput(exceptions, ReportException);
   }

   private void InitializeDiagnosticReporting(
      IncrementalGeneratorInitializationContext context,
      IncrementalValuesProvider<SourceGenContext> unionTypeOrException)
   {
      var exceptions = unionTypeOrException.SelectMany(static (state, _) => state.Diagnostic is not null
                                                                               ? [state.Diagnostic.Value]
                                                                               : ImmutableArray<SourceGenDiagnostic>.Empty);
      context.RegisterSourceOutput(exceptions, ReportDiagnostic);
   }

   private readonly record struct SourceGenContext(
      RegularUnionSourceGenState? ValidState,
      SourceGenException? Exception,
      SourceGenDiagnostic? Diagnostic)
   {
      public static implicit operator SourceGenContext(RegularUnionSourceGenState state)
      {
         return new SourceGenContext(state, null, null);
      }

      public static implicit operator SourceGenContext(SourceGenException exception)
      {
         return new SourceGenContext(null, exception, null);
      }

      public static implicit operator SourceGenContext(SourceGenDiagnostic diagnostic)
      {
         return new SourceGenContext(null, null, diagnostic);
      }
   }
}
