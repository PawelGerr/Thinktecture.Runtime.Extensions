Issues found

- Warning: Default-initializable struct may yield null FilePath
  - readonly record struct can be created via default(LoggingOptions) or uninitialized local, resulting in FilePath == null, Level == 0, etc. This violates NRT expectations and can cause downstream NREs or invalid path usage.
  - Recommendation: Prefer an internal class or a non-record struct with a non-defaultable factory pattern; alternatively, ensure all consumers guard against default instances before use.

- Warning: No argument validation
  - FilePath is not validated for null/empty/whitespace; InitialBufferSize is not validated against negative values. If used to allocate buffers (e.g., StringBuilder) or open files, this can cause ArgumentOutOfRangeException or IO errors later and makes diagnostics harder.
  - Recommendation: Add validation in the constructor (or factory) to enforce non-empty FilePath and InitialBufferSize >= 0 (or a minimum). Consider normalizing FilePath (full path) if required by sinks.

- Warning: Unnecessary public surface
  - Type is declared public but appears to be an internal configuration detail of the source generator’s logging plumbing. Exposing it publicly expands the analyzer assembly’s public API unnecessarily and may create long-term compatibility constraints.
  - Recommendation: Change to internal unless there is a strong reason for public exposure.
