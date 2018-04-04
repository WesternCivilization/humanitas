using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Humanitas.Services.Interfaces;
using System.Linq;
using SqlCache;

namespace Humanitas.Services.Tests
{
    /// <summary>
    /// Summary description for SynchronizationServiceTests
    /// </summary>
    [TestClass]
    public class SqlLiteServiceTests
    {

        [TestMethod]
        public void Creates_HappyPath()
        {
            using (var conn = new SqlCacheConnection(TestHelper.GetAppConfiguration().ConnectionString))
            {
                System.IO.File.WriteAllLines(@"C:\temp\Humanitas.sql", conn.DbScripts());
            }
            ISqlLiteService service = new SqlLiteService(TestHelper.GetAppConfiguration());
            var creates = service.CreatesTables();
            var tables = service.Tables();
            foreach (var table in tables)
            {
                Assert.IsTrue(creates.Contains($"CREATE TABLE IF NOT EXISTS {table}"), $"The create script for table '{table}' was not found.");
            }
        }

        [TestMethod]
        public void Inserts_HappyPath()
        {
            ISqlLiteService service = new SqlLiteService(TestHelper.GetAppConfiguration());
            var inserts = service.Inserts(200);
            inserts.ToString();
            Assert.IsTrue(inserts.Length > 5000, "The inserts scripts are incorrect.");
        }

    }
}
