using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Humanitas.Interfaces
{
    public class Tag
    {

        public Tag()
        {
            this.Tags = new List<Option>();
        }

        public string TagId { get; set; }

        public int Type { get; set; }

        public string Name { get; set; }

        public string Text { get; set; }

        public string FileName { get; set; }

        public string ReferencesName { get; set; }

        public string ParentId { get; set; }

        public string Keywords { get; set; }

        public DateTime? BeginDate { get; set; }

        public short? BeginYear { get; set; }

        public short? BeginCentury { get; set; }

        public string BeginLocal { get; set; }

        public DateTime? EndDate { get; set; }

        public short? EndYear { get; set; }

        public short? EndCentury { get; set; }

        public string EndLocal { get; set; }

        public string Language { get; set; }

        public string FunctionName { get; set; }

        public string Latitude { get; set; }

        public string Longitude { get; set; }

        public string Title { get; set; }

        public string Author { get; set; }

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

        public long? DomainId { get; set; }

        public DateTime CreatedAt { get; set; }

        public string CreatedBy { get; set; }

        public DateTime UpdatedAt { get; set; }

        public string UpdatedBy { get; set; }

        public List<Option> Tags { get; set; }

        public LibraryBook LibraryBook { get; set; }

        public List<TagEvent> Events { get; set; }

        public List<TagLink> Links { get; set; }

        public long TotalOfFragments { get; set; }

    }
}
