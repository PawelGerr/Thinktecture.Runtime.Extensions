using Thinktecture.CodeAnalysis;

namespace Thinktecture.Logging;

public sealed class LoggerFactory
{
   private readonly FileSystemSinkProvider _fileSystemSinkProvider;
   private readonly ThinktectureSourceGeneratorBase _owner;

   public LoggerFactory(ThinktectureSourceGeneratorBase owner)
   {
      _owner = owner;
      _fileSystemSinkProvider = FileSystemSinkProvider.GetOrCreate();
   }

   public ILogger CreateLogger(LogLevel logLevel, string filePath, bool filePathMustBeUnique, int initialBufferSize, string source)
   {
      if (logLevel is < LogLevel.Trace or > LogLevel.Error || String.IsNullOrWhiteSpace(filePath))
         return new SelfLogErrorLogger(source);

      try
      {
         var sink = _fileSystemSinkProvider.GetSinkOrNull(filePath, filePathMustBeUnique, initialBufferSize, _owner);

         return sink is null ? new SelfLogErrorLogger(source) : CreateLogger(logLevel, sink, source);
      }
      catch (Exception ex)
      {
         SelfLog.Write(ex.ToString());
         return new SelfLogErrorLogger(source);
      }
   }

   private ILogger CreateLogger(LogLevel logLevel, ILoggingSink sink, string source)
   {
      return logLevel switch
      {
         LogLevel.Information => new InformationLogger(this, sink, source),
         LogLevel.Warning => new WarningLogger(this, sink, source),
         LogLevel.Error => new ErrorLogger(this, sink, source),
         _ => new SelfLogErrorLogger(source),
      };
   }

   public void ReleaseSink(ILoggingSink sink)
   {
      _fileSystemSinkProvider.ReleaseSink(sink, _owner);
   }
}
