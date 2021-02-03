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

            var sqlWriter = new SqlServerWriter(log, "");

            var headers = new List<string>() { "head1", "head2" };
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
                        ""batchSize"": 4,
                        ""forceCreateTable"": true
                                             }
                ]
            }";

            var importTasks = (List<ImportFileOptions>)CsvToSql.Configuration.ImportTasks.ReadTasks(log, jsonSettings);

            var sqlWriter = new SqlServerWriter(log, "");

            var headers = new List<string>() { "head1", "head2" };
            // Act
            sqlWriter.Init(importTasks.First(), headers);
            var tableDropSql = sqlWriter.GetDropTableStatement();
            var tableCreateSql = sqlWriter.GetCreateTableStatement();

            // Assert
            Assert.IsTrue(sqlWriter != null);
            Assert.IsTrue(tableCreateSql.Contains("CREATE TABLE"));
            Assert.IsTrue(tableDropSql.Contains("DROP TABLE"));
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

            var sqlWriter = new SqlServerWriter(log, "");

            var headers = new List<string>() { "head1", "head2" };
            // Act
            sqlWriter.Init(importTasks.First(), headers);
            var tableTruncSql = sqlWriter.GetTruncateTableStatement();
            var tableDropSql = sqlWriter.GetDropTableStatement();

            // Assert
            Assert.IsTrue(sqlWriter != null);
            Assert.IsTrue(tableTruncSql.Contains("TRUNCATE TABLE"));
            Assert.IsFalse(tableDropSql.Contains("DROP TABLE"));
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

            var sqlWriter = new SqlServerWriter(log, "");

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

            var sqlWriter = new SqlServerWriter(log, "");

            var headers = new List<string>() { "head1", "head2", "head3" };
            var linesToWrite = new List<List<string>>() {
                new List<string>(){ "a0", "a1", "a2"},
                new List<string>(){ "b0", "b1", "b2"}
            };

            sqlWriter.Init(importTasks.First(), headers);

            // Act

            string insertSql = sqlWriter.GetInsertStatements(linesToWrite);

            // Assert
            Assert.IsTrue(sqlWriter != null);
            Assert.AreEqual(sqlWriter.GetHeaderFields().Count, 3); // { "head1", "head2", "head3"}
            Assert.IsTrue(insertSql.Contains("INSERT INTO"));
        }
        [Test]
        public void SqlWriterTest_InitHeadersWithcolumnMapping()
        {

            // Arrange
            var jsonSettings = @"
            {
            ""importFiles"": 
                [
                    {
                        ""file"": ""..\\..\\..\\..\\TestCsv\\simpleComma.csv"",
                        ""batchSize"": 4,
                        ""columnMapping"": [
                            {
                                ""head1"": ""NewHeadOne"",
                                ""head2"": ""NewHeadTwo""
                            }
                        ]
                    }
                ]
            }";

            var importTasks = (List<ImportFileOptions>)CsvToSql.Configuration.ImportTasks.ReadTasks(log, jsonSettings);

            var sqlWriter = new SqlServerWriter(log, "");

            var headers = new List<string>() { "head1", "head2", "head3" };

            // Act
            sqlWriter.Init(importTasks.First(), headers);
            var headerFields = sqlWriter.GetHeaderFields();

            // Assert
            Assert.AreEqual(headerFields.Count, 3); // { "NewHeadOne", "NewHeadTwo", "head3"}
            Assert.AreEqual(headerFields[0].Name, "NewHeadOne");
            Assert.AreEqual(headerFields[1].Name, "NewHeadTwo");
            Assert.AreEqual(headerFields[2].Name, "head3");
            Assert.IsTrue(sqlWriter != null);
        }

        [Test]
        public void SqlWriterTest_InitHeadersWithcolumnMappingImportDate()
        {

            // Arrange
            var jsonSettings = @"
            {
            ""importFiles"": 
                [
                    {
                        ""file"": ""..\\..\\..\\..\\TestCsv\\simpleComma.csv"",
                        ""batchSize"": 4,
                        ""saveMode"": true,
                        ""columnMapping"": [
                            {
                                ""head1"": ""NewHeadOne"",
                                ""head2"": ""NewHeadTwo"",
                                ""##ImportDate"" : ""DCImportDate""
                            }
                        ]
                    }
                ]
            }";

            var importTasks = (List<ImportFileOptions>)CsvToSql.Configuration.ImportTasks.ReadTasks(log, jsonSettings);

            var sqlWriter = new SqlServerWriter(log, "");

            var headers = new List<string>() { "head1", "head2", "head3" };

            // Act
            sqlWriter.Init(importTasks.First(), headers);
            var headerFields = sqlWriter.GetHeaderFields();

            // Assert
            Assert.AreEqual(headerFields.Count, 4); // { "NewHeadOne", "NewHeadTwo", "head3", "DCImportDate"}
            Assert.AreEqual(headerFields[0].Name, "NewHeadOne");
            Assert.AreEqual(headerFields[1].Name, "NewHeadTwo");
            Assert.AreEqual(headerFields[2].Name, "head3");
            Assert.AreEqual(headerFields[3].Name, "DCImportDate");
            Assert.IsTrue(sqlWriter != null);
        }

        [Test]
        public void SqlWriterTest_GetInsertStatmentsWithImportDate()
        {

            // Arrange
            var jsonSettings = @"
            {
            ""importFiles"": 
                [
                    {
                        ""file"": ""..\\..\\..\\..\\TestCsv\\simpleComma.csv"",
                        ""batchSize"": 4,
                        ""saveMode"": false,
                        ""columnMapping"": [
                            {
                                ""head1"": ""NewHeadOne"",
                                ""head2"": ""NewHeadTwo"",
                                ""##ImportDate"": ""DCImportDate""
                            }
                        ]
                    }
                ]
            }";

            var importTasks = (List<ImportFileOptions>)CsvToSql.Configuration.ImportTasks.ReadTasks(log, jsonSettings);

            var sqlWriter = new SqlServerWriter(log, "");

            var headers = new List<string>() { "head1", "head2", "head3" };
            var linesToWrite = new List<List<string>>() {
                new List<string>(){ "a0", "a1", "a2"},
                new List<string>(){ "b0", "b1", "b2"}
            };

            sqlWriter.Init(importTasks.First(), headers);

            // Act

            string insertSql = sqlWriter.GetInsertStatements(linesToWrite);

            // Assert
            Assert.IsTrue(sqlWriter != null);
            Assert.AreEqual(sqlWriter.GetHeaderFields().Count, 4); // { "head1", "head2", "head3", "DCImportDate"}
            Assert.IsTrue(insertSql.Contains("INSERT INTO"));
            Assert.IsTrue(insertSql.Contains("CONVERT(DATETIME"));
        }

        [Test]
        public void SqlWriterTest_GetInsertStatmentsWithImportFileName()
        {

            // Arrange
            var jsonSettings = @"
            {
            ""importFiles"": 
                [
                    {
                        ""file"": ""..\\..\\..\\..\\TestCsv\\simpleComma.csv"",
                        ""batchSize"": 4,
                        ""columnMapping"": [
                            {
                                ""head1"": ""NewHeadOne"",
                                ""head2"": ""NewHeadTwo"",
                                ""##ImportFileName"": ""DCSource""
                            }
                        ]
                    }
                ]
            }";

            var importTasks = (List<ImportFileOptions>)CsvToSql.Configuration.ImportTasks.ReadTasks(log, jsonSettings);

            var sqlWriter = new SqlServerWriter(log, "");

            var headers = new List<string>() { "head1", "head2", "head3" };
            var linesToWrite = new List<List<string>>() {
                new List<string>(){ "a0", "a1", "a2"},
                new List<string>(){ "b0", "b1", "b2"}
            };

            sqlWriter.Init(importTasks.First(), headers);

            // Act

            string insertSql = sqlWriter.GetInsertStatements(linesToWrite);

            // Assert
            Assert.IsTrue(sqlWriter != null);
            Assert.AreEqual(sqlWriter.GetHeaderFields().Count, 4); // { "head1", "head2", "head3", "DCSource"}
            Assert.AreEqual(sqlWriter.GetHeaderFields()[3].Name, "DCSource");
            Assert.IsTrue(insertSql.Contains("INSERT INTO"));
            Assert.IsTrue(insertSql.Contains("simpleComma.csv"));
        }

        [Test]
        public void SqlWriterTest_GetInsertStatmentsWithConkretImportFileName()
        {

            // Arrange
            var jsonSettings = @"
                {
                ""importFiles"": 
                    [
                        {
                            ""file"": ""..\\..\\..\\..\\TestCsv\\simpleComma.csv"",
                            ""batchSize"": 4,
                            ""columnMapping"": [
                                {
                                    ""head1"": ""NewHeadOne"",
                                    ""head2"": ""NewHeadTwo"",
                                    ""**20200911132530_20200907_Clients_SE.csv"": ""DCSource""
                                }
                            ]
                        }
                    ]
                }";

            var importTasks = (List<ImportFileOptions>)CsvToSql.Configuration.ImportTasks.ReadTasks(log, jsonSettings);

            var sqlWriter = new SqlServerWriter(log, "");

            var headers = new List<string>() { "head1", "head2", "head3" };
            var linesToWrite = new List<List<string>>() {
                    new List<string>(){ "a0", "a1", "a2"},
                    new List<string>(){ "b0", "b1", "b2"}
                };

            sqlWriter.Init(importTasks.First(), headers);

            // Act

            string insertSql = sqlWriter.GetInsertStatements(linesToWrite);

            // Assert
            Assert.IsTrue(sqlWriter != null);
            Assert.AreEqual(sqlWriter.GetHeaderFields().Count, 4); // { "head1", "head2", "head3", "DCSource"}
            Assert.AreEqual(sqlWriter.GetHeaderFields()[3].Name, "DCSource");
            Assert.IsTrue(insertSql.Contains("INSERT INTO"));
            Assert.IsTrue(insertSql.Contains("20200911132530_20200907_Clients_SE.csv"));
        }

        [Test]
        public void SqlWriterTest_GetInsertStatmentMix()
        {

            // Arrange
            var jsonSettings = @"
                {
                ""importFiles"": 
                    [
                        {
                            ""file"": ""..\\..\\..\\..\\TestCsv\\simpleComma.csv"",
                            ""batchSize"": 4,
                            ""forceCreateTable"": true,
                            ""columnMapping"": [
                                {
                                    ""head1"": ""NewHeadOne"",
                                    ""head2"": ""NewHeadTwo"",
                                    ""**20200911132530_20200907_Clients_SE.csv"": ""DCSourceOne"",
                                    ""##ImportFileName"": ""DCSourceTwo"",
                                    ""##ImportDate"" : ""DCImportDate""
                                }
                            ]
                        }
                    ]
                }";

            var importTasks = (List<ImportFileOptions>)CsvToSql.Configuration.ImportTasks.ReadTasks(log, jsonSettings);

            var sqlWriter = new SqlServerWriter(log, "");

            var headers = new List<string>() { "head1", "head2", "head3" };
            var linesToWrite = new List<List<string>>() {
                    new List<string>(){ "a0", "a1", "a2"},
                    new List<string>(){ "b0", "b1", "b2"}
                };

            sqlWriter.Init(importTasks.First(), headers);

            // Act

            string insertSql = sqlWriter.GetInsertStatements(linesToWrite);

            // Assert
            Assert.IsTrue(sqlWriter != null);
            var headerFields = sqlWriter.GetHeaderFields();

            // Assert
            Assert.AreEqual(headerFields.Count, 6); // { "NewHeadOne", "NewHeadTwo", "head3", "DCImportDate"}
            Assert.AreEqual(headerFields[0].Name, "NewHeadOne");
            Assert.AreEqual(headerFields[1].Name, "NewHeadTwo");
            Assert.AreEqual(headerFields[2].Name, "head3");
            Assert.AreEqual(headerFields[3].Name, "DCImportDate");
            Assert.AreEqual(headerFields[4].Name, "DCSourceTwo");
            Assert.AreEqual(headerFields[5].Name, "DCSourceOne");
            Assert.IsTrue(insertSql.Contains("INSERT INTO"));
            Assert.IsTrue(insertSql.Contains("simpleComma.csv"));
            Assert.IsTrue(insertSql.Contains("20200911132530_20200907_Clients_SE.csv"));
            Assert.IsTrue(insertSql.Contains("CONVERT(DATETIME"));
        }
    }
}


