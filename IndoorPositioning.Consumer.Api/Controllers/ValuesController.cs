using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace IndoorPositioning.Consumer.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoggerController : ControllerBase
    {
        private static string logFile = "raspberry-logs.log";
        private static FileStream file = System.IO.File.OpenWrite(logFile);

        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] Log log)
        {
            byte[] bytes = Encoding.Default.GetBytes(log.ToString());
            file.Write(bytes);
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }

    public class Log
    {
        public string log { get; set; }

        public override string ToString()
        {
            return $"{{log : {log}}}{Environment.NewLine}";
        }
    }
}
