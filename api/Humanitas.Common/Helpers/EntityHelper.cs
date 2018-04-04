using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Humanitas.Common.Helpers
{
    public class EntityHelper
    {

        public static string GetEntityType(string entity, int type)
        {
            if (entity == "fragment")
            {
                if (type == 0) return "quote";
                else if (type == 1) return "note";
                else if (type == 2) return "question";
                else if (type == 3) return "video";
                else if (type == 4) return "article";
                else if (type == 5) return "audio";
                else return "unknown-fragment";
            }
            else if (entity == "tag")
            {
                if (type == 0) return "area";
                else if (type == 1) return "period";
                else if (type == 2) return "author";
                else if (type == 3) return "institution";
                else if (type == 4) return "book";
                else if (type == 7) return "topic";
                else if (type == 8) return "law";
                else if (type == 9) return "state";
                else if (type == 10) return "skill";
                else if (type == 12) return "library";
                else return "unknown-tag";
            }
            else
                return "unknonwn";
        }

    }
}
