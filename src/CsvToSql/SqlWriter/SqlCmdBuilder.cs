using CsvToSql.Core;
using CsvToSql.Exstensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;

namespace CsvToSql.SqlWriter
{
    class SqlCmdBuilder
    {
        private ImportFileOptions ImportTask;
        private string ImportDateTimeString;
        private string ImportExactFileName;


        public SqlCmdBuilder(ImportFileOptions importTask)
        {
            ImportTask = importTask;
        }

        internal List<SqlField> GetHeaderFields(List<string> headers)
        {

            var sqlFieldsHeaders = headers
                .Select(x => HeaderLineToSqlField(x))
                .ToList();

            sqlFieldsHeaders = HandleSqlImportDate(sqlFieldsHeaders); // "##ImportDate": "DCImportDate"
            sqlFieldsHeaders = HandleSqlDataSource(sqlFieldsHeaders); // "##ImportFileName": "DCSource" Or "**20200911132530_20200907_Clients_SE.csv": "DCSource"

            return sqlFieldsHeaders;
        }

        private List<SqlField> HandleSqlDataSource(List<SqlField> sqlFieldsHeaders)
        {
            // 1. Handle "##ImportFileName": "DCSource"
            var importFileName = "##ImportFileName";
            if (ImportTask.columnMapping.ContainsKey(importFileName))
            {
                sqlFieldsHeaders.Add(new SqlField { Name = ImportTask.columnMapping[importFileName], SqlType = System.Data.SqlDbType.Structured });
            }

            ImportExactFileName = "";
            // 2. Handle "**ExactFileName.csv": "DCSource"
            foreach (var item in ImportTask.columnMapping)
            {
                var fileNameKey = item.Key;
                var fieldName = item.Value;
                if (fileNameKey.StartsWith("**")) {
                    ImportExactFileName = fileNameKey.Replace("**", "");
                    sqlFieldsHeaders.Add(new SqlField { Name = fieldName, SqlType = System.Data.SqlDbType.Money });
                }
            }
            return sqlFieldsHeaders;
        }

        private List<SqlField> HandleSqlImportDate(List<SqlField> sqlFieldsHeaders)
        {
            //1. SetSqlImportDate
            const string fmt120 = "yyyy-MM-dd HH:mm:ss";
            var dt120 = ImportTask.ImportDateTime.ToString(fmt120);
            ImportDateTimeString = string.Format($"CONVERT(DATETIME, '{dt120}', 120)");
            
            //2. Add to headers, if nesessary
            var importDayKey = "##ImportDate";
            if (ImportTask.columnMapping.ContainsKey(importDayKey))
            {
                sqlFieldsHeaders.Add(new SqlField { Name = ImportTask.columnMapping[importDayKey], SqlType = System.Data.SqlDbType.DateTime });
            }
            return sqlFieldsHeaders;
        }

        internal string GetDropTableStatement()
        {
            return ImportTask.forceCreateTable ?
                string.Format($"IF OBJECT_ID('{ImportTask.table}', 'U') IS NOT NULL\n\tDROP TABLE [{ImportTask.table}];") :
                "";
        }

        private SqlField HeaderLineToSqlField(string fieldName) {
            var fName = fieldName;
            var sqlType = System.Data.SqlDbType.VarChar;
            if (ImportTask.columnMapping.ContainsKey(fieldName)) {
                fName = ImportTask.columnMapping[fieldName];
            }
            return new SqlField { Name = fName, SqlType = sqlType };
        }

        internal string GetCreateTableStatement(List<SqlField> headerFields)
        {
            /*CREATE TABLE [dbo].[Table_1](
                [Header1][nchar](50) NULL,
                [Header2] [nvarchar](max)NULL,
                [Header3] [datetime] NULL,
                [Header4] [int] NULL
            );
            GO;*/

            var body = headerFields.Select(f => FieldToCreateRow(ref f));
            return string.Format($"IF OBJECT_ID('{ImportTask.table}', 'U') IS NULL\n\tCREATE TABLE [{ImportTask.table}] ( {string.Join(",", body)} );");
        }


        internal string FieldToCreateRow(ref SqlField sqlField)
        {
            switch (sqlField.SqlType)
            {
                case System.Data.SqlDbType.DateTime:   //"##ImportDate"
                    return string.Format($"[{sqlField.Name}] [datetime] NULL");

                case System.Data.SqlDbType.Structured: // "##ImportFileName"
                case System.Data.SqlDbType.Money:      // "**ExactFileName.csv"
                    sqlField.Length = 128;
                    return string.Format($"[{sqlField.Name}] [nvarchar]({sqlField.Length}) NULL");

                case System.Data.SqlDbType.Int:
                    return string.Format($"[{sqlField.Name}] [int] NULL");

                case System.Data.SqlDbType.VarChar:
                    if (sqlField.Name.ToLower().Contains("com") ||
                          sqlField.Name.ToLower().Contains("review") ||
                          sqlField.Name.ToLower().Contains("hist") ||
                          sqlField.Name.ToLower().Contains("desc") ||
                          sqlField.Name.ToLower().Contains("info") ||
                          sqlField.Name.ToLower().Contains("cmnt") ||
                          sqlField.Name.ToLower().Contains("reason")
                          )
                    {
                        sqlField.Length = 512;
                        return string.Format($"[{sqlField.Name}] [nvarchar]({sqlField.Length}) NULL");
                    }
                    if (sqlField.Name.ToLower().Contains("path") )
                    {
                        sqlField.Length = 256;
                        return string.Format($"[{sqlField.Name}] [nvarchar]({sqlField.Length}) NULL");
                    }
                    if (sqlField.Name.ToLower().Contains("code") ||
                        sqlField.Name.ToLower().Contains("date") ||
                        sqlField.Name.ToLower().Contains("pmo") ||
                        sqlField.Name.ToLower().Contains("impl") ||
                        sqlField.Name.ToLower().Contains("total") ||
                        sqlField.Name.ToLower().Contains("dez") ||
                        sqlField.Name.ToLower().Contains("scope")
                        )
                    {
                        sqlField.Length = 32;
                        return string.Format($"[{sqlField.Name}] [nvarchar]({sqlField.Length}) NULL");
                    }
                    sqlField.Length = 128;
                    return string.Format($"[{sqlField.Name}] [nvarchar]({sqlField.Length}) NULL");

                default:
                    throw new Exception($"Unknown sqlField.SqlType {sqlField.SqlType}.");
            }

        }

        internal string GetTruncateTableStatement()
        {
            return ImportTask.truncate ?
                string.Format($"TRUNCATE TABLE [{ImportTask.table}];") :
                ";";
        }

        internal string GetInsertStatements(List<SqlField> headers, List<List<string>> linesToWrite)
        {
            if (linesToWrite.Count <= 0) return "";
            string insertIntoPart = GetInsertIntoPart(headers);
            string values = string.Join(",", linesToWrite.Select(r => BuildInsertStatmentForValues(headers, r)));
            return insertIntoPart + values + ";";
        }

        private string BuildInsertStatmentForValues(List<SqlField> headers, List<string> rowToWrite)
        {
            List<string> acc = new List<string>();     
            for (int i = 0; i < headers.Count; i++)
            {
                switch (headers[i].SqlType)
                {
                    case System.Data.SqlDbType.DateTime:   //"##ImportDate"
                        acc.Add(ImportDateTimeString);
                        break;
                    case System.Data.SqlDbType.Structured: //"##ImportFileName"
                        acc.Add(string.Format($"'{ImportTask.file.Replace("'", "''")}'"));
                        break;
                    case System.Data.SqlDbType.Money:      //"**ExactFileName.csv"
                        acc.Add(string.Format($"'{ImportExactFileName.Replace("'", "''")}'"));
                        break;
                    default:
                        var varCharValue = rowToWrite[i];
                        if (ImportTask.saveMode)
                            varCharValue = varCharValue.Left(headers[i].Length - 1);
                        acc.Add(string.Format($"'{varCharValue.Replace("'", "''")}'"));
                        break;
                }
            }
            return string.Format($"({string.Join(", ", acc )})");
        }

        private string GetInsertIntoPart(List<SqlField> headers)
        {
            var fNames = headers.Select(o => string.Format($"[{o.Name}]"));
            return string.Format($"INSERT INTO [{ImportTask.table}] ({string.Join(", ", fNames)}) VALUES ");
        }

    }
}