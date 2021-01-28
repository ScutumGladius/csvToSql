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
            var importTasks = CsvToSql.Configuration.ImportTasks.ReadFromJsonFile(log, pathToJsonSettings);

            // Assert
            Assert.AreEqual(importTasks.Count, 2);
        }
        [Test]
        public void ImportTasksTest_ReadColumnMapping()
        {
            // Arrange
            var jsonSettings = @"
            {
            ""importFiles"": 
                [
                    {
                        ""file"": ""2020.xlsx"",
                        ""batchSize"": 500,
                        ""columnMapping"": [
                            {
                                ""SAL code"": ""SAL"",
                                ""site (city, street, zip code)"": ""City"",
                                ""ownerGID"": ""OwnerGID""
                            }
                        ]
                    
                    }
                ]
            }";

            // Act
            var importTasks = CsvToSql.Configuration.ImportTasks.ReadTasks(log, jsonSettings);

            // Assert
            Assert.AreEqual(importTasks.Count, 1);
            Assert.AreEqual(importTasks[0].columnMapping.Count, 3);
            Assert.AreEqual(importTasks[0].batchSize, 500);
        }

    }
}