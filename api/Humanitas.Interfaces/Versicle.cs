using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Humanitas.Interfaces
{
    public class Versicle
    {

        public long VersicleId { get; set; }

        public long DomainId { get; set; }

        public long? ChapterId { get; set; }

        public string Text { get; set; }

        public int SortNum { get; set; }

    }
}
