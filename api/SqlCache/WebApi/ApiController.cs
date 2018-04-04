using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Text;
using System.Web;
using System.Configuration;
using System.Linq;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace SqlCache
{
    /*
        // List all files available - with the total of rows syncronized and the ones not syncronized
        api/cachedb/all/list

        // Download all files
        api/cachedb/all/download

        // Get Json of the fragment
        api/cachedb/fragments/read/23

        // Update Json of the fragment
        api/cachedb/fragments/write/23

        // Delete Json of the fragment
        api/cachedb/fragments/delete/23

        // Open Form to edit fragment
        api/cachedb/fragments/edit/23

        // List json of fragments with the name 'Rafael'
        api/cachedb/fragments/search/Rafael+Melo

        // List json of fragments with the parent equals to 123123
        api/cachedb/fragments/foreign/Parent/123123

        // List all fragments of page 1 (with 50 rows each page)
        api/cachedb/fragments/list/1/50

        // List history of changes for fragment 23
        api/cachedb/fragments/history/23

        // List all rows not syncronized with DB
        api/cachedb/fragments/changes

        // Download fragments file
        api/cachedb/fragments/download     

    */

    [RoutePrefix("api/sqlcache")]
    public class ApiController : System.Web.Http.ApiController
    {


        [HttpGet]
        [Route("execute")]
        public HttpResponseMessage Execute(string connection = null, string sql = null, string token = null)
        {
            if (token != SqlCacheConnection.GetPublicToken()) throw new System.ArgumentOutOfRangeException("Invalid token.");
            var sb = new StringBuilder();
            if (!string.IsNullOrEmpty(connection) && !string.IsNullOrEmpty(sql))
            {
                var connString = ConfigurationManager.ConnectionStrings[connection].ConnectionString;
                using (var conn = new SqlCacheConnection(connString))
                {
                    var data = conn.Execute(sql);
                    sb.Append(JsonConvert.SerializeObject(new { Count = data }));
                }
            }
            var content = new StringContent(sb.ToString(), System.Text.Encoding.UTF8, "text/html");
            var response = new HttpResponseMessage { Content = content };
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
            return response;
        }

        [HttpGet]
        [Route("scalar")]
        public HttpResponseMessage Scalar(string connection = null, string sql = null, string token = null)
        {
            if (token != SqlCacheConnection.GetPublicToken()) throw new System.ArgumentOutOfRangeException("Invalid token.");
            var sb = new StringBuilder();
            if (!string.IsNullOrEmpty(connection) && !string.IsNullOrEmpty(sql))
            {
                var connString = ConfigurationManager.ConnectionStrings[connection].ConnectionString;
                using (var conn = new SqlCacheConnection(connString))
                {
                    var data = conn.ExecuteScalar<object>(sql);
                    sb.Append(JsonConvert.SerializeObject(new { Result = data }));
                }
            }
            var content = new StringContent(sb.ToString(), System.Text.Encoding.UTF8, "text/html");
            var response = new HttpResponseMessage { Content = content };
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
            return response;
        }

        [HttpGet]
        [Route("newrow")]
        public HttpResponseMessage NewRow(string connection = null, string table = null, string token = null)
        {
            if (token != SqlCacheConnection.GetPublicToken()) throw new System.ArgumentOutOfRangeException("Invalid token.");
            var sb = new StringBuilder();
            if (!string.IsNullOrEmpty(connection) && !string.IsNullOrEmpty(table))
            {
                var connString = ConfigurationManager.ConnectionStrings[connection].ConnectionString;
                using (var conn = new SqlCacheConnection(connString))
                {
                    var data = conn.NewRow(table);
                    sb.Append(JsonConvert.SerializeObject(data));
                }
            }
            var content = new StringContent(sb.ToString(), System.Text.Encoding.UTF8, "text/html");
            var response = new HttpResponseMessage { Content = content };
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
            return response;
        }

        [HttpPost]
        [Route("save")]
        public HttpResponseMessage Save(string connection, string table, string token, [FromBody] dynamic row)
        {
            if (token != SqlCacheConnection.GetPublicToken()) throw new System.ArgumentOutOfRangeException("Invalid token.");
            var sb = new StringBuilder();
            if (!string.IsNullOrEmpty(connection) && !string.IsNullOrEmpty(table))
            {
                var connString = ConfigurationManager.ConnectionStrings[connection].ConnectionString;
                using (var conn = new SqlCacheConnection(connString))
                {
                    var id = conn.Save(table, row);
                    sb.Append(JsonConvert.SerializeObject(new { Id = id }));
                }
            }
            var content = new StringContent(sb.ToString(), System.Text.Encoding.UTF8, "text/html");
            var response = new HttpResponseMessage { Content = content };
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
            return response;
        }

        [HttpGet]
        [Route("query")]
        public HttpResponseMessage Query(string connection = null, string sql = null, string token = null)
        {
            if (token != SqlCacheConnection.GetPublicToken()) throw new System.ArgumentOutOfRangeException("Invalid token.");
            var sb = new StringBuilder();
            if (!string.IsNullOrEmpty(connection) && !string.IsNullOrEmpty(sql))
            {
                var connString = ConfigurationManager.ConnectionStrings[connection].ConnectionString;
                using (var conn = new SqlCacheConnection(connString))
                {
                    var data = conn.Query(sql).ToList();
                    sb.Append(JsonConvert.SerializeObject(data));
                }
            }
            var content = new StringContent(sb.ToString(), System.Text.Encoding.UTF8, "text/html");
            var response = new HttpResponseMessage { Content = content };
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
            return response;
        }



        [HttpGet]
        [Route("terminal")]
        public HttpResponseMessage Terminal(string connectionString = null, string sql = null, string token = null)
        {
            if (token != SqlCacheConnection.GetPublicToken()) throw new System.ArgumentOutOfRangeException("Invalid token.");
            var sb = new StringBuilder();
            try
            {
                sb.AppendLine("<html><head><style> td { padding: 10px; }</style></head><body>");
                sb.AppendLine("<h1>SQL Cache [Terminal]</h1>");
                sb.AppendLine("<form method='GET'>");
                sb.AppendLine($"<input name='token' id='token' style='width:100%; display:block;' value='{token}'>");
                sb.AppendLine($"<select name='connectionString' id='connectionString' style='width:100%; display:block;'>");
                sb.AppendLine($"<option value=''>- Select Connnection String -</option>");
                foreach (ConnectionStringSettings connString in ConfigurationManager.ConnectionStrings)
                {
                    sb.AppendLine($"<option {(connString.Name == connectionString ? "selected" : string.Empty)}>{connString.Name}</option>");
                }
                sb.AppendLine($"</select>");
                sb.AppendLine($"<textarea name='sql' id='sql' style='width:100%; display:block; height: 100px'>{HttpUtility.HtmlEncode(sql)}</textarea>");
                sb.AppendLine("<input type='submit' value='Execute' style='display:block'>");
                sb.AppendLine("</form>");
                if (!string.IsNullOrEmpty(connectionString) && !string.IsNullOrEmpty(sql))
                {
                    var connString = ConfigurationManager.ConnectionStrings[connectionString].ConnectionString;
                    using (var conn = new SqlCacheConnection(connString))
                    {
                        var data = conn.Query(sql).ToList();
                        JObject first = data.FirstOrDefault();
                        sb.AppendLine("<hr />");
                        sb.AppendLine("<table border=1 cellspacing='0' style='width:100%'>");
                        sb.AppendLine("<tr>");
                        var hasImage = false;
                        var imageKey = string.Empty;
                        foreach (var col in first)
                        {
                            if (conn.ImageUrl != null && conn.ImageUrl.Contains($"{{{col.Key}}}"))
                            {
                                imageKey = col.Key;
                                hasImage = true;
                            }
                        }
                        if (hasImage)
                        {
                            sb.AppendLine($"<td></td>");
                        }
                        foreach (var col in first)
                        {
                            sb.AppendLine($"<td>{col.Key}</td>");
                        }
                        sb.AppendLine("</tr>");
                        foreach (JObject row in data)
                        {
                            sb.AppendLine("<tr>");
                            if (hasImage)
                            {
                                var imagePath = conn.ImageUrl.Replace($"{{{imageKey}}}", row[imageKey].ToString());
                                sb.AppendLine($"<td><img src='{imagePath}' /></td>");
                            }
                            foreach (var col in first)
                            {
                                sb.AppendLine($"<td>{HttpUtility.HtmlEncode(row[col.Key])}</td>");
                            }
                            sb.AppendLine("</tr>");
                        }
                        sb.AppendLine("</table>");
                    }
                }
                sb.AppendLine("</body><html>");
            }
            catch(System.Exception ex)
            {
                sb.AppendLine(ex.ToString());
            }
            var content = new StringContent(sb.ToString(), System.Text.Encoding.UTF8, "text/html");
            var response = new HttpResponseMessage { Content = content };
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
            return response;
        }


    }
}