using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Humanitas.Interfaces
{
    public class FragmentLike
    {

        public string FragmentLikeId { get; set; }

        public string FragmentId { get; set; }

        public string UserId { get; set; }

        public short? Score { get; set; }

        public DateTime? ListenAt { get; set; }

        public long? ListenCount { get; set; }

    }
}
