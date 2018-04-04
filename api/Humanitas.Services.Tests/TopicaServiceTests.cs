using System;
using Humanitas.Services.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Humanitas.Services.Tests
{
    [TestClass]
    public class TopicaServiceTests
    {

        [TestMethod]
        public void Search_All_HappyPath()
        {
            ITopicaService service = new TopicaService(TestHelper.GetAppConfiguration());
            var results = service.AutoComplete("all", "Rafael Melo", null);
            Assert.IsTrue(results.Count == 1, "Tag not found.");
        }

        [TestMethod]
        public void Search_Tag_HappyPath()
        {
            ITopicaService service = new TopicaService(TestHelper.GetAppConfiguration());
            var results = service.AutoComplete("tag", "Rafael Melo", null);
            Assert.IsTrue(results.Count == 1, "Tag not found.");
        }

        [TestMethod]
        public void Search_Area_HappyPath()
        {
            ITopicaService service = new TopicaService(TestHelper.GetAppConfiguration());
            var results = service.AutoComplete("area", "brasil", null);
            Assert.IsTrue(results.Count == 1, "Tag not found.");
        }

        [TestMethod]
        public void Search_Period_HappyPath()
        {
            ITopicaService service = new TopicaService(TestHelper.GetAppConfiguration());
            var results = service.AutoComplete("period", "Brasil da República Velha", null);
            Assert.IsTrue(results.Count == 1, "Tag not found.", null);
        }

        [TestMethod]
        public void Search_Author_HappyPath()
        {
            ITopicaService service = new TopicaService(TestHelper.GetAppConfiguration());
            var results = service.AutoComplete("author", "aristóteles", null);
            Assert.IsTrue(results.Count == 1, "Tag not found.");
        }

        [TestMethod]
        public void Search_Institution_HappyPath()
        {
            ITopicaService service = new TopicaService(TestHelper.GetAppConfiguration());
            var results = service.AutoComplete("institution", "igreja católica", null);
            Assert.IsTrue(results.Count == 1, "Tag not found.");
        }

        [TestMethod]
        public void Search_Book_HappyPath()
        {
            ITopicaService service = new TopicaService(TestHelper.GetAppConfiguration());
            var results = service.AutoComplete("book", "política [aristóteles]", null);
            Assert.IsTrue(results.Count == 1, "Tag not found.");
        }

        [TestMethod]
        public void Search_Topic_HappyPath()
        {
            ITopicaService service = new TopicaService(TestHelper.GetAppConfiguration());
            var results = service.AutoComplete("topic", "Tensão entre cultura e razão", null);
            Assert.IsTrue(results.Count == 1, "Tag not found.");
        }

        [TestMethod]
        public void Search_Law_HappyPath()
        {
            ITopicaService service = new TopicaService(TestHelper.GetAppConfiguration());
            var results = service.AutoComplete("law", "Artigo 1 da Constituição de 1988", null);
            Assert.IsTrue(results.Count == 1, "Tag not found.");
        }

        [TestMethod]
        public void Search_State_HappyPath()
        {
            ITopicaService service = new TopicaService(TestHelper.GetAppConfiguration());
            var results = service.AutoComplete("state", "Governo Dilma Rousseff", null);
            Assert.IsTrue(results.Count == 1, "Tag not found.");
        }

        [TestMethod]
        public void Search_Skill_HappyPath()
        {
            ITopicaService service = new TopicaService(TestHelper.GetAppConfiguration());
            var results = service.AutoComplete("skill", "Mitose e meiose", null);
            Assert.IsTrue(results.Count == 1, "Tag not found.");
        }

        [TestMethod]
        public void Search_Library_HappyPath()
        {
            ITopicaService service = new TopicaService(TestHelper.GetAppConfiguration());
            var results = service.AutoComplete("library", "Biblioteca do Rafael", null);
            Assert.IsTrue(results.Count == 1, "Tag not found.");
        }

        [TestMethod]
        public void Nodes_Domain9_HappyPath()
        {
            ITopicaService service = new TopicaService(TestHelper.GetAppConfiguration());
            var results = service.Folders(9, null, null);
            Assert.IsTrue(results.Count >= 20, "Tags not found.");
        }

        [TestMethod]
        public void Nodes_Domain9_WithParent_HappyPath()
        {
            ITopicaService service = new TopicaService(TestHelper.GetAppConfiguration());
            var results = service.Folders(9, "0B8E195E-4A21-4EC7-960E-E90C681D06C6", null);
            Assert.IsTrue(results.Count >= 20, "Tags not found.");
        }

        [TestMethod]
        public void Nodes_Domain10_HappyPath()
        {
            ITopicaService service = new TopicaService(TestHelper.GetAppConfiguration());
            var results = service.Folders(10, null, null);
            Assert.IsTrue(results.Count >= 5, "Tags not found.");
        }

        [TestMethod]
        public void Nodes_Domain10_WithParent_HappyPath()
        {
            ITopicaService service = new TopicaService(TestHelper.GetAppConfiguration());
            var results = service.Folders(10, "2D2B6D91-7927-455F-9C84-E8D6C2F7BF31", null);
            Assert.IsTrue(results.Count >= 17, "Tags not found.");
        }

        [TestMethod]
        public void Nodes_Domain11_HappyPath()
        {
            ITopicaService service = new TopicaService(TestHelper.GetAppConfiguration());
            var results = service.Folders(11, null, null);
            Assert.IsTrue(results.Count >= 4, "Tags not found.");
        }

        [TestMethod]
        public void Nodes_Domain11_WithParent_HappyPath()
        {
            ITopicaService service = new TopicaService(TestHelper.GetAppConfiguration());
            var results = service.Folders(11, "358C9E53-831C-48BB-B5BE-06660B8043B1", null);
            Assert.IsTrue(results.Count >= 17, "Tags not found.");
        }

        [TestMethod]
        public void Nodes_Domain12_HappyPath()
        {
            ITopicaService service = new TopicaService(TestHelper.GetAppConfiguration());
            var results = service.Folders(12, null, null);
            Assert.IsTrue(results.Count >= 4, "Tags not found.");
        }

        [TestMethod]
        public void Nodes_Domain12_WithParent_HappyPath()
        {
            ITopicaService service = new TopicaService(TestHelper.GetAppConfiguration());
            var results = service.Folders(12, "F53F9C67-AAE4-428B-979B-A21DFA0333AE", null);
            Assert.IsTrue(results.Count >= 17, "Tags not found.");
        }

        [TestMethod]
        public void Nodes_Domain13_HappyPath()
        {
            ITopicaService service = new TopicaService(TestHelper.GetAppConfiguration());
            var results = service.Folders(13, null, null);
            Assert.IsTrue(results.Count >= 4, "Tags not found.");
        }

        [TestMethod]
        public void Nodes_Domain13_WithParent_HappyPath()
        {
            ITopicaService service = new TopicaService(TestHelper.GetAppConfiguration());
            var results = service.Folders(13, "D90C2AE6-F320-43D3-8AAE-5DF8FDE6DFBF", null);
            Assert.IsTrue(results.Count >= 17, "Tags not found.");
        }

        [TestMethod]
        public void Nodes_Domain14_HappyPath()
        {
            ITopicaService service = new TopicaService(TestHelper.GetAppConfiguration());
            var results = service.Folders(14, null, null);
            Assert.IsTrue(results.Count >= 4, "Tags not found.");
        }

        [TestMethod]
        public void Nodes_Domain14_WithParent_HappyPath()
        {
            ITopicaService service = new TopicaService(TestHelper.GetAppConfiguration());
            var results = service.Folders(14, "E4A86DF7-1663-4684-A987-CBB3117E850E", null);
            Assert.IsTrue(results.Count >= 17, "Tags not found.");
        }

        [TestMethod]
        public void Nodes_Domain15_HappyPath()
        {
            ITopicaService service = new TopicaService(TestHelper.GetAppConfiguration());
            var results = service.Folders(15, null, null);
            Assert.IsTrue(results.Count >= 4, "Tags not found.");
        }

        [TestMethod]
        public void Nodes_Domain15_WithParent_HappyPath()
        {
            ITopicaService service = new TopicaService(TestHelper.GetAppConfiguration());
            var results = service.Folders(15, "89E5E726-B4A8-42EF-88A1-C37304FD69EC", null);
            Assert.IsTrue(results.Count >= 17, "Tags not found.");
        }

        [TestMethod]
        public void Nodes_Domain16_HappyPath()
        {
            ITopicaService service = new TopicaService(TestHelper.GetAppConfiguration());
            var results = service.Folders(16, null, null);
            Assert.IsTrue(results.Count >= 4, "Tags not found.");
        }

        [TestMethod]
        public void Nodes_Domain16_WithParent_HappyPath()
        {
            ITopicaService service = new TopicaService(TestHelper.GetAppConfiguration());
            var results = service.Folders(16, "BDE6F621-BBBD-4FA2-8012-01F8394E03E1", null);
            Assert.IsTrue(results.Count >= 17, "Tags not found.");
        }

        [TestMethod]
        public void Nodes_Domain17_HappyPath()
        {
            ITopicaService service = new TopicaService(TestHelper.GetAppConfiguration());
            var results = service.Folders(17, null, null);
            Assert.IsTrue(results.Count >= 4, "Tags not found.");
        }

        [TestMethod]
        public void Nodes_Domain17_WithParent_HappyPath()
        {
            ITopicaService service = new TopicaService(TestHelper.GetAppConfiguration());
            var results = service.Folders(17, "D0130669-9112-46DA-B230-A269F9E7138E", null);
            Assert.IsTrue(results.Count >= 17, "Tags not found.");
        }

        [TestMethod]
        public void Nodes_Domain18_HappyPath()
        {
            ITopicaService service = new TopicaService(TestHelper.GetAppConfiguration());
            var results = service.Folders(18, null, null);
            Assert.IsTrue(results.Count >= 4, "Tags not found.");
        }

        [TestMethod]
        public void Nodes_Domain18_WithParent_HappyPath()
        {
            ITopicaService service = new TopicaService(TestHelper.GetAppConfiguration());
            var results = service.Folders(18, "C450304D-0417-481D-95B8-62DB723F8F03", null);
            Assert.IsTrue(results.Count >= 17, "Tags not found.");
        }

        [TestMethod]
        public void Nodes_Domain19_HappyPath()
        {
            ITopicaService service = new TopicaService(TestHelper.GetAppConfiguration());
            var results = service.Folders(19, null, null);
            Assert.IsTrue(results.Count >= 4, "Tags not found.");
        }

        [TestMethod]
        public void Nodes_Domain19_WithParent_HappyPath()
        {
            ITopicaService service = new TopicaService(TestHelper.GetAppConfiguration());
            var results = service.Folders(19, "4B068F0F-634D-450F-BEC7-CB63B3C4838A", null);
            Assert.IsTrue(results.Count >= 17, "Tags not found.");
        }

        [TestMethod]
        public void Nodes_Domain20_HappyPath()
        {
            ITopicaService service = new TopicaService(TestHelper.GetAppConfiguration());
            var results = service.Folders(20, null, null);
            Assert.IsTrue(results.Count >= 4, "Tags not found.");
        }

        [TestMethod]
        public void Nodes_Domain20_WithParent_HappyPath()
        {
            ITopicaService service = new TopicaService(TestHelper.GetAppConfiguration());
            var results = service.Folders(20, "B3975D78-60D1-4FD0-AB02-4886EDD41645", null);
            Assert.IsTrue(results.Count >= 17, "Tags not found.");
        }

        [TestMethod]
        public void Nodes_Domain41_HappyPath()
        {
            ITopicaService service = new TopicaService(TestHelper.GetAppConfiguration());
            var results = service.Folders(41, null, null);
            Assert.IsTrue(results.Count >= 4, "Tags not found.");
        }

        [TestMethod]
        public void Books_HappyPath()
        {
            ITopicaService service = new TopicaService(TestHelper.GetAppConfiguration());
            var tag = service.Books("CAC0C087-974C-401C-85CA-ABCDAA1D09D9", null, 0, 10, null);
            Assert.IsTrue(tag != null, "Tags not found.");
        }

        [TestMethod]
        public void Books_HappyPath2()
        {
            ITopicaService service = new TopicaService(TestHelper.GetAppConfiguration());
            var tag = service.Books(null, new string[] { "5DFD3BB7-3281-4AAF-B426-5C923A2DF9D0" }, 0, 10, null);
            Assert.IsTrue(tag != null, "Tags not found.");
        }

        [TestMethod]
        public void Detail_HappyPath()
        {
            ITopicaService service = new TopicaService(TestHelper.GetAppConfiguration());
            var tag = service.Detail("B3975D78-60D1-4FD0-AB02-4886EDD41645", null);
            Assert.IsTrue(tag != null, "Tags not found.");
            Assert.IsTrue(tag.Name == "Filosofia do Entendimento [Pasta Principal]", "Tags not found.");
        }

    }
}
