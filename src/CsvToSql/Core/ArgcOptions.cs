using CommandLine;
using System;
using System.Collections.Generic;
using System.Text;

namespace CsvToSql.Core
{
    public class ArgcOptions
    {
        [Option("settings", Required = false, Default = "settings.json", HelpText = "Path to JsonSettings file")]
        public string JsonCfgFile { get; set; }

        [Option("database", Required = false, HelpText = "Database Name")]
        public string Database { get; set; }

        [Option("NoID", Required = false, HelpText = "keine ID-Spalte")]
        public bool NoID { get; set; }

        [Option("truncate", Required = false, Default=false, HelpText = "truncate table, if exists")]
        public bool Truncate { get; set; }

        [Option("importdate", Required = false, HelpText = "value for _DCImportDate_")]
        public string ImportDate { get; set; }

        public DateTime ImportDateTime { get; set; }

        [Value(0)]
        public IEnumerable<string> StringSequence { get; set; }
    }
}
