using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Humanitas.Interfaces
{
    public class Chapter
    {

        public long ChapterId { get; set; }

        public long DomainId { get; set; }

        public string Title { get; set; }

        public int SortNum { get; set; }

    }
}
