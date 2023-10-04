using System.Diagnostics;

namespace Thinktecture.Logging;

public class SelfLog
{
   private const string _FILE_NAME = "ThinktectureRuntimeExtensionsSourceGenerator.log";

   public static void Write(string message)
   {
      try
      {
#pragma warning disable RS1035
         var fullPath = Path.Combine(Path.GetTempPath(), _FILE_NAME);
         File.AppendAllText(fullPath, $"[{DateTime.Now:O}] {message}{Environment.NewLine}");
#pragma warning restore RS1035
      }
      catch (Exception ex)
      {
         Debug.WriteLine(ex);
      }
   }

   public static void Write(DateTime datetime, string logLevel, string source, string message)
   {
      Write($"[{datetime} | {logLevel}] | [{source}] {message}");
   }
}
