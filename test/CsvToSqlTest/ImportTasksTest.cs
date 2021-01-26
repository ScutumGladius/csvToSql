using System;
using CsvToSql.Core;
using CsvToSql.logging;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace CsvToSqlTest
{
    public class ImportTasksTest
    {

        private readonly Logging log;

        public ImportTasksTest()
        {
            log = new Logging();
        }

        [Test]
        public void ImportTasksTest_ReadFile()
        {
            // Arrange
           var pathToJsonSettings = "..\\..\\..\\..\\JsonCfg\\settings.json";

            // Act
            var importTasks = CsvToSql.Configuration.ImportTasks.Read(log, pathToJsonSettings);

            // Assert
            Assert.AreEqual(importTasks.Count, 2);
        }

    }
}