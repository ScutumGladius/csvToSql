using CsvToSql.logging;
using CsvToSql.Configuration;
using System;
using CsvToSql.Core;

namespace CsvToSql
{
    class Program
    {
        static void Main(string[] args)
        {
            var l = new Logging();
            ArgcOptions programCfg = ProgramConfiguration.Read(l, args);
            //var jsonCfg = JsonConfiguration.Read(args);
        }
    }
}
