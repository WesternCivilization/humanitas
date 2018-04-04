using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Humanitas.Interfaces
{
    public class AppConfiguration
    {

        public AppConfiguration()
        {
            this.CachePath = ConfigurationManager.AppSettings["CachePath"];
            this.RepositoryPath = ConfigurationManager.AppSettings["RepositoryPath"];
            this.ConnectionString = ConfigurationManager.ConnectionStrings["Humanitas"].ConnectionString;
        }

        public string CachePath { get; set; }

        public string RepositoryPath { get; set; }

        public string ConnectionString { get; private set; }

    }
}
