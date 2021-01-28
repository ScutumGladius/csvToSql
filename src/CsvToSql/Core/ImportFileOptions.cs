using System;
using System.Collections.Generic;
using System.Text;

namespace CsvToSql.Core
{
    public class ImportFileOptions
    {
        public string connectionString { get; set; }
        public string file { get; set; }
        public string sheet { get; set; }
        public int row { get; set; }
        public int col { get; set; }
        public int skipline { get; set; }
        public string table { get; set; }
        public string prefix { get; set; }
        public string key { get; set; }
        public string macFix { get; set; }
        public bool csv { get; set; } // true || false
        public int batchSize { get; set; }
        public string delimiter { get; set; }
        

        public Dictionary<string, string> columnMapping { get; set; }
    }

}
