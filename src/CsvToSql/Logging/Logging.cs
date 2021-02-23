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
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("Trace:" + toLog);
            Console.ResetColor();
        }
        public void Error(string toLog)
        {

            Console.BackgroundColor = ConsoleColor.Red;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Error:" + toLog);
            Console.ResetColor();
        }
    }
}
