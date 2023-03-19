namespace Thinktecture.Logging;

public interface ILoggingSink
{
   void Write(string source, LogLevel logLevel, DateTime datetime, string message);
}
