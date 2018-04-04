using Humanitas.Common.Helpers;
using Humanitas.Interfaces;
using Humanitas.Services.Interfaces;
using Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Humanitas.Api.Controllers
{
    [RoutePrefix("api/terminal")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class TerminalController : ApiController
    {

        private Logger log = new Logger(typeof(TerminalController));

        private ITerminalService _service = null;

        public TerminalController(ITerminalService service)
        {
            this._service = service;
        }

        [HttpPost]
        [Route("run")]
        public dynamic Run(string token, dynamic cmds)
        {
            var html = this._service.Run(cmds.Content.ToString(), token);
            CacheHelper.Clear();
            return new { Html = html };
        }

    }
}
