using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Humanitas.Interfaces
{
    public class Option
    {

        public string type { get; set; }

        public string value { get; set; }

        public string label { get; set; }

        public string link { get; set; }

        public bool canExpand { get; set; }

        public override string ToString()
        {
            return $"[{(canExpand ? "+" : "-")}] {label} ({value})";
        }

    }
}
