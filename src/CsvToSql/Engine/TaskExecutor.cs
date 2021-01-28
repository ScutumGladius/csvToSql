using CsvToSql.Core;
using CsvToSql.FileReader;
using CsvToSql.logging;
using CsvToSql.SqlWriter;
using LumenWorks.Framework.IO.Csv;
using System;
using System.Collections.Generic;
using System.Text;

namespace CsvToSql.Engine
{
    public class TaskExecutor
    {
        private Logging Log;
        private readonly ReadCsv CsvReader;
        private readonly ISqlWriter SqlWriter;

        public TaskExecutor(Logging log, ReadCsv csvReader, ISqlWriter sqlWriter)
        {
            Log = log;
            CsvReader = csvReader;
            SqlWriter = sqlWriter;
        }

        public int Run(ImportFileOptions importTask) {
            Log.Debug($"TaskExecutor Run for '{importTask.file}'");

            // 1. Read csv 
            // 2. Store csv

            return CsvReader.Read(importTask, SqlWriter);

        }

    }
}
