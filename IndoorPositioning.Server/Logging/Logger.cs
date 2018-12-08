using Microsoft.Extensions.Logging;

namespace IndoorPositioning.Server.Logging
{
    public class Logger
    {
        //public static ILoggerFactory LoggerFactory { get; } = new LoggerFactory();
        //public static ILogger CreateLogger<T>() => LoggerFactory.CreateLogger<T>();


        private static ILoggerFactory _Factory = null;

        public static void ConfigureLogger(ILoggerFactory factory)
        {
            factory.AddConsole(false);
            factory.AddDebug();
        }

        public static ILoggerFactory LoggerFactory
        {
            get
            {
                if (_Factory == null)
                {
                    _Factory = new LoggerFactory();
                    ConfigureLogger(_Factory);
                }
                return _Factory;
            }
            set { _Factory = value; }
        }
        public static ILogger CreateLogger<T>() => LoggerFactory.CreateLogger<T>();
    }
}
