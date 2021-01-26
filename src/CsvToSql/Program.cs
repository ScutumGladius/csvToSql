using CsvToSql.logging;
using CsvToSql.Configuration;
using System;
using CsvToSql.Core;
using Microsoft.Extensions.Configuration;

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
            var jsonCfg = ImportTasks.Read(log, programCfg.JsonCfgFile);
        }
    }
}
