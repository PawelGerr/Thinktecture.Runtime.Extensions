### New Rules

 Rule ID   | Category                              | Severity | Notes
-----------|---------------------------------------|----------|-----------------------------------------------------------------------------
 TTRESG071 | ThinktectureRuntimeExtensionsAnalyzer | Error    | TypeParamRef index exceeds the number of type parameters
 TTRESG072 | ThinktectureRuntimeExtensionsAnalyzer | Error    | TypeParamRef cannot be used on non-generic ad-hoc union
 TTRESG073 | ThinktectureRuntimeExtensionsAnalyzer | Error    | Ad-hoc unions do not support 'allows ref struct' type parameters
 TTRESG107 | ThinktectureRuntimeExtensionsAnalyzer | Warning  | Generic ad-hoc union does not reference any type parameter via TypeParamRef

### Removed Rules

 Rule ID   | Category                              | Severity | Notes
-----------|---------------------------------------|----------|---------------------------------------------
 TTRESG033 | ThinktectureRuntimeExtensionsAnalyzer | Error    | Ad hoc unions are now allowed to be generic
