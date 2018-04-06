using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Humanitas.Services.Interfaces
{
    public interface ITerminalService
    {

        void Run(string cmds, string userId, out string html, out string sql);

        string SaveObject(Dictionary<string, string> keyValues, string userId);

    }
}
