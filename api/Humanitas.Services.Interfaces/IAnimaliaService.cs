using Humanitas.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Humanitas.Services.Interfaces
{
    public interface IAnimaliaService
    {

        IEnumerable<Specie> GetSpeciesByDomain(long domainId, string token);

        IEnumerable<Specie> GetSpeciesBySpecie(long domainId, string token);

        long SaveSpecie(Specie specie, long? domainId, long? parentId, string token);

        long SaveFeature(SpeciesFeature feature, long? domainId, long? specieId, string token);

        long SavePhoto(HttpPostedFile file, long specieId, string token);

        long SaveQuestion(SpeciesQuestion question, long? domainId, long? specieId, long? featureId, string token);

        bool DeleteSpecie(long specieId, string token);

        bool DeleteFeature(long featureId, string token);

        bool DeletePhoto(long photoId, string token);

        bool DeleteQuestion(long quetionId, string token);

        Specie GetSpecie(long specieId);

    }
}
