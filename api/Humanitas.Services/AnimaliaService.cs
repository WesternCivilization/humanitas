using Humanitas.Interfaces;
using Humanitas.Services.Interfaces;
using Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Humanitas.Services
{
    public class AnimaliaService : IAnimaliaService
    {

        private Logger log = new Logger(typeof(AnimaliaService));
        private AppConfiguration _config = null;

        public AnimaliaService(AppConfiguration config)
        {
            this._config = config;
        }

        bool IAnimaliaService.DeleteFeature(long featureId, string token)
        {
            using (var scope = log.Scope("DeleteFeature()"))
            {
                try
                {

                }
                catch (Exception ex)
                {
                    log.Error(scope, ex);
                    throw;
                }
                return false;
            }
        }

        bool IAnimaliaService.DeletePhoto(long photoId, string token)
        {
            using (var scope = log.Scope("DeletePhoto()"))
            {
                try
                {

                }
                catch (Exception ex)
                {
                    log.Error(scope, ex);
                    throw;
                }
                return false;
            }
        }

        bool IAnimaliaService.DeleteQuestion(long quetionId, string token)
        {
            using (var scope = log.Scope("DeleteQuestion()"))
            {
                try
                {

                }
                catch (Exception ex)
                {
                    log.Error(scope, ex);
                    throw;
                }
                return false;
            }
        }

        bool IAnimaliaService.DeleteSpecie(long specieId, string token)
        {
            using (var scope = log.Scope("DeleteSpecie()"))
            {
                try
                {

                }
                catch (Exception ex)
                {
                    log.Error(scope, ex);
                    throw;
                }
                return false;
            }
        }

        Specie IAnimaliaService.GetSpecie(long specieId)
        {
            using (var scope = log.Scope("GetSpecie()"))
            {
                try
                {

                }
                catch (Exception ex)
                {
                    log.Error(scope, ex);
                    throw;
                }
                return null;
            }
        }

        IEnumerable<Specie> IAnimaliaService.GetSpeciesByDomain(long domainId, string token)
        {
            using (var scope = log.Scope("GetSpeciesByDomain()"))
            {
                try
                {

                }
                catch (Exception ex)
                {
                    log.Error(scope, ex);
                    throw;
                }
                return null;
            }
        }

        IEnumerable<Specie> IAnimaliaService.GetSpeciesBySpecie(long domainId, string token)
        {
            using (var scope = log.Scope("GetSpeciesBySpecie()"))
            {
                try
                {

                }
                catch (Exception ex)
                {
                    log.Error(scope, ex);
                    throw;
                }
                return null;
            }
        }

        long IAnimaliaService.SaveFeature(SpeciesFeature feature, long? domainId, long? specieId, string token)
        {
            using (var scope = log.Scope("SaveFeature()"))
            {
                try
                {

                }
                catch (Exception ex)
                {
                    log.Error(scope, ex);
                    throw;
                }
                return -1;
            }
        }

        long IAnimaliaService.SavePhoto(HttpPostedFile file, long specieId, string token)
        {
            using (var scope = log.Scope("SavePhoto()"))
            {
                try
                {

                }
                catch (Exception ex)
                {
                    log.Error(scope, ex);
                    throw;
                }
                return -1;
            }
        }

        long IAnimaliaService.SaveQuestion(SpeciesQuestion question, long? domainId, long? specieId, long? featureId, string token)
        {
            using (var scope = log.Scope("SaveQuestion()"))
            {
                try
                {

                }
                catch (Exception ex)
                {
                    log.Error(scope, ex);
                    throw;
                }
                return -1;
            }
        }

        long IAnimaliaService.SaveSpecie(Specie specie, long? domainId, long? parentId, string token)
        {
            using (var scope = log.Scope("SaveSpecie()"))
            {
                try
                {

                }
                catch (Exception ex)
                {
                    log.Error(scope, ex);
                    throw;
                }
                return -1;
            }
        }

    }
}
