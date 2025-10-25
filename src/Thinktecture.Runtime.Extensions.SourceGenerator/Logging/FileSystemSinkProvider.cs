using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Thinktecture.CodeAnalysis;

namespace Thinktecture.Logging;

public sealed class FileSystemSinkProvider : IDisposable
{
   private static readonly object _instanceLock = new();
   private static readonly Lazy<FilePathComparer> _comparer = new(GetComparer);
   private static readonly ConcurrentDictionary<string, object?> _loggedNotFoundPath = new(); // to prevent spamming the log with the same message
   private static WeakReference<FileSystemSinkProvider>? _instance;

   public static FileSystemSinkProvider GetOrCreate()
   {
      lock (_instanceLock)
      {
         FileSystemSinkProvider provider;

         if (_instance is null)
         {
            provider = new FileSystemSinkProvider();
            _instance = new WeakReference<FileSystemSinkProvider>(provider);
         }
         else if (!_instance.TryGetTarget(out provider))
         {
            provider = new FileSystemSinkProvider();
            _instance.SetTarget(provider);
         }

         return provider;
      }
   }

   private readonly Dictionary<(string, bool), FileSystemSinkContext> _sinksByFilePath;
   private readonly object _lock;
   private readonly PeriodicCleanup _periodicCleanup;

   private FileSystemSinkProvider()
   {
      _sinksByFilePath = new Dictionary<(string, bool), FileSystemSinkContext>(_comparer.Value);
      _lock = new object();
      _periodicCleanup = new PeriodicCleanup(this);
   }

   private static FilePathComparer GetComparer()
   {
      try
      {
         return RuntimeInformation.IsOSPlatform(OSPlatform.Windows) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX)
                   ? new FilePathComparer(StringComparer.OrdinalIgnoreCase)
                   : new FilePathComparer(StringComparer.Ordinal);
      }
      catch
      {
         return new FilePathComparer(StringComparer.Ordinal);
      }
   }

   public bool HasSinks()
   {
      lock (_lock)
      {
         return _sinksByFilePath.Count > 0;
      }
   }

   public ILoggingSink? GetSinkOrNull(string filePath, bool filePathMustBeUnique, int initialBufferSize, ThinktectureSourceGeneratorBase owner)
   {
      var fullPath = Path.GetFullPath(filePath);

      lock (_lock)
      {
         if (_sinksByFilePath.TryGetValue((fullPath, filePathMustBeUnique), out var sinkContext))
         {
            sinkContext.AddOwner(owner);
            _periodicCleanup.Start();

            return sinkContext.Sink;
         }
      }

      var logFileInfo = GetFileInfo(fullPath);
      var newSink = CreateLogFileOrNull(logFileInfo, filePathMustBeUnique, initialBufferSize);

      lock (_lock)
      {
         if (_sinksByFilePath.TryGetValue((fullPath, filePathMustBeUnique), out var sinkContext))
         {
            newSink?.Dispose();

            sinkContext.AddOwner(owner);
            _periodicCleanup.Start();

            return sinkContext.Sink;
         }

         if (newSink is null)
            return null;

         _sinksByFilePath.Add((fullPath, filePathMustBeUnique), new FileSystemSinkContext(fullPath, filePathMustBeUnique, newSink, owner));
         _periodicCleanup.Start();

         return newSink;
      }
   }

   public void ReleaseSink(ILoggingSink sink, ThinktectureSourceGeneratorBase owner)
   {
      lock (_lock)
      {
         foreach (var kvp in _sinksByFilePath)
         {
            if (kvp.Value.Sink != sink)
               continue;

            kvp.Value.RemoveOwner(owner);
            break;
         }
      }

      Cleanup();
   }

   public void Cleanup()
   {
      lock (_lock)
      {
         List<FileSystemSinkContext>? obsoleteSinks = null;

         foreach (var kvp in _sinksByFilePath)
         {
            kvp.Value.RemoveReclaimedOwners();

            if (!kvp.Value.HasOwners())
               (obsoleteSinks ??= []).Add(kvp.Value);
         }

         if (obsoleteSinks is null)
            return;

         foreach (var sinkContext in obsoleteSinks)
         {
            _sinksByFilePath.Remove((sinkContext.OriginalFilePath, sinkContext.FilePathIsUnique));
            sinkContext.Sink.Dispose();
         }
      }
   }

   private static FileSystemLoggingSink? CreateLogFileOrNull(LogFileInfo logFileInfos, bool filePathMustBeUnique, int initialBufferSize)
   {
      if (logFileInfos.FolderPath is null)
         return null;

      var utcNow = DateTime.UtcNow;
      var fileName = filePathMustBeUnique
                        ? $"{logFileInfos.FileName ?? "ThinktectureRuntimeExtensions_logs"}_{utcNow.Year}{utcNow.Month:00}{utcNow.Day:00}_{utcNow.Hour:00}{utcNow.Minute:00}{utcNow.Second:00}_{Guid.NewGuid():N}{logFileInfos.FileExtension ?? ".log"}"
                        : $"{logFileInfos.FileName ?? "ThinktectureRuntimeExtensions_logs"}{logFileInfos.FileExtension ?? ".log"}";
      var logFilePath = Path.Combine(logFileInfos.FolderPath, fileName);

      return new FileSystemLoggingSink(logFilePath, initialBufferSize);
   }

   [SuppressMessage("MicrosoftCodeAnalysisCorrectness", "RS1035:Do not use APIs banned for analyzers")]
   private static LogFileInfo GetFileInfo(string fullPath)
   {
      try
      {
         // Path is a file?
         if (File.Exists(fullPath))
         {
            return new LogFileInfo(Path.GetDirectoryName(fullPath),
                                   Path.GetFileNameWithoutExtension(fullPath),
                                   Path.GetExtension(fullPath));
         }

         // Path is a directory?
         if (Directory.Exists(fullPath))
            return new LogFileInfo(fullPath, null, null);

         var folderPath = Path.GetDirectoryName(fullPath);

         if (String.IsNullOrWhiteSpace(folderPath))
         {
            if (_loggedNotFoundPath.TryAdd(fullPath, null))
               SelfLog.Write($"Log folder path does not exist: '{fullPath}'.");

            return default;
         }

         if (!Directory.Exists(folderPath))
         {
            if (_loggedNotFoundPath.TryAdd(folderPath, null))
               SelfLog.Write($"Log folder path does not exist. Checked paths: '{fullPath}' and '{folderPath}'.");

            return default;
         }

         return new LogFileInfo(folderPath,
                                Path.GetFileNameWithoutExtension(fullPath),
                                Path.GetExtension(fullPath));
      }
      catch (Exception ex)
      {
         SelfLog.Write(ex.ToString());
         return default;
      }
   }

   private readonly struct LogFileInfo
   {
      public string? FolderPath { get; }
      public string? FileName { get; }
      public string? FileExtension { get; }

      public LogFileInfo(string? folderPath, string? fileName, string? fileExtension)
      {
         FolderPath = String.IsNullOrWhiteSpace(folderPath) ? null : folderPath;
         FileName = String.IsNullOrWhiteSpace(fileName) ? null : fileName;
         FileExtension = String.IsNullOrWhiteSpace(fileExtension) ? null : fileExtension;
      }
   }

   public void Dispose()
   {
      _periodicCleanup.Dispose();
   }

   private sealed class FilePathComparer(StringComparer stringComparer) : IEqualityComparer<(string, bool)>
   {
      public bool Equals((string, bool) x, (string, bool) y)
      {
         return stringComparer.Equals(x.Item1, y.Item1) && x.Item2 == y.Item2;
      }

      public int GetHashCode((string, bool) obj)
      {
         return unchecked(stringComparer.GetHashCode(obj.Item1) * 397) ^ obj.Item2.GetHashCode();
      }
   }
}
