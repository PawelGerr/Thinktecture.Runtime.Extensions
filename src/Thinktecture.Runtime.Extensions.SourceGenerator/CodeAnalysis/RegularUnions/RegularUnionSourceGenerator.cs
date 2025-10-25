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

      InitializeErrorReporting(context, unionTypeOrError);
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

         if (type.TypeKind == TypeKind.Error
             || context.Attributes.IsDefaultOrEmpty
             || context.Attributes.Length != 1)
         {
            return null;
         }

         var attributeType = context.Attributes[0].AttributeClass;

         if (attributeType is null
             || attributeType.TypeKind == TypeKind.Error
             || attributeType.Arity != 0)
         {
            return null;
         }

         var derivedTypeInfos = type.FindDerivedInnerTypes();

         if (derivedTypeInfos.Count == 0)
            return null;

         var factory = TypedMemberStateFactoryProvider.GetFactoryOrNull(context.SemanticModel.Compilation);

         if (factory is null)
            return new SourceGenContext(new SourceGenError("Could not fetch type information for code generation of a discriminated union", tds));

         var derivedTypes = new List<RegularUnionTypeMemberState>(derivedTypeInfos.Count);
         var singleArgCtorsPerType = GetSingleArgumentConstructors(factory, derivedTypeInfos);

         for (var i = 0; i < derivedTypeInfos.Count; i++)
         {
            var derivedTypeInfo = derivedTypeInfos[i];

            if (derivedTypeInfo.Type.Arity != 0)
            {
               return null; // Derived type of a union must not have generic parameters
            }

            derivedTypes.Add(new RegularUnionTypeMemberState(derivedTypeInfo.Type, derivedTypeInfo.TypeDef, singleArgCtorsPerType[i]));
         }

         var switchMapOverloads = GetSwitchMapOverloads(type);
         var settings = new RegularUnionSettings(context.Attributes[0], switchMapOverloads);

         var unionState = new RegularUnionSourceGenState(type,
                                                         derivedTypes,
                                                         settings);

         return new SourceGenContext(unionState);
      }
      catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
      {
         throw;
      }
      catch (Exception ex)
      {
         Logger.LogError("Error during extraction of relevant information out of semantic model for generation of a discriminated union", tds, ex);

         return new SourceGenContext(new SourceGenException(ex, tds));
      }
   }

#pragma warning disable CA1859 // Use concrete types when possible for improved performance
   private static IReadOnlyList<IReadOnlyList<DefaultMemberState>> GetSingleArgumentConstructors(
      TypedMemberStateFactory factory,
      IReadOnlyList<DerivedTypeInfo> derivedTypeInfos)
#pragma warning restore CA1859 // Use concrete types when possible for improved performance
   {
      var (typeInfoCtors, foundArgTypes) = GetTypeInfoCtors(derivedTypeInfos);

      // Remove duplicates
      var statesPerTypeInfo = new List<IReadOnlyList<DefaultMemberState>>(typeInfoCtors.Count);

      for (var i = 0; i < typeInfoCtors.Count; i++)
      {
         var paramPerCtors = typeInfoCtors[i];

         List<DefaultMemberState>? states = null;

         for (var j = 0; j < paramPerCtors.Count; j++)
         {
            var ctorParam = paramPerCtors[j];
            var foundParam = foundArgTypes.First(p => SymbolEqualityComparer.Default.Equals(p.Type, ctorParam.Type));

            if (foundParam.Counter != 1)
               continue;

            var parameterState = new DefaultMemberState(factory.Create(ctorParam.Type), ctorParam.Name, ArgumentName.Create(ctorParam.Name));

            (states ??= []).Add(parameterState);
         }

         statesPerTypeInfo.Add(states ?? (IReadOnlyList<DefaultMemberState>)[]);
      }

      return statesPerTypeInfo;
   }

   private static (IReadOnlyList<IReadOnlyList<IParameterSymbol>> TypeInfoCtors, IReadOnlyList<(ITypeSymbol Type, int Counter)> ArgTypes) GetTypeInfoCtors(
      IReadOnlyList<DerivedTypeInfo> derivedTypeInfos)
   {
      var typeInfoCtors = new List<IReadOnlyList<IParameterSymbol>>(derivedTypeInfos.Count);
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

#pragma warning disable CA1859 // Use concrete types when possible for improved performance
   private static IReadOnlyList<RegularUnionSwitchMapOverload> GetSwitchMapOverloads(INamedTypeSymbol type)
#pragma warning restore CA1859 // Use concrete types when possible for improved performance
   {
      var allAttributes = type.GetAttributes();

      if (allAttributes.IsDefaultOrEmpty)
         return [];

      List<RegularUnionSwitchMapOverload>? switchMapOverloads = null;

      for (var i = 0; i < allAttributes.Length; i++)
      {
         var attribute = allAttributes[i];

         if (attribute.AttributeClass?.IsUnionSwitchMapOverloadAttribute() != true)
            continue;

         var stopAtTypes = attribute.FindUnionSwitchMapOverloadStopAtTypes();

         if (stopAtTypes.Count > 0)
            (switchMapOverloads ??= new(allAttributes.Length)).Add(new RegularUnionSwitchMapOverload(stopAtTypes));
      }

      return switchMapOverloads ?? [];
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
                       .WithComparer(new SetComparer<RegularUnionSourceGenState>())
                       .SelectMany((states, _) => states);

      context.RegisterSourceOutput(unionTypes.Combine(options), (ctx, tuple) => GenerateCode(ctx, tuple.Left, tuple.Right, RegularUnionCodeGeneratorFactory.Instance));
   }

   private void InitializeErrorReporting(
      IncrementalGeneratorInitializationContext context,
      IncrementalValuesProvider<SourceGenContext> unionTypeOrException)
   {
      var exceptions = unionTypeOrException.SelectMany(static (state, _) => state.Error is not null
                                                                               ? [state.Error.Value]
                                                                               : ImmutableArray<SourceGenError>.Empty);
      context.RegisterSourceOutput(exceptions, ReportError);
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

   private readonly record struct SourceGenContext(RegularUnionSourceGenState? ValidState, SourceGenException? Exception, SourceGenError? Error)
   {
      public SourceGenContext(RegularUnionSourceGenState validState)
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
