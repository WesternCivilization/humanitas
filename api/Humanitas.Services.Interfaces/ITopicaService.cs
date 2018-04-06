using System.Collections.Generic;
using System.Web;
using Humanitas.Interfaces;
using Newtonsoft.Json.Linq;

namespace Humanitas.Services.Interfaces
{
    public interface ITopicaService
    {

        IList<Option> AutoComplete(string type, string expression, string userId);

        Option Select(string type, string id, string userId);

        IList<Option> Folders(int domainId, string parent, string userId);

        Tag Detail(string tagId, string userId);

        PagedList<TimelineActivity> References(string tagId, string userId);

        PagedList<Tag> Books(string libraryId, string[] tags, int start, int count, string userId);

        IList<Tag> Libraries(string userId);

        bool SaveImage(string userId, HttpPostedFile file);

        string SaveTag(JObject tag, string userId, out string sqlExecuted);

        long SaveLibraryBook(JObject book, string userId);

        LibraryBook LibraryBook(string bookId, string libraryId, string userId);
        void SaveLinks(string tagId, JToken jToken, string token);
        void SaveEvents(string tagId, JToken jToken, string token);
    }
}
