using Humanitas.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Humanitas.Services.Interfaces
{
    public interface IDogmaticaService
    {

        IEnumerable<Chapter> ListChapters(long domainId, string token);

        IEnumerable<Versicle> ListVersicles(long chapterId, string token);

        long SaveChapter(Chapter chapter, long[] tags, string token);

        long SaveVersicle(Versicle versicle, string token);

        bool DeleteChapter(long chapterId, string token);

        bool DeleteVersible(long versibleId, string token);

        IEnumerable<Tag> GetReferences(long chapterId);

        List<Option> AutoComplete(string type, string expression);

    }
}
