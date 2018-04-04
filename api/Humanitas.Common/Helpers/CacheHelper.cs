using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Humanitas.Common.Helpers
{
    static public class CacheHelper
    {

        static public bool TryFromCache(string key, out dynamic data)
        {
            var path = HttpContext.Current.Server.MapPath($"~/Cache/{key}_{DateTime.Now.ToString("dd")}.cache");
            if (System.IO.File.Exists(path))
            {
                var json = System.IO.File.ReadAllText(path);
                var serializerSettings = new JsonSerializerSettings();
                serializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                data = JsonConvert.DeserializeObject(json, serializerSettings);
                return true;
            }
            else
            {
                data = null;
                return false;
            }
        }

        public static void Clear()
        {
            var files = Directory.GetFiles(HttpContext.Current.Server.MapPath($"~/Cache/"), "*.cache");
            foreach(var file in files)
                System.IO.File.Delete(file);
        }

        public static void SetCache(string key, dynamic result)
        {
            var path = System.Web.HttpContext.Current.Server.MapPath($"~/Cache/{key}_{DateTime.Now.ToString("dd")}.cache");
            var serializerSettings = new JsonSerializerSettings();
            serializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            System.IO.File.WriteAllText(path, JsonConvert.SerializeObject(result, serializerSettings).ToString());
        }

    }
}
