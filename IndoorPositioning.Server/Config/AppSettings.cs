using Microsoft.Extensions.Configuration;
using System.IO;

namespace IndoorPositioning.Server.Config
{
    public class AppSettings
    {
        private static IConfiguration config;

        public static string DataSource => config["indoorpositioning:database:datasource"];

        public static int GatewayPort => int.Parse(config["indoorpositioning:communication:gatewayport"]);
        public static int ServicePort => int.Parse(config["indoorpositioning:communication:serviceport"]);

        static AppSettings()
        {
            config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();
        }
    }
}
