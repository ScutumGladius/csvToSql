using CsvToSql.Core;
using CsvToSql.logging;
using CsvToSql.SqlWriter;
using LumenWorks.Framework.IO.Csv;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsvToSql.FileReader
{
    public class ReadCsv
    {
        private readonly Logging Log;

        public ReadCsv(Logging log)
        {
            Log = log;
        }

        public int Read(ImportFileOptions importTask, ISqlWriter sqlWriter) {
            Log.Debug($"ReadCsv for '{importTask.file}'");

            List<List<string>> allLineFields = new List<List<string>>();
            var fileInfo = new System.IO.FileInfo(importTask.file);

            Char delimiter = string.IsNullOrWhiteSpace(importTask.delimiter) ? GuessDelimeter(fileInfo) : importTask.delimiter.First<char>();
            var rowCounter = 0;

            try
            {
                using (var reader = new System.IO.StreamReader(fileInfo.FullName, Encoding.Default))
                {
                    //Char quotingCharacter = '\0'; // no quoting-character;
                    Char quotingCharacter = '"'; // "Value1","Value2"
                    Char escapeCharacter = quotingCharacter;
                    using (var csv = new CsvReader(reader, false, delimiter, quotingCharacter, escapeCharacter, '\0', ValueTrimmingOptions.All))
                    {
                        var headers = ReadHeaders(csv);
                        Log.Debug("Read:Headers : " + string.Join(",", headers));

                        csv.DefaultParseErrorAction = ParseErrorAction.ThrowException;
                        csv.SkipEmptyLines = true;
 
                        do
                        {
                            allLineFields = ReadNextBatch(csv, importTask.batchSize);
                            rowCounter += allLineFields.Count;
                            Log.Debug($"Read Next Batch : {rowCounter}/{allLineFields.Count}");

                            sqlWriter.Write(importTask, rowCounter, headers, allLineFields);
                        } while (allLineFields.Count > 0);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error($"ReadCsv catch exception : {ex.Message}");
                throw;
            }
            return rowCounter;
        }

        private List<string> ReadHeaders(CsvReader csv)
        {
            csv.ReadNextRecord();
            return CsvRowToList(csv);
        }

        private List<List<string>> ReadNextBatch(CsvReader csv, int batchSize)
        {
            List<List<string>> allLineFields = new List<List<string>>();
            var currentRow = 0; 

            while (currentRow++ < batchSize && csv.ReadNextRecord())
            {
                List<string> fields = CsvRowToList(csv);
                allLineFields.Add(fields);
            }
            return allLineFields;
        }

        private List<string> CsvRowToList(CsvReader csv)
        {
            List<string> fields = new List<string>(csv.FieldCount);
            for (int i = 0; i < csv.FieldCount; i++)
            {
                try
                {
                    string field = csv[i];
                    fields.Add(field.Trim('"'));
                }
                catch (MalformedCsvException ex)
                {
                    Log.Error($"ReadCsv catch MalformedCsvException : i={i}, {ex.Message}");
                    fields.Add("");
                }
            }
            return fields;
        }
        
        private char GuessDelimeter(System.IO.FileInfo fileInfo)
        {
            return ',';
        }
    }
}
