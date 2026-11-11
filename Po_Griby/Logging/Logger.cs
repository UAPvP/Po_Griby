using System.Runtime.CompilerServices;

namespace Po_Griby.Logging;

[Flags]
public enum LogLevel
{
    None = 0,
    Debug = 1 << 0,
    Info = 1 << 1,
    Warning = 1 << 2,
    Error = 1 << 3,
    Critical = 1 << 4,
    All = Debug | Info | Warning | Error | Critical
}

public struct LogEntry(
    DateTime timestamp,
    LogLevel level,
    string message,
    string sourceFile = "",
    string sourceMethod = "",
    int sourceLine = 0,
    Exception? exception = null)
{
    public DateTime Timestamp = timestamp;
    public readonly LogLevel Level = level;
    public readonly string Message = message;
    public readonly string SourceFile = sourceFile;
    public readonly string SourceMethod = sourceMethod;
    public readonly int SourceLine = sourceLine;
    public readonly Exception? Exception = exception;
}

public abstract class LogProvider
{
    public abstract void Write(LogEntry entry);

    protected virtual string Format(LogEntry entry)
    {
        string message =
            $"[{entry.Timestamp.ToShortTimeString()}] " +
            $"[{entry.Level}] " +
            $"[{Path.GetFileName(entry.SourceFile)}::{entry.SourceMethod}:{entry.SourceLine}] " +
            entry.Message;

        if (entry.Exception != null)
            message += Environment.NewLine + entry.Exception;

        return message;
    }
}

public class ConsoleLogProvider : LogProvider
{
    public LogLevel EnabledLogLevels { get; set; } = LogLevel.All;

    public Dictionary<LogLevel, ConsoleColor> Colors { get; } = new()
    {
        [LogLevel.Debug] = ConsoleColor.Green,
        [LogLevel.Info] = ConsoleColor.White,
        [LogLevel.Warning] = ConsoleColor.Yellow,
        [LogLevel.Error] = ConsoleColor.Red,
        [LogLevel.Critical] = ConsoleColor.DarkRed
    };

    public override void Write(LogEntry entry)
    {
        if ((EnabledLogLevels & entry.Level) == 0)
            return;

        var previousColor = Console.ForegroundColor;
        try
        {
            if (Colors.TryGetValue(entry.Level, out var color))
                Console.ForegroundColor = color;

            Console.WriteLine(Format(entry));
        }
        finally
        {
            Console.ForegroundColor = previousColor;
        }
    }
}

public class FileLogProvider(string filePath) : LogProvider
{
    private readonly string _filePath = filePath;
    public LogLevel EnabledLogLevels { get; set; } = LogLevel.All;

    public override void Write(LogEntry entry)
    {
        if ((EnabledLogLevels & entry.Level) != 0)
            File.AppendAllText(_filePath, Format(entry) + Environment.NewLine);
    }
}

public class Logger
{
    private readonly LogProvider[] _providers;
    public static Logger Default { get; } = new(new ConsoleLogProvider());

    public T? GetProvider<T>() where T : LogProvider => _providers.OfType<T>().FirstOrDefault();

    public Logger(params LogProvider[] providers)
    {
        if (providers.DistinctBy(provider => provider.GetType()).Count() != providers.Length)
            throw new ArgumentException("Duplicate provider types are not allowed.");

        _providers = providers;
    }

    public void Log(
        LogLevel level,
        string message,
        Exception? exception = null,
        [CallerMemberName] string method = "",
        [CallerFilePath] string file = "",
        [CallerLineNumber] int line = 0)
    {
        if (level == LogLevel.None || (level & (level - 1)) != 0 || (level & ~LogLevel.All) != 0)
            throw new ArgumentOutOfRangeException(nameof(level), "Log levels must contain exactly one valid flag.");

        var entry = new LogEntry(DateTime.Now, level, message, file, method, line, exception);
        foreach (var provider in _providers)
            provider.Write(entry);
    }
}
