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

        public SqlCmdBuilder(ImportFileOptions importTask)
        {
            ImportTask = importTask;
        }

        internal List<SqlField> GetHeaderFields(List<string> headers)
        {
            return headers
            .Select(x => new SqlField { Name = x, SqlType = System.Data.SqlDbType.VarChar })
            .ToList();
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

            var body = headerFields.Select(f => FieldToCreateRow(f));
            return string.Format($"IF OBJECT_ID('{ImportTask.table}', 'U') IS NULL\n\tCREATE TABLE [{ImportTask.table}] ( {string.Join(",", body)} );");
        }


        internal string FieldToCreateRow(SqlField sqlField)
        {
            switch (sqlField.SqlType)
            {
                case System.Data.SqlDbType.DateTime:
                    return string.Format($"[{sqlField.Name}] [datetime] NULL");
                case System.Data.SqlDbType.Int:
                    return string.Format($"[{sqlField.Name}] [int] NULL");
                case System.Data.SqlDbType.VarChar:
                    return string.Format($"[{sqlField.Name}] [nchar](50) NULL");
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
                acc.Add(string.Format($"'{rowToWrite[i].Replace("'", "''")}'"));
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