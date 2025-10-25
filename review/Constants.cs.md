# Review: CodeAnalysis/Constants.cs

Issues found (errors/warnings only):

1) Naming bug: AD_HOCH typo
- Location: Constants.Attributes.Union
- Problem: Constant names use “AD_HOCH” instead of “AD_HOC”:
  - NAME_AD_HOCH
  - FULL_NAME_AD_HOCH
- Why it matters: Typo in public constant names is confusing and propagates into call sites. It also harms discoverability (search for “AdHoc” won’t find “AD_HOCH”) and violates naming consistency.
- Recommendation: Rename to NAME_AD_HOC and FULL_NAME_AD_HOC and update all references in the codebase. Consider keeping the old names as [Obsolete] aliases for one version if this is consumed externally.

2) Consistency warning: Hardcoded namespace vs. shared constant
- Location: Constants.Attributes.ValueObject and Constants.Attributes.SmartEnum
- Problem: Some FULL_NAME constants hardcode the namespace “Thinktecture” while others reuse Attributes.NAMESPACE via string interpolation (e.g., ObjectFactory does). Mixed patterns increase maintenance risk if the namespace ever changes.
- Example:
  - Uses hardcoded: "Thinktecture.ValueObjectAttribute`1", "Thinktecture.SmartEnumAttribute`1"
  - Uses NAMESPACE: $"{NAMESPACE}.{NAME}`1"
- Recommendation: Standardize on using Attributes.NAMESPACE for all FULL_NAME constants for consistency and to reduce drift risk.

3) Robustness warning: Matching modules by .dll file name
- Location: Constants.Modules
- Problem: The module presence check (likely elsewhere) will rely on .dll file names. Comparing by file name/path can be brittle across environments and may be case-sensitive on non-Windows systems.
- Recommendation: Prefer comparing assembly identities (e.g., IAssemblySymbol.Identity or Compilation.ReferencedAssemblyNames) rather than file names. If file names must be used, compare case-insensitively and document the expectation.

4) Minor naming consistency
- Location: Constants.Interfaces.DisallowDefaultStructs
- Problem: Group name suggests “DefaultStructs” while the interface NAME is “IDisallowDefaultValue”. The mismatch can be confusing when searching by concept.
- Recommendation: Align grouping name and interface concept or add a short comment clarifying intent. Low severity.
