using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Humanitas.Interfaces
{
    public class TimelineActivity
    {

        public TimelineActivity()
        {
            this.Tags = new List<Option>();
        }

        public string Title { get; set; }

        public string Content { get; set; }

        public string Author { get; set; }

        public string CreatedTimeElapsed { get; set; }

        public string ListenTimeElapsed { get; set; }

        public long? TotalListen { get; set; }

        public long? TotalVotes { get; set; }

        public long? TotalScore { get; set; }

        public string FileName { get; set; }

        public List<Option> Tags { get; set; }

        public int? Page { get; set; }

        public string Type { get; set; }

        public string Id { get; set; }

        public string Reference { get; set; }

        public int? DomainId { get; set; }

        public string UserPhotoUrl { get; set; }

        public string UserName { get; set; }

        public string UserId { get; set; }

        public override string ToString()
        {
            return $"{Title} [{Author}] ({Type})";
        }

    }
}
