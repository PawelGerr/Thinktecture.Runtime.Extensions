# Review: src/Thinktecture.Runtime.Extensions.SourceGenerator/Properties/launchSettings.json

Status: Reviewed

File contents:
{
  "profiles": {
    "Source Generator": {
      "commandName": "DebugRoslynComponent",
      "targetProject": "../../samples/Thinktecture.Runtime.Extensions.Samples/Thinktecture.Runtime.Extensions.Samples.csproj"
    }
  }
}

Summary:
Visual Studio launch profile for debugging the Source Generator via DebugRoslynComponent against a target project. The configuration is minimal and aligned with the standard VS experience for Roslyn component debugging.

Strengths:
- Uses "commandName": "DebugRoslynComponent", which is the correct mechanism to debug a source generator in Visual Studio.
- Keeps the target project external to the generator, which is appropriate for exercising the generator in a real project.

Issues, Risks, and Suggestions:

1) Broken targetProject path (directory does not exist in repo)
- The path points to "../../samples/Thinktecture.Runtime.Extensions.Samples/Thinktecture.Runtime.Extensions.Samples.csproj", but this repository contains different sample projects:
  - samples/Basic.Samples/Basic.Samples.csproj
  - samples/AspNetCore.Samples/AspNetCore.Samples.csproj
  - samples/MessagePack.Samples/MessagePack.Samples.csproj
  - samples/Newtonsoft.Json.AspNetCore.Samples/Newtonsoft.Samples.csproj
  - samples/Benchmarking/Benchmarking.csproj
  - samples/EntityFrameworkCore.Samples/EntityFrameworkCore.Samples.csproj

Recommendation:
- Update targetProject to a valid sample, for example:
  "../../samples/Basic.Samples/Basic.Samples.csproj"
- Consider adding multiple profiles for different target projects to exercise various generator features, e.g.:
  - "Source Generator (Basic)"
  - "Source Generator (AspNetCore)"
  - "Source Generator (MessagePack)"
  - etc.

2) Relative path base clarification
- In Visual Studio, launchSettings.json paths are resolved relative to the project directory (not the Properties folder). Using "../../samples/..." is correct to reach the repo's samples folder from src/Thinktecture.Runtime.Extensions.SourceGenerator. Only the subpath after samples needs correction as noted above.

3) Developer experience niceties (optional)
- If you frequently switch targets, define multiple profiles rather than editing the path each time.
- Keep paths relative to avoid committing machine-specific absolute paths.

4) Tooling support notes
- "DebugRoslynComponent" is Visual Studio-specific; dotnet CLI will not use this profile. This is expected for source generator debugging and does not require changes.

Overall Assessment:
- Correct mechanism, but the current targetProject path is stale and should be updated to one of the existing sample projects. Consider adding multiple profiles to cover different integration paths and improve workflow.
