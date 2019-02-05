using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Terragistry.Web
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogsController : ControllerBase
    {
        public static List<string> Logs = new List<string>();

        [HttpGet]
        public async Task<IEnumerable<string>> ListLogsAsync()
        {
            return await Task.FromResult(Logs.ToArray().Reverse());
        }

        [HttpGet]
        [Route("clean")]
        public async Task<IEnumerable<string>> CleanLogsAsync()
        {
            Logs.Clear();
            return await Task.FromResult(Logs.ToArray().Reverse());
        }
    }
}