using CsvToSql.logging;
using CsvToSql.Configuration;
using System;
using System.Linq;
using CsvToSql.Core;
using Microsoft.Extensions.Configuration;
using CsvToSql.Engine;
using System.Collections.Generic;

namespace CsvToSql
{
    class Program
    {
        public IConfiguration Configuration { get; set; }

        static void Main(string[] args)
        {
            var appSettings = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json").Build();

            var log = new Logging();
            ArgcOptions programCfg = ProgramConfiguration.Read(log, args);
            var importTasks = (List<ImportFileOptions>)ImportTasks.ReadFromJsonFile(log, programCfg.JsonCfgFile);

            var executor = new TaskExecutor(log);
            importTasks.ForEach(impTask => executor.Run(impTask));

        }
    }
}
