﻿using CsvToSql.logging;
using CsvToSql.Configuration;
using System;
using System.Linq;
using CsvToSql.Core;
using Microsoft.Extensions.Configuration;
using CsvToSql.Engine;
using System.Collections.Generic;
using System.Diagnostics;

namespace CsvToSql
{
    class Program
    {
        public IConfiguration Configuration { get; set; }

        static void Main(string[] args)
        {
            IConfiguration appSettings = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json").Build();
            
            var defaultConnectionString = appSettings.GetSection("ConnectionStrings").GetChildren().First().Value;

            var log = new Logging();
            ArgcOptions programCfg = ProgramConfiguration.Read(log, args);
            var importTasks = (List<ImportFileOptions>)ImportTasks.ReadFromJsonFile(log, programCfg);

            var csvReader = new FileReader.ReadCsv(log);
            var sqlWriter = new SqlWriter.SqlServerWriter(log, defaultConnectionString);

            var executor = new TaskExecutor(log, csvReader, sqlWriter);
            importTasks.ForEach(impTask => executor.Run(impTask));
 
        }
    }
}
