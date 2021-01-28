using CsvToSql.Core;
using CsvToSql.FileReader;
using CsvToSql.logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace CsvToSql.Engine
{
    public class TaskExecutor
    {
        private Logging Log;
        
        public TaskExecutor(Logging log = null)
        {
            Log = log;
        }

        public int Run(ImportFileOptions importTask) {
            Log.Debug($"TaskExecutor Run for '{importTask.file}'");

            // 1. Read csv 
            var reader = new ReadCsv(Log);
            return reader.Read(importTask);

            // 2. Store csv
        }

    }
}
