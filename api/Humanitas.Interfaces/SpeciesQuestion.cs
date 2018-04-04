using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Humanitas.Interfaces
{
    public class SpeciesQuestion
    {

        public long QuestionId { get; set; }

        public long? KingdomId { get; set; }

        public long? SpecieId { get; set; }

        public long? FeatureId { get; set; }

        public string Question { get; set; }

        public string Resolution { get; set; }

    }
}
