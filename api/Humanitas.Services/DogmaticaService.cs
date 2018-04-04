using Humanitas.Interfaces;
using Humanitas.Services.Interfaces;
using Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Humanitas.Services
{
    public class DogmaticaService : IDogmaticaService
    {

        private Logger log = new Logger(typeof(DogmaticaService));
        private AppConfiguration _config = null;

        public DogmaticaService(AppConfiguration config)
        {
            this._config = config;
        }

        bool IDogmaticaService.DeleteChapter(long chapterId, string token)
        {
            using (var scope = log.Scope("DeleteChapter()"))
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

        bool IDogmaticaService.DeleteVersible(long versibleId, string token)
        {
            using (var scope = log.Scope("DeleteVersible()"))
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

        IEnumerable<Tag> IDogmaticaService.GetReferences(long chapterId)
        {
            using (var scope = log.Scope("GetReferences()"))
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

        IEnumerable<Chapter> IDogmaticaService.ListChapters(long domainId, string token)
        {
            using (var scope = log.Scope("ListChapters()"))
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

        IEnumerable<Versicle> IDogmaticaService.ListVersicles(long chapterId, string token)
        {
            using (var scope = log.Scope("ListVersicles()"))
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

        long IDogmaticaService.SaveChapter(Chapter chapter, long[] tags, string token)
        {
            using (var scope = log.Scope("SaveChapter()"))
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

        long IDogmaticaService.SaveVersicle(Versicle versicle, string token)
        {
            using (var scope = log.Scope("SaveVersicle()"))
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

        List<Option> IDogmaticaService.AutoComplete(string type, string expression)
        {
            using (var scope = log.Scope("AutoComplete()"))
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


    }
}
