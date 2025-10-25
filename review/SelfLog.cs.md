SelfLog.cs — Review

Summary
- Purpose: Fallback logging utility writing diagnostic messages to a temp file for the source generator/analyzers.
- Scope reviewed: Timestamping, formatting consistency, thread-safety, I/O robustness, analyzer constraints, API design.

Warnings
1) Inconsistent timestamp usage and culture
- Details: Write(string) uses DateTime.Now.ToString("O") (ISO-8601 round-trip) while Write(DateTime, ...) uses datetime.ToString(CultureInfo.InvariantCulture) (default invariant pattern).
- Impact: Mixed timestamp formats make log parsing and correlation harder.
- Recommendation: Use a single consistent format across both methods (prefer DateTimeOffset.UtcNow.ToString("O")).

2) Local time used instead of UTC
- Details: Write(string) uses DateTime.Now.
- Impact: Local time varies by machine/time zone and DST, complicating cross-machine diagnostics.
- Recommendation: Use DateTimeOffset.UtcNow for stable, unambiguous timestamps.

3) No synchronization for file appends
- Details: File.AppendAllText is called without any locking. Multiple concurrent writes (threads/processes) can interleave content.
- Impact: Corrupted or interleaved log lines under concurrency.
- Recommendation: Introduce a static lock or use a Mutex for cross-process coordination. At minimum, local in-process locking (static object) to serialize writes. For multi-process safety, named system Mutex.

4) Repeated path resolution and open/close on each write
- Details: Each call recomputes Path.GetTempPath() and opens/closes the file.
- Impact: Minor overhead. For high-frequency logs, this can add contention.
- Recommendation: Cache the log path in a static readonly field. Optionally use FileStream with buffered StreamWriter and AutoFlush inside a short critical section, though maintaining a long-lived writer would need lifecycle management which may not be desirable in generators.

5) Exception handling lacks context
- Details: On failure, only Debug.WriteLine(ex) is invoked.
- Impact: Loses details (e.g., target file path). Harder to investigate failures to write logs.
- Recommendation: Include the target path and operation in the debug output. Consider catching IOException/UnauthorizedAccessException specifically to hint at common causes (locked file, permissions).

6) Hard-coded filename may collide across processes
- Details: _FILE_NAME is constant, placed in the common temp directory.
- Impact: Different generator runs (potentially across solutions/users) will share the same file; high risk of interleaving and size bloat.
- Recommendation: Incorporate process id, generator identity, or solution hash into the filename (e.g., ThinktectureRuntimeExtensionsSourceGenerator_{ProcessId}.log). If a single file is desired, ensure the concurrency protection is robust and add rotation/cleanup.

7) Potential unbounded growth without rotation
- Details: The file may grow indefinitely. There is a PeriodicCleanup.cs in the plan; ensure it is wired up to this filename/location.
- Impact: Large files in temp over time.
- Recommendation: Implement size-based rotation or ensure PeriodicCleanup targets this exact path and runs reliably.

Notes
- RS1035 suppression: Using System.IO in analyzer context is generally discouraged; suppression is tightly scoped to the method which is good. Given this class is for self-diagnostics, this may be acceptable for the project, but consider a feature flag to disable file I/O entirely in certain environments (e.g., CI).
- The second Write overload takes logLevel as string; consider using the enum to avoid stringly-typed API.

No errors found
- Code is straightforward and functional. The primary concerns are robustness and consistency.

Suggested patch (illustrative)

public class SelfLog
{
   private const string _BASE_FILE_NAME = "ThinktectureRuntimeExtensionsSourceGenerator";
   private static readonly string _LogPath =
      Path.Combine(Path.GetTempPath(), $"{_BASE_FILE_NAME}_{Environment.ProcessId}.log");

   private static readonly object _lock = new();

   public static void Write(string message)
   {
      try
      {
#pragma warning disable RS1035
         var line = $"[{DateTimeOffset.UtcNow.ToString("O", CultureInfo.InvariantCulture)}] {message}{Environment.NewLine}";
         lock (_lock)
         {
            File.AppendAllText(_LogPath, line);
         }
#pragma warning restore RS1035
      }
      catch (Exception ex)
      {
         Debug.WriteLine($"SelfLog.Write failed for '{_LogPath}': {ex}");
      }
   }

   public static void Write(DateTimeOffset timestampUtc, string logLevel, string source, string message)
   {
      var line = $"[{timestampUtc.ToString("O", CultureInfo.InvariantCulture)} | {logLevel}] | [{source}] {message}";
      Write(line);
   }
}

Optional improvements
- Cross-process safety: replace the in-process lock with a named Mutex when concurrent processes are expected.
- Sanitization: normalize embedded newlines in message to keep one-line-per-entry formatting if log parsers rely on it.
- Internal visibility: if only used within the logging infrastructure, consider making the class internal.
