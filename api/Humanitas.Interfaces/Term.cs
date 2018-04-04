using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Humanitas.Interfaces
{
    public class Term
    {

        public long TermId { get; set; }

        public string Name { get; set; }

        public int Type { get; set; }

        public long? ParentId { get; set; }

    }
}
