using System;
using CsvToSql.Engine;
using CsvToSql.Core;
using CsvToSql.logging;
using CsvToSql.FileReader;
using NUnit.Framework;
using System.Collections.Generic;
using CsvToSql.SqlWriter;

namespace CsvToSqlTest
{
    public class FileReaderTest
    {

        private readonly Logging log;

        public FileReaderTest()
        {
            log = new Logging();
        }

        [TestCase("simpleStandard.csv")]
        [TestCase("simpleComma.csv")]
        [TestCase("simpleMultiline.csv")]
        public void FileReaderTest_ReadSimpleCSVs(string testFileName)
        {

            // Arrange
            var jsonSettings = @"
            {
            ""importFiles"": 
                [
                    {
                        ""file"": ""..\\..\\..\\..\\TestCsv\\{0}"",
                        ""batchSize"": 4
                    }
                ]
            }";

            var jsettings = jsonSettings.Replace("{0}", testFileName);
            var importTasks = (List<ImportFileOptions>)CsvToSql.Configuration.ImportTasks.ReadTasks(log, jsettings);

            var csvReader = new ReadCsv(log);
            var sqlWriter = new SqlServerWriterMoq(log);
            var executor = new TaskExecutor(log, csvReader, sqlWriter);

            // Act
            int rowCnt = 0;
            importTasks.ForEach(impTask => rowCnt += executor.Run(impTask));

            // Assert
            Assert.AreEqual(importTasks.Count, 1);
            Assert.IsTrue(rowCnt > 4);
        }

        public class SqlServerWriterMoq : ISqlWriter
        {

            private readonly Logging Log;

            public SqlServerWriterMoq(Logging log)
            {
                Log = log;
            }

            public int Write(ImportFileOptions importTask, int rowCounter, List<string> headers, List<List<string>> linesToWrite)
            {
                Log.Debug($"SqlServerWriter: Write for '{importTask.file}'");
                return 0;
            }
        }
    }
}