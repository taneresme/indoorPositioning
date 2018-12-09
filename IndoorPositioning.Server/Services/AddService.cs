using IndoorPositioning.Server.Clients;
using IndoorPositioning.Server.Database.Dao;
using IndoorPositioning.Server.Database.Model;
using Newtonsoft.Json;

namespace IndoorPositioning.Server.Services
{
    public class AddService : BaseService, IService
    {
        public AddService(ServiceClient serviceClient) : base(serviceClient) { }

        public void Service(string data)
        {
            string inData = data.ToLower();
            string[] dataItems = inData.Split(' ');

            /* Check data */
            if (dataItems.Length < 2)
            {
                ServiceClient.Send(INVALID_PARAMETERS_ERROR);
                return;
            }

            string command = dataItems[1];

            /* Determine the command */
            if ("environment".Equals(command)) AddEnvironment(data);
            else ServiceClient.Send(UNKNOWN_COMMAND_ERROR);
        }

        /* Updates the beacon that is provided as json */
        private void AddEnvironment(string data)
        {
            /* we are triming data below, splitting it by blank chars is not a valid way.
             * because blank char can become in the json object as well */
            string command = "add environment ";
            string json = data.Substring(command.Length);

            Environment environment = JsonConvert.DeserializeObject<Environment>(json);
            EnvironmentDao environmentDao = new EnvironmentDao();
            environmentDao.NewEnvironment(environment);
            ServiceClient.Send(OK);
        }
    }
}
