using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Humanitas.Interfaces
{
    public class Fragment
    {

        public Fragment()
        {
            this.Tags = new List<Option>();
        }


        public string FragmentId { get; set; }

        public long DomainId { get; set; }

        public int Type { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public string Author { get; set; }

        public string AudioFileName { get; set; }

        public string ImageFileName { get; set; }

        public int? Page { get; set; }

        public string Reference { get; set; }

        public string Answer { get; set; }

        public string ParentId { get; set; }

        public string AreaId { get; set; }

        public string BookId { get; set; }

        public string InstitutionId1 { get; set; }

        public string InstitutionId2 { get; set; }

        public string LawId { get; set; }

        public string PeriodId { get; set; }

        public string PersonId1 { get; set; }

        public string PersonId2 { get; set; }

        public string PersonId3 { get; set; }

        public string PersonId4 { get; set; }

        public string PersonId5 { get; set; }

        public string SkillId1 { get; set; }

        public string SkillId2 { get; set; }

        public string SkillId3 { get; set; }

        public string SkillId4 { get; set; }

        public string SkillId5 { get; set; }

        public string StateId1 { get; set; }

        public string StateId2 { get; set; }

        public string TopicId1 { get; set; }

        public string TopicId2 { get; set; }

        public string TopicId3 { get; set; }

        public string TopicId4 { get; set; }

        public string TopicId5 { get; set; }

        public DateTime CreatedAt { get; set; }

        public string CreatedBy { get; set; }

        public DateTime UpdatedAt { get; set; }

        public string UpdatedBy { get; set; }

        public List<Option> Tags { get; set; }

    }
}
