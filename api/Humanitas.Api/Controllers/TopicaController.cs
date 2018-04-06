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
    [RoutePrefix("api/topica")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class TopicaController : ApiController
    {

        private Logger log = new Logger(typeof(TopicaController));

        private ITopicaService _topicaService = null;
        private IUserAccessService _userService = null;

        public TopicaController(ITopicaService topicaService, IUserAccessService userService)
        {
            this._topicaService = topicaService;
            this._userService = userService;
        }


        [HttpGet]
        [Route("autocomplete")]
        public IList<Option> Autocomplete(string type, string exp, string token)
        {
            using (var scope = log.Scope("Autocomplete()", Request.Headers))
            {
                try
                {
                    dynamic result = null;
                    result = this._topicaService.AutoComplete(type, exp, token);
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
        [Route("select")]
        public Option Select(string type, string value, string token)
        {
            CacheHelper.Clear();
            using (var scope = log.Scope("Select()", Request.Headers))
            {
                try
                {
                    return this._topicaService.Select(type, value, token);
                }
                catch (Exception ex)
                {
                    log.Error(scope, ex);
                    throw;
                }
            }
        }

        [HttpGet]
        [Route("folders")]
        public IList<Option> Folders(int domainId, string parent, string token)
        {
            using (var scope = log.Scope("Folders()", Request.Headers))
            {
                try
                {
                    return this._topicaService.Folders(domainId, parent != "null" ? parent : null, token);
                }
                catch (Exception ex)
                {
                    log.Error(scope, ex);
                    throw;
                }
            }
        }

        [HttpGet]
        [Route("references")]
        public IList<TimelineActivity> References(string parent, string token)
        {
            using (var scope = log.Scope("References()", Request.Headers))
            {
                try
                {
                    var list = this._topicaService.References(parent != "null" ? parent : null, token);
                    return list;
                }
                catch (Exception ex)
                {
                    log.Error(scope, ex);
                    throw;
                }
            }
        }

        [HttpGet]
        [Route("libraries")]
        public IList<Tag> Libraries(string token)
        {
            using (var scope = log.Scope("Libraries()", Request.Headers))
            {
                try
                {
                    return this._topicaService.Libraries(token);
                }
                catch (Exception ex)
                {
                    log.Error(scope, ex);
                    throw;
                }
            }
        }

        [HttpGet]
        [Route("books")]
        public dynamic Books(string libraryId, string tags, int start, string token)
        {
            using (var scope = log.Scope("Books()", Request.Headers))
            {
                try
                {
                    if (libraryId == "null") libraryId = null;
                    var list = this._topicaService.Books(libraryId, tags != null ? tags.Split(',') : null, start, 20, token);
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
        public Tag Detail(string tagId, string token)
        {
            using (var scope = log.Scope("Detail()", Request.Headers))
            {
                try
                {
                    var detail = this._topicaService.Detail(tagId, token);
                    if (tagId.Contains(','))
                        detail.Type = int.Parse(tagId.Split(',').LastOrDefault());
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
        [Route("book-detail")]
        public LibraryBook BookDetail(string bookId, string libraryId, string token)
        {
            using (var scope = log.Scope("BookDetail()", Request.Headers))
            {
                try
                {
                    var detail = this._topicaService.LibraryBook(bookId, libraryId, token);
                    return detail;
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
                        var success = this._topicaService.SaveImage(token, file);
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
        public dynamic Save(string token, Tag tag)
        {
            using (var scope = log.Scope("Image()", Request.Headers))
            {
                try
                {
                    if (tag.CreatedBy == Guid.Empty.ToString() ||
                        tag.CreatedBy == null)
                    {
                        tag.CreatedBy = this._userService.TokenToUserId(token);
                        tag.CreatedAt = DateTime.Now;
                    }
                    tag.UpdatedBy = this._userService.TokenToUserId(token);
                    tag.UpdatedAt = DateTime.Now;
                    if (tag.LibraryBook != null)
                    {
                        var book = JObject.FromObject(tag.LibraryBook);
                        this._topicaService.SaveLibraryBook(book, token);
                    }
                    var obj = JObject.FromObject(tag);
                    obj.Remove("LibraryBook");
                    obj.Remove("Tags");
                    obj.Remove("TotalOfFragments");
                    obj["TagId"] = obj["TagId"].ToString().ToUpper();
                    this._topicaService.SaveLinks(tag.TagId, obj["Links"], token);
                    this._topicaService.SaveEvents(tag.TagId, obj["Events"], token);
                    obj.Remove("Links");
                    obj.Remove("Events");
                    string sqlExecuted;
                    var id = this._topicaService.SaveTag(obj, token, out sqlExecuted);
                    return new
                    {
                        id = id,
                        sql = sqlExecuted
                    };

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