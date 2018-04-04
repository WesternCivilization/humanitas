using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Humanitas.Interfaces;

namespace Humanitas.Services.Tests
{
    internal static class TestHelper
    {
        internal static AppConfiguration GetAppConfiguration()
        {
            return new AppConfiguration
            {
                CachePath = @"C:\Data\Repositories\humanitas-cache",
                RepositoryPath = @"C:\Data\Repositories\humanitas-repos\images"
            };
        }
    }
}
