KeyedValueObjectCodeGenerator.cs – Issues

1) Create method nullability can be violated when EmptyStringInFactoryMethodsYieldsNull is true
- Severity: Bug
- Location: GenerateCreateMethod and GenerateValidateMethod
- Details:
  - Validate(...) returns (validationError: null, obj: default) for empty/whitespace input when emptyStringYieldsNull = true.
  - Create(...) then returns obj, which is null.
  - The Create return type is only made nullable when allowNullOutput is true (based solely on NullInFactoryMethodsYieldsNull), not when EmptyStringInFactoryMethodsYieldsNull is true.
  - This leads to Create returning null while having a non-nullable return type when EmptyStringInFactoryMethodsYieldsNull = true and NullInFactoryMethodsYieldsNull = false.
- Why this is a problem: Violates nullability contract and can trigger NREs downstream despite a non-nullable return type. Static analysis is suppressed with !, hiding the issue.
- Suggested fix:
  - Make the Create return type nullable if either:
    - allowNullOutput is true, or
    - emptyStringYieldsNull is true.
  - Example: Append &#34;?&#34; when allowNullOutput || emptyStringYieldsNull.
  - Ensure XML return documentation reflects potential null.
- Tests to add:
  - For a ref-type VO with string key where EmptyStringInFactoryMethodsYieldsNull = true and NullInFactoryMethodsYieldsNull = false: assert Create(&#34;&#34;) returns null (and signature is nullable).
  - Ensure TryCreate returns true with obj == null in this scenario.

2) Metadata ConvertFromKey delegate return nullability ignores NullInFactoryMethodsYieldsNull
- Severity: Bug
- Location: file static class Extensions.GenerateDelegateConvertFromKey
- Details:
  - The delegate return type is annotated as nullable only when EmptyStringInFactoryMethodsYieldsNull is true.
  - It does not account for NullInFactoryMethodsYieldsNull (allowNullOutput) which also allows null to be returned (for null key input).
- Why this is a problem: Metadata consumers may assume non-null return while Create can return null due to NullInFactoryMethodsYieldsNull, causing mismatches in serializers or resolvers that rely on metadata.
- Suggested fix:
  - Mirror the Create method&#39;s nullability logic for the delegate&#39;s return type:
    - Return type should be nullable if allowNullOutput || emptyStringYieldsNull.
- Tests to add:
  - Verify Metadata.ConvertFromKey delegate type is T? when either EmptyStringInFactoryMethodsYieldsNull or NullInFactoryMethodsYieldsNull is true.

3) XML doc param name mismatch for Validate provider parameter when key argument is named &#34;provider&#34;
- Severity: Warning
- Location: GenerateValidateMethod
- Details:
  - When the key argument name equals &#34;provider&#34;, the code renames the IFormatProvider parameter to &#34;formatProvider&#34; to avoid collision.
  - The XML doc still uses &#34;<param name=&#34;provider&#34;>...</param>&#34;, which no longer matches the generated parameter name (formatProvider).
- Why this is a problem: Produces CS1573 XML documentation warnings; documentation becomes incorrect.
- Suggested fix:
  - Use providerArgumentName when generating the XML doc param name to match the actual parameter identifier.
- Tests to add:
  - Generate a VO where the key argument name is &#34;provider&#34; and assert the XML doc param name matches the generated parameter name.

4) Repeated typo in XML docs: &#34;covert&#34; instead of &#34;convert&#34;
- Severity: Minor
- Location: GenerateSafeConversionToKey, GenerateUnsafeConversionToKey, GenerateConversionFromKey
- Details: The parameter doc uses &#34;Object to covert.&#34; across conversion operators.
- Why this is a problem: Documentation quality/readability.
- Suggested fix: Replace &#34;covert&#34; with &#34;convert&#34; in all affected locations.

5) Possibly confusing nullability attribute on TryGetFromKey delegate out error
- Severity: Minor
- Location: Extensions.GenerateDelegateTryGetFromKey
- Details:
  - The signature uses [MaybeNullWhen(true)] on out object error, and returns true when error is null.
  - More idiomatic is [NotNullWhen(false)] to indicate error is non-null when the method returns false.
- Why this is a problem: The current attribute technically matches the behavior but is less clear/idiomatic; consumers often expect NotNullWhen(false) for error out values.
- Suggested fix: Switch to [NotNullWhen(false)] out object error for readability/clarity.

6) Unnecessary [NotNullIfNotNull] on unsafe conversion to value-type key
- Severity: Minor
- Location: GenerateUnsafeConversionToKey
- Details:
  - The unsafe conversion operator returns a non-nullable value type (keyMember is value type per the guard), but still applies [return: NotNullIfNotNull(&#34;obj&#34;)].
- Why this is a problem: The attribute has no effect for non-nullable value type return values; it may confuse readers.
- Suggested fix: Omit the attribute in this specific operator when the return type is a non-nullable value type.

Notes on TryCreate nullability handling:
- The code correctly avoids applying NotNullWhen(true) to obj when EmptyStringInFactoryMethodsYieldsNull is true, because obj can be null while the method returns true (for empty string). This is consistent with the semantics. No change suggested here.
