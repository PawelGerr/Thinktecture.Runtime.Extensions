# Review: src/Thinktecture.Runtime.Extensions.SourceGenerator/Logging/PeriodicCleanup.cs

Summary
- Periodically triggers FileSystemSinkProvider.Cleanup() using a System.Timers.Timer set to run every 5 seconds.
- AutoReset is false; each tick explicitly restarts the timer via Start() if there are still active sinks, otherwise it stops and unsubscribes.
- Synchronizes timer start/stop with a private lock to avoid concurrent state changes.

Strengths
- Cleanup is decoupled and runs asynchronously on timer callbacks.
- Uses a lock to ensure Start() is idempotent while enabling the timer.
- Stops scheduling when no sinks remain, which avoids unnecessary background activity.

Critical Issue
- Event handler accumulation on restart:
  - Start() subscribes _timer.Elapsed += TimerOnElapsed every time it restarts after a tick (AutoReset = false) without first unsubscribing (when there are still sinks).
  - In TimerOnElapsed, the handler is only removed if there are no sinks. Otherwise, Start() is called and another subscription is added.
  - Over time, multiple identical subscriptions accumulate, causing TimerOnElapsed to fire multiple times per tick, performing repeated Cleanup calls and increasing CPU usage.
  - Fix options:
    - Subscribe once (e.g., in the constructor) and never re-subscribe. With AutoReset = false, just toggle _timer.Enabled = true in Start() to schedule the next tick.
    - Or, defensively de-duplicate in Start():
      - _timer.Elapsed -= TimerOnElapsed; _timer.Elapsed += TimerOnElapsed; then enable.
    - Alternatively, set AutoReset = true, subscribe once, and just enable/disable. On Elapsed, if no sinks then disable; else do nothing (timer keeps ticking).

Other Issues / Risks
- Timer disposal:
  - PeriodicCleanup does not dispose the Timer. If the provider is meant to be GC-collectable when no sinks, it’s safer to dispose the timer after unsubscribing to release native resources and avoid keeping the instance rooted inadvertently.
- Potential race with Start() vs TimerOnElapsed:
  - Both manipulate _timer.Enabled and event subscription. The shared _cleanupLock in Start() and the locked section in TimerOnElapsed (for HasSinks check and potential unsubscribe) mitigate races, but consistent subscription management (see fix above) further reduces risk.
- Interval tuning:
  - Cleanup every 5 seconds may be frequent in low-activity scenarios. Consider making the interval configurable or backing off when repeatedly finding no obsolete sinks.

Suggestions / Improvements
- Fix handler duplication (high priority):
  - Easiest: subscribe once in the constructor and remove in the stop branch, but don’t re-add on every Start. Example:
    - Constructor: _timer.Elapsed += TimerOnElapsed;
    - Start(): if (!_timer.Enabled) _timer.Enabled = true;
    - TimerOnElapsed: if (!HasSinks) { _timer.Enabled = false; /* optionally keep handler */ }
  - If you prefer current add/remove pattern, ensure Start() first unsubscribes before adding to avoid duplicates.
- Dispose timer when stopping permanently:
  - When there are no sinks, call _timer.Dispose(); set a flag to prevent further use, or create a new timer on the next Start.
- Consider System.Threading.Timer:
  - For analyzers/source generators, System.Threading.Timer with Change(...) can be simpler and avoids event subscription pitfalls.
- Optional: exponential backoff or configurable interval.

Testing Recommendations
- Verify no handler multiplication:
  - After N cycles with active sinks, Elapsed should invoke exactly once per interval; assert invocation count.
- Lifecycle tests:
  - Sinks appear/disappear around tick boundaries; ensure the timer stops when no sinks and restarts when new sinks are created.
- Resource cleanup:
  - After stopping (no sinks), timer should not keep GC roots; validate by weak-referencing the provider and forcing GC in tests (if feasible).

Overall Assessment
- The logic is straightforward and appropriate for periodic cleanup, but the event re-subscription bug is critical and can lead to multiple callbacks per tick and increased overhead. Addressing this and disposing the timer when idle will make the component robust and leak-free.
