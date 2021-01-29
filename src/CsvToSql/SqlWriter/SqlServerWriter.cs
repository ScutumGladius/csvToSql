using CsvToSql.Core;
using CsvToSql.logging;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Data.SqlClient;
using System.Data;

namespace CsvToSql.SqlWriter
{
    public struct SqlField {
        public string Name { set; get; }
        public SqlDbType SqlType { set; get; }
    }

    public class SqlServerWriter : ISqlWriter
    {

        private readonly Logging Log;

        private ImportFileOptions ImportTask = null;
        private List<string> Headers = null;
        private List<SqlField> HeaderFields = new List<SqlField>();
        private HeaderHandler HeaderHandler;

        public SqlServerWriter(Logging log)
        {
            Log = log;
        }

        public void Init(ImportFileOptions importTask, List<string> headers) {
            ImportTask = importTask;
            Headers = headers;
            Log.Debug($"SqlServerWriter: init for '{ImportTask.file}'");
            HeaderHandler = new HeaderHandler(importTask);
            HeaderFields = HeaderHandler.GetHeaderFields(Headers);
        }


        public int Write( List<List<string>> linesToWrite)
        {
            Log.Debug($"SqlServerWriter: Write; Count='{linesToWrite.Count}'");
            return 0;
        }
    }
}
