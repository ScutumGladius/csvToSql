using CommandLine;
using CsvToSql.Core;
using CsvToSql.logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
            if (string.IsNullOrWhiteSpace(argcOptions.JsonCfgFile)) argcOptions.JsonCfgFile = "settings.json";
            return argcOptions;
        }
    }
}
