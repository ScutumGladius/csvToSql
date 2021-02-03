using CsvToSql.Core;
using CsvToSql.logging;
using CsvToSql.SqlWriter;
using LumenWorks.Framework.IO.Csv;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

            Stopwatch _timer = new Stopwatch();
            _timer.Start();
            
            List<List<string>> batchLineFields = new List<List<string>>();
            var fileInfo = new System.IO.FileInfo(importTask.file);

            if (!System.IO.File.Exists(importTask.file)) {
                Log.Error($"ReadCSV : '{importTask.file}' not exists!");
                return -1;
            }

            var firstLine = ReadFirstLine(fileInfo);

            Char delimiter = string.IsNullOrWhiteSpace(importTask.delimiter) ? GuessDelimeter(firstLine) : importTask.delimiter.First<char>();
            //Char quotingCharacter = '\0'; // no quoting-character;
            Char quotingCharacter = string.IsNullOrWhiteSpace(importTask.quoting) ? GuessquotingCharacter(firstLine) : importTask.quoting.First<char>(); 
            var rowCounter = 0;

            try
            {
                using (var reader = new System.IO.StreamReader(fileInfo.FullName, Encoding.Default))
                {
                    Char escapeCharacter = quotingCharacter;
                    using (var csv = new CsvReader(reader, false, delimiter, quotingCharacter, escapeCharacter, '\0', ValueTrimmingOptions.All))
                    {
                        csv.DefaultParseErrorAction = ParseErrorAction.ThrowException;
                        csv.SkipEmptyLines = true;

                        var headers = ReadHeaders(csv);
                        Log.Debug("Read:Headers : " + string.Join(",", headers));
                        sqlWriter.Init(importTask, headers);
 
                        do
                        {
                            // Read a pies of CSV
                            batchLineFields = ReadNextBatch(csv, importTask.batchSize);
                            if (batchLineFields.Count == 0) break;
                            Log.Debug($"Read Next Batch : {batchLineFields.Count} entries. From {rowCounter} to {batchLineFields.Count + rowCounter}");

                            //Create & Write SQL
                            sqlWriter.Write(batchLineFields);

                            rowCounter += batchLineFields.Count;
                        } while (batchLineFields.Count > 0);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error($"ReadCsv catch exception : {ex.Message}");
                return -1;
            }

            _timer.Stop();

            var timeSpan = TimeSpan.FromMilliseconds(_timer.ElapsedMilliseconds);
            Log.Debug($"Import from '{importTask.file}' takes {_timer.ElapsedMilliseconds / 1000.0} seconds or {timeSpan.Minutes}:{timeSpan.Seconds} Minutes.");
            sqlWriter.UpdateStatusTable(rowCounter, timeSpan);

            return rowCounter;

        }


        private string ReadFirstLine(System.IO.FileInfo fileInfo)
        {
            try
            {
                using (var reader = new System.IO.StreamReader(fileInfo.FullName, Encoding.Default))
                {
                    return reader.ReadLine();
                }
            }
            catch (Exception ex)
            {
                Log.Error($"ReadCsv ReadFirstLine exception : {ex.Message}");
                return "";
            }
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
            var exWasRaised = false;
            for (int i = 0; i < csv.FieldCount; i++)
            {
                try
                {
                    string field = csv[i];
                    fields.Add(field.Trim('"'));
                }
                catch (MalformedCsvException ex)
                {
                    Log.Error($"ReadCsv catch MalformedCsvException : i={i}/{csv.FieldCount}, {ex.Message.Substring(0,60)}...; ");
                    fields.Add("");
                    exWasRaised = true;
                }
                if (exWasRaised) Log.Error($"MalformedCsvException was rised for\n\"{string.Join(",", fields)}\"");
            }
            return fields;
        }
        
        private char GuessDelimeter(string firstLine)
        {
            return ',';
        }
        private char GuessquotingCharacter(string firstLine)
        {
            var firstlineChar = firstLine.TrimStart().First<char>();
            return firstlineChar == '"' ? '"' : '\0';
        }

    }
}
