using Humanitas.Interfaces;
using Humanitas.Services.Interfaces;
using Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Humanitas.Api.Controllers
{
    [RoutePrefix("api/user")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class UserController : ApiController
    {

        private Logger log = new Logger(typeof(UserController));

        private IUserAccessService _service = null;

        public UserController(IUserAccessService service)
        {
            this._service = service;
        }


        [HttpPost]
        [Route("login")]
        public string Login(dynamic user)
        {
            using (var scope = log.Scope("Login()", Request.Headers))
            {
                try
                {
                    return this._service.Login(new Interfaces.User
                    {
                        UserId = Guid.NewGuid().ToString().ToUpper(),
                        Name = user.name.ToString(),
                        Email = user.email.ToString(),
                        PhotoUrl = user.photoUrl.ToString(),
                        Provider = user.provider.ToString(),
                        ExternalId = user.id.ToString(),
                        Token = user.token.ToString(),
                        UserTypeId = "2",
                    });
                }
                catch (Exception ex)
                {
                    log.Error(scope, ex);
                    throw;
                }
            }
        }

        [HttpPost]
        [Route("logout")]
        public string Logout(dynamic user)
        {
            using (var scope = log.Scope("Logout()", Request.Headers))
            {
                try
                {
                    this._service.Logout(new Interfaces.User
                    {
                        UserId = Guid.NewGuid().ToString().ToUpper(),
                        Name = user.name.ToString(),
                        Email = user.email.ToString(),
                        PhotoUrl = user.photoUrl.ToString(),
                        Provider = user.provider.ToString(),
                        ExternalId = user.id.ToString(),
                        Token = user.token.ToString(),
                        UserTypeId = "2",
                    });
                    return user.token.ToString();
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
        public User Detail(string userId, string token)
        {
            using (var scope = log.Scope("Detail()", Request.Headers))
            {
                try
                {
                    var detail = this._service.Detail(userId, token);
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
        [Route("metrics")]
        public dynamic Metrics(string userId, string token)
        {
            using (var scope = log.Scope("Metrics()", Request.Headers))
            {
                try
                {
                    return this._service.Metrics(userId, token);
                }
                catch (Exception ex)
                {
                    log.Error(scope, ex);
                    throw;
                }
            }
        }

        [HttpGet]
        [Route("list")]
        public dynamic List(int start, string token)
        {
            using (var scope = log.Scope("List()", Request.Headers))
            {
                try
                {
                    var list = this._service.List(start, 20, token);
                    return new { totalOfRecords = list.TotalOfRecords, rows = list };
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
