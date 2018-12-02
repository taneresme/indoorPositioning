using System;

namespace IndoorPositioning.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Server server = new Server();
                server.Start();

                Console.WriteLine("Server started...");
            }
            catch (Exception ex)
            {
                
            }
            Console.WriteLine("Press any key to terminate the application...");
            Console.ReadKey();
        }
    }
}
