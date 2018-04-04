using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Humanitas.Interfaces
{
    public class SpeciesFeature
    {

        public long FeatureId { get; set; }

        public string Description { get; set; }

        public long? KingdomId { get; set; }

        public long? SpecieId { get; set; }

        public long? QuestionId { get; set; }

    }
}
