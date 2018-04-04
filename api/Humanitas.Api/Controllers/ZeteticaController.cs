using Humanitas.Common.Helpers;
using Humanitas.Interfaces;
using Humanitas.Services.Interfaces;
using Logging;
using Newtonsoft.Json.Linq;
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
    [RoutePrefix("api/zetetica")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ZeteticaController : ApiController
    {

        private Logger log = new Logger(typeof(ZeteticaController));

        private IZeteticaService _zeteticaService = null;
        private IUserAccessService _userService = null;

        public ZeteticaController(IZeteticaService zeteticaService, IUserAccessService userService)
        {
            this._zeteticaService = zeteticaService;
            this._userService = userService;
        }


        [HttpGet]
        [Route("autocomplete")]
        public IList<Option> Autocomplete(string type, string expression, string token)
        {
            using (var scope = log.Scope("Autocomplete()", Request.Headers))
            {
                try
                {
                    return this._zeteticaService.AutoComplete(type, expression, token);
                }
                catch (Exception ex)
                {
                    log.Error(scope, ex);
                    throw;
                }
            }
        }

        [HttpGet]
        [Route("activities")]
        public dynamic Activities(string tag, string sort, int start, string token)
        {
            using (var scope = log.Scope("Activities()", Request.Headers))
            {
                try
                {
                    tag = tag != "undefined" ? tag : null;
                    tag = tag != "null" ? tag : null;
                    sort = sort != "undefined" ? sort : null;
                    sort = sort != "null" ? sort : null;
                    dynamic result = null;
                    var key = "activities_" + start + "_" + sort + "__";
                    var list = this._zeteticaService.Activities(tag, sort, start, 10, token);
                    result = new { totalOfRecords = list.TotalOfRecords, rows = list };
                    return result;
                }
                catch (Exception ex)
                {
                    log.Error(scope, ex);
                    throw;
                }
            }
        }

        [HttpGet]
        [Route("batchdata")]
        public dynamic BatchData(string type, string tag, string token)
        {
            using (var scope = log.Scope("BatchData()", Request.Headers))
            {
                try
                {
                    var lines = this._zeteticaService.BatchData(type != "undefined" ? type : null,
                        tag != "undefined" ? tag : null, token);
                    return new { lines = lines };
                }
                catch (Exception ex)
                {
                    log.Error(scope, ex);
                    throw;
                }
            }
        }

        [HttpGet]
        [Route("fragment")]
        public TimelineActivity Fragments(string fragmentId, string token)
        {
            using (var scope = log.Scope("Fragments()", Request.Headers))
            {
                try
                {
                    var fragment = this._zeteticaService.Fragment(fragmentId, token);
                    return fragment;
                }
                catch (Exception ex)
                {
                    log.Error(scope, ex);
                    throw;
                }
            }
        }

        [HttpGet]
        [Route("fragments")]
        public dynamic Fragments(long domainId, string tagId, int start, string token)
        {
            using (var scope = log.Scope("Fragments()", Request.Headers))
            {
                try
                {
                    if (tagId.Contains(",")) return null;
                    var list = this._zeteticaService.Fragments(domainId, tagId, start, 10, token);
                    return new { totalOfRecords = list.TotalOfRecords, rows = list };
                }
                catch (Exception ex)
                {
                    log.Error(scope, ex);
                    throw;
                }
            }
        }

        [HttpGet]
        [Route("detail")]
        public Fragment Detail(string fragmentId, string token)
        {
            using (var scope = log.Scope("Detail()", Request.Headers))
            {
                try
                {
                    var detail = this._zeteticaService.Detail(fragmentId, token);
                    if (fragmentId.Contains(','))
                        detail.Type = int.Parse(fragmentId.Split(',').LastOrDefault());
                    return detail;
                }
                catch (Exception ex)
                {
                    log.Error(scope, ex);
                    throw;
                }
            }
        }

        [HttpGet]
        [Route("score")]
        public bool Score(string fragmentId, short score, string token)
        {
            using (var scope = log.Scope("Score()", Request.Headers))
            {
                try
                {
                    return this._zeteticaService.Score(fragmentId, score, token);
                }
                catch (Exception ex)
                {
                    log.Error(scope, ex);
                    throw;
                }
            }
        }

        [HttpGet]
        [Route("listen")]
        public bool Listen(string fragmentId, string token)
        {
            using (var scope = log.Scope("Listen()", Request.Headers))
            {
                try
                {
                    return this._zeteticaService.Listen(fragmentId, token);
                }
                catch (Exception ex)
                {
                    log.Error(scope, ex);
                    throw;
                }
            }
        }

        [HttpPost]
        [Route("image")]
        public string Image(string token)
        {
            using (var scope = log.Scope("Image()", Request.Headers))
            {
                try
                {
                    if (HttpContext.Current.Request.Files.Count > 0)
                    {
                        var file = HttpContext.Current.Request.Files[0];
                        var success = this._zeteticaService.SaveImage(token, file);
                        return success ? "Saved." : "Not Saved.";
                    }
                    return "Not Saved.";
                }
                catch (Exception ex)
                {
                    log.Error(scope, ex);
                    throw;
                }
            }
        }

        [HttpPost]
        [Route("audio")]
        public string Audio(string token)
        {
            using (var scope = log.Scope("Audio()", Request.Headers))
            {
                try
                {
                    if (HttpContext.Current.Request.Files.Count > 0)
                    {
                        var file = HttpContext.Current.Request.Files[0];
                        var success = this._zeteticaService.SaveAudio(token, file);
                        return success ? "Saved." : "Not Saved.";
                    }
                    return "Not Saved.";
                }
                catch (Exception ex)
                {
                    log.Error(scope, ex);
                    throw;
                }
            }
        }

        [HttpPost]
        [Route("save")]
        public string Save(string token, Fragment fragment)
        {
            using (var scope = log.Scope("Save()", Request.Headers))
            {
                try
                {
                    if (fragment.CreatedBy == Guid.Empty.ToString() ||
                        fragment.CreatedBy == null)
                    {
                        fragment.CreatedBy = this._userService.TokenToUserId(token);
                        fragment.CreatedAt = DateTime.Now;
                    }
                    fragment.UpdatedBy = this._userService.TokenToUserId(token);
                    fragment.UpdatedAt = DateTime.Now;
                    var obj = JObject.FromObject(fragment);
                    obj["FragmentId"] = obj["FragmentId"].ToString().ToUpper();
                    obj.Remove("Tags");
                    return this._zeteticaService.SaveFragment(obj, token);
                }
                catch (Exception ex)
                {
                    log.Error(scope, ex);
                    throw;
                }
            }
        }
    }
}