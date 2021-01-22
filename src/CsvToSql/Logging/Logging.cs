using System;
using System.Collections.Generic;
using System.Text;

namespace CsvToSql.logging
{
    public class Logging
    {
        public void Debug(string toLog)
        {
            Console.WriteLine("Debug:" + toLog);
        }
        public void Trace(string toLog)
        {
            Console.WriteLine("Trace:" + toLog);
        }
    }
}
