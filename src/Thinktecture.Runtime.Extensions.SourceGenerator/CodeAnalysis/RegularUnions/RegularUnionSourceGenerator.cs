using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Thinktecture.CodeAnalysis.RegularUnions;

[Generator]
public class RegularUnionSourceGenerator : ThinktectureSourceGeneratorBase, IIncrementalGenerator
{
   public RegularUnionSourceGenerator()
      : base(25_000)
   {
   }

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

         if (type.TypeKind == TypeKind.Error)
         {
            Logger.LogDebug("Type from semantic model is erroneous", tds);
            return null;
         }

         if (context.Attributes.IsDefaultOrEmpty)
            return null;

         if (context.Attributes.Length > 1)
         {
            Logger.LogDebug($"Type has more than 1 '{Constants.Attributes.Union.NAME}'", tds);
            return null;
         }

         var attributeType = context.Attributes[0].AttributeClass;

         if (attributeType is null)
         {
            Logger.LogDebug("The attribute type is null", tds);
            return null;
         }

         if (attributeType.TypeKind == TypeKind.Error)
         {
            Logger.LogDebug("The attribute type is erroneous", tds);
            return null;
         }

         if (attributeType.Arity != 0)
         {
            Logger.LogDebug($"Expected the union attribute type to have no type arguments but found {attributeType.Arity.ToString()}", tds);
            return null;
         }

         var derivedTypeInfos = type.FindDerivedInnerTypes();

         if (derivedTypeInfos.Count == 0)
         {
            Logger.LogDebug("Union has no derived types", tds);
            return null;
         }

         var factory = TypedMemberStateFactoryProvider.GetFactoryOrNull(context.SemanticModel.Compilation, Logger);

         if (factory is null)
            return new SourceGenContext(new SourceGenError("Could not fetch type information for code generation of a discriminated union", tds));

         var derivedTypes = new List<RegularUnionTypeMemberState>(derivedTypeInfos.Count);
         var singleArgCtorsPerType = GetSingleArgumentConstructors(factory, derivedTypeInfos);

         for (var i = 0; i < derivedTypeInfos.Count; i++)
         {
            var derivedTypeInfo = derivedTypeInfos[i];

            if (derivedTypeInfo.Type.Arity != 0)
            {
               Logger.LogDebug("Derived type of a union must not have generic parameters", tds);
               return null;
            }

            derivedTypes.Add(new RegularUnionTypeMemberState(derivedTypeInfo.Type, derivedTypeInfo.TypeDef, singleArgCtorsPerType[i]));
         }

         List<RegularUnionSwitchMapOverload>? switchMapOverloads = null;
         var allAttributes = type.GetAttributes();

         foreach (var attribute in allAttributes)
         {
            if (attribute.AttributeClass?.IsUnionSwitchMapOverloadAttribute() != true)
               continue;

            var stopAtTypes = attribute.FindUnionSwitchMapOverloadStopAtTypes();

            if (stopAtTypes.Count > 0)
               (switchMapOverloads ??= []).Add(new RegularUnionSwitchMapOverload(stopAtTypes));
         }

         var settings = new RegularUnionSettings(context.Attributes[0], switchMapOverloads ?? []);

         var unionState = new RegularUnionSourceGenState(type,
                                                         derivedTypes,
                                                         settings);

         Logger.LogDebug("The type declaration is a valid union", null, unionState);

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

   private static IReadOnlyList<IReadOnlyList<DefaultMemberState>> GetSingleArgumentConstructors(
      TypedMemberStateFactory factory,
      IReadOnlyList<DerivedTypeInfo> derivedTypeInfos)
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

            var parameterState = new DefaultMemberState(factory.Create(ctorParam.Type), ctorParam.Name, ctorParam.Name.MakeArgumentName());

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
         typeInfoCtors.Add(parameters);

         for (var j = 0; j < parameters.Count; j++)
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

   private static IReadOnlyList<IParameterSymbol> GetSingleArgumentConstructors(
      DerivedTypeInfo derivedTypeInfo)
   {
      if (derivedTypeInfo.Type.Constructors.IsDefaultOrEmpty)
         return [];

      List<IParameterSymbol>? parameters = null;

      foreach (var ctor in derivedTypeInfo.Type.Constructors)
      {
         if (ctor.DeclaredAccessibility != Accessibility.Public || ctor.Parameters.IsDefaultOrEmpty)
            continue;

         if (ctor.Parameters.Length > 1)
            continue;

         var parameterCandidate = ctor.Parameters[0];

         if (SymbolEqualityComparer.Default.Equals(parameterCandidate.Type, derivedTypeInfo.Type) // Ignore copy constructor
             || IsBaseTypeOf(parameterCandidate.Type, derivedTypeInfo.Type)                       // Ignore base type constructor
             || IsBaseTypeOf(derivedTypeInfo.Type, parameterCandidate.Type))                      // Ignore constructors with derived types
            continue;

         (parameters ??= []).Add(parameterCandidate);
      }

      return parameters ?? (IReadOnlyList<IParameterSymbol>)[];
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
