using System.Collections.Generic;
using Serilog.Core;
using Serilog.Events;

namespace Thinktecture.Runtime.Tests;

public sealed class InMemorySink : ILogEventSink
{
   private readonly List<LogEvent> _events = new();

   public IReadOnlyList<LogEvent> Events => _events;

   public void Emit(LogEvent logEvent) => _events.Add(logEvent);
}
