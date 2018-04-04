using System.Linq;
using Humanitas.Services.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Humanitas.Services.Tests
{
    [TestClass]
    public class ZeteticaServiceTests
    {

        [TestMethod]
        public void Activities_Top100_HappyPath()
        {
            IZeteticaService service = new ZeteticaService(TestHelper.GetAppConfiguration(), new UserAccessService(TestHelper.GetAppConfiguration()));
            var results = service.Activities(null, "4", 0, 100, "7E26AB2D-C568-4BDD-A413-D2EAF79DF842");
            Assert.IsTrue(results.Count == 100, "Not all activities were loaded.");
        }

        [TestMethod]
        public void Activities_Top10WithSkip_HappyPath()
        {
            IZeteticaService service = new ZeteticaService(TestHelper.GetAppConfiguration(), new UserAccessService(TestHelper.GetAppConfiguration()));
            var results = service.Activities(null, "4", 0, 10, "7E26AB2D-C568-4BDD-A413-D2EAF79DF842");
            Assert.IsTrue(results.Count == 10, "Not all activities were loaded.");
            var results2 = service.Activities(null, "4", 9, 1, "7E26AB2D-C568-4BDD-A413-D2EAF79DF842");
            Assert.IsTrue(results.Last().Title == results2.First().Title, "Skip operation is not working");
        }

        [TestMethod]
        public void Search_All_HappyPath()
        {
            IZeteticaService service = new ZeteticaService(TestHelper.GetAppConfiguration(), new UserAccessService(TestHelper.GetAppConfiguration()));
            var results = service.AutoComplete("all", "Ninguém pode combater uma época", "7E26AB2D-C568-4BDD-A413-D2EAF79DF842");
            Assert.IsTrue(results.Count == 1, "Fragment not found.");
        }

        [TestMethod]
        public void Search_Fragment_HappyPath()
        {
            IZeteticaService service = new ZeteticaService(TestHelper.GetAppConfiguration(), new UserAccessService(TestHelper.GetAppConfiguration()));
            var results = service.AutoComplete("fragment", "Ninguém pode combater uma época", "7E26AB2D-C568-4BDD-A413-D2EAF79DF842");
            Assert.IsTrue(results.Count == 1, "Fragment not found.");
        }

        [TestMethod]
        public void Search_Quote_HappyPath()
        {
            IZeteticaService service = new ZeteticaService(TestHelper.GetAppConfiguration(), new UserAccessService(TestHelper.GetAppConfiguration()));
            var results = service.AutoComplete("quote", "Imperativo de utilidade e imperativo de nobreza", "7E26AB2D-C568-4BDD-A413-D2EAF79DF842");
            Assert.IsTrue(results.Count == 1, "Fragment not found.");
        }

        [TestMethod]
        public void Search_Note_HappyPath()
        {
            IZeteticaService service = new ZeteticaService(TestHelper.GetAppConfiguration(), new UserAccessService(TestHelper.GetAppConfiguration()));
            var results = service.AutoComplete("note", "O trabalho sujo de protestar contra o PT", "7E26AB2D-C568-4BDD-A413-D2EAF79DF842");
            Assert.IsTrue(results.Count == 1, "Fragment not found.");
        }

        [TestMethod]
        public void Search_Video_HappyPath()
        {
            IZeteticaService service = new ZeteticaService(TestHelper.GetAppConfiguration(), new UserAccessService(TestHelper.GetAppConfiguration()));
            var results = service.AutoComplete("video", "Da Analogia Entis e da Analogia Fidei", "7E26AB2D-C568-4BDD-A413-D2EAF79DF842");
            Assert.IsTrue(results.Count == 1, "Fragment not found.");
        }

        [TestMethod]
        public void Search_Article_HappyPath()
        {
            IZeteticaService service = new ZeteticaService(TestHelper.GetAppConfiguration(), new UserAccessService(TestHelper.GetAppConfiguration()));
            var results = service.AutoComplete("article", "Os fins ordenam os meios", "7E26AB2D-C568-4BDD-A413-D2EAF79DF842");
            Assert.IsTrue(results.Count == 1, "Fragment not found.");
        }

        [TestMethod]
        public void Search_Audio_HappyPath()
        {
            IZeteticaService service = new ZeteticaService(TestHelper.GetAppConfiguration(), new UserAccessService(TestHelper.GetAppConfiguration()));
            var results = service.AutoComplete("audio", "[COF-400] Que é uma obra de arte? Considerações sobre o caso Santander", "7E26AB2D-C568-4BDD-A413-D2EAF79DF842");
            Assert.IsTrue(results.Count == 1, "Fragment not found.");
        }

        [TestMethod]
        public void Listen_HappyPath()
        {
            IZeteticaService service = new ZeteticaService(TestHelper.GetAppConfiguration(), new UserAccessService(TestHelper.GetAppConfiguration()));
            var actOk = service.Activities(null, "3", 0, 5, "7E26AB2D-C568-4BDD-A413-D2EAF79DF842").FirstOrDefault();
            var act = service.ActivityByFragmentId(actOk.Id);
            var totalListen = act.TotalListen;
            var listenTimeElapsed = act.LastTimeListen;
            service.Listen(actOk.Id, "EAAE6keP0ZCAABAIEVkAOc0C6NhkZAUA4WDVkfoNkzZCSoBV2ZCjXZBMIpjwz0ytCveodG0Eh98m5l4OHPVDLZAWZAl6YidNsW2XkIGdZACz0ioLvX9IpcSidJbp4FRlZCU0B46KsZCtbUtspmFd5VGZBHyec3cjtKozD4mLKqvdYTGkK8mGYllZCSZAaQhzkwnP4EDkjxWTGr2HzWMAZDZD");
            act = service.ActivityByFragmentId(actOk.Id);
            Assert.IsTrue(act != null && act.TotalListen != totalListen, "Listen not saved.");
            Assert.IsTrue(act != null && act.LastTimeListen != listenTimeElapsed, "Listen not saved.");
        }

    }
}
