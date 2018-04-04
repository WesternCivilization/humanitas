using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using Logging;
using System.Text;
using System.Web;
using System.IO;
using System.Configuration;
using System;
using System.Text.RegularExpressions;

namespace Logging
{
    [RoutePrefix("api/logger")]
    public class LoggerController : ApiController
    {

        [HttpGet]
        [Route("trace")]
        public HttpResponseMessage Trace(string file = null)
        {
            //if (!LogTracer.Instance.EnabledWebApiTrace) return new HttpResponseMessage(System.Net.HttpStatusCode.NotFound);
            var sb = new StringBuilder();
            if (string.IsNullOrEmpty(file))
            {
                sb.AppendLine("<html><body>");
                sb.AppendLine($"<h1>Calendar</h1>");
                sb.AppendLine("<table border='1' style='width:100%'>");

                sb.AppendLine("<tr>");
                sb.AppendLine("<td>Day</td>");
                sb.AppendLine("<td>00h</td>");
                sb.AppendLine("<td>01h</td>");
                sb.AppendLine("<td>02h</td>");
                sb.AppendLine("<td>03h</td>");
                sb.AppendLine("<td>04h</td>");
                sb.AppendLine("<td>05h</td>");
                sb.AppendLine("<td>06h</td>");
                sb.AppendLine("<td>07h</td>");
                sb.AppendLine("<td>08h</td>");
                sb.AppendLine("<td>09h</td>");
                sb.AppendLine("<td>10h</td>");
                sb.AppendLine("<td>11h</td>");
                sb.AppendLine("<td>12h</td>");
                sb.AppendLine("<td>13h</td>");
                sb.AppendLine("<td>14h</td>");
                sb.AppendLine("<td>15h</td>");
                sb.AppendLine("<td>16h</td>");
                sb.AppendLine("<td>17h</td>");
                sb.AppendLine("<td>18h</td>");
                sb.AppendLine("<td>19h</td>");
                sb.AppendLine("<td>20h</td>");
                sb.AppendLine("<td>21h</td>");
                sb.AppendLine("<td>22h</td>");
                sb.AppendLine("<td>23h</td>");
                sb.AppendLine("</tr>");

                var dates = Directory.GetFiles(ConfigurationManager.AppSettings["LogFolder"], "*.log")
                    .Select(f => Path.GetFileNameWithoutExtension(f).Split('_')[0])
                    .Distinct()
                    .Select(f => new DateTime(int.Parse(f.Substring(0, 4)), int.Parse(f.Substring(4, 2)), int.Parse(f.Substring(6, 2))))
                    .ToList();

                foreach (var date in dates)
                {
                    sb.AppendLine("<tr>");
                    sb.AppendLine($"<td>{date.ToString("yyyy-MM-dd")}</td>");
                    sb.AppendLine($"<td>{GetDateHourLink(date, 0)}</td>");
                    sb.AppendLine($"<td>{GetDateHourLink(date, 1)}</td>");
                    sb.AppendLine($"<td>{GetDateHourLink(date, 2)}</td>");
                    sb.AppendLine($"<td>{GetDateHourLink(date, 3)}</td>");
                    sb.AppendLine($"<td>{GetDateHourLink(date, 4)}</td>");
                    sb.AppendLine($"<td>{GetDateHourLink(date, 5)}</td>");
                    sb.AppendLine($"<td>{GetDateHourLink(date, 6)}</td>");
                    sb.AppendLine($"<td>{GetDateHourLink(date, 7)}</td>");
                    sb.AppendLine($"<td>{GetDateHourLink(date, 8)}</td>");
                    sb.AppendLine($"<td>{GetDateHourLink(date, 9)}</td>");
                    sb.AppendLine($"<td>{GetDateHourLink(date, 10)}</td>");
                    sb.AppendLine($"<td>{GetDateHourLink(date, 11)}</td>");
                    sb.AppendLine($"<td>{GetDateHourLink(date, 12)}</td>");
                    sb.AppendLine($"<td>{GetDateHourLink(date, 13)}</td>");
                    sb.AppendLine($"<td>{GetDateHourLink(date, 14)}</td>");
                    sb.AppendLine($"<td>{GetDateHourLink(date, 15)}</td>");
                    sb.AppendLine($"<td>{GetDateHourLink(date, 16)}</td>");
                    sb.AppendLine($"<td>{GetDateHourLink(date, 17)}</td>");
                    sb.AppendLine($"<td>{GetDateHourLink(date, 18)}</td>");
                    sb.AppendLine($"<td>{GetDateHourLink(date, 19)}</td>");
                    sb.AppendLine($"<td>{GetDateHourLink(date, 20)}</td>");
                    sb.AppendLine($"<td>{GetDateHourLink(date, 21)}</td>");
                    sb.AppendLine($"<td>{GetDateHourLink(date, 22)}</td>");
                    sb.AppendLine($"<td>{GetDateHourLink(date, 23)}</td>");
                    sb.AppendLine("</tr>");
                }

                sb.AppendLine("</table>");
                sb.AppendLine("</body></html>");
            }
            else
            {
                sb.AppendLine("<html><body>");
                sb.AppendLine($"<h1>Calendar</h1>");
                sb.AppendLine("<table border='1' style='width:100%'>");

                sb.AppendLine("<tr>");
                sb.AppendLine("<td>Level</td>");
                sb.AppendLine("<td>EventDate</td>");
                sb.AppendLine("<td>Type</td>");
                sb.AppendLine("<td>Namespace</td>");
                sb.AppendLine("<td>ClassName</td>");
                sb.AppendLine("<td>Process</td>");
                sb.AppendLine("<td>Elapsed</td>");
                sb.AppendLine("<td>Message</td>");
                sb.AppendLine("<td>Exception</td>");
                sb.AppendLine("<td>Origin</td>");
                sb.AppendLine("</tr>");

                var fileContent = File.ReadAllText(Path.Combine(ConfigurationManager.AppSettings["LogFolder"], $"{file}"));

                var row = new StringBuilder();
                for (var i = fileContent.Length - 1; i >= 0; i--)
                {
                    row.Insert(0, fileContent[i]);
                    if (row.Length >= 3 && fileContent[i] == '|' && fileContent[i + 1] == ';' && fileContent[i + 2] == '|')
                    {
                        var str = row.ToString().Trim("|;|".ToArray());
                        PrintLogRow(ref sb, str);
                        row = new StringBuilder();
                    }
                }
                PrintLogRow(ref sb, row.ToString().Trim("|;|".ToArray()));

                sb.AppendLine("</table>");
                sb.AppendLine("</body></html>");
            }

            var content = new StringContent(sb.ToString(), System.Text.Encoding.UTF8, "text/html");
            var response = new HttpResponseMessage { Content = content };
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
            return response;
        }

        private void PrintLogRow(ref StringBuilder sb, string str)
        {
            if(str != null && str.Length > 0)
            {
                var fields = Regex.Split(str.TrimEnd('|', '~'), "\\|~\\|");
                sb.AppendLine("<tr>");
                sb.AppendLine($"<td>{fields.ElementAtOrDefault(9)}</td>");
                sb.AppendLine($"<td>{fields.ElementAtOrDefault(0)}</td>");
                sb.AppendLine($"<td>{fields.ElementAtOrDefault(1)}</td>");
                sb.AppendLine($"<td>{fields.ElementAtOrDefault(2)}</td>");
                sb.AppendLine($"<td>{fields.ElementAtOrDefault(3)}</td>");
                sb.AppendLine($"<td>{fields.ElementAtOrDefault(4)}</td>");
                sb.AppendLine($"<td>{fields.ElementAtOrDefault(5)}</td>");
                sb.AppendLine($"<td>{fields.ElementAtOrDefault(6)}</td>");
                sb.AppendLine($"<td>{fields.ElementAtOrDefault(7)}</td>");
                sb.AppendLine($"<td>{fields.ElementAtOrDefault(8)}</td>");
                sb.AppendLine("</tr>");
            }
        }

        private string GetDateHourLink(DateTime date, int hour)
        {
            var fileName = $"{date.ToString("yyyyMMdd")}_{hour.ToString("00")}.log";
            var filePath = Directory.GetFiles(ConfigurationManager.AppSettings["LogFolder"], fileName).FirstOrDefault();
            if (filePath != null)
            {
                var info = new FileInfo(filePath);
                var size = info.Length / (decimal)1024;
                return $"<a href='trace?file={fileName}'>{size.ToString("#.00")}Kb</a>";
            }
            else
            {
                return null;
            }
        }
    }
}