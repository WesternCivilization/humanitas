using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Humanitas.Services.Interfaces
{
    public class UserAlreadyExistsException : ApplicationException
    {
        public UserAlreadyExistsException(string user) 
            : base($"The user '{user}' already exists in the database. Not able to create it again.")
        {
        }
    }
}
