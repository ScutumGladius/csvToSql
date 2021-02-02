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
            Assert.AreEqual(sqlWriter.GetHeaderFields().Count, 2); // { "head1", "head2"}
            Assert.IsTrue(sqlWriter != null);
        }

        [Test]
        public void SqlWriterTest_CreateTableStatement()
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

            var headers = new List<string>() { "head1", "head2" };
            // Act
            sqlWriter.Init(importTasks.First(), headers);
            var tableCreateSql = sqlWriter.GetCreateTableStatement();

            // Assert
            Assert.IsTrue(sqlWriter != null);
            Assert.IsTrue(tableCreateSql.Contains("CREATE TABLE"));
            Assert.IsTrue(tableCreateSql.Contains("head1"));
            Assert.IsTrue(tableCreateSql.Contains("head2"));
        }


        [Test]
        public void SqlWriterTest_TruncateTableStatement_YES()
        {

            // Arrange
            var jsonSettings = @"
            {
            ""importFiles"": 
                [
                    {
                        ""file"": ""..\\..\\..\\..\\TestCsv\\simpleComma.csv"",
                        ""batchSize"": 4,
                        ""truncate"": true
                    }
                ]
            }";

            var importTasks = (List<ImportFileOptions>)CsvToSql.Configuration.ImportTasks.ReadTasks(log, jsonSettings);

            var sqlWriter = new SqlServerWriter(log);

            var headers = new List<string>() { "head1", "head2" };
            // Act
            sqlWriter.Init(importTasks.First(), headers);
            var tableTruncSql = sqlWriter.GetTruncateTableStatement();

            // Assert
            Assert.IsTrue(sqlWriter != null);
            Assert.IsTrue(tableTruncSql.Contains("TRUNCATE TABLE"));
        }

        [Test]
        public void SqlWriterTest_TruncateTableStatement_NO()
        {

            // Arrange
            var jsonSettings = @"
            {
            ""importFiles"": 
                [
                    {
                        ""file"": ""..\\..\\..\\..\\TestCsv\\simpleComma.csv"",
                        ""batchSize"": 4,
                        ""truncate"": false
                    }
                ]
            }";

            var importTasks = (List<ImportFileOptions>)CsvToSql.Configuration.ImportTasks.ReadTasks(log, jsonSettings);

            var sqlWriter = new SqlServerWriter(log);

            var headers = new List<string>() { "head1", "head2" };
            // Act
            sqlWriter.Init(importTasks.First(), headers);
            var tableTruncSql = sqlWriter.GetTruncateTableStatement();

            // Assert
            Assert.IsTrue(sqlWriter != null);
            Assert.IsTrue(tableTruncSql.Length < 3);
        }

        [Test]
        public void SqlWriterTest_GetInsertStatments()
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

            var headers = new List<string>() { "head1", "head2", "head3" };
            var linesToWrite = new List<List<string>>() {
                new List<string>(){ "a0", "a1", "a2"},
                new List<string>(){ "b0", "b1", "b2"}
            };

            sqlWriter.Init(importTasks.First(), headers);

            // Act

            string insertSql = sqlWriter.GetInsertStatements(linesToWrite);
            // Assert
            Assert.AreEqual(sqlWriter.GetHeaderFields().Count, 2); // { "head1", "head2"}
            Assert.IsTrue(sqlWriter != null);
        }
    }
}