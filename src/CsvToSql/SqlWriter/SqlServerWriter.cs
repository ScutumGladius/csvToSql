using CsvToSql.Core;
using CsvToSql.logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace CsvToSql.SqlWriter
{

    public class SqlServerWriter : ISqlWriter
    {

        private readonly Logging Log;

        public SqlServerWriter(Logging log)
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
