using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Humanitas.Interfaces
{
    public class LibraryBook
    {

        public long LibraryBookId { get; set; }

        public string LibraryId { get; set; }

        public string BookId { get; set; }

        public string FileName { get; set; }

        public int? Edition { get; set; }

        public int? Year { get; set; }

        public string Isbn { get; set; }

        public string Publisher { get; set; }

        public string Translator { get; set; }

        public DateTime? PurchasedDate { get; set; }

        public decimal? Price { get; set; }

        public int? TotalOfPages { get; set; }

        public decimal? SizeX { get; set; }

        public decimal? SizeY { get; set; }

        public decimal? SizeZ { get; set; }

        public string Comments { get; set; }

    }
}
