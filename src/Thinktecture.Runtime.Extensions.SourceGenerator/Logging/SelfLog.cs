using System.Diagnostics;

namespace Thinktecture.Logging;

public static class SelfLog
{
   private const string _FILE_NAME = "ThinktectureRuntimeExtensionsSourceGenerator.log";

   private static readonly object _lock = new();

#pragma warning disable RS1035 // The symbol is banned for use by analyzers: Do not do file IO in analyzers
   private static readonly Lazy<string> _path = new(() => Path.GetFullPath(Path.Combine(Path.GetTempPath(), _FILE_NAME)));
#pragma warning restore RS1035 // The symbol is banned for use by analyzers: Do not do file IO in analyzers

   private static bool _dontWrite; // to avoid flooding if the file system is not available

   public static void Write(string message, DateTime? datetime = null)
   {
      if (_dontWrite)
         return;

      try
      {
         message = $"[{datetime ?? DateTime.Now:O}] {message}\n";

         lock (_lock)
         {
#pragma warning disable RS1035 // The symbol is banned for use by analyzers: Do not do file IO in analyzers
            File.AppendAllText(_path.Value, message);
#pragma warning restore RS1035 // The symbol is banned for use by analyzers: Do not do file IO in analyzers
         }
      }
      catch (Exception ex)
      {
         Debug.WriteLine(ex);
         _dontWrite = true;
      }
   }

   public static void Write(DateTime datetime, string logLevel, string source, string message)
   {
      Write($"| [{logLevel}] | [{source}] {message}", datetime);
   }
}
