using CommandLine;
using CsvToSql.Core;
using CsvToSql.logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace CsvToSql.Configuration
{
    public static class ProgramConfiguration
    {
        public static ArgcOptions Read(Logging l, string[] args)
        {
            var parser = new Parser(settings =>
            {
                settings.CaseSensitive = false;
                settings.HelpWriter = Console.Error;
                settings.IgnoreUnknownArguments = false;
            });
            var result = parser.ParseArguments<ArgcOptions>(args);

            var argcOptions = new ArgcOptions();
            result
                .MapResult(
                      options =>
                      {
                          argcOptions = options;
                          return 0;
                      },
                      errors =>
                      {
                          l.Debug(
                              String.Join(",",
                              errors.Select(x => x.ToString())
                              )
                          );
                          return 1;
                      });
            return Validation(argcOptions);
        }

        private static ArgcOptions Validation(ArgcOptions argcOptions) {
            if (string.IsNullOrWhiteSpace(argcOptions.JsonCfgFile)) argcOptions.JsonCfgFile = "settings.json";
            argcOptions.ImportDateTime = parseDateTime(argcOptions.ImportDate);
            return argcOptions;
        }

        private static DateTime parseDateTime(string importDate)
        {
            var dateTimeToReturn = DateTime.Now;
            if (string.IsNullOrWhiteSpace(importDate)) return dateTimeToReturn;

            var importDateClean = importDate.Trim().ToLower();
            if (importDateClean.StartsWith("now")) return dateTimeToReturn;
            
            try
            {
                if (importDateClean.IndexOf("-") > 0)
                {
                   
                    dateTimeToReturn = DateTime.ParseExact(importDateClean, "yyyy-MM-dd HH:mm:ss".Substring(0, importDateClean.Length),
                        System.Globalization.CultureInfo.InvariantCulture);
                }
                else
                {
                    dateTimeToReturn = DateTime.ParseExact(importDateClean, "yyyy/dd/MM HH:mm:ss".Substring(0, importDateClean.Length),
                        System.Globalization.CultureInfo.InvariantCulture);
                }
            }
            catch (Exception)
            {
                throw;
            }

            return dateTimeToReturn;
        }
    }
}
