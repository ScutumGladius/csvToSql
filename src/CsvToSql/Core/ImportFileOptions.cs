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
        public string quoting { get; set; }
        public bool truncate { get; set; }
        public DateTime ImportDateTime { get; set; } // from args
        public bool saveMode { get; set; } // prevent "String or binary data would be truncated." Adjust the length of the Data to the field size 
        public bool forceCreateTable { get; set; } // Remove old and create new SQL-Table
        public string comment { get; set; }


        public Dictionary<string, string> columnMapping { get; set; }
    }

}
