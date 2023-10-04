using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Thinktecture.Logging;

public class FileSystemLoggingSink : ILoggingSink, IDisposable
{
   // We are using quite primitive approach with "UnsafeQueueUserWorkItem" to prevent always-running task,
   // because source generator doesn't support IDisposable to stop the task.

   private readonly string _filePath;
   private readonly object _disposeAndWriterLock;
   private readonly CancellationTokenSource _disposeCts;
   private readonly object _messagesLock;
   private readonly ManualResetEvent _currentlyWriting;
   private readonly Queue<LogItem> _messages;

   private StreamWriter? _writer;
   private bool _isWritingMessages;
   private bool _isDisposed;

   public FileSystemLoggingSink(
      string filePath,
      int initialBufferSize)
   {
      _filePath = filePath;
      _disposeAndWriterLock = new object();
      _disposeCts = new CancellationTokenSource();
      _messagesLock = new object();
      _messages = new Queue<LogItem>(initialBufferSize);
      _currentlyWriting = new ManualResetEvent(true);
   }

   public void Write(string source, LogLevel logLevel, DateTime datetime, string message)
   {
      if (_isDisposed)
         return;

      try
      {
         lock (_messagesLock)
         {
            _messages.Enqueue(new(source, logLevel, datetime, message));

            if (_isWritingMessages)
               return;

            _isWritingMessages = true;
            ThreadPool.UnsafeQueueUserWorkItem(WriteMessages, null);
         }
      }
      catch (Exception ex)
      {
         // most likely concurrency: disposed

         SelfLog.Write(ex.ToString());
      }
   }

   private void WriteMessages(object? state)
   {
      _currentlyWriting.Reset();

      try
      {
         WriteMessagesAsync(_disposeCts.Token).ConfigureAwait(false).GetAwaiter().GetResult();
      }
      catch (Exception ex)
      {
         SelfLog.Write(ex.ToString());
      }
      finally
      {
         _currentlyWriting.Set();
      }
   }

   private async Task WriteMessagesAsync(CancellationToken cancellationToken)
   {
      var mustFlush = false;

      while (!cancellationToken.IsCancellationRequested)
      {
         try
         {
            LogItem? item = null;

            lock (_messagesLock)
            {
               if (_messages.Count == 0)
               {
                  // don't return until Flush
                  if (!mustFlush)
                  {
                     _isWritingMessages = false;
                     return;
                  }
               }
               else
               {
                  item = _messages.Dequeue();
               }
            }

            if (!TryGetOrCreateWriter(out var writer))
               continue;

            if (item is not null)
            {
               mustFlush |= await WriteInternalAsync(writer, item.Value);
            }
            else
            {
               await writer.FlushAsync();
               mustFlush = false;

#pragma warning disable RS1035
               // Writer/FileStream doesn't throw an exception if file is deleted,
               // so we check for the file once per batch to recreate it on the next batch.
               if (!File.Exists(_filePath))
                  ReleaseWriter();
#pragma warning restore RS1035
            }
         }
         catch (Exception) when (cancellationToken.IsCancellationRequested)
         {
            return;
         }
         catch (Exception ex)
         {
            mustFlush = false;
            ReleaseWriter();

            SelfLog.Write(ex.ToString());
         }
      }
   }

   private bool TryGetOrCreateWriter([MaybeNullWhen(false)] out StreamWriter writer)
   {
      writer = _writer;

      if (writer is not null)
         return true;

      lock (_disposeAndWriterLock)
      {
         if (_isDisposed)
            return false;

         writer = _writer;

         if (writer is not null)
            return true;

         if (!TryCreateWriter(_filePath, out writer))
            return false;

         _writer = writer;
         return true;
      }
   }

   private static bool TryCreateWriter(
      string filePath,
      [MaybeNullWhen(false)] out StreamWriter writer)
   {
      try
      {
#pragma warning disable RS1035
         var stream = File.Open(filePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read | FileShare.Write | FileShare.Delete);
#pragma warning restore RS1035

         try
         {
            writer = new StreamWriter(stream, Encoding.UTF8);
            return true;
         }
         catch
         {
            stream.Dispose();

            writer = null;
            return false;
         }
      }
      catch (Exception ex)
      {
         SelfLog.Write(ex.ToString());

         writer = null;
         return false;
      }
   }

   private static async Task<bool> WriteInternalAsync(StreamWriter writer, LogItem item)
   {
      if (item.LogLevel == LogLevel.Error)
         SelfLog.Write(item.Datetime, GetLogLevel(item.LogLevel), item.Source, item.Message);

      await writer.WriteAsync("[");
      await WriteDateTimeAsync(writer, item.Datetime);
      await writer.WriteAsync(" | ");
      await writer.WriteAsync(GetLogLevel(item.LogLevel));
      await writer.WriteAsync("] ");

      await writer.WriteAsync("[");
      await writer.WriteAsync(item.Source);
      await writer.WriteAsync("] ");

      await writer.WriteLineAsync(item.Message);

      return true;
   }

   private static async Task WriteDateTimeAsync(StreamWriter writer, DateTime datetime)
   {
      await writer.WriteAsync(datetime.Year.ToString("0000"));
      await writer.WriteAsync("-");
      await writer.WriteAsync(datetime.Month.ToString("00"));
      await writer.WriteAsync("-");
      await writer.WriteAsync(datetime.Day.ToString("00"));
      await writer.WriteAsync(" ");
      await writer.WriteAsync(datetime.Hour.ToString("00"));
      await writer.WriteAsync(":");
      await writer.WriteAsync(datetime.Minute.ToString("00"));
      await writer.WriteAsync(":");
      await writer.WriteAsync(datetime.Second.ToString("00"));
      await writer.WriteAsync(":");
      await writer.WriteAsync(datetime.Millisecond.ToString("000"));
   }

   private static string GetLogLevel(LogLevel logLevel)
   {
      return logLevel switch
      {
         LogLevel.Trace => nameof(LogLevel.Trace),
         LogLevel.Debug => nameof(LogLevel.Debug),
         LogLevel.Information => nameof(LogLevel.Information),
         LogLevel.Warning => nameof(LogLevel.Warning),
         LogLevel.Error => nameof(LogLevel.Error),
         _ => throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, null)
      };
   }

   private void ReleaseWriter()
   {
      lock (_disposeAndWriterLock)
      {
         _writer?.Dispose();
         _writer = null;
      }
   }

   public void Dispose()
   {
      try
      {
         if (_isDisposed)
            return;

         lock (_disposeAndWriterLock)
         {
            if (_isDisposed)
               return;

            _isDisposed = true;
         }

         _currentlyWriting.WaitOne(5000); // wait until all logs are written down

         _disposeCts.Cancel();
         _disposeCts.Dispose();
         _currentlyWriting.Dispose();

         ReleaseWriter();
      }
      catch (Exception ex)
      {
         SelfLog.Write(ex.ToString());
      }
   }

   private readonly record struct LogItem(string Source, LogLevel LogLevel, DateTime Datetime, string Message);
}
