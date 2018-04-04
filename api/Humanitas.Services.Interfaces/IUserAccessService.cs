using Humanitas.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Humanitas.Services.Interfaces
{
    public interface IUserAccessService
    {

        string Login(User user);

        string Logout(User user);

        string TokenToUserId(string token);

        PagedList<User> List(int start, int count, string token);

        User Detail(string userId, string token);

        dynamic Metrics(string userId, string token);

    }
}
