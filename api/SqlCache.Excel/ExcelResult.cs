using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace SqlCache.Excel
{
    public class ExcelDocumentResult : IHttpActionResult
    {
        private string _filePath;
        private Dictionary<string, string> _header;

        public ExcelDocumentResult(List<dynamic> results, Dictionary<string, string> header = null,
            string filePath = null)
        {
            if (results == null) throw new ArgumentNullException("results");
            this._filePath = filePath;
            this._header = header;
            using (var doc = new ClosedXML.Excel.XLWorkbook())
            {
                var first = results.FirstOrDefault() as JObject;
                if (first != null)
                {
                    var sheet = doc.AddWorksheet("Results");
                    var row = 1;
                    var col = 1;
                    if (this._header == null)
                    {
                        this._header = new Dictionary<string, string>();
                        foreach (var field in first)
                        {
                            this._header[field.Key] = field.Key;
                            col++;
                        }

                    }
                    col = 1;
                    foreach (var field in this._header)
                    {
                        sheet.Cell(row, col).Value = field.Value;
                        col++;
                    }
                    row++;
                    foreach (JObject item in results)
                    {
                        col = 1;
                        foreach (var field in this._header)
                        {
                            var sb = new StringBuilder((string)item[field.Key].ToObject(typeof(string)));
                            if (sb.Length < 32000)
                            {
                                sheet.Cell(row, col).Value = sb.ToString();
                            }
                            else
                            {
                                sheet.Cell(row, col).Value = sb.ToString().Substring(0, 32000) + "[.....]";
                            }
                            col++;
                        }
                        row++;
                    }
                }
                doc.SaveAs(_filePath);
            }
        }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StreamContent(File.OpenRead(_filePath))
            };

            var contentType = MimeMapping.GetMimeMapping(Path.GetExtension(_filePath));
            response.Content.Headers.ContentType = new MediaTypeHeaderValue(contentType);
            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = Path.GetFileName(_filePath)
            };

            return Task.FromResult(response);
        }
    }

}
