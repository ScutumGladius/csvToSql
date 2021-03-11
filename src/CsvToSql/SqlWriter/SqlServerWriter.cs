﻿using CsvToSql.Core;
using CsvToSql.logging;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Data.SqlClient;
using System.Data;
using CsvToSql.SqlServer;

namespace CsvToSql.SqlWriter
{
    public class SqlField {
        public string Name { set; get; }
        public SqlDbType SqlType { set; get; }
        public int Length;
    }

    public class SqlServerWriter : ISqlWriter
    {

        private readonly Logging Log;

        private ImportFileOptions ImportTask = null;
        private List<string> Headers = null;
        private List<SqlField> HeaderFields = new List<SqlField>();
        private SqlCmdBuilder sqlCmdBuilder;
        private SqlServerService sqlServerService;
        private readonly string defaultConnectionString;
 
        public SqlServerWriter(Logging log, string defaultConnectionString)
        {
            Log = log;
            this.defaultConnectionString = defaultConnectionString;
        }

        public void Init(ImportFileOptions importTask, List<string> headers) {
            ImportTask = importTask;
            Headers = headers;
            Log.Debug($"SqlServerWriter: init for '{ImportTask.file}'");

            sqlCmdBuilder = new SqlCmdBuilder(importTask);

            HeaderFields = sqlCmdBuilder.GetHeaderFields(Headers);

            sqlServerService = new SqlServerService(Log, getSqlConnectionString());

            // Drop table
            sqlServerService.simpleExecQuery(GetDropTableStatement());

            // Create table
            sqlServerService.simpleExecQuery(GetCreateTableStatement());

            // Truncate Table
            sqlServerService.simpleExecQuery(GetTruncateTableStatement());

        }

        private string getSqlConnectionString()
        {
            var connectionString =  string.IsNullOrWhiteSpace(ImportTask.connectionString) ? defaultConnectionString : ImportTask.connectionString;
            Log.Debug($"getSqlConnectionString : connectionString = '{connectionString}'");
            return connectionString;
        }

        public int Write( List<List<string>> linesToWrite)
        {
            Log.Debug($"SqlServerWriter: Write; Count='{linesToWrite.Count}'");

            // insert
            var insertStatement = GetInsertStatements(linesToWrite);

            sqlServerService.simpleExecQuery(insertStatement, ImportTask.retryPolicyNumRetries, ImportTask.retryPolicyDelayRetries);

            return 0;
        }

        public List<SqlField> GetHeaderFields() {
            return HeaderFields;
        }

        public String GetCreateTableStatement()
        {
            return sqlCmdBuilder.GetCreateTableStatement(HeaderFields);
        }

        public String GetDropTableStatement()
        {
            return sqlCmdBuilder.GetDropTableStatement();
        }

        public String GetTruncateTableStatement()
        {
            return sqlCmdBuilder.GetTruncateTableStatement();
        }

        public string GetInsertStatements(List<List<string>> linesToWrite)
        {
            return sqlCmdBuilder.GetInsertStatements(HeaderFields, linesToWrite);
        }

        public void UpdateStatusTable(int rowCounter, TimeSpan timeSpan, long fileLenght)
        {
            sqlServerService.simpleExecQuery(sqlCmdBuilder.GetUpdateTatusStatement(rowCounter, timeSpan, fileLenght), ImportTask.retryPolicyNumRetries, ImportTask.retryPolicyDelayRetries);
        }

        public void ExecuteAdditionalSql()
        {
            var additionalSqStatement = sqlCmdBuilder.GetAdditionalSqlStatement();
            if (string.IsNullOrWhiteSpace(additionalSqStatement)) return;
            Log.Debug($"ExecuteAdditionalSql : additionalSqStatement = \"{additionalSqStatement}\"");
            sqlServerService.simpleExecQuery(additionalSqStatement);
        }
    }
}
