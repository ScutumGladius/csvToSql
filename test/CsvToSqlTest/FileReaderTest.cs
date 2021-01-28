using System;
using CsvToSql.Engine;
using CsvToSql.Core;
using CsvToSql.logging;
using NUnit.Framework;
using NUnit.Framework.Internal;
using System.Collections.Generic;

namespace CsvToSqlTest
{
    public class FileReaderTest
    {

        private readonly Logging log;

        public FileReaderTest()
        {
            log = new Logging();
        }

        [Test]
        public void FileReaderTest_ReadColumnMapping()
        {

            // Arrange
            var jsonSettings = @"
            {
            ""importFiles"": 
                [
                    {
                        ""file"": ""..\\..\\..\\..\\TestCsv\\simpleStandard.csv""
                    }
                ]
            }";

            var importTasks = (List<ImportFileOptions>)CsvToSql.Configuration.ImportTasks.ReadTasks(log, jsonSettings);

            // Act
            var executor = new TaskExecutor(log);
            int rowCnt = 0;
            importTasks.ForEach(impTask => rowCnt += executor.Run(impTask));

            // Assert
            Assert.AreEqual(importTasks.Count, 1);
            Assert.IsTrue(rowCnt > 1);
        }

    }
}