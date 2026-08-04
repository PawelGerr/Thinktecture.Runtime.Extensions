### New Rules

 Rule ID | Category | Severity | Notes
---------|----------|----------|------
 TTRESG077 | ThinktectureRuntimeExtensionsAnalyzer | Error | Ad-hoc union member type is less accessible than the union
 TTRESG078 | ThinktectureRuntimeExtensionsAnalyzer | Error | Object factories with a ref struct value type must not enable 'UseWithEntityFramework' or 'UseForModelBinding'
 TTRESG079 | ThinktectureRuntimeExtensionsAnalyzer | Error | Ad-hoc union member type is not implicitly convertible to 'SingleBackingFieldType'
 TTRESG080 | ThinktectureRuntimeExtensionsAnalyzer | Error | 'AllowDefaultStructs' must be 'false' if the type implements 'IDisallowDefaultValue'
 TTRESG081 | ThinktectureRuntimeExtensionsAnalyzer | Error | 'DefaultValueHandling = MapToFirstMember' requires a struct union
 TTRESG082 | ThinktectureRuntimeExtensionsAnalyzer | Error | 'DefaultValueHandling = MapToFirstMember' requires a stateless first member
 TTRESG108 | ThinktectureRuntimeExtensionsAnalyzer | Warning | Object factory with a ref struct value type is ignored by the configured serialization frameworks
 TTRESG109 | ThinktectureRuntimeExtensionsAnalyzer | Warning | 'EmptyStringInFactoryMethodsYieldsNull' has no effect on structs
 TTRESG110 | ThinktectureRuntimeExtensionsAnalyzer | Warning | 'IDisallowDefaultValue' has no effect on reference types

### Removed Rules

 Rule ID | Category | Severity | Notes
---------|----------|----------|------
