using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Humanitas.Interfaces
{
    public class User
    {

        public User()
        {
            this.UserId = Guid.NewGuid().ToString().ToUpper();
        }

        public String UserId { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string PhotoUrl { get; set; }

        public string Provider { get; set; }

        public string ExternalId { get; set; }

        public string Token { get; set; }

        public string UserTypeId { get; set; }

        public DateTime? CreatedAt { get; set; }

    }
}
