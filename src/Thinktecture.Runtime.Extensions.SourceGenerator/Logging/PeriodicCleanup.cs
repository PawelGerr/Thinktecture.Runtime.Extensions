using System.Timers;
using Timer = System.Timers.Timer;

namespace Thinktecture.Logging;

public sealed class PeriodicCleanup : IDisposable
{
   private readonly FileSystemSinkProvider _fileSystemSinkProvider;
   private readonly object _cleanupLock;
   private readonly Timer _timer;

   public PeriodicCleanup(FileSystemSinkProvider fileSystemSinkProvider)
   {
      _fileSystemSinkProvider = fileSystemSinkProvider;
      _cleanupLock = new object();
      _timer = new Timer
               {
                  Interval = TimeSpan.FromSeconds(5).TotalMilliseconds,
                  AutoReset = false
               };
      _timer.Elapsed += TimerOnElapsed;
   }

   public void Start()
   {
      lock (_cleanupLock)
      {
         if (_timer.Enabled)
            return;

         _timer.Enabled = true;
      }
   }

   private void TimerOnElapsed(object sender, ElapsedEventArgs e)
   {
      _fileSystemSinkProvider.Cleanup();

      lock (_cleanupLock)
      {
         if (!_fileSystemSinkProvider.HasSinks())
         {
            _timer.Enabled = false;
            return;
         }

         _timer.Enabled = true;
      }
   }

   public void Dispose()
   {
      _timer.Stop();
      _timer.Elapsed -= TimerOnElapsed;
      _timer.Dispose();
   }
}
