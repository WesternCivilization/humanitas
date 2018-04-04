using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Humanitas.Services.Interfaces
{
    public interface ITerminalService
    {

        string Run(string cmds, string userId);

        string SaveObject(Dictionary<string, string> keyValues, string userId);

    }
}
