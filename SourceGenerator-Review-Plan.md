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

Roslyn symbol/type:
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/Extensions/SymbolExtensions.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/Extensions/NamedTypeSymbolExtensions.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/Extensions/TypeSymbolExtensions.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/Extensions/MethodSymbolExtensions.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/Extensions/PropertySymbolExtensions.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/Extensions/FieldSymbolExtensions.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/Extensions/TypeParameterSymbolExtensions.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/Extensions/ImplementedOperatorsExtensions.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/Extensions/ImplementedComparisonOperatorsExtensions.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/Extensions/MemberInformationExtensions.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/Extensions/KeyMemberSettingsExtensions.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/Extensions/MetadataReferenceExtensions.cs

Roslyn syntax:
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/Extensions/TypeDeclarationSyntaxExtensions.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/Extensions/MemberDeclarationSyntaxExtensions.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/Extensions/SyntaxListExtensions.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/Extensions/SyntaxTokenListExtensions.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/Extensions/SeparatedSyntaxListExtensions.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/Extensions/ObjectCreationOperationExtensions.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/Extensions/AttributeDataExtensions.cs

Misc/source-gen plumbing:
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/Extensions/SerializationFrameworksExtensions.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/Extensions/SourceProductionContextExtensions.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/Extensions/ContainingTypesExtensions.cs

## 4. CodeAnalysis – Core abstractions, states, utilities
Start with constants/enums/options:
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/Constants.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/SerializationFrameworks.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/OperatorsGeneration.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/ConversionOperatorsGeneration.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/SwitchMapMethodsGeneration.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/UnionConstructorAccessModifier.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/ValueObjectAccessModifier.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/ValueObjectMemberKind.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/LoggingOptions.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/GeneratorOptions.cs

Core interfaces (type info, members, codegen):
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/ITypeInformation.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/ITypeInformationWithNullability.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/ITypeKindInformation.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/ITypeFullyQualified.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/ITypeMinimallyQualified.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/ITypeInformationProvider.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/INamespaceAndName.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/IHasGenerics.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/ITypedMemberState.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/IMemberState.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/IMemberInformation.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/IParsableTypeInformation.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/ICodeGeneratorFactory.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/IInterfaceCodeGenerator.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/IKeyedSerializerCodeGeneratorFactory.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/IComplexSerializerCodeGeneratorFactory.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/IKeyedSerializerGeneratorTypeInformation.cs

States and metadata:
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/AttributeInfo.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/GenericTypeParameterState.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/ContainingTypeState.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/KeyMemberState.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/ValidationErrorState.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/DefaultMemberState.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/InstanceMemberInfo.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/EqualityInstanceMemberInfo.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/DerivedTypeInfo.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/ParameterState.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/ObjectFactoryState.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/ImplementedOperators.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/ImplementedComparisonOperators.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/DelegateMethodState.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/Extensions/ArgumentName.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/BackingFieldName.cs

Typed member state fabrication:
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/TypedMemberState.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/TypedMemberStateFactory.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/TypedMemberStateFactoryProvider.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/CachedTypedMemberState.cs

Comparers/utilities:
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/TypeInformationComparer.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/TypeOnlyComparer.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/MemberInformationComparer.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/ComparerInfo.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/SetComparer.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/Helper.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/ModuleInfo.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/SourceGenError.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/SourceGenException.cs

Interface and common code generation (reused by features):
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/CodeGeneratorBase.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/InterfaceCodeGeneratorFactory.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/InterfaceCodeGeneratorState.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/ParsableCodeGenerator.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/ParsableGeneratorState.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/ComparableCodeGenerator.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/ComparableGeneratorState.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/FormattableCodeGenerator.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/FormattableGeneratorState.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/ComparisonOperatorsCodeGenerator.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/ComparisonOperatorsGeneratorState.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/EqualityComparisonOperatorsCodeGenerator.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/EqualityComparisonOperatorsGeneratorState.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/OperatorsGeneratorState.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/KeyedSerializerGeneratorState.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/KeyedJsonCodeGenerator.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/KeyedMessagePackCodeGenerator.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/KeyedNewtonsoftJsonCodeGenerator.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/SmartEnumAndValueObjectCodeGeneratorBase.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/ThinktectureSourceGeneratorBase.cs

Diagnostics descriptors used by analyzers:
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/DiagnosticsDescriptors.cs

## 5. Smart Enums generator family
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/SmartEnums/SmartEnumSourceGenerator.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/SmartEnums/SmartEnumSourceGeneratorState.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/SmartEnums/SmartEnumSettings.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/SmartEnums/AllEnumSettings.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/SmartEnums/SmartEnumDerivedTypes.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/SmartEnums/BaseTypeState.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/SmartEnums/ConstructorState.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/SmartEnums/EnumItem.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/SmartEnums/EnumItems.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/SmartEnums/SmartEnumCodeGeneratorFactory.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/SmartEnums/SmartEnumCodeGenerator.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/SmartEnums/InterfaceCodeGenerator.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/SmartEnums/JsonSmartEnumCodeGeneratorFactory.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/SmartEnums/MessagePackSmartEnumCodeGeneratorFactory.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/SmartEnums/NewtonsoftJsonSmartEnumCodeGeneratorFactory.cs

## 6. Value Objects generator family
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/ValueObjects/ValueObjectSourceGenerator.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/ValueObjects/KeyedValueObjectSourceGeneratorState.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/ValueObjects/ComplexValueObjectSourceGeneratorState.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/ValueObjects/ValueObjectSettings.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/ValueObjects/AllValueObjectSettings.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/ValueObjects/ValueObjectMemberSettings.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/ValueObjects/KeyedValueObjectCodeGenerator.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/ValueObjects/ComplexValueObjectCodeGenerator.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/ValueObjects/ComplexSerializerGeneratorState.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/ValueObjects/JsonValueObjectCodeGeneratorFactory.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/ValueObjects/MessagePackValueObjectCodeGeneratorFactory.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/ValueObjects/NewtonsoftJsonValueObjectCodeGeneratorFactory.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/ValueObjects/ValueObjectCodeGeneratorFactory.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/ValueObjects/IOperatorsCodeGeneratorProvider.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/ValueObjects/AdditionOperatorsCodeGeneratorProvider.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/ValueObjects/SubtractionOperatorsCodeGeneratorProvider.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/ValueObjects/MultiplyOperatorsCodeGeneratorProvider.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/ValueObjects/DivisionOperatorsCodeGeneratorProvider.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/ValueObjects/AdditionOperatorsCodeGenerator.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/ValueObjects/SubtractionOperatorsCodeGenerator.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/ValueObjects/MultiplyOperatorsCodeGenerator.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/ValueObjects/DivisionOperatorsCodeGenerator.cs
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/ValueObjects/IValueObjectSerializerCodeGeneratorFactory.cs

## 7. Ad-hoc Unions generator family
- [x] src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/AdHocUnions/AdHocUnionSourceGenerator.cs
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
