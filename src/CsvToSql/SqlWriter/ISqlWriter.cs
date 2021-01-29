using CsvToSql.Core;
using System.Collections.Generic;

namespace CsvToSql.SqlWriter
{
    public interface ISqlWriter
    {
        void Init(ImportFileOptions importTask, List<string> headers);
        int Write(List<List<string>> linesToWrite);
    }
}