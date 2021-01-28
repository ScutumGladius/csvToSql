using CsvToSql.Core;
using System.Collections.Generic;

namespace CsvToSql.SqlWriter
{
    public interface ISqlWriter
    {
        int Write(ImportFileOptions importTask, int rowCounter, List<string> headers, List<List<string>> linesToWrite);
    }
}