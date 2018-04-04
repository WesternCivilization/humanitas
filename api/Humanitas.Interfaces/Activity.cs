using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Humanitas.Interfaces
{
    public class Activity
    {

        public string ActivityId { get; set; }

        public string UserId { get; set; }

        public string TagId { get; set; }

        public string FragmentId { get; set; }

        public DateTime CreatedAt { get; set; }

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

        public int? DomainId { get; set; }

        public long? TotalVotes { get; set; }

        public long? TotalScore { get; set; }

        public long? TotalListen { get; set; }

        public DateTime? LastTimeListen { get; set; }

    }
}
