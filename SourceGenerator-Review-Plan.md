# Thinktecture.Runtime.Extensions.SourceGenerator – Review Plan

Scope: Only source files in src/Thinktecture.Runtime.Extensions.SourceGenerator. Build outputs (bin/, obj/, nupkg) are excluded from review.

Read .clinerules to get more context about the project.

Legend:
- [ ] = to review
- [x] = done (you will check off as you proceed)

## 0. Project metadata and global helpers
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/Thinktecture.Runtime.Extensions.SourceGenerator.csproj
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/Thinktecture.Runtime.Extensions.SourceGenerator.csproj.DotSettings
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/NullableAttributes.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/IsExternalInit.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/IHashCodeComputable.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/ReusableHashSet.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/SelectWhere.cs

## 1. Json and Properties
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/Json/JsonIgnoreCondition.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/Properties/launchSettings.json

## 2. Logging (used by generators; read in this order)
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/Logging/LogLevel.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/Logging/ILogger.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/Logging/ILoggingSink.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/Logging/LoggerBase.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/Logging/LoggerFactory.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/Logging/SelfLog.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/Logging/SelfLogErrorLogger.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/Logging/TraceLogger.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/Logging/DebugLogger.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/Logging/InformationLogger.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/Logging/WarningLogger.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/Logging/ErrorLogger.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/Logging/FileSystemSinkContext.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/Logging/FileSystemSinkProvider.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/Logging/FileSystemLoggingSink.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/Logging/PeriodicCleanup.cs

## 3. Roslyn and utility Extensions (general → symbol → syntax → misc)
General collections/strings:
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/Extensions/ReadOnlyCollectionExtensions.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/Extensions/EnumerableExtension.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/Extensions/ImmutableArrayExtensions.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/Extensions/StringExtensions.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/Extensions/StringBuilderExtensions.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/Extensions/EnumExtensions.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/Extensions/NullableExtensions.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/Extensions/ArgumentName.cs

Roslyn symbol/type:
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/Extensions/SymbolExtensions.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/Extensions/NamedTypeSymbolExtensions.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/Extensions/TypeSymbolExtensions.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/Extensions/MethodSymbolExtensions.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/Extensions/PropertySymbolExtensions.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/Extensions/FieldSymbolExtensions.cs
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/Extensions/TypeParameterSymbolExtensions.cs
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/Extensions/ImplementedOperatorsExtensions.cs
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/Extensions/ImplementedComparisonOperatorsExtensions.cs
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/Extensions/MemberInformationExtensions.cs
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/Extensions/KeyMemberSettingsExtensions.cs
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/Extensions/MetadataReferenceExtensions.cs

Roslyn syntax:
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/Extensions/TypeDeclarationSyntaxExtensions.cs
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/Extensions/MemberDeclarationSyntaxExtensions.cs
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/Extensions/SyntaxListExtensions.cs
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/Extensions/SyntaxTokenListExtensions.cs
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/Extensions/SeparatedSyntaxListExtensions.cs
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/Extensions/ObjectCreationOperationExtensions.cs
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/Extensions/AttributeDataExtensions.cs

Misc/source-gen plumbing:
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/Extensions/SerializationFrameworksExtensions.cs
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/Extensions/SourceProductionContextExtensions.cs
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/Extensions/ContainingTypesExtensions.cs

## 4. CodeAnalysis – Core abstractions, states, utilities
Start with constants/enums/options:
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/Constants.cs
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/SerializationFrameworks.cs
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/OperatorsGeneration.cs
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/ConversionOperatorsGeneration.cs
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/SwitchMapMethodsGeneration.cs
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/UnionConstructorAccessModifier.cs
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/ValueObjectAccessModifier.cs
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/ValueObjectMemberKind.cs
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/LoggingOptions.cs
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/GeneratorOptions.cs

Core interfaces (type info, members, codegen):
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/ITypeInformation.cs
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/ITypeInformationWithNullability.cs
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/ITypeKindInformation.cs
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/ITypeFullyQualified.cs
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/ITypeMinimallyQualified.cs
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/ITypeInformationProvider.cs
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/INamespaceAndName.cs
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/IHasGenerics.cs
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/ITypedMemberState.cs
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/IMemberState.cs
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/IMemberInformation.cs
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/IParsableTypeInformation.cs
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/ICodeGeneratorFactory.cs
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/IInterfaceCodeGenerator.cs
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/IKeyedSerializerCodeGeneratorFactory.cs
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/IComplexSerializerCodeGeneratorFactory.cs
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/IKeyedSerializerGeneratorTypeInformation.cs

States and metadata:
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/AttributeInfo.cs
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/GenericTypeParameterState.cs
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/ContainingTypeState.cs
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/KeyMemberState.cs
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/ValidationErrorState.cs
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/DefaultMemberState.cs
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/InstanceMemberInfo.cs
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/EqualityInstanceMemberInfo.cs
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/DerivedTypeInfo.cs
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/ParameterState.cs
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/ObjectFactoryState.cs
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/ImplementedOperators.cs
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/ImplementedComparisonOperators.cs
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/DelegateMethodState.cs

Typed member state fabrication:
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/TypedMemberState.cs
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/TypedMemberStateFactory.cs
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/TypedMemberStateFactoryProvider.cs
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/CachedTypedMemberState.cs

Comparers/utilities:
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/TypeInformationComparer.cs
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/TypeOnlyComparer.cs
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/MemberInformationComparer.cs
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/ComparerInfo.cs
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/SetComparer.cs
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/Helper.cs
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/ModuleInfo.cs
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/SourceGenError.cs
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/SourceGenException.cs

Interface and common code generation (reused by features):
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/CodeGeneratorBase.cs
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/InterfaceCodeGeneratorFactory.cs
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/InterfaceCodeGeneratorState.cs
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/ParsableCodeGenerator.cs
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/ParsableGeneratorState.cs
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/ComparableCodeGenerator.cs
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/ComparableGeneratorState.cs
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/FormattableCodeGenerator.cs
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/FormattableGeneratorState.cs
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/ComparisonOperatorsCodeGenerator.cs
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/ComparisonOperatorsGeneratorState.cs
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/EqualityComparisonOperatorsCodeGenerator.cs
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/EqualityComparisonOperatorsGeneratorState.cs
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/OperatorsGeneratorState.cs
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/KeyedSerializerGeneratorState.cs
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/KeyedJsonCodeGenerator.cs
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/KeyedMessagePackCodeGenerator.cs
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/KeyedNewtonsoftJsonCodeGenerator.cs
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/SmartEnumAndValueObjectCodeGeneratorBase.cs
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/ThinktectureSourceGeneratorBase.cs

Diagnostics descriptors used by analyzers:
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/DiagnosticsDescriptors.cs

## 5. Smart Enums generator family
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/SmartEnums/SmartEnumSourceGenerator.cs
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/SmartEnums/SmartEnumSourceGeneratorState.cs
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/SmartEnums/SmartEnumSettings.cs
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/SmartEnums/AllEnumSettings.cs
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/SmartEnums/SmartEnumDerivedTypes.cs
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/SmartEnums/BaseTypeState.cs
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/SmartEnums/ConstructorState.cs
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/SmartEnums/EnumItem.cs
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/SmartEnums/EnumItems.cs
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/SmartEnums/SmartEnumCodeGeneratorFactory.cs
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/SmartEnums/SmartEnumCodeGenerator.cs
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/SmartEnums/InterfaceCodeGenerator.cs
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/SmartEnums/JsonSmartEnumCodeGeneratorFactory.cs
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/SmartEnums/MessagePackSmartEnumCodeGeneratorFactory.cs
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/SmartEnums/NewtonsoftJsonSmartEnumCodeGeneratorFactory.cs

## 6. Value Objects generator family
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/ValueObjects/ValueObjectSourceGenerator.cs
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/ValueObjects/KeyedValueObjectSourceGeneratorState.cs
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/ValueObjects/ComplexValueObjectSourceGeneratorState.cs
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/ValueObjects/ValueObjectSettings.cs
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/ValueObjects/AllValueObjectSettings.cs
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/ValueObjects/ValueObjectMemberSettings.cs
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/ValueObjects/KeyedValueObjectCodeGenerator.cs
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/ValueObjects/ComplexValueObjectCodeGenerator.cs
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/ValueObjects/ComplexSerializerGeneratorState.cs
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/ValueObjects/JsonValueObjectCodeGeneratorFactory.cs
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/ValueObjects/MessagePackValueObjectCodeGeneratorFactory.cs
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/ValueObjects/NewtonsoftJsonValueObjectCodeGeneratorFactory.cs
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/ValueObjects/ValueObjectCodeGeneratorFactory.cs
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/ValueObjects/IOperatorsCodeGeneratorProvider.cs
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/ValueObjects/AdditionOperatorsCodeGeneratorProvider.cs
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/ValueObjects/SubtractionOperatorsCodeGeneratorProvider.cs
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/ValueObjects/MultiplyOperatorsCodeGeneratorProvider.cs
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/ValueObjects/DivisionOperatorsCodeGeneratorProvider.cs
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/ValueObjects/AdditionOperatorsCodeGenerator.cs
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/ValueObjects/SubtractionOperatorsCodeGenerator.cs
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/ValueObjects/MultiplyOperatorsCodeGenerator.cs
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/ValueObjects/DivisionOperatorsCodeGenerator.cs
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/ValueObjects/IValueObjectSerializerCodeGeneratorFactory.cs

## 7. Ad-hoc Unions generator family
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/AdHocUnions/AdHocUnionSourceGenerator.cs
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/AdHocUnions/AdHocUnionSourceGenState.cs
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/AdHocUnions/AdHocUnionSettings.cs
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/AdHocUnions/AdHocUnionMemberTypeSetting.cs
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/AdHocUnions/AdHocUnionMemberTypeState.cs
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/AdHocUnions/AdHocUnionCodeGeneratorFactory.cs
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/AdHocUnions/AdHocUnionCodeGenerator.cs

## 8. Regular (inheritance-based) Unions generator family
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/RegularUnions/RegularUnionSourceGenerator.cs
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/RegularUnions/RegularUnionSourceGenState.cs
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/RegularUnions/RegularUnionSettings.cs
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/RegularUnions/RegularUnionTypeMemberState.cs
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/RegularUnions/RegularUnionSwitchMapOverload.cs
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/RegularUnions/RegularUnionCodeGeneratorFactory.cs
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/RegularUnions/RegularUnionCodeGenerator.cs

## 9. Object Factory generator family
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/ObjectFactories/ObjectFactorySourceGenerator.cs
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/ObjectFactories/ObjectFactorySourceGeneratorState.cs
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/ObjectFactories/ObjectFactoryCodeGeneratorFactory.cs
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/ObjectFactories/ObjectFactoryCodeGenerator.cs
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/ObjectFactories/JsonObjectFactoryCodeGeneratorFactory.cs
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/ObjectFactories/MessagePackObjectFactoryCodeGeneratorFactory.cs
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/ObjectFactories/NewtonsoftJsonObjectFactoryCodeGeneratorFactory.cs

## 10. Annotations generator
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/Annotations/AnnotationsSourceGenerator.cs

## 11. Analyzers and diagnostics
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/DiagnosticsDescriptors.cs  (revisit here in analyzer context)
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/Diagnostics/ThinktectureRuntimeExtensionsAnalyzer.cs
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/Diagnostics/ThinktectureRuntimeExtensionsInternalUsageAnalyzer.cs

## 12. Code Fix provider
- [ ] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/CodeFixes/ThinktectureRuntimeExtensionsCodeFixProvider.cs

---

Notes:
- Order is dependency-aware: shared utilities → Roslyn helpers → core CodeAnalysis abstractions → per-feature pipelines (SmartEnums, ValueObjects, Unions, ObjectFactory) → Annotations → Analyzers/CodeFix.
- This project is using Nullable Reference Types, i.e., null-checks of method arguments are usually not necessary, unless the method is called via reflection or by framework/roslyn compiler.
- When reviewing each feature family, follow: SourceGenerator → State/Settings → Member/Type states → CodeGeneratorFactory → CodeGenerator(s) → Serializer-specific factories.
- After reviewing each file, write all warnings and errors to folder review. Don't write information that is not an issue, I don't need documentation but issues. The name of the markdown should be the name of the original file but with file extension "md", e.g. "Thinktecture.Runtime.Extensions.SourceGenerator.csproj" => "Thinktecture.Runtime.Extensions.SourceGenerator.csproj.md"
- After reviewing each file, update this plan
