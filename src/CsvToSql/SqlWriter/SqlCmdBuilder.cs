using CsvToSql.Core;
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


        public SqlCmdBuilder(ImportFileOptions importTask)
        {
            ImportTask = importTask;
        }

        internal List<SqlField> GetHeaderFields(List<string> headers)
        {
            SetSqlImportDate();

            var sqlFieldsHeaders = headers
                .Select(x => HeaderLineToSqlField(x))
                .ToList();

            var importDayKey = "##ImportDate";
            if (ImportTask.columnMapping.ContainsKey(importDayKey))
            {
                sqlFieldsHeaders.Add(new SqlField { Name = ImportTask.columnMapping[importDayKey], SqlType = System.Data.SqlDbType.DateTime });
            }
            return sqlFieldsHeaders;
        }

        private void SetSqlImportDate()
        {
            const string fmt120 = "yyyy-MM-dd HH:mm:ss";
            var dt120 = ImportTask.ImportDateTime.ToString(fmt120);
            ImportDateTimeString = string.Format($"CONVERT(DATETIME, '{dt120}', 120)");
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
                case System.Data.SqlDbType.DateTime:
                    return string.Format($"[{sqlField.Name}] [datetime] NULL");
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
                        return string.Format($"[{sqlField.Name}] [nvarchar](512) NULL");
                    }
                    if (sqlField.Name.ToLower().Contains("code") ||
                        sqlField.Name.ToLower().Contains("date") ||
                        sqlField.Name.ToLower().Contains("pmo")  ||
                        sqlField.Name.ToLower().Contains("impl") ||
                        sqlField.Name.ToLower().Contains("total") ||
                        sqlField.Name.ToLower().Contains("dez") ||
                        sqlField.Name.ToLower().Contains("scope")
                        )
                    {
                        sqlField.Length = 32;
                        return string.Format($"[{sqlField.Name}] [nvarchar](32) NULL");
                    }
                    sqlField.Length = 128;
                    return string.Format($"[{sqlField.Name}] [nvarchar](128) NULL");

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
                if (headers[i].SqlType == System.Data.SqlDbType.DateTime)
                {
                    acc.Add(ImportDateTimeString);
                }
                else
                {
                    acc.Add(string.Format($"'{rowToWrite[i].Left(headers[i].Length).Replace("'", "''")}'"));
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
    public static class StringExtensions
    {
        public static string Left(this string value, int maxLength)
        {
#if DEBUG            
            maxLength = 500;
#endif


            if (string.IsNullOrEmpty(value)) return value;
            maxLength = Math.Abs(maxLength);

            return (value.Length <= maxLength
                   ? value
                   : value.Substring(0, maxLength)
                   );
        }
    }
}