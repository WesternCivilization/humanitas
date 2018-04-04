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
    [RoutePrefix("api/sqllite")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class SqlLiteController : ApiController
    {

        private Logger log = new Logger(typeof(SqlLiteController));

        private ISqlLiteService _service = null;

        public SqlLiteController(ISqlLiteService service)
        {
            this._service = service;
        }

        [HttpPost]
        [Route("post")]
        public HttpResponseMessage Post()
        {
            try
            {
                HttpResponseMessage result = null;

                var httpRequest = HttpContext.Current.Request;
                if (httpRequest.Files.Count > 0)
                {
                    var docfiles = new List<string>();
                    foreach (string file in httpRequest.Files)
                    {
                        var postedFile = httpRequest.Files[file];
                        var filePath = HttpContext.Current.Server.MapPath("~/Packages/" + postedFile.FileName);
                        postedFile.SaveAs(filePath);
                        try
                        {
                            this._service.InstallPackage(System.IO.File.ReadAllText(filePath));
                        }
                        catch(Exception ex)
                        {
                            var errorPath = System.IO.Path.ChangeExtension(filePath, ".exception");
                            System.IO.File.WriteAllText(errorPath, ex.ToString());
                        }
                        docfiles.Add(filePath);
                    }
                    result = Request.CreateResponse(HttpStatusCode.Created, docfiles);
                }
                else
                {
                    result = Request.CreateResponse(HttpStatusCode.BadRequest);
                }
                return result;
            }
            catch(Exception ex)
            {
                System.IO.File.WriteAllText(HttpContext.Current.Server.MapPath("~/Posted/Error.txt"), ex.ToString());
                throw;
            }
        }

        [HttpGet]
        [Route("autocomplete")]
        public string AutoComplete()
        {
            return this._service.AutoCompleteDb();
        }

        [HttpGet]
        [Route("latest")]
        public string Latest()
        {
            var fileName = this._service.LatestVersion(HostingEnvironment.MapPath("~/Downloads"));
            return "/Downloads/" + fileName;
        }

        [HttpGet]
        [Route("version")]
        public string Version(int version)
        {
            var fileName = this._service.GetVersion(HostingEnvironment.MapPath("~/Downloads"), version);
            return "/Downloads/" + fileName;
        }

    }
}
