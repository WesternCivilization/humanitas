using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Humanitas.Services.Interfaces;

namespace Humanitas.Services.Tests
{
    [TestClass]
    public class EncryptionServiceTests
    {
        [TestMethod]
        public void Encrypt_HappyPath()
        {
            IEncryptionService service = new EncryptionService();
            var key = "+Adse3hjesS/ef";
            var planText = "d24m11";
            var encrypted = service.Encrypt(planText, key);
            Assert.IsTrue(encrypted == "7PIlzyHgVkneZgTlTSaQEg==", "Invalid encryption!");
            Assert.IsTrue(planText == service.Decrypt(encrypted, key), "Invalid decription!");
        }
    }
}
