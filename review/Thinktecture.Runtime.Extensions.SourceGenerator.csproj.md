# Thinktecture.Runtime.Extensions.SourceGenerator.csproj – Fixes

All items are actionable changes (no informational notes).

- [warn] Re-enable symbol packages and SourceLinked PDBs
  - Change: Remove the project-level setting `<IncludeSymbols>false</IncludeSymbols>` so `src/Directory.Build.props` defaults apply (`IncludeSymbols=true`, `SymbolPackageFormat=snupkg`).
  - Rationale: Improves debuggability and traceability; aligns with repository-wide policy.
  - Patch: Remove this line from the csproj: `<IncludeSymbols>false</IncludeSymbols>`

- [warn] Prevent unintended transitive analyzer/generator flow
  - Change: Add `<DevelopmentDependency>true</DevelopmentDependency>` to the SourceGenerator csproj.
  - Rationale: Ensures the package is treated as a development-only dependency and is not included as a dependency when other libraries are packed.
  - Patch: Inside the main `PropertyGroup` add: `<DevelopmentDependency>true</DevelopmentDependency>`

- [warn] Harden build quality by failing on warnings
  - Change: Add `<TreatWarningsAsErrors>true</TreatWarningsAsErrors>` to this project (or enforce in CI).
  - Rationale: Prevents warning regressions in the generator/analyzers.
  - Patch: Inside the main `PropertyGroup` add: `<TreatWarningsAsErrors>true</TreatWarningsAsErrors>`

- [warn] Stabilize analyzer rule set across SDK upgrades
  - Change: Add an explicit analysis level, e.g. `<AnalysisLevel>latest</AnalysisLevel>` (or pin to a specific version if desired).
  - Rationale: Stabilizes analyzer behavior and warning baselines.
  - Patch: Inside the main `PropertyGroup` add: `<AnalysisLevel>latest</AnalysisLevel>`

- [warn] Avoid Workspace API dependency unless it is strictly needed
  - Change: Replace `Microsoft.CodeAnalysis.CSharp.Workspaces` with `Microsoft.CodeAnalysis.CSharp` if Workspace APIs are not required at generator runtime.
  - Rationale: Reduces load surface and avoids potential `AssemblyLoadException` when the compiler loads the generator (Workspaces are typically not available at runtime).
  - Patch:
    - Replace: `<PackageReference Include="Microsoft.CodeAnalysis.CSharp.Workspaces" PrivateAssets="all" />`
    - With: `<PackageReference Include="Microsoft.CodeAnalysis.CSharp" PrivateAssets="all" />`
    - Note: Action is conditional on verifying no Workspace APIs are used on generator execution paths.

- [warn] Improve reproducibility and debugging artifacts
  - Change: Add `<EmbedUntrackedSources>true</EmbedUntrackedSources>` (project or repo level).
  - Rationale: Complements SourceLink for long-term debugging and archival of exact sources used to build the package.
  - Patch: Inside the main `PropertyGroup` add: `<EmbedUntrackedSources>true</EmbedUntrackedSources>`

- [warn] Proper analyzer release tracking (optional but recommended)
  - Change: Add AnalyzerReleases files (`AnalyzerReleases.Unshipped.md`/`AnalyzerReleases.Shipped.md`) and remove `RS2008` from `NoWarn` in the generator csproj once tracking is in place.
  - Rationale: Conforms to analyzer release tracking best practices and removes the need for RS2008 suppression.
  - Patch:
    - Add the two release files under the analyzer project and then remove `RS2008` from `<NoWarn>` in the generator csproj.
