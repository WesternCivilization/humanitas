using Humanitas.Interfaces;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Humanitas.Services.Interfaces
{
    public interface IZeteticaService
    {

        List<Fragment> AllQuotes(string userId);

        List<Fragment> AllNotes(string userId);

        List<Fragment> AllVideos(string userId);

        List<Fragment> AllAudios(string userId);

        List<Fragment> All(string userId);

        PagedList<TimelineActivity> Activities(string tag, string sort, int start, int count, string userId);

        IList<Option> AutoComplete(string type, string expression, string userId);

        PagedList<TimelineActivity> Fragments(long domainId, string tagId, int start, int count, string userId);

        Fragment Detail(string fragmentId, string userId);

        bool SaveAudio(string userId, HttpPostedFile file);

        bool SaveImage(string userId, HttpPostedFile file);

        string SaveFragment(JObject obj, string userId, out string sqlExecuted);

        bool Score(string fragmentId, short score, string userId);

        TimelineActivity Fragment(string fragmentId, string userId);

        bool Listen(string fragmentId, string userId);

        Activity ActivityByFragmentId(string id);

        List<string> BatchData(string type, string tagName, string userId);

    }
}
