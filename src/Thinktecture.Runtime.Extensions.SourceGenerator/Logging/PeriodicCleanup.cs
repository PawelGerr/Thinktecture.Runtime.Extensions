using System.Timers;
using Timer = System.Timers.Timer;

namespace Thinktecture.Logging;

public class PeriodicCleanup
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
   }

   public void Start()
   {
      lock (_cleanupLock)
      {
         if (_timer.Enabled)
            return;

         _timer.Elapsed += TimerOnElapsed;
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
            _timer.Elapsed -= TimerOnElapsed;
            return;
         }
      }

      Start();
   }
}
