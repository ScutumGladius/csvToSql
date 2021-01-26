using CsvToSql.Core;
using CsvToSql.logging;
using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Linq;

namespace CsvToSql.Configuration
{
    public static class ImportTasks
    {
        public static IList<ImportFileOptions> Read(Logging l, string JsonCfgFile)
        {
            var fileContent = File.ReadAllText(JsonCfgFile);
            JObject importTasksRoot = JObject.Parse(fileContent);

            var importFiles = new List<ImportFileOptions>();

            var importTasks = importTasksRoot.SelectToken("importFiles");
            foreach (var importFile in importTasks)
            {
                var importFileOptions = new ImportFileOptions();
                
                importFileOptions.connectionString = getTokenAsString(importFile, "connectionString", "");
                importFileOptions.file = getTokenAsString(importFile, "file", "");
                importFileOptions.sheet = getTokenAsString(importFile, "sheet", "");
                importFileOptions.row = getTokenAsInt(importFile, "row", -1);
                importFileOptions.col = getTokenAsInt(importFile, "col", -1);
                importFileOptions.skipline = getTokenAsInt(importFile, "skipline", -1);

                importFileOptions.table = getTokenAsString(importFile, "table", "");
                importFileOptions.prefix = getTokenAsString(importFile, "prefix", "");
                importFileOptions.key = getTokenAsString(importFile, "key", "");
                importFileOptions.macFix = getTokenAsString(importFile, "macFix", "");
                importFileOptions.csv = getTokenAsBoolean(importFile, "csv", true);
//        public Dictionary<string, string> columnMapping { get; set; }

                importFiles.Add(importFileOptions);
            }

            //importFiles = (List<ImportFileOptions>)importTasks.ToObject<IList<ImportFileOptions>>(); ' Boom :(

            return importFiles;
        }

        private static string getTokenAsString(JToken importFile, string key, string defaultVal)
        {
            try
            {
                var t = importFile.SelectToken(key);
                return t == null ? defaultVal : t.ToString();
            }
            catch (Exception)
            {
                return defaultVal;
            }
        }
        private static int getTokenAsInt(JToken importFile, string key, int defaultVal)
        {
            try
            {
                var t = importFile.SelectToken(key);
                return t == null ? defaultVal : (int)t;
            }
            catch (Exception)
            {
                return defaultVal;
            }
        }
        private static bool getTokenAsBoolean(JToken importFile, string key, bool defaultVal)
        {
            try
            {
                var t = importFile.SelectToken(key);
                return t == null ? defaultVal : (bool)t;
            }
            catch (Exception)
            {
                return defaultVal;
            }
        }

    }
}
