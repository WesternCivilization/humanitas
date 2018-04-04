using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Humanitas.Interfaces
{
    public class SpeciesPhoto
    {

        public long PhotoId { get; set; }

        public string FileName { get; set; }

        public long? KingdomId { get; set; }

        public long? SpecieId { get; set; }

        public long? FeatureId { get; set; }

    }
}
