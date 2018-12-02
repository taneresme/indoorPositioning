using Microsoft.Extensions.Logging;

namespace IndoorPositioning.Server.Logging
{
    public class Logger
    {
        public static ILoggerFactory LoggerFactory { get; } = new LoggerFactory();
        public static ILogger CreateLogger<T>() => LoggerFactory.CreateLogger<T>();
    }
}
