using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Humanitas.Services.Interfaces
{
    public interface ISqlLiteService
    {

        string[] Tables();

        string CreatesTables();

        string CreatesIndexes();

        string Inserts(int? top = null);

        string LatestVersion(string rootPath);

        string GetVersion(string rootPath, int version);

        void InstallPackage(string scripts);

        string AutoCompleteDb();

    }
}
