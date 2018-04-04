using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Humanitas.Interfaces
{
    public class PagedList<T> : List<T>
    {

        public PagedList(IList<T> list)
        {
            this.AddRange(list);
        }

        public PagedList()
        {
        }

        public long TotalOfRecords { get; set; }


    }
}
