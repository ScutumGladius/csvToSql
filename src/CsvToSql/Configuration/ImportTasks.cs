using CsvToSql.Core;
using CsvToSql.logging;
using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace CsvToSql.Configuration
{
    public static class ImportTasks
    {
        public static IList<ImportFileOptions> ReadFromJsonFile(Logging l, string JsonCfgFile)
        {
            var fileContent = File.ReadAllText(JsonCfgFile);
            //importTasks = (List<ImportFileOptions>)importTasks.ToObject<IList<ImportFileOptions>>(); ' Boom :(
            List<ImportFileOptions> importTasks = (List<ImportFileOptions>)ReadTasks(l, fileContent);
            return importTasks;
        }

        public static IList<ImportFileOptions> ReadTasks(Logging l, string fileContent)
        {
            JObject importTasksRoot = JObject.Parse(fileContent);

            var importFiles = new List<ImportFileOptions>();

            var importTasks = importTasksRoot.SelectToken("importFiles");
            foreach (var importFile in importTasks)
            {
                var importFileOptions = new ImportFileOptions();

                importFileOptions.connectionString = GetTokenAsString(l, importFile, "connectionString", "");
                importFileOptions.file = GetTokenAsString(l, importFile, "file", "");
                importFileOptions.sheet = GetTokenAsString(l, importFile, "sheet", "");
                importFileOptions.row = GetTokenAsInt(l, importFile, "row", -1);
                importFileOptions.col = GetTokenAsInt(l, importFile, "col", -1);
                importFileOptions.skipline = GetTokenAsInt(l, importFile, "skipline", -1);

                importFileOptions.table = GetTokenAsString(l, importFile, "table", "");
                importFileOptions.prefix = GetTokenAsString(l, importFile, "prefix", "");
                importFileOptions.key = GetTokenAsString(l, importFile, "key", "");
                importFileOptions.macFix = GetTokenAsString(l, importFile, "macFix", "");
                importFileOptions.csv = GetTokenAsBoolean(l, importFile, "csv", true);
                importFileOptions.columnMapping = GetTokenAsDictionary(l, importFile, "columnMapping");
                importFileOptions.batchSize = Math.Abs(GetTokenAsInt(l, importFile, "batchSize", 1000));
                importFileOptions.delimiter = GetTokenAsString(l, importFile, "delimiter", "");

                importFiles.Add(importFileOptions);
            }
            return importFiles;
        }

        private static Dictionary<string, string> GetTokenAsDictionary(Logging l, JToken importFile, string key)
        {

            Dictionary<string, string> columnMapping = new Dictionary<string, string>();
            try
            {
                var section = importFile.SelectToken(key);
                if (section == null)
                {
                    l.Trace($"GetTokenAsDictionary : section '{key}' is empty!");
                    return columnMapping; 
                }
                if (section.Count<object>() != 1)
                {
                    l.Trace($"GetTokenAsDictionary : section '{key}' has not any object(s)!");
                    return columnMapping;
                }
                foreach (JProperty dPair in section.First) {
                    columnMapping.Add(dPair.Name, dPair.Value.ToString());
                    l.Trace($"GetTokenAsDictionary : Add key='{dPair.Name}' value='{dPair.Value.ToString()}'");
                }
                return columnMapping;
            }
            catch (Exception)
            {
                return columnMapping;
            }
        }

        private static string GetTokenAsString(Logging l, JToken importFile, string key, string defaultVal)
        {
            var ret = defaultVal;
            try
            {
                var t = importFile.SelectToken(key);
                ret = t == null ? defaultVal : t.ToString();
            }
            catch (Exception)
            {
            }
            l.Trace($"GetTokenAsString {key} = '{ret}'");
            return ret;
        }
        private static int GetTokenAsInt(Logging l, JToken importFile, string key, int defaultVal)
        {
            var ret = defaultVal;
            try
            {
                var t = importFile.SelectToken(key);
                ret = t == null ? defaultVal : (int)t;
            }
            catch (Exception)
            {
            }
            l.Trace($"GetTokenAsInt {key} = {ret}.");
            return ret;
        }
        private static bool GetTokenAsBoolean(Logging l, JToken importFile, string key, bool defaultVal)
        {
            var ret = defaultVal;
            try
            {
                var t = importFile.SelectToken(key);
                ret = t == null ? defaultVal : (bool)t;
            }
            catch (Exception)
            {
            }
            l.Trace($"GetTokenAsBoolean {key} = {ret}");
            return ret;
        }

    }
}
