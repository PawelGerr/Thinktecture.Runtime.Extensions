using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Thinktecture.CodeAnalysis;

namespace Thinktecture.Logging;

public class FileSystemSinkProvider
{
   private static readonly object _instanceLock = new();
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
      _sinksByFilePath = new Dictionary<(string, bool), FileSystemSinkContext>();
      _lock = new object();
      _periodicCleanup = new PeriodicCleanup(this);
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

      var logFileInfos = GetFileInfos(fullPath);
      var newSink = CreateLogFileOrNull(logFileInfos, filePathMustBeUnique, initialBufferSize);

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
               (obsoleteSinks ??= new List<FileSystemSinkContext>()).Add(kvp.Value);
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

      var now = DateTime.Now;
      var fileName = filePathMustBeUnique
                        ? $"{logFileInfos.FileName ?? "ThinktectureRuntimeExtensions_logs"}_{now.Year}{now.Month:00}{now.Day:00}_{now.Hour:00}{now.Minute:00}{now.Second:00}_{Guid.NewGuid():N}{logFileInfos.FileExtension ?? ".log"}"
                        : $"{logFileInfos.FileName ?? "ThinktectureRuntimeExtensions_logs"}{logFileInfos.FileExtension ?? ".log"}";
      var logFilePath = Path.Combine(logFileInfos.FolderPath, fileName);

      return new FileSystemLoggingSink(logFilePath, initialBufferSize);
   }

   [SuppressMessage("MicrosoftCodeAnalysisCorrectness", "RS1035:Do not use APIs banned for analyzers")]
   private static LogFileInfo GetFileInfos(string fullPath)
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

         if (String.IsNullOrWhiteSpace(folderPath) || !Directory.Exists(folderPath))
            return default;

         return new LogFileInfo(folderPath,
                                Path.GetFileNameWithoutExtension(fullPath),
                                Path.GetExtension(fullPath));
      }
      catch (Exception ex)
      {
         Debug.WriteLine(ex);
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
}
