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
            var argv = new ArgcOptions()
            {
                JsonCfgFile = "..\\..\\..\\..\\JsonCfg\\settings.json"
            };

            // Act
            var importTasks = CsvToSql.Configuration.ImportTasks.ReadFromJsonFile(log, argv);

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
                        ""uniqueOnly"": true,
                        ""columnMapping"": [
                            {
                                ""SAL code"": ""SAL"",
                                ""site (city, street, zip code)"": ""City"",
                                ""ownerGID"": ""OwnerGID""
                            }
                        ],
                        ""additionalSQL"": ""Update table""
                    },
                    {
                        ""file"": ""2020.xlsx"",
                        ""batchSize"": 500,
                        ""uniqueOnly"": false
                    },
                    {
                        ""file"": ""2020.xlsx"",
                        ""batchSize"": 500
                    }
                ]
            }";

            // Act
            var importTasks = CsvToSql.Configuration.ImportTasks.ReadTasks(log, jsonSettings);

            // Assert
            Assert.AreEqual(importTasks.Count, 3);
            Assert.AreEqual(importTasks[0].columnMapping.Count, 3);
            Assert.AreEqual(importTasks[0].batchSize, 500);

            Assert.AreEqual(importTasks[0].uniqueOnly, true);
            Assert.AreEqual(importTasks[1].uniqueOnly, false);
            Assert.AreEqual(importTasks[2].uniqueOnly, false);

            Assert.AreEqual(importTasks[0].additionalSQL, "Update table");
            Assert.AreEqual(importTasks[1].additionalSQL, "");
        }

    }
}