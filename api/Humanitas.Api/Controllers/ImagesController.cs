using Humanitas.Interfaces;
using Humanitas.Services.Interfaces;
using Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Humanitas.Api.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ImagesController : ApiController
    {
        private AppConfiguration _config;

        public ImagesController(AppConfiguration config)
        {
            this._config = config;
        }

        [HttpGet]
        [Route("api/images/{*imagePath}")]
        public IHttpActionResult Get(string imagePath)
        {
            var serverPath = Path.Combine(this._config.RepositoryPath, imagePath.Replace('/', '\\'));
            var fileInfo = new FileInfo(serverPath);

            return !fileInfo.Exists
                ? (IHttpActionResult)NotFound()
                : new FileResult(fileInfo.FullName);
        }

    }

    class FileResult : IHttpActionResult
    {
        private readonly string _filePath;
        private readonly string _contentType;

        public FileResult(string filePath, string contentType = null)
        {
            if (filePath == null) throw new ArgumentNullException("filePath");

            _filePath = filePath;
            _contentType = contentType;
        }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StreamContent(File.OpenRead(_filePath))
            };

            var contentType = _contentType ?? MimeMapping.GetMimeMapping(Path.GetExtension(_filePath));
            response.Content.Headers.ContentType = new MediaTypeHeaderValue(contentType);

            return Task.FromResult(response);
        }
    }
}