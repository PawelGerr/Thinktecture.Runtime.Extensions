; Unshipped analyzer release
; See https://github.com/dotnet/roslyn-analyzers/blob/main/docs/Analyzer%20Release%20Tracking.md

## Release 9.0

### New Rules

 Rule ID    | Category                                           | Severity | Notes                                                                            
------------|----------------------------------------------------|----------|----------------------------------------------------------------------------------
 TTRESG001  | ThinktectureRuntimeExtensionsAnalyzer              | Error    | Field must be read-only                                                          
 TTRESG002  | ThinktectureRuntimeExtensionsAnalyzer              | Error    | Smart Enum item must be public                                                   
 TTRESG003  | ThinktectureRuntimeExtensionsAnalyzer              | Error    | Property must be read-only                                                       
 TTRESG004  | ThinktectureRuntimeExtensionsAnalyzer              | Error    | The type must be a class or a struct                                             
 TTRESG006  | ThinktectureRuntimeExtensionsAnalyzer              | Error    | Type must be partial                                                             
 TTRESG009  | ThinktectureRuntimeExtensionsAnalyzer              | Error    | The constructors must be private                                                 
 TTRESG012  | ThinktectureRuntimeExtensionsAnalyzer              | Error    | Provided key member name is not allowed                                          
 TTRESG014  | ThinktectureRuntimeExtensionsAnalyzer              | Error    | Inner Smart Enum on first level must be private                                  
 TTRESG015  | ThinktectureRuntimeExtensionsAnalyzer              | Error    | Inner Smart Enum on non-first level must be public                               
 TTRESG017  | ThinktectureRuntimeExtensionsAnalyzer              | Error    | The key member must not be nullable                                              
 TTRESG033  | ThinktectureRuntimeExtensionsAnalyzer              | Error    | Ad hoc unions must not be generic                                                
 TTRESG034  | ThinktectureRuntimeExtensionsAnalyzer              | Error    | Field of the base class must be read-only                                        
 TTRESG035  | ThinktectureRuntimeExtensionsAnalyzer              | Error    | Property of the base class must be read-only                                     
 TTRESG036  | ThinktectureRuntimeExtensionsAnalyzer              | Error    | The key type must not be nullable                                                
 TTRESG037  | ThinktectureRuntimeExtensionsAnalyzer              | Error    | Smart Enum without derived types must be sealed                                  
 TTRESG041  | ThinktectureRuntimeExtensionsAnalyzer              | Error    | The type of the comparer doesn't match the type of the member                    
 TTRESG042  | ThinktectureRuntimeExtensionsAnalyzer              | Error    | Property 'init' accessor must be private                                         
 TTRESG043  | ThinktectureRuntimeExtensionsAnalyzer              | Error    | Primary constructor is not allowed                                               
 TTRESG044  | ThinktectureRuntimeExtensionsAnalyzer              | Error    | Custom implementation of the key member not found                                
 TTRESG045  | ThinktectureRuntimeExtensionsAnalyzer              | Error    | Key member type mismatch                                                         
 TTRESG046  | ThinktectureRuntimeExtensionsAnalyzer              | Error    | The arguments of 'Switch' and 'Map' must be named                                
 TTRESG047  | ThinktectureRuntimeExtensionsAnalyzer              | Error    | Variable must be initialized with non-default value                              
 TTRESG048  | ThinktectureRuntimeExtensionsAnalyzer              | Warning  | String-based Value Object needs equality comparer                                
 TTRESG049  | ThinktectureRuntimeExtensionsAnalyzer              | Warning  | Complex Value Object with string members needs equality comparer                 
 TTRESG050  | ThinktectureRuntimeExtensionsAnalyzer              | Error    | Method with UseDelegateFromConstructor must be partial                           
 TTRESG051  | ThinktectureRuntimeExtensionsAnalyzer              | Error    | Method with UseDelegateFromConstructor must not have generics                    
 TTRESG052  | ThinktectureRuntimeExtensionsAnalyzer              | Error    | The type must not be inside generic type                                         
 TTRESG053  | ThinktectureRuntimeExtensionsAnalyzer              | Error    | Derived type of a union must not be generic                                      
 TTRESG054  | ThinktectureRuntimeExtensionsAnalyzer              | Error    | Discriminated union must be sealed or have private constructors only             
 TTRESG055  | ThinktectureRuntimeExtensionsAnalyzer              | Error    | Discriminated union implemented using a record must be sealed                    
 TTRESG056  | ThinktectureRuntimeExtensionsAnalyzer              | Error    | Non-abstract derived union is less accessible than base union                    
 TTRESG057  | ThinktectureRuntimeExtensionsAnalyzer              | Error    | AllowDefaultStructs must be false if VO is struct but key type is reference type 
 TTRESG058  | ThinktectureRuntimeExtensionsAnalyzer              | Error    | AllowDefaultStructs must be false if some members disallow default values        
 TTRESG059  | ThinktectureRuntimeExtensionsAnalyzer              | Error    | ObjectFactory must have corresponding constructor                                
 TTRESG061  | ThinktectureRuntimeExtensionsAnalyzer              | Error    | DiagnosticsDescriptors                                                           
 TTRESG062  | ThinktectureRuntimeExtensionsAnalyzer              | Error    | DiagnosticsDescriptors                                                           
 TTRESG063  | ThinktectureRuntimeExtensionsAnalyzer              | Error    | DiagnosticsDescriptors                                                           
 TTRESG064  | ThinktectureRuntimeExtensionsAnalyzer              | Error    | DiagnosticsDescriptors                                                           
 TTRESG065  | ThinktectureRuntimeExtensionsAnalyzer              | Error    | DiagnosticsDescriptors                                                           
 TTRESG066  | ThinktectureRuntimeExtensionsAnalyzer              | Error    | DiagnosticsDescriptors                                                           
 TTRESG067  | ThinktectureRuntimeExtensionsAnalyzer              | Error    | DiagnosticsDescriptors                                                           
 TTRESG068  | ThinktectureRuntimeExtensionsAnalyzer              | Error    | DiagnosticsDescriptors                                                           
 TTRESG069  | ThinktectureRuntimeExtensionsAnalyzer              | Error    | DiagnosticsDescriptors                                                           
 TTRESG070  | ThinktectureRuntimeExtensionsAnalyzer              | Error    | DiagnosticsDescriptors                                                           
 TTRESG060  | ThinktectureRuntimeExtensionsAnalyzer              | Error    | Smart Enums with ObjectFactory must not have HasCorrespondingConstructor=true    
 TTRESG097  | ThinktectureRuntimeExtensionsAnalyzer              | Error    | Error during analysis of referenced modules                                      
 TTRESG098  | ThinktectureRuntimeExtensionsAnalyzer              | Warning  | Error during code analysis                                                       
 TTRESG099  | ThinktectureRuntimeExtensionsAnalyzer              | Error    | Error during code generation                                                     
 TTRESG100  | ThinktectureRuntimeExtensionsAnalyzer              | Warning  | The Smart Enum has no items                                                      
 TTRESG101  | ThinktectureRuntimeExtensionsAnalyzer              | Warning  | Static properties are not considered Smart Enum items                            
 TTRESG102  | ThinktectureRuntimeExtensionsAnalyzer              | Warning  | The type has a comparer defined but no equality comparer                         
 TTRESG103  | ThinktectureRuntimeExtensionsAnalyzer              | Warning  | The type has an equality comparer defined but no comparer                        
 TTRESG104  | ThinktectureRuntimeExtensionsAnalyzer              | Warning  | Members disallowing default values must be required                              
 TTRESG105  | ThinktectureRuntimeExtensionsAnalyzer              | Warning  | DiagnosticsDescriptors                                                           
 TTRESG106  | ThinktectureRuntimeExtensionsAnalyzer              | Warning  | DiagnosticsDescriptors                                                           
 TTRESG1000 | ThinktectureRuntimeExtensionsInternalUsageAnalyzer | Warning  | Internal Thinktecture.Runtime.Extensions API usage                               
