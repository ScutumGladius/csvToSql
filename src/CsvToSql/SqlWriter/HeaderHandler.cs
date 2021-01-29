using CsvToSql.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsvToSql.SqlWriter
{
    class HeaderHandler
    {
        private ImportFileOptions ImportTask;

        public HeaderHandler(ImportFileOptions importTask)
        {
            ImportTask = importTask;
        }

        internal List<SqlField> GetHeaderFields(List<string> headers)
        {
            return headers
            .Select(x => new SqlField { Name = x, SqlType = System.Data.SqlDbType.VarChar })
            .ToList();
        }
    }
}
