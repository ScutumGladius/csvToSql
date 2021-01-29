using System;
using CsvToSql.Engine;
using CsvToSql.Core;
using CsvToSql.logging;
using CsvToSql.FileReader;
using NUnit.Framework;
using System.Collections.Generic;
using CsvToSql.SqlWriter;
using System.Linq;

namespace CsvToSqlTest
{
    public class SqlWriterTest
    {

        private readonly Logging log;

        public SqlWriterTest()
        {
            log = new Logging();
        }

        [Test]
        public void SqlWriterTest_InitHeaders()
        {

            // Arrange
            var jsonSettings = @"
            {
            ""importFiles"": 
                [
                    {
                        ""file"": ""..\\..\\..\\..\\TestCsv\\simpleComma.csv"",
                        ""batchSize"": 4
                    }
                ]
            }";

            var importTasks = (List<ImportFileOptions>)CsvToSql.Configuration.ImportTasks.ReadTasks(log, jsonSettings);

            var sqlWriter = new SqlServerWriter(log);

            var headers = new List<string>(){ "head1", "head2"};
            var linesToWrite = new List<List<string>>() {
                new List<string>(){ "a0", "a1", "a2"},
                new List<string>(){ "b0", "b1", "b2"}
            }; 
            // Act
            sqlWriter.Init(importTasks.First(), headers);

            // Assert
            //Assert.AreEqual(sqlWriter..Count, 1);
            Assert.IsTrue(sqlWriter != null);
        }
    
    }
}