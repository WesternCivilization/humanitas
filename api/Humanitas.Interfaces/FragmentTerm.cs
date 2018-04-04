using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Humanitas.Interfaces
{
    public class FragmentTerm
    {

        public long FragmentTermId { get; set; }

        public long FragmentId { get; set; }

        public long TermId { get; set; }

        public long Type { get; set; }

        public string Description { get; set; }

    }
}
